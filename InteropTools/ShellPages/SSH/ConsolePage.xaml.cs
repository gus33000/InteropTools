// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Text;
using InteropTools.CorePages;
using InteropTools.Providers;
using InteropTools.ShellPages.Core;
using Renci.SshNet;
using Renci.SshNet.Common;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Shell = InteropTools.CorePages.Shell;

namespace InteropTools.ShellPages.SSH
{
    public sealed partial class ConsolePage : Page
    {
        public string CMDLoc = @"C:\Windows\System32\cmd.exe";
        private readonly IRegistryProvider _helper;

        public ConsolePage()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            Refresh();
        }

        public PageGroup PageGroup => PageGroup.SSH;
        public string PageName => "System Console";
        public ShellStream ShellStream { get; set; }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.CharacterReceived -= CoreWindow_CharacterReceived;
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async void RunInUiThread(Action function)
        {
            await
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ShowKeybHack.Focus(FocusState.Keyboard);
            MainScroll.ChangeView(0, MainScroll.ScrollableHeight, 1);
        }

        private void ConsoleBox_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            MenuFlyout flyout = new() { Placement = FlyoutPlacementMode.Top };
            MenuFlyoutItem flyoutitem = new()
            {
                Text =
                ResourceManager.Current.MainResourceMap.GetValue("Resources/Paste",
                ResourceContext.GetForCurrentView()).ValueAsString
            };
            flyoutitem.Click += async (sender_, e_) =>
            {
                DataPackageView content = Clipboard.GetContent();
                string str = await content.GetTextAsync();

                if (ShellStream == null)
                {
                    return;
                }

                if (ShellStream.CanWrite)
                {
                    ShellStream.Write(str);
                }
            };
            DataPackageView datacontent = Clipboard.GetContent();

            if (!datacontent.Contains(StandardDataFormats.Text))
            {
                flyoutitem.IsEnabled = false;
            }

            MenuFlyoutItem flyoutitem2 = new()
            {
                Text =
                ResourceManager.Current.MainResourceMap.GetValue("Resources/Select_All",
                ResourceContext.GetForCurrentView()).ValueAsString
            };
            flyoutitem2.Click += (sender_, e_) => ConsoleBox.SelectAll();

            if (flyout.Items != null)
            {
                flyout.Items.Add(flyoutitem);
                flyout.Items.Add(flyoutitem2);
            }

            flyout.ShowAt((TextBlock)sender, e.GetPosition((TextBlock)sender));
        }

        private void ConsoleBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ConsoleBox.Text.Split('\n').Length > 200)
            {
                string[] lines = ConsoleBox.Text.Split('\n');
                Array.Reverse(lines);
                string[] newcontent = new string[200];

                for (int i = 0; i < 200; i++)
                {
                    newcontent[i] = lines[i];
                }

                Array.Reverse(newcontent);
                ConsoleBox.Text = string.Join("\n", newcontent);
            }

            MainScroll.ChangeView(0, MainScroll.ScrollableHeight, 1);
        }

        private void CoreWindow_CharacterReceived(CoreWindow sender, CharacterReceivedEventArgs args)
        {
            if (ShellStream == null)
            {
                return;
            }

            if (ShellStream.CanWrite)
            {
                ShellStream.Write(((char)args.KeyCode).ToString());
            }
        }

        private async void Refresh()
        {
            Window.Current.CoreWindow.CharacterReceived += CoreWindow_CharacterReceived;

            if (!await SessionManager.IsCMDSupported())
            {
                await new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
                  ResourceManager.Current.MainResourceMap.GetValue("Resources/In_order_to_use_this_page", ResourceContext.GetForCurrentView()).ValueAsString,
                  ResourceManager.Current.MainResourceMap.GetValue("Resources/You_can_t_use_this_right_now", ResourceContext.GetForCurrentView()).ValueAsString);
                Shell shell = (Shell)App.AppContent;
                shell.RootFrame.Navigate(typeof(WelcomePage));
                return;
            }

            RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\CI", "UMCIAuditMode",
                                RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                await new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
                  ResourceManager.Current.MainResourceMap.GetValue("Resources/In_order_to_use_this_page", ResourceContext.GetForCurrentView()).ValueAsString,
                  ResourceManager.Current.MainResourceMap.GetValue("Resources/You_can_t_use_this_right_now", ResourceContext.GetForCurrentView()).ValueAsString);
                Shell shell = (Shell)App.AppContent;
                shell.RootFrame.Navigate(typeof(WelcomePage));
                return;
            }

            Start();
        }

        private void ShowKeybHack_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (!ConsoleBox.Text.EndsWith(ShowKeybHack.Text) && (ShowKeybHack.Text != "") &&
                (ShowKeybHack.Text.Length > 1))
            {
                if (ShellStream != null)
                {
                    if (ShellStream.CanWrite)
                    {
                        ShellStream.Write(ShowKeybHack.Text);
                    }
                }
            }

            ShowKeybHack.Text = "";
        }

        private void Start()
        {
            RunInThreadPool(() =>
            {
                try
                {
                    SshClient client = SessionManager.SshClient;
                    ShellStream = client.CreateShellStream("cmd", 80, 24, 800, 600, 1024);
                    ShellStream.DataReceived += Stream_DataReceived;
                }
                catch (Exception ex)
                {
                    RunInUiThread(async () => await new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(ex.Message));
                }
            });
        }

        private void Stream_DataReceived(object sender, ShellDataEventArgs e)
        {
            byte[] data_ = e.Data;
            RunInUiThread(() =>
            {
                string curtext = ConsoleBox.Text;
                string newtext = Encoding.ASCII.GetString(data_);
                curtext += newtext.Replace("\r\r", "\r");
                ConsoleBox.Text = curtext;
                MainScroll.ChangeView(0, MainScroll.ScrollableHeight, 1);
            });
        }
    }
}