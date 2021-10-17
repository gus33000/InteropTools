// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using InteropTools.Providers;
using Renci.SshNet;
using Windows.ApplicationModel;
using Windows.Storage;

namespace InteropTools.Classes
{
    public class SSHSystemAccessHelper
    {
        public enum UnlockStates
        {
            DONE_NEEDS_REBOOT,
            NOT_DONE_REBOOT_PENDING,
            ALREADY_UNLOCKED,
            FAILED
        }

        public async Task<UnlockStates> UnlockSSHSystemAccess()
        {
            IRegistryProvider helper = App.MainRegistryHelper;
            CorePages.Shell shell = (CorePages.Shell)App.AppContent;
            bool useCMD = await SessionManager.IsCMDSupported();

            if (useCMD)
            {
                return UnlockStates.ALREADY_UNLOCKED;
            }

            RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                @"SYSTEM\ControlSet001\Services\MpsSvc", "Start", RegTypes.REG_DWORD);
            _ = ret.regtype;
            regvalue = ret.regvalue;

            if (regvalue != "4")
            {
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\CI",
                    "UMCIAuditMode",
                    RegTypes.REG_DWORD, "1");
                HelperErrorCodes result3 = await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"SYSTEM\ControlSet001\Services\MpsSvc", "Start", RegTypes.REG_DWORD, "4");

                if (result3 != HelperErrorCodes.Success)
                {
                    return UnlockStates.FAILED;
                }

                return UnlockStates.NOT_DONE_REBOOT_PENDING;
            }

            ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\CI",
                "UMCIAuditMode",
                RegTypes.REG_DWORD);
            _ = ret.regtype;
            regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\CI",
                    "UMCIAuditMode",
                    RegTypes.REG_DWORD, "1");
                return UnlockStates.NOT_DONE_REBOOT_PENDING;
            }

            try
            {
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                    "default-shell",
                    RegTypes.REG_SZ, @"%SystemRoot%\system32\cmd.exe");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                    "default-env",
                    RegTypes.REG_SZ, "currentdir,async,autoexec");
                ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                    "user-list",
                    RegTypes.REG_SZ);
                regtype = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue == null || regvalue?.Length == 0)
                {
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                        "user-list",
                        RegTypes.REG_SZ, "Sirepuser");
                }

                bool add
                    = true;

                const string username = "InteropTools";
                ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh",
                    "user-list",
                    RegTypes.REG_SZ);
                regtype = ret.regtype;
                regvalue = ret.regvalue;

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
                        "auth-method", RegTypes.REG_SZ, "password");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"system\CurrentControlSet\control\ssh\" + username,
                        "user-pin", RegTypes.REG_SZ, SessionManager.SessionId);
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"system\CurrentControlSet\control\ssh\" + username,
                        "subsystems", RegTypes.REG_SZ, "default,sftp");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"system\CurrentControlSet\control\ssh\" + username,
                        "default-home-dir", RegTypes.REG_SZ, @"%SystemRoot%\system32\");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "default-shell", RegTypes.REG_SZ, @"%SystemRoot%\system32\cmd.exe");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-home-dir", RegTypes.REG_SZ, "C:\\");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-mkdir-rex", RegTypes.REG_SZ, ".*");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-open-dir-rex", RegTypes.REG_SZ, ".*");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-read-file-rex", RegTypes.REG_SZ, ".*");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-remove-file-rex", RegTypes.REG_SZ, ".*");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-rmdir-rex", RegTypes.REG_SZ, ".*");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-stat-rex", RegTypes.REG_SZ, ".*");
                    await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Currentcontrolset\\control\\ssh\\" + username,
                        "sftp-write-file-rex", RegTypes.REG_SZ, ".*");
                }

                ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"system\CurrentControlSet\control\ssh\" + username,
                    "user-pin", RegTypes.REG_SZ);
                regtype = ret.regtype;
                regvalue = ret.regvalue;

                try
                {
                    string Server = helper.GetHostName();
                    const string Username = "InteropTools";
                    string Password = regvalue;
                    PasswordConnectionInfo coninfo = new(Server, Username, Password)
                    {
                        Timeout = new TimeSpan(0, 0, 5), RetryAttempts = 1
                    };
                    SftpClient sclient = new(coninfo) {OperationTimeout = new TimeSpan(0, 0, 5)};
                    sclient.Connect();
                    sclient.BufferSize = 4 * 1024;
                    Stream cmd =
                        await (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//cmd.exe",
                            UriKind.Absolute))).OpenStreamForReadAsync();
                    Stream cmdmui =
                        await (await StorageFile.GetFileFromApplicationUriAsync(
                            new Uri("ms-appx:///SSH//en-US//cmd.exe.mui", UriKind.Absolute))).OpenStreamForReadAsync();
                    Stream reg =
                        await (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//reg.exe",
                            UriKind.Absolute))).OpenStreamForReadAsync();
                    Stream regmui =
                        await (await StorageFile.GetFileFromApplicationUriAsync(
                            new Uri("ms-appx:///SSH//en-US//reg.exe.mui", UriKind.Absolute))).OpenStreamForReadAsync();
                    Stream netisol =
                        await (await StorageFile.GetFileFromApplicationUriAsync(
                                new Uri("ms-appx:///SSH//CheckNetIsolation.exe", UriKind.Absolute)))
                            .OpenStreamForReadAsync();
                    Stream netisolmui =
                        await (await StorageFile.GetFileFromApplicationUriAsync(
                                new Uri("ms-appx:///SSH//en-US//CheckNetIsolation.exe.mui", UriKind.Absolute)))
                            .OpenStreamForReadAsync();
                    sclient.UploadFile(cmd, "/C/Windows/System32/cmd.exe");
                    sclient.UploadFile(cmdmui, "/C/Windows/System32/en-US/cmd.exe.mui");
                    sclient.UploadFile(reg, "/C/Windows/System32/reg.exe");
                    sclient.UploadFile(regmui, "/C/Windows/System32/en-US/reg.exe.mui");
                    sclient.UploadFile(netisol, "/C/Windows/System32/CheckNetIsolation.exe");
                    sclient.UploadFile(netisolmui, "/C/Windows/System32/en-US/CheckNetIsolation.exe.mui");
                    sclient.Disconnect();
                    SshClient SshClient = new(coninfo);
                    SshClient.Connect();
                    SshClient.KeepAliveInterval = new TimeSpan(0, 0, 10);
                    string str = SshClient
                        .RunCommand(@"%SystemRoot%\system32\CheckNetIsolation.exe LoopbackExempt -a -n=" +
                                    Package.Current.Id.FamilyName).Execute();
                    await new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(str);
                    HelperErrorCodes result2 = await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SYSTEM\ControlSet001\Services\MpsSvc", "Start", RegTypes.REG_DWORD, "2");

                    if (result2 != HelperErrorCodes.Success)
                    {
                        return UnlockStates.FAILED;
                    }

                    return UnlockStates.DONE_NEEDS_REBOOT;
                }
                catch
                {
                    HelperErrorCodes result2 = await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SYSTEM\ControlSet001\Services\MpsSvc", "Start", RegTypes.REG_DWORD, "2");

                    if (result2 != HelperErrorCodes.Success)
                    {
                        return UnlockStates.FAILED;
                    }

                    return UnlockStates.FAILED;
                }
            }
            catch
            {
                HelperErrorCodes result2 = await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                    @"SYSTEM\ControlSet001\Services\MpsSvc", "Start", RegTypes.REG_DWORD, "2");

                if (result2 != HelperErrorCodes.Success)
                {
                    return UnlockStates.FAILED;
                }

                return UnlockStates.FAILED;
            }
        }
    }
}
