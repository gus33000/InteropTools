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
        public string PageName => "Settings";
        public PageGroup PageGroup => PageGroup.Bottom;

        private readonly RemoteServer _server = App.Server;

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
                    App.DisplayRequest.RequestActive();
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

                App.DisplayRequest.RequestRelease();
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

        private static async void Server_OnDataReceived(string data,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string reply =
              await new RegistryActions().RegistryAction(data, args.Socket.Information.RemoteAddress.ToString());
            DataWriter writer = new DataWriter(args.Socket.OutputStream);
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

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }

        public SettingsViewModel ViewModel { get; }

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
            string buildString = "";
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("InteropTools.Resources.BuildDate.txt");
            string builddate = new StreamReader(resource).ReadLine().Replace("\r", "");
            PackageVersion appver = Package.Current.Id.Version;
            string appverstr = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
            buildString = appverstr + " (fbl_prerelease(gustavem)";
            Type myType = Type.GetType("InteropTools.ShellPages.Private.YourWindowsBuildPage");

            if (myType != null)
            {
                buildString = buildString + "/private";
            }

            buildString = buildString + "." + builddate + ")";
            VersionText.Text = buildString;
            string title = ApplicationView.GetForCurrentView().Title;

            if (title == "")
            {
                title = Package.Current.DisplayName;
            }

            AppTitle.Text = title;
            VersionShortText.Text = string.Format("{0}.{1}", appver.Major, appver.Minor);
            //new AcerRPCComponent.AcerRPC().CopyFile(@"C:\Windows\System32\ntoskrnl.exe", @"C:\Data\Users\Public\Documents\ntoskrnl.exe");
            //string sid = new SecureBootRuntimeComponent.PolicyProvisionNative().GetAppContainerSID();
            //Test1.Text = sid;
            //Test1.Text = BitConverter.ToString(new SecureBootRuntimeComponent.PolicyProvisionNative().GetDeviceID());
            //Test1.Text = HostInformation.PublisherHostId;
            //Test2.Text = new SecureBootRuntimeComponent.PolicyProvisionNative().IsRunningLaterThanTH2().ToString();
            /*Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Ahead of Time.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2106");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Archipelago.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2140");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Beep Alarm.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2107");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Birds in the Woods.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2108");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Clokktastrophe.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2109");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Early Chill.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2110");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Epic Day.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2111");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Lumia Clock.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2216");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Riverbank Dawn.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2113");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Alarms", "Torchbearer.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2116");

			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Americano.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2117");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Appointment.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2100");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Ball.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2119");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Bird.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2261");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Bubbles.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2262");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Cat.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2153");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Diamond.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2121");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Digital Friend.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2263");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Early.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2259");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Glass.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2122");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Glaze.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2184");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Hazel.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2101");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Hybrid.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2291");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Kantele.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2260");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Knock.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2123");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Lumia Calendar.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2215");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Lumia Email.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2218");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Lumia Message.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2219");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Lumia Voicemail.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2220");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Minutes.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2102");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Neo.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2124");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Pear.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2128");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Pouch.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2130");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Reservation.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2104");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Notifications", "Sauna.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2105");

			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Air Display.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2264");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Alablaster.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2265");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Bird Box.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2145");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Bouncey Bounce.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2146");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Breeze.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2147");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Brikabrak.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2148");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Brimful.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2149");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Candy.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2151");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Concierge.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2170");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Easy for You.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2269");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Exoplanet.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2181");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Friendship.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2183");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Good Times.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2271");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Horizon.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2270");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Ice.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2187");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Lucky Five.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2283");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Marbles.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2221");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Mbira.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2180");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Microsoft Ringtone.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2217");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Miniature of Troy.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2229");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Nana.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2284");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Nostalgia.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2234");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "On the Bridge.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2235");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Pebble.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2286");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Shimmering.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2287");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Silver.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2245");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Skate.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2288");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Slow Coffee.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2246");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Summer Fruit.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2289");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "That Girl from Copenhagen.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2250");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "The Shakes.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2251");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Tomorrow.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2252");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Two Cats.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2254");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Universe.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2255");
			Helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\Sounds\\Ringtones", "Waterways.wma", RegTypes.REG_SZ, "@SoundsStrings.dll,-2290");*/
        }
    }
}
