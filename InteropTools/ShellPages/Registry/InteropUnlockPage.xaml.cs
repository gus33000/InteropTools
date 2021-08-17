using InteropTools.Classes;
using InteropTools.CorePages;
using InteropTools.Providers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class InteropUnlockPage : Page
    {
        private readonly IRegistryProvider _helper;
        private bool _initialized;

        public InteropUnlockPage()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            RunInThreadPool(DoChecks);
        }

        public PageGroup PageGroup => PageGroup.Registry;
        public string PageName => "Interop Unlock";

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async Task RunInUiThread(Action function)
        {
            await
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RunInThreadPool(async () =>
            {
                RegTypes regtype;
                string regvalue;
                GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                    "PhoneManufacturer", RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

                if (string.Equals(regvalue, "NOKIA", StringComparison.OrdinalIgnoreCase))
                {
                    ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                        "PhoneManufacturerBak", RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

                    if (regvalue != "")
                    {
                        await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                            "PhoneManufacturer", RegTypes.REG_SZ, regvalue);
                        await _helper.DeleteValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                            "PhoneManufacturerBak");
                    }
                }
                else
                {
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                        "PhoneManufacturerBak", RegTypes.REG_SZ, regvalue);
                    await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                        "PhoneManufacturer", RegTypes.REG_SZ, "NOKIA");
                }

                DoChecks();
            });
        }

        private void CapUnlock_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (CapUnlock.IsOn)
            {
                RunInThreadPool(async () =>
                {
                    bool result = await InteropUnlockHelper.TryInteropUnlockAsync();
                    if (!result)
                    {
                        await RunInUiThread(() => CapUnlock.IsOn = false);
                    }
                });
            }
            else
            {
                RunInThreadPool(async () =>
                {
                    bool result = await InteropUnlockHelper.TryUninteropUnlockAsync();
                    if (!result)
                    {
                        await RunInUiThread(() => CapUnlock.IsOn = true);
                    }
                });
            }
        }

        private async Task<bool> CheckFSAccess()
        {
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp",
              "ObjectName",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "LocalSystem")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SYSTEM\\controlset001\\services\\SdStor\\Parameters",
              "PackedCommandEnable",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp",
              "Type",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "16")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp",
              "ServiceSidType",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
              "Data0",
              RegTypes.REG_BINARY); _ = ret.regtype; regvalue = ret.regvalue;

            if (
              !((string.Equals(regvalue, "7508bca3290b900c", StringComparison.OrdinalIgnoreCase)) ||
                (string.Equals(regvalue, "7508bca3290b900c000000000000000000000000", StringComparison.OrdinalIgnoreCase))))
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
              "Data1",
              RegTypes.REG_BINARY); _ = ret.regtype; regvalue = ret.regvalue;

            if (
              !((string.Equals(regvalue, "0000000001000000", StringComparison.OrdinalIgnoreCase)) ||
                (string.Equals(regvalue, "0000000001000000000000000000000000000000", StringComparison.OrdinalIgnoreCase))))
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
              "DataType0",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
              "DataType1",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
              "Guid",
              RegTypes.REG_BINARY); _ = ret.regtype; regvalue = ret.regvalue;

            if (
              !((string.Equals(regvalue, "16287a2d5e0cfc459ce7570e5ecde9c9", StringComparison.OrdinalIgnoreCase)) ||
                (string.Equals(regvalue, "16287a2d5e0cfc459ce7570e5ecde9c900000000", StringComparison.OrdinalIgnoreCase))))
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
              "Type",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;
            return regvalue == "7";
        }

        private async Task<bool> CheckNewCapUnlock()
        {
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\capabilityRule_DevUnlock",
              "CapabilityClass",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "capabilityClass_DevUnlock_Internal")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\capabilityRule_DevUnlock",
              "PrincipalClass",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            return regvalue == "principalClass_DevUnlock_Internal";
        }

        private async Task<bool> CheckRestoreNDTK()
        {
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\OEM\\Nokia\\NokiaSvcHost\\Plugins\\NsgExtA\\NdtkSvc",
              "Path",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;
            return string.Equals(regvalue, "c:\\windows\\system32\\ndtksvc.dll", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> CheckRestoreNDTKx50()
        {
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\OEM\\Nokia\\NokiaSvcHost\\Plugins\\NsgExtA\\NdtkSvc",
              "Path",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;
            return string.Equals(regvalue, "c:\\data\\users\\public\\ndtk\\ndtksvc.dll", StringComparison.OrdinalIgnoreCase);
        }

        private async void DoChecks()
        {
            _initialized = false;

            if (await CheckFSAccess())
            {
                await RunInUiThread(() => MTPPathOption.Visibility = Visibility.Visible);
            }
            else
            {
                await RunInUiThread(() => MTPPathOption.Visibility = Visibility.Collapsed);
            }

            RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "Software\\Microsoft\\MTP",
              "datastore",
              RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;
            await RunInUiThread(() => MTPPathInput.Text = regvalue);
            bool RestoreNDTKState = await CheckRestoreNDTK();
            bool RestoreNDTKx50State = await CheckRestoreNDTKx50();
            bool CheckFSAccessState = await CheckFSAccess();
            bool CheckCapUnlockState = await InteropUnlockHelper.CheckInteropUnlockStateAsync();
            bool NewCapUnlockState = await CheckNewCapUnlock();
            InstallNDTKCheck();
            await RunInUiThread(() =>
            {
                RestoreNDTK.IsOn = RestoreNDTKState;
                RestoreNDTKx50.IsOn = RestoreNDTKx50State;
                FSAccess.IsOn = CheckFSAccessState;
                CapUnlock.IsOn = CheckCapUnlockState;
                NewCapUnlock.IsOn = NewCapUnlockState;
                _initialized = true;
            });
        }

        private void FSAccess_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (FSAccess.IsOn)
            {
                RunInThreadPool(async () =>
                {
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "Software\\Microsoft\\MTP",
                      "datastore",
                      RegTypes.REG_SZ,
                      "C:"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SYSTEM\\controlset001\\services\\SdStor\\Parameters",
                      "PackedCommandEnable",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp",
                      "ObjectName",
                      RegTypes.REG_SZ,
                      "LocalSystem"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp",
                      "Type",
                      RegTypes.REG_DWORD,
                      "16"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp",
                      "ServiceSidType",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
                      "Data0",
                      RegTypes.REG_BINARY,
                      "7508bca3290b900c"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
                      "Data1",
                      RegTypes.REG_BINARY,
                      "0000000001000000"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
                      "DataType0",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
                      "DataType1",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
                      "Guid",
                      RegTypes.REG_BINARY,
                      "16287a2d5e0cfc459ce7570e5ecde9c9"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
                      "Type",
                      RegTypes.REG_DWORD,
                      "7"
                    );
                    DoChecks();
                });
            }
            else
            {
                RunInThreadPool(async () =>
                {
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "System\\ControlSet001\\services\\Mtp",
                      "ObjectName",
                      RegTypes.REG_SZ,
                      ".\\WPNONETWORK"
                    );
                    DoChecks();
                });
            }
        }

        private async void InstallNDTKCheck()
        {
            RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo", "PhoneManufacturer",
                                RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (string.Equals(regvalue, "NOKIA", StringComparison.OrdinalIgnoreCase))
            {
                ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                    "PhoneManufacturerBak", RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

                if (regvalue?.Length == 0)
                {
                    await RunInUiThread(() => InstallNDTK.IsEnabled = false);
                }
                else
                {
                    await RunInUiThread(() =>
                    {
                        InstallNDTK.IsEnabled = true;
                        InstallNDTKText.Text = "Restore default manufacturer";
                    });
                }
            }
            else
            {
                await RunInUiThread(() =>
                {
                    InstallNDTK.IsEnabled = true;
                    InstallNDTKText.Text = "Allow the installation of NDTK on any device";
                });
            }
        }

        private void NewCapUnlock_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (NewCapUnlock.IsOn)
            {
                RunInThreadPool(async () =>
                {
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\capabilityRule_DevUnlock",
                      "CapabilityClass",
                      RegTypes.REG_SZ,
                      "capabilityClass_DevUnlock_Internal"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\capabilityRule_DevUnlock",
                      "PrincipalClass",
                      RegTypes.REG_SZ,
                      "principalClass_DevUnlock_Internal"
                    );
                    DoChecks();
                });
            }
            else
            {
                RunInThreadPool(DoChecks);
            }
        }

        private void RestoreNDTK_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (RestoreNDTK.IsOn)
            {
                RunInThreadPool(async () =>
                {
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\OEM\\Nokia\\NokiaSvcHost\\Plugins\\NsgExtA\\NdtkSvc",
                      "Path",
                      RegTypes.REG_SZ,
                      "c:\\windows\\system32\\ndtksvc.dll"
                    );
                    DoChecks();
                });
            }
            else
            {
                RunInThreadPool(DoChecks);
            }
        }

        private void RestoreNDTKx50_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (RestoreNDTKx50.IsOn)
            {
                RunInThreadPool(async () =>
                {
                    bool fileexists = _helper.DoesFileExists("c:\\data\\users\\public\\ndtk\\ndtksvc.dll");

                    if (fileexists)
                    {
                        await _helper.SetKeyValue(
                          RegHives.HKEY_LOCAL_MACHINE,
                          "SOFTWARE\\OEM\\Nokia\\NokiaSvcHost\\Plugins\\NsgExtA\\NdtkSvc",
                          "Path",
                          RegTypes.REG_SZ,
                          "c:\\data\\users\\public\\ndtk\\ndtksvc.dll"
                        );
                    }
                    else
                    {
                        await RunInUiThread(
                          async () =>
                          {
                              await new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
                            "We didn't find the ndtksvc.dll file at the following location:\nC:\\Data\\Users\\Public\\ndtk\\ndtksvc.dll\nSo we're unable to turn on that option",
                            "We didn't find the required files to turn on that option");
                          });
                    }

                    DoChecks();
                });
            }
            else
            {
                RunInThreadPool(DoChecks);
            }
        }

        private void SetMTPPathButton_Click(object sender, RoutedEventArgs e)
        {
            string newval = MTPPathInput.Text;
            RunInThreadPool(async () =>
            {
                await _helper.SetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  "Software\\Microsoft\\MTP",
                  "datastore",
                  RegTypes.REG_SZ,
                  newval
                );
                DoChecks();
            });
        }
    }
}