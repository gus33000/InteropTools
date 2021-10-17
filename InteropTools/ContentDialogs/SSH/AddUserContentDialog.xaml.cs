// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using InteropTools.Providers;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.SSH
{
    public sealed partial class AddUserContentDialog : ContentDialog
    {
        private readonly IRegistryProvider helper;

        public AddUserContentDialog()
        {
            InitializeComponent();
            helper = App.MainRegistryHelper;
            Init();
        }

        private async void AddUser(string username)
        {
            GetKeyValueReturn ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                @"system\CurrentControlSet\control\ssh", "user-list",
                RegTypes.REG_SZ);

            string regvalue = ret.regvalue;

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
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                    "user-list",
                    RegTypes.REG_SZ, regvalue + ";" + username);
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "user-name", RegTypes.REG_SZ, "LocalSystem");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "auth-method", RegTypes.REG_SZ, "mac@microsoft.com,publickey");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "subsystems", RegTypes.REG_SZ, "default,sftp");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "default-shell", RegTypes.REG_SZ, @"%SystemRoot%\system32\WpConAppDev.exe");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "default-env", RegTypes.REG_SZ, "currentdir,async,autoexec");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "default-home-dir", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\PhoneTools\\");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-home-dir", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\PhoneTools\\");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-mkdir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-open-dir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools(\\\\.*)*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-read-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-remove-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-rmdir-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-stat-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "sftp-write-file-rex", RegTypes.REG_SZ, "%FOLDERID_SharedData%\\\\PhoneTools\\\\.*");
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (NewUser.Text.Trim() != "")
            {
                AddUser(NewUser.Text.Trim());
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void Init()
        {
            await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                "default-shell",
                RegTypes.REG_SZ, @"%SystemRoot%\system32\cmd.exe");
            await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                "default-env",
                RegTypes.REG_SZ, "currentdir,async,autoexec");
            GetKeyValueReturn ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                @"system\CurrentControlSet\control\ssh", "user-list",
                RegTypes.REG_SZ);

            string regvalue = ret.regvalue;

            if (regvalue == null || regvalue?.Length == 0)
            {
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                    "user-list",
                    RegTypes.REG_SZ, "Sirepuser");
            }
        }

        private async void RunInThreadPool(Action function) => await ThreadPool.RunAsync(x => function());

        private async void RunInUIThread(Action function) =>
            await
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => function());
    }
}
