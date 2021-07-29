using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using InteropTools.Providers;
using Renci.SshNet;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using InteropTools.RemoteClasses.Server;
using Windows.System.Display;
using Shell = InteropTools.CorePages.Shell;
using Windows.ApplicationModel.Resources.Core;

namespace InteropTools
{
    public static class SessionManager
    {
        public static readonly string RemoteLoc = ResourceManager.Current.MainResourceMap.GetValue(
              "Resources/The_following_Remote_device_wants_to_access_your_phone_Registry",
              ResourceContext.GetForCurrentView()).ValueAsString;
        public static readonly string RemoteAllowLoc = ResourceManager.Current.MainResourceMap.GetValue("Resources/Allow", ResourceContext.GetForCurrentView()).ValueAsString;
        public static readonly string RemoteDenyLoc = ResourceManager.Current.MainResourceMap.GetValue("Resources/Deny", ResourceContext.GetForCurrentView()).ValueAsString;

        public static SshClient SshClient { get; set; }

        public static readonly ObservableRangeCollection<Session> Sessions = new();

        public static int? CurrentSession;

        public static readonly DisplayRequest DisplayRequest = new();

        // External stuff
        public static readonly RemoteServer Server = new();

        private static readonly Random Random = new();
        public static readonly string SessionId = RandomString(10);
        public static readonly List<Remote> AllowedRemotes = new();
        public static readonly List<Remote> DeniedRemotes = new();
        // End of external stuff

        public static async Task<bool> IsCMDSupported()
        {
            IRegistryProvider helper = App.MainRegistryHelper;
            RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SYSTEM\ControlSet001\Services\MpsSvc", "Start", RegTypes.REG_DWORD); _ = ret.regtype; _ = ret.regvalue;

            //if (regvalue != "2") return false;

            if (SshClient?.IsConnected == true)
            {
                return true;
            }

            await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "default-shell",
                               RegTypes.REG_SZ, @"%SystemRoot%\system32\cmd.exe");
            await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "default-env",
                               RegTypes.REG_SZ, "currentdir,async,autoexec");
            ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                               RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if ((regvalue == null) || (regvalue?.Length == 0))
            {
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                                   RegTypes.REG_SZ, "Sirepuser");
            }

            bool add
                  = true;

            const string username = "InteropTools";
            ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                               RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

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
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh", "user-list",
                                   RegTypes.REG_SZ, regvalue + ";" + username);
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                   "user-name", RegTypes.REG_SZ, "LocalSystem");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                   "auth-method", RegTypes.REG_SZ, "password");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                   "user-pin", RegTypes.REG_SZ, SessionId);
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                   "subsystems", RegTypes.REG_SZ, "default,sftp");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                                   "default-home-dir", RegTypes.REG_SZ, @"%SystemRoot%\system32\");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "default-shell", RegTypes.REG_SZ, @"%SystemRoot%\system32\cmd.exe");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-home-dir", RegTypes.REG_SZ, "C:\\");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-mkdir-rex", RegTypes.REG_SZ, ".*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-open-dir-rex", RegTypes.REG_SZ, ".*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-read-file-rex", RegTypes.REG_SZ, ".*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-remove-file-rex", RegTypes.REG_SZ, ".*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-rmdir-rex", RegTypes.REG_SZ, ".*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-stat-rex", RegTypes.REG_SZ, ".*");
                await helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Currentcontrolset\\control\\ssh\\" + username,
                                   "sftp-write-file-rex", RegTypes.REG_SZ, ".*");
            }

            ret = await helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"system\CurrentControlSet\control\ssh\" + username,
                               "user-pin", RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            try
            {
                string Server = helper.GetHostName();
                const string Username = "InteropTools";
                string Password = regvalue;
                PasswordConnectionInfo coninfo = new(Server, Username, Password)
                {
                    Timeout = new TimeSpan(0, 0, 5),
                    RetryAttempts = 1
                };
                SftpClient sclient = new(coninfo)
                {
                    OperationTimeout = new TimeSpan(0, 0, 5)
                };
                sclient.Connect();
                sclient.BufferSize = 4 * 1024;
                IAsyncOperation<StorageFile> op = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//cmd.exe", UriKind.Absolute));

                while (op.Status == AsyncStatus.Started)
                {
                }

                Task<Stream> op2 = op.GetResults().OpenStreamForReadAsync();

                while (op2.Status == TaskStatus.Running)
                {
                }

                Stream cmd = op2.Result;
                op =
                  StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//en-US//cmd.exe.mui",
                      UriKind.Absolute));

                while (op.Status == AsyncStatus.Started)
                {
                }

                op2 = op.GetResults().OpenStreamForReadAsync();

                while (op2.Status == TaskStatus.Running)
                {
                }

                Stream cmdmui = op2.Result;
                op = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//reg.exe", UriKind.Absolute));

                while (op.Status == AsyncStatus.Started)
                {
                }

                op2 = op.GetResults().OpenStreamForReadAsync();

                while (op2.Status == TaskStatus.Running)
                {
                }

                Stream reg = op2.Result;
                op =
                  StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//en-US//reg.exe.mui",
                      UriKind.Absolute));

                while (op.Status == AsyncStatus.Started)
                {
                }

                op2 = op.GetResults().OpenStreamForReadAsync();

                while (op2.Status == TaskStatus.Running)
                {
                }

                Stream regmui = op2.Result;
                op =
                  StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//CheckNetIsolation.exe",
                      UriKind.Absolute));

                while (op.Status == AsyncStatus.Started)
                {
                }

                op2 = op.GetResults().OpenStreamForReadAsync();

                while (op2.Status == TaskStatus.Running)
                {
                }

                Stream netisol = op2.Result;
                op =
                  StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SSH//en-US//CheckNetIsolation.exe.mui",
                      UriKind.Absolute));

                while (op.Status == AsyncStatus.Started)
                {
                }

                op2 = op.GetResults().OpenStreamForReadAsync();

                while (op2.Status == TaskStatus.Running)
                {
                }

                Stream netisolmui = op2.Result;
                sclient.UploadFile(cmd, "/C/Windows/System32/cmd.exe");
                sclient.UploadFile(cmdmui, "/C/Windows/System32/en-US/cmd.exe.mui");
                sclient.UploadFile(reg, "/C/Windows/System32/reg.exe");
                sclient.UploadFile(regmui, "/C/Windows/System32/en-US/reg.exe.mui");
                sclient.UploadFile(netisol, "/C/Windows/System32/CheckNetIsolation.exe");
                sclient.UploadFile(netisolmui, "/C/Windows/System32/en-US/CheckNetIsolation.exe.mui");
                sclient.Disconnect();
                SshClient = new SshClient(coninfo);
                SshClient.Connect();
                SshClient.KeepAliveInterval = new TimeSpan(0, 0, 10);
                return true;
            }
            catch
            {
                SshClient = null;
                return false;
            }
        }

        public static void AddNewSession(object args)
        {
            Session session = new()
            {
                Helper = null,
                WindowContent = new Shell(args),//new SelectProviderPage(args),
                CreationDate = DateTime.Now
            };
            Sessions.Add(session);
            SwitchSession(session);
        }

        public static async void SwitchSession(Session session)
        {
            if (CurrentSession != null)
            {
                Sessions[(int)CurrentSession].WindowContent = App.AppContent;
                RenderTargetBitmap renderTargetBitmap = new();
                await renderTargetBitmap.RenderAsync(Sessions[(int)CurrentSession].WindowContent);
                Sessions[(int)CurrentSession].Preview = renderTargetBitmap;
            }

            if (session.WindowContent is Shell)
            {
                Shell shell = (Shell)session.WindowContent;
                AppViewBackButtonVisibility visibility = AppViewBackButtonVisibility.Collapsed;

                if (shell.RootFrame.CanGoBack)
                {
                    visibility = AppViewBackButtonVisibility.Visible;
                }

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                  AppViewBackButtonVisibility.Collapsed;
            }

            CurrentSession = Sessions.IndexOf(session);
            App.AppContent = session.WindowContent;
            RenderTargetBitmap renderTargetBitmap_ = new();
            await renderTargetBitmap_.RenderAsync(Sessions[(int)CurrentSession].WindowContent);
            Sessions[(int)CurrentSession].Preview = renderTargetBitmap_;
            Window.Current.Activate();

            if (App.AppContent is Shell)
            {
                Shell shell = (Shell)session.WindowContent;
                shell.ReSetupTitlebar();
            }
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public class Session
        {
            public UIElement WindowContent { get; set; }
            public DateTime CreationDate { get; set; }
            public IRegistryProvider Helper { get; set; }
            public RenderTargetBitmap Preview { get; set; }
        }

        public class Remote
        {
            public string SessionID { get; set; }
            public string Hostname { get; set; }
        }
    }
}
