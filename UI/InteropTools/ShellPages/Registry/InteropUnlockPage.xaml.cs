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
        public string PageName => "Interop Unlock";
        public PageGroup PageGroup => PageGroup.Registry;

        private readonly IRegistryProvider _helper;

        private bool _initialized;

        public InteropUnlockPage()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            RunInThreadPool(DoChecks);
        }

        private async Task<bool> CheckRestoreNDTK()
        {
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\OEM\\Nokia\\NokiaSvcHost\\Plugins\\NsgExtA\\NdtkSvc",
              "Path",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;
            return regvalue.ToLower() == "c:\\windows\\system32\\ndtksvc.dll";
        }

        private async Task<bool> CheckRestoreNDTKx50()
        {
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\OEM\\Nokia\\NokiaSvcHost\\Plugins\\NsgExtA\\NdtkSvc",
              "Path",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;
            return regvalue.ToLower() == "c:\\data\\users\\public\\ndtk\\ndtksvc.dll";
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
              !((regvalue.ToLower() == "7508bca3290b900c") ||
                (regvalue.ToLower() == "7508bca3290b900c000000000000000000000000")))
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "System\\ControlSet001\\services\\Mtp\\TriggerInfo\\0",
              "Data1",
              RegTypes.REG_BINARY); _ = ret.regtype; regvalue = ret.regvalue;

            if (
              !((regvalue.ToLower() == "0000000001000000") ||
                (regvalue.ToLower() == "0000000001000000000000000000000000000000")))
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
              !((regvalue.ToLower() == "16287a2d5e0cfc459ce7570e5ecde9c9") ||
                (regvalue.ToLower() == "16287a2d5e0cfc459ce7570e5ecde9c900000000")))
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
        /*
        private async Task<bool> CheckCapUnlock()
        {
            RegTypes regtype;
            string regvalue;
            var ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SYSTEM\\controlset001\\Control\\CI",
              "CI_DEVELOPERMODE",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;
            
            if (regvalue != "1")
            {
                //return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SYSTEM\\controlset001\\services\\SdStor\\Parameters",
              "PackedCommandEnable",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\DeviceReg\\Install",
              "MaxUnsignedApp",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "65539")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\DeviceReg",
              "PortalUrlProd",
              RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "https://127.0.0.1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\DeviceReg",
              "PortalUrlInt",
              RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "https://127.0.0.1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\PackageManager",
              "EnableAppLicenseCheck",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "0")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\PackageManager",
              "EnableAppSignatureCheck",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "0")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\PackageManager",
              "EnableAppProvisioning",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "0")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager",
              "DeveloperUnlockState",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\DeveloperUnlock",
              "DeveloperUnlockState",
              RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
              "CapabilityClass",
              RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
              "PrincipalClass",
              RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "PRINCIPAL_CLASS_OEM_DEVELOPER_UNLOCK")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_FIRST_PARTY_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue !=
                "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS\nCAPABILITY_CLASS_ENTERPRISE_APPLICATIONS\nCAPABILITY_CLASS_DEVELOPER_UNLOCK")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_ENTERPRISE_OEM_LOW_ACCESS_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_VERY_HIGH_ACCESS_APPLICATIONS")
            {
                return false;
            }

            /*var items = await _helper.GetRegistryItems2(RegHives.HKEY_LOCAL_MACHINE,
                                                 @"SOFTWARE\Microsoft\SecurityManager\CapabilityClasses");

            foreach (var item in items)
            {
                if ((item.Type == RegistryItemType.VALUE) && (item.ValueType == (uint)RegTypes.REG_MULTI_SZ))
                {

                    var add
                          = true;

                    foreach (var val in item.Value.Split('\n'))
                    {
                        if (val.ToUpper().Contains("CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS"))
                        {
                            add
                                  = false;
                        }
                    }

                    if (add)
                    {
                        return false;
                    }
                }
            }*/
        /*
        return true;
    }
*/


        private async Task<bool> CheckCapUnlock()
        {
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SYSTEM\\controlset001\\Control\\CI",
              "CI_DEVELOPERMODE",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\DeviceReg\\Install",
              "MaxUnsignedApp",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "65539")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\DeviceReg",
              "PortalUrlProd",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "https://127.0.0.1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\DeviceReg",
              "PortalUrlInt",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "https://127.0.0.1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\PackageManager",
              "EnableAppLicenseCheck",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "0")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\PackageManager",
              "EnableAppSignatureCheck",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "0")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\PackageManager",
              "EnableAppProvisioning",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "0")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager",
              "DeveloperUnlockState",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\DeveloperUnlock",
              "DeveloperUnlockState",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
              "CapabilityClass",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_DEVELOPER_UNLOCK")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
              "PrincipalClass",
              RegTypes.REG_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "PRINCIPAL_CLASS_ISV_DEVELOPER_UNLOCK")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_FIRST_PARTY_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue !=
                "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS\nCAPABILITY_CLASS_ENTERPRISE_APPLICATIONS\nCAPABILITY_CLASS_DEVELOPER_UNLOCK")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_ENTERPRISE_OEM_LOW_ACCESS_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
              "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS",
              RegTypes.REG_MULTI_SZ); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_VERY_HIGH_ACCESS_APPLICATIONS")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
              "AttachEnabled",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
              "Enabled",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "0")
            {
                return false;
            }

            ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              @"SOFTWARE\Microsoft\Silverlight\Debugger",
              "WaitForAttach",
              RegTypes.REG_DWORD); _ = ret.regtype; regvalue = ret.regvalue;

            if (regvalue != "1")
            {
                return false;
            }

            System.Collections.Generic.IReadOnlyList<RegistryItemCustom> items = await _helper.GetRegistryItems2(RegHives.HKEY_LOCAL_MACHINE,
                                                 @"SOFTWARE\Microsoft\SecurityManager\CapabilityClasses");

            foreach (RegistryItemCustom item in items)
            {
                if ((item.Type == RegistryItemType.VALUE) && (item.ValueType == (uint)RegTypes.REG_MULTI_SZ))
                {

                    bool add
                          = true;

                    foreach (string val in item.Value.Split('\n'))
                    {
                        if (val.ToUpper().Contains("CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS"))
                        {
                            add
                                  = false;
                        }
                    }

                    if (add)
                    {
                        return false;
                    }
                }
            }

            return true;
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

            if (regvalue != "principalClass_DevUnlock_Internal")
            {
                return false;
            }

            return true;
        }

        private async void DoChecks()
        {
            _initialized = false;

            if (await CheckFSAccess())
            {
                RunInUiThread(() => { MTPPathOption.Visibility = Visibility.Visible; });
            }

            else
            {
                RunInUiThread(() => { MTPPathOption.Visibility = Visibility.Collapsed; });
            }

            RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(
              RegHives.HKEY_LOCAL_MACHINE,
              "Software\\Microsoft\\MTP",
              "datastore",
              RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;
            RunInUiThread(() => { MTPPathInput.Text = regvalue; });
            bool RestoreNDTKState = await CheckRestoreNDTK();
            bool RestoreNDTKx50State = await CheckRestoreNDTKx50();
            bool CheckFSAccessState = await CheckFSAccess();
            bool CheckCapUnlockState = await CheckCapUnlock();
            bool NewCapUnlockState = await CheckNewCapUnlock();
            InstallNDTKCheck();
            RunInUiThread(() =>
            {
                RestoreNDTK.IsOn = RestoreNDTKState;
                RestoreNDTKx50.IsOn = RestoreNDTKx50State;
                FSAccess.IsOn = CheckFSAccessState;
                CapUnlock.IsOn = CheckCapUnlockState;
                NewCapUnlock.IsOn = NewCapUnlockState;
                _initialized = true;
            });
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
        /*
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
                    var ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppLicenseCheck",
                      RegTypes.REG_DWORD,
                      "0"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppSignatureCheck",
                      RegTypes.REG_DWORD,
                      "0"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppProvisioning",
                      RegTypes.REG_DWORD,
                      "0"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SYSTEM\\controlset001\\services\\SdStor\\Parameters",
                      "PackedCommandEnable",
                      RegTypes.REG_DWORD,
                      "1"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager",
                      "DeveloperUnlockState",
                      RegTypes.REG_DWORD,
                      "1"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\DeveloperUnlock",
                      "DeveloperUnlockState",
                      RegTypes.REG_DWORD,
                      "1"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg\\Install",
                      "MaxUnsignedApp",
                      RegTypes.REG_DWORD,
                      "65539"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SYSTEM\\controlset001\\Control\\CI",
                      "CI_DEVELOPERMODE",
                      RegTypes.REG_DWORD,
                      "1"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                      "CapabilityClass",
                      RegTypes.REG_SZ,
                      "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                      "PrincipalClass",
                      RegTypes.REG_SZ,
                      "PRINCIPAL_CLASS_OEM_DEVELOPER_UNLOCK"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg",
                      "PortalUrlProd",
                      RegTypes.REG_SZ,
                      "https://127.0.0.1"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg",
                      "PortalUrlInt",
                      RegTypes.REG_SZ,
                      "https://127.0.0.1"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_FIRST_PARTY_APPLICATIONS"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS\nCAPABILITY_CLASS_ENTERPRISE_APPLICATIONS\nCAPABILITY_CLASS_DEVELOPER_UNLOCK"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_LOW_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    ret = await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_VERY_HIGH_ACCESS_APPLICATIONS"
                    );

                    if (ret != HelperErrorCodes.SUCCESS)
                    {
                        DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                        return;
                    }

                    /*var items = await _helper.GetRegistryItems2(RegHives.HKEY_LOCAL_MACHINE,
                                                         @"SOFTWARE\Microsoft\SecurityManager\CapabilityClasses");

                    foreach (var item in items)
                    {
                        if ((item.Type == RegistryItemType.VALUE) && (item.ValueType == (uint)RegTypes.REG_MULTI_SZ))
                        {

                            var add
                                  = true;

                            foreach (var val in item.Value.Split('\n'))
                            {
                                if (val.ToUpper().Contains("CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS"))
                                {
                                    add
                                          = false;
                                }
                            }

                            if (add)
                            {
                                ret = await _helper.SetKeyValue(item.Hive, item.Key, item.Name, item.ValueType,
                                                    item.Value + "\nCAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS");


                                if (ret != HelperErrorCodes.SUCCESS)
                                {
                                    DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
                                    return;
                                }
                            }
                        }
                    }*/
        /*
        DoChecks();
    });
}
else
{
    RunInThreadPool(async () =>
    {
        var ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
          "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS",
          RegTypes.REG_MULTI_SZ,
          "CAPABILITY_CLASS_ENTERPRISE_OEM_VERY_HIGH_ACCESS_APPLICATIONS"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
          "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS",
          RegTypes.REG_MULTI_SZ,
          "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
          "CAPABILITY_CLASS_ENTERPRISE_OEM_LOW_ACCESS_APPLICATIONS",
          RegTypes.REG_MULTI_SZ,
          "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
          "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
          RegTypes.REG_MULTI_SZ,
          "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
          "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS",
          RegTypes.REG_MULTI_SZ,
          "CAPABILITY_CLASS_FIRST_PARTY_APPLICATIONS\nCAPABILITY_CLASS_ENTERPRISE_APPLICATIONS\nCAPABILITY_CLASS_DEVELOPER_UNLOCK"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\DeviceReg",
          "PortalUrlInt",
          RegTypes.REG_SZ,
          "https://developerservices.windowsphone.com/Services/WindowsPhoneRegistration.svc/01/2010"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\DeviceReg",
          "PortalUrlProd",
          RegTypes.REG_SZ,
          "https://developerservices.windowsphone.com/Services/WindowsPhoneRegistration.svc/01/2010"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
          "PrincipalClass",
          RegTypes.REG_SZ,
          "PRINCIPAL_CLASS_ISV_DEVELOPER_UNLOCK"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
          "CapabilityClass",
          RegTypes.REG_SZ,
          "CAPABILITY_CLASS_DEVELOPER_UNLOCK"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SYSTEM\\controlset001\\Control\\CI",
          "CI_DEVELOPERMODE",
          RegTypes.REG_DWORD,
          "0"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\DeviceReg\\Install",
          "MaxUnsignedApp",
          RegTypes.REG_DWORD,
          "20"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager\\DeveloperUnlock",
          "DeveloperUnlockState",
          RegTypes.REG_DWORD,
          "0"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\SecurityManager",
          "DeveloperUnlockState",
          RegTypes.REG_DWORD,
          "0"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SYSTEM\\controlset001\\services\\SdStor\\Parameters",
          "PackedCommandEnable",
          RegTypes.REG_DWORD,
          "0"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\PackageManager",
          "EnableAppProvisioning",
          RegTypes.REG_DWORD,
          "1"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
          RegHives.HKEY_LOCAL_MACHINE,
          "SOFTWARE\\Microsoft\\PackageManager",
          "EnableAppSignatureCheck",
          RegTypes.REG_DWORD,
          "1"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        ret = await _helper.SetKeyValue(
             RegHives.HKEY_LOCAL_MACHINE,
             "SOFTWARE\\Microsoft\\PackageManager",
             "EnableAppLicenseCheck",
             RegTypes.REG_DWORD,
             "1"
        );

        if (ret != HelperErrorCodes.SUCCESS)
        {
            DoChecks();//await RunInUiThread(() => CapUnlock.IsOn = !CapUnlock.IsOn);
            return;
        }

        DoChecks();
    });
}
}
*/


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
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SYSTEM\\controlset001\\Control\\CI",
                      "CI_DEVELOPERMODE",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg\\Install",
                      "MaxUnsignedApp",
                      RegTypes.REG_DWORD,
                      "65539"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg",
                      "PortalUrlProd",
                      RegTypes.REG_SZ,
                      "https://127.0.0.1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg",
                      "PortalUrlInt",
                      RegTypes.REG_SZ,
                      "https://127.0.0.1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppLicenseCheck",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppSignatureCheck",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppProvisioning",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager",
                      "DeveloperUnlockState",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\DeveloperUnlock",
                      "DeveloperUnlockState",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                      "CapabilityClass",
                      RegTypes.REG_SZ,
                      "CAPABILITY_CLASS_DEVELOPER_UNLOCK"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                      "PrincipalClass",
                      RegTypes.REG_SZ,
                      "PRINCIPAL_CLASS_ISV_DEVELOPER_UNLOCK"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_FIRST_PARTY_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS\nCAPABILITY_CLASS_ENTERPRISE_APPLICATIONS\nCAPABILITY_CLASS_DEVELOPER_UNLOCK"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_LOW_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_VERY_HIGH_ACCESS_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
                      "AttachEnabled",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
                      "Enabled",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      @"SOFTWARE\Microsoft\Silverlight\Debugger",
                      "WaitForAttach",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    System.Collections.Generic.IReadOnlyList<RegistryItemCustom> items = await _helper.GetRegistryItems2(RegHives.HKEY_LOCAL_MACHINE,
                                                         @"SOFTWARE\Microsoft\SecurityManager\CapabilityClasses");

                    foreach (RegistryItemCustom item in items)
                    {
                        if ((item.Type == RegistryItemType.VALUE) && (item.ValueType == (uint)RegTypes.REG_MULTI_SZ))
                        {

                            bool add
                                  = true;

                            foreach (string val in item.Value.Split('\n'))
                            {
                                if (val.ToUpper().Contains("CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS"))
                                {
                                    add
                                          = false;
                                }
                            }

                            if (add)
                            {
                                await _helper.SetKeyValue(item.Hive, item.Key, item.Name, item.ValueType,
                                                    item.Value + "\nCAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS");
                            }
                        }
                    }

                    DoChecks();
                });
            }
            else
            {
                RunInThreadPool(async () =>
                {
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SYSTEM\\controlset001\\Control\\CI",
                      "CI_DEVELOPERMODE",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg\\Install",
                      "MaxUnsignedApp",
                      RegTypes.REG_DWORD,
                      "20"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg",
                      "PortalUrlProd",
                      RegTypes.REG_SZ,
                      "https://developerservices.windowsphone.com/Services/WindowsPhoneRegistration.svc/01/2010"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\DeviceReg",
                      "PortalUrlInt",
                      RegTypes.REG_SZ,
                      "https://developerservices.windowsphone.com/Services/WindowsPhoneRegistration.svc/01/2010"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppLicenseCheck",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppSignatureCheck",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\PackageManager",
                      "EnableAppProvisioning",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager",
                      "DeveloperUnlockState",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\DeveloperUnlock",
                      "DeveloperUnlockState",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                      "CapabilityClass",
                      RegTypes.REG_SZ,
                      "CAPABILITY_CLASS_DEVELOPER_UNLOCK"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                      "PrincipalClass",
                      RegTypes.REG_SZ,
                      "PRINCIPAL_CLASS_ISV_DEVELOPER_UNLOCK"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_FIRST_PARTY_APPLICATIONS\nCAPABILITY_CLASS_ENTERPRISE_APPLICATIONS\nCAPABILITY_CLASS_DEVELOPER_UNLOCK"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_LOW_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS",
                      RegTypes.REG_MULTI_SZ,
                      "CAPABILITY_CLASS_ENTERPRISE_OEM_VERY_HIGH_ACCESS_APPLICATIONS"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
                      "AttachEnabled",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
                      "Enabled",
                      RegTypes.REG_DWORD,
                      "1"
                    );
                    await _helper.SetKeyValue(
                      RegHives.HKEY_LOCAL_MACHINE,
                      @"SOFTWARE\Microsoft\Silverlight\Debugger",
                      "WaitForAttach",
                      RegTypes.REG_DWORD,
                      "0"
                    );
                    DoChecks();
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

        private async void InstallNDTKCheck()
        {
            RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo", "PhoneManufacturer",
                                RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

            if (regvalue.ToUpper() == "NOKIA")
            {
                ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                    "PhoneManufacturerBak", RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

                if (regvalue == "")
                {
                    RunInUiThread(() => { InstallNDTK.IsEnabled = false; });
                }

                else
                {
                    RunInUiThread(() =>
                    {
                        InstallNDTK.IsEnabled = true;
                        InstallNDTKText.Text = "Restore default manufacturer";
                    });
                }
            }

            else
            {
                RunInUiThread(() =>
                {
                    InstallNDTK.IsEnabled = true;
                    InstallNDTKText.Text = "Allow the installation of NDTK on any device";
                });
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
                        RunInUiThread(
                          async () =>
                          {
                              await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
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

        private static async Task RunInUiThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => { function(); });
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RunInThreadPool(async () =>
            {
                RegTypes regtype;
                string regvalue;
                GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                    "PhoneManufacturer", RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

                if (regvalue.ToUpper() == "NOKIA")
                {
                    ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\Platform\DeviceTargetingInfo",
                                        "PhoneManufacturerBak", RegTypes.REG_SZ); regtype = ret.regtype; regvalue = ret.regvalue;

                    if (regvalue == "")
                    {
                    }
                    else
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
    }
}