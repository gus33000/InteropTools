// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Threading.Tasks;
using InteropTools.ContentDialogs.SSH;
using InteropTools.CorePages;
using InteropTools.Providers;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteropTools.ShellPages.SSH
{
    public sealed partial class SSHAccountManagerPage : Page
    {
        private readonly IRegistryProvider _helper;
        private bool _initialized;

        public SSHAccountManagerPage()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            Refresh();
        }

        public PageGroup PageGroup => PageGroup.SSH;
        public string PageName => "SSH Account Manager";

        private async void RunInUiThread(Action function)
        {
            await
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private async Task AddUser(string username)
        {
            _initialized = false;
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                                RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            bool add
                  = true;

            if (regvalue.Contains(";"))
            {
                foreach (string user in regvalue.Split(';'))
                {
                    if (string.Equals(user, username, StringComparison.OrdinalIgnoreCase))
                    {
                        add
                              = false;
                    }
                }
            }
            else
            {
                if (string.Equals(regvalue, username, StringComparison.OrdinalIgnoreCase))
                {
                    add
                          = false;
                }
            }

            if (add)
            {
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                                    RegTypes.REG_SZ, regvalue + ";" + username);
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "user-name", RegTypes.REG_SZ, "LocalSystem");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "auth-method", RegTypes.REG_SZ, "mac@microsoft.com,publickey");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "subsystems", RegTypes.REG_SZ, "default,sftp");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-shell", RegTypes.REG_SZ, @"%SystemRoot%\system32\WpConAppDev.exe");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-env", RegTypes.REG_SZ, "currentdir,async,autoexec");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-home-dir", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\PhoneTools\\");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-home-dir", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\PhoneTools\\");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-mkdir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-open-dir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools(\\\\.*)*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-read-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-remove-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-rmdir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-stat-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-write-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
            }

            await RefreshUserList();
            _initialized = true;
        }

        private async void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;
            await new AddUserContentDialog().ShowAsync();
            await RefreshUserList();
            _initialized = true;
        }

        private async Task ApplyCMDAccessSelectedUser()
        {
            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-home-dir", RegTypes.REG_SZ, @"C:\");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                    "default-shell", RegTypes.REG_SZ, @"%SystemRoot%\system32\cmd.exe");
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async Task ApplyCMDAccessTempSelectedUser()
        {
            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-home-dir", RegTypes.REG_SZ, @"C:\");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                    "default-shell", RegTypes.REG_SZ, @"%SystemDrive%\data\users\public\documents\cmd.exe");
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async Task ApplyFullSFTPSelectedUser()
        {
            _initialized = false;
            string username = UserList.SelectedItem.ToString();
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-home-dir", RegTypes.REG_SZ, "C:\\");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-mkdir-rex", RegTypes.REG_SZ, ".*");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-open-dir-rex", RegTypes.REG_SZ, ".*");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-read-file-rex", RegTypes.REG_SZ, ".*");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-remove-file-rex", RegTypes.REG_SZ, ".*");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-rmdir-rex", RegTypes.REG_SZ, ".*");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-stat-rex", RegTypes.REG_SZ, ".*");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                "sftp-write-file-rex", RegTypes.REG_SZ, ".*");
            await RefreshSelected();
            _initialized = true;
        }

        private async void AuthMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                    "auth-method", RegTypes.REG_SZ,
                                    AuthMethod.SelectedIndex == 0 ? "mac@microsoft.com,publickey" : "password");
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await RestoreSelectedDefaults();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await ApplyFullSFTPSelectedUser();
        }

        private async void Button_Click_10(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-rmdir-rex", RegTypes.REG_SZ, sftprmdirrex.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-open-dir-rex", RegTypes.REG_SZ, sftpopendirrex.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-remove-file-rex", RegTypes.REG_SZ, sftpremovefilerex.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_13(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-write-file-rex", RegTypes.REG_SZ, sftpwritefilerex.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_14(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-stat-rex", RegTypes.REG_SZ, sftpstatrex.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_15(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-read-file-rex", RegTypes.REG_SZ, sftpreadfilerex.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await ApplyCMDAccessTempSelectedUser();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await ApplyCMDAccessSelectedUser();
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "user-pin", RegTypes.REG_SZ, pass.Password);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "user-name", RegTypes.REG_SZ, user_name.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-shell", RegTypes.REG_SZ, defaultshell.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-home-dir", RegTypes.REG_SZ, defaulthomedir.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-home-dir", RegTypes.REG_SZ, sftphomedir.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-mkdir-rex", RegTypes.REG_SZ, sftpmkdirrex.Text);
            }

            await RefreshSelected();
            _initialized = true;
        }

        private void Reboot()
        {
            RunInUiThread(
              async () =>
            {
                await
                new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
                  ResourceManager.Current.MainResourceMap.GetValue("Resources/To_complete_this_operation__you_ll_need_to_reboot_your_device_", ResourceContext.GetForCurrentView()).ValueAsString,
                  ResourceManager.Current.MainResourceMap.GetValue("Resources/Reboot_required", ResourceContext.GetForCurrentView()).ValueAsString);
            });
        }

        private async Task Refresh()
        {
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "default-shell",
                                RegTypes.REG_SZ, @"%SystemRoot%\system32\cmd.exe");
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "default-env",
                                RegTypes.REG_SZ, "currentdir,async,autoexec");
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                                RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if ((regvalue == null) || (regvalue?.Length == 0))
            {
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                                    RegTypes.REG_SZ, "Sirepuser");
            }

            await RefreshUMCI();
            await RefreshUserList();
            _initialized = true;
        }

        private async Task RefreshSelected()
        {
            if (UserList.SelectedItem == null)
            {
                return;
            }

            string username = UserList.SelectedItem.ToString();
            ShellPanel.Visibility = Visibility.Collapsed;
            SFTPPanel.Visibility = Visibility.Collapsed;
            sxsDefault.IsChecked = false;
            sxsSFTP.IsChecked = false;
            PassPanel.Visibility = Visibility.Collapsed;
            pass.Password = "";
            AuthMethod.SelectedIndex = 0;
            defaultshell.Text = "";
            user_name.Text = "";
            sftphomedir.Text = "";
            defaulthomedir.Text = "";
            sftpmkdirrex.Text = "";
            sftpopendirrex.Text = "";
            sftpreadfilerex.Text = "";
            sftpremovefilerex.Text = "";
            sftprmdirrex.Text = "";
            sftpstatrex.Text = "";
            sftpwritefilerex.Text = "";
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "subsystems", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue.Contains(","))
            {
                foreach (string sxs in regvalue.Split(','))
                {
                    if (string.Equals(sxs, "default", StringComparison.OrdinalIgnoreCase))
                    {
                        sxsDefault.IsChecked = true;
                    }

                    if (string.Equals(sxs, "sftp", StringComparison.OrdinalIgnoreCase))
                    {
                        sxsSFTP.IsChecked = true;
                    }
                }
            }
            else
            {
                if (string.Equals(regvalue, "default", StringComparison.OrdinalIgnoreCase))
                {
                    sxsDefault.IsChecked = true;
                }

                if (string.Equals(regvalue, "sftp", StringComparison.OrdinalIgnoreCase))
                {
                    sxsSFTP.IsChecked = true;
                }
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "auth-method", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue == "mac@microsoft.com,publickey")
            {
                AuthMethod.SelectedIndex = 0;
                PassPanel.Visibility = Visibility.Collapsed;
            }

            if (regvalue == "password")
            {
                AuthMethod.SelectedIndex = 1;
                PassPanel.Visibility = Visibility.Visible;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "user-pin", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                pass.Password = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "default-shell", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                defaultshell.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "user-name", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                user_name.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-home-dir", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftphomedir.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "default-home-dir", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                defaulthomedir.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-mkdir-rex", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftpmkdirrex.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-open-dir-rex", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftpopendirrex.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-read-file-rex", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftpreadfilerex.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-remove-file-rex", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftpremovefilerex.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-rmdir-rex", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftprmdirrex.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-stat-rex", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftpstatrex.Text = regvalue;
            }

            ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                "sftp-write-file-rex", RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != null)
            {
                sftpwritefilerex.Text = regvalue;
            }

            if (sxsDefault.IsChecked == true)
            {
                ShellPanel.Visibility = Visibility.Visible;
            }

            if (sxsSFTP.IsChecked == true)
            {
                SFTPPanel.Visibility = Visibility.Visible;
            }
        }

        private async Task RefreshUMCI()
        {
            string value;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\CI", "UMCIAuditMode",
                                RegTypes.REG_DWORD); _ = ret.regtype; value = ret.regvalue;
            UMCIAuditModeBox.IsChecked = value == "1";
        }

        private async Task RefreshUserList()
        {
            if (UserList.Items == null)
            {
                return;
            }

            for (int i = UserList.Items.Count - 1; i >= 0; i--)
            {
                UserList.Items.RemoveAt(i);
            }

            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                                RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue.Contains(";"))
            {
                foreach (string user in regvalue.Split(';'))
                {
                    UserList.Items.Add(user);
                }
            }
            else
            {
                UserList.Items.Add(regvalue);
            }

            if (UserList.Items.Count != 0)
            {
                UserList.SelectedIndex = 0;
            }
        }

        private async Task RestoreSelectedDefaults()
        {
            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "user-name", RegTypes.REG_SZ, "LocalSystem");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "auth-method", RegTypes.REG_SZ, "mac@microsoft.com,publickey");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "subsystems", RegTypes.REG_SZ, "default,sftp");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-shell", RegTypes.REG_SZ, @"%SystemRoot%\system32\WpConAppDev.exe");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-env", RegTypes.REG_SZ, "currentdir,async,autoexec");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "default-home-dir", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\PhoneTools\\");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-home-dir", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\PhoneTools\\");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-mkdir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-open-dir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools(\\\\.*)*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-read-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-remove-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-rmdir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-stat-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                    "sftp-write-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void SxsDefault_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();

                if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default,sftp");
                }
                else
                    if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default");
                }
                else
                        if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "sftp");
                }
                else
                            if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "");
                }
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void SxsDefault_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();

                if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default,sftp");
                }
                else
                    if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default");
                }
                else
                        if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "sftp");
                }
                else
                            if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "");
                }
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void SxsSFTP_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();

                if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default,sftp");
                }
                else
                    if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default");
                }
                else
                        if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "sftp");
                }
                else
                            if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "");
                }
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void SxsSFTP_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;

            if (UserList.SelectedItem != null)
            {
                string username = UserList.SelectedItem.ToString();

                if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default,sftp");
                }
                else
                    if ((sxsDefault.IsChecked == true) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "default");
                }
                else
                        if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == true))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "sftp");
                }
                else
                            if ((sxsDefault.IsChecked == false) && (sxsSFTP.IsChecked == false))
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                        "subsystems", RegTypes.REG_SZ, "");
                }
            }

            await RefreshSelected();
            _initialized = true;
        }

        private async void UMCIAuditModeBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\CI", "UMCIAuditMode",
                                RegTypes.REG_DWORD, "1");
            await RefreshUMCI();
            Reboot();
            _initialized = true;
        }

        private async void UMCIAuditModeBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;
            await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\CI", "UMCIAuditMode",
                                RegTypes.REG_DWORD, "0");
            await RefreshUMCI();
            Reboot();
            _initialized = true;
        }

        private async void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _initialized = false;
            await RefreshSelected();
            _initialized = true;
        }
    }
}