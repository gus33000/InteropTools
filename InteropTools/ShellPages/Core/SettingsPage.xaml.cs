using InteropTools.CorePages;
using InteropTools.Presentation;
using InteropTools.RemoteClasses.Server;
using System;
using System.IO;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private readonly RemoteServer _server = SessionManager.Server;

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = new SettingsViewModel();

            try
            {
                ColorPicker.Color = ViewModel.SelectedBrush.Color;
            }
            catch
            {
            }

            Refresh();

            ServerSwitch.IsOn = _server.Started;

            if (_server.Started)
            {
                PortNumber.Text = _server.Port.ToString();
            }
        }

        public PageGroup PageGroup => PageGroup.Bottom;
        public string PageName => "Settings";
        public SettingsViewModel ViewModel { get; }

        private static async void Server_OnDataReceived(string data,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string reply =
              await new RegistryActions().RegistryAction(data, args.Socket.Information.RemoteAddress.ToString());
            DataWriter writer = new(args.Socket.OutputStream);
            writer.WriteUInt32(writer.MeasureString(reply));
            writer.WriteString(reply);

            try
            {
                await writer.StoreAsync();
                writer.DetachStream();
            }
            catch
            {
                // ignored
            }
        }

        private static void Server_OnError(string message)
        {
        }

        private void ColorPicker_ColorChanged(Microsoft.UI.Xaml.Controls.ColorPicker sender, Microsoft.UI.Xaml.Controls.ColorChangedEventArgs args)
        {
            if (!ViewModel.Brushes.Contains(new Windows.UI.Xaml.Media.SolidColorBrush(args.NewColor)))
            {
                if (ViewModel.Brushes.Count == 48)
                {
                    ViewModel.Brushes.Add(new Windows.UI.Xaml.Media.SolidColorBrush(args.NewColor));
                }

                ViewModel.Brushes[48] = new Windows.UI.Xaml.Media.SolidColorBrush(args.NewColor);
            }
            ViewModel.SelectedBrush = new Windows.UI.Xaml.Media.SolidColorBrush(args.NewColor);
        }

        private void Refresh()
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("InteropTools.Resources.BuildDate.txt");
            string builddate = new StreamReader(resource).ReadLine().Replace("\r", "");
            PackageVersion appver = Package.Current.Id.Version;
            string appverstr = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
            string buildString = appverstr + " (fbl_prerelease(gustavem)";
            Type myType = Type.GetType("InteropTools.ShellPages.Private.YourWindowsBuildPage");

            if (myType != null)
            {
                buildString += "/private";
            }

            buildString = buildString + "." + builddate + ")";
            VersionText.Text = buildString;
            string title = ApplicationView.GetForCurrentView().Title;

            if (title?.Length == 0)
            {
                title = Package.Current.DisplayName;
            }

            AppTitle.Text = title;
            VersionShortText.Text = string.Format("{0}.{1}", appver.Major, appver.Minor);
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private void ServerSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (ServerSwitch.IsOn)
            {
                if (_server.Started)
                {
                    return;
                }

                try
                {
                    StartRemoteServer(int.Parse(PortNumber.Text));
                    SessionManager.DisplayRequest.RequestActive();
                }
                catch
                {
                    ServerSwitch.IsOn = false;
                }
            }
            else
            {
                if (!_server.Started)
                {
                    return;
                }

                SessionManager.DisplayRequest.RequestRelease();
                _server.Stop();
            }
        }

        private void StartRemoteServer(int portnumber)
        {
            try
            {
                RunInThreadPool(() =>
                {
                    _server.OnError += Server_OnError;
                    _server.OnDataReceived += Server_OnDataReceived;
                    _server.Start(portnumber);
                });
            }
            catch
            {
            }
        }
    }
}