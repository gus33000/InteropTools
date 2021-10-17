// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InteropTools.Providers;

namespace InteropTools.Classes
{
    public static class InteropUnlockHelper
    {
        public static async Task<bool> CheckInteropUnlockStateAsync()
        {
            IRegistryProvider _helper = App.MainRegistryHelper;
            try
            {
                string regvalue;
                GetKeyValueReturn ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SYSTEM\\controlset001\\Control\\CI",
                    "CI_DEVELOPERMODE",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "1")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\DeviceReg\\Install",
                    "MaxUnsignedApp",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "65539")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\DeviceReg",
                    "PortalUrlProd",
                    RegTypes.REG_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "https://127.0.0.1")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\DeviceReg",
                    "PortalUrlInt",
                    RegTypes.REG_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "https://127.0.0.1")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\PackageManager",
                    "EnableAppLicenseCheck",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "0")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\PackageManager",
                    "EnableAppSignatureCheck",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "0")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\PackageManager",
                    "EnableAppProvisioning",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "0")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager",
                    "DeveloperUnlockState",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "1")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\DeveloperUnlock",
                    "DeveloperUnlockState",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "1")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                    "CapabilityClass",
                    RegTypes.REG_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "CAPABILITY_CLASS_DEVELOPER_UNLOCK")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\AuthorizationRules\\Capability\\CAPABILITY_RULE_ISV_DEVELOPER_UNLOCK",
                    "PrincipalClass",
                    RegTypes.REG_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "PRINCIPAL_CLASS_ISV_DEVELOPER_UNLOCK")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                    "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS",
                    RegTypes.REG_MULTI_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "CAPABILITY_CLASS_FIRST_PARTY_APPLICATIONS")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                    "CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
                    RegTypes.REG_MULTI_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue !=
                    "CAPABILITY_CLASS_SECOND_PARTY_APPLICATIONS\nCAPABILITY_CLASS_ENTERPRISE_APPLICATIONS\nCAPABILITY_CLASS_DEVELOPER_UNLOCK")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                    "CAPABILITY_CLASS_ENTERPRISE_OEM_LOW_ACCESS_APPLICATIONS",
                    RegTypes.REG_MULTI_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                    "CAPABILITY_CLASS_ENTERPRISE_OEM_MED_ACCESS_APPLICATIONS",
                    RegTypes.REG_MULTI_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    "SOFTWARE\\Microsoft\\SecurityManager\\CapabilityClasses\\Inheritance",
                    "CAPABILITY_CLASS_ENTERPRISE_OEM_HIGH_ACCESS_APPLICATIONS",
                    RegTypes.REG_MULTI_SZ);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "CAPABILITY_CLASS_ENTERPRISE_OEM_VERY_HIGH_ACCESS_APPLICATIONS")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
                    "AttachEnabled",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "1")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    @"SOFTWARE\Microsoft\.NETCompactFramework\Managed Debugger",
                    "Enabled",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "0")
                {
                    return false;
                }

                ret = await _helper.GetKeyValue(
                    RegHives.HKEY_LOCAL_MACHINE,
                    @"SOFTWARE\Microsoft\Silverlight\Debugger",
                    "WaitForAttach",
                    RegTypes.REG_DWORD);
                _ = ret.regtype;
                regvalue = ret.regvalue;

                if (regvalue != "1")
                {
                    return false;
                }

                IReadOnlyList<RegistryItemCustom> items = await _helper.GetRegistryItems2(RegHives.HKEY_LOCAL_MACHINE,
                    @"SOFTWARE\Microsoft\SecurityManager\CapabilityClasses");

                foreach (RegistryItemCustom item in items)
                {
                    if (item.Type == RegistryItemType.Value && item.ValueType == (uint)RegTypes.REG_MULTI_SZ)
                    {
                        bool add
                            = true;

                        foreach (string val in item.Value.Split('\n'))
                        {
                            if (val.IndexOf("CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
                                StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                add = false;
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
            catch
            {
            }

            return false;
        }

        public static async Task<bool> TryInteropUnlockAsync()
        {
            IRegistryProvider _helper = App.MainRegistryHelper;
            try
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

                IReadOnlyList<RegistryItemCustom> items = await _helper.GetRegistryItems2(RegHives.HKEY_LOCAL_MACHINE,
                    @"SOFTWARE\Microsoft\SecurityManager\CapabilityClasses");

                foreach (RegistryItemCustom item in items)
                {
                    if (item.Type == RegistryItemType.Value && item.ValueType == (uint)RegTypes.REG_MULTI_SZ)
                    {
                        bool add = true;

                        foreach (string val in item.Value.Split('\n'))
                        {
                            if (val.IndexOf("CAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS",
                                StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                add = false;
                            }
                        }

                        if (add)
                        {
                            await _helper.SetKeyValue(item.Hive, item.Key, item.Name, item.ValueType,
                                item.Value + "\nCAPABILITY_CLASS_THIRD_PARTY_APPLICATIONS");
                        }
                    }
                }

                return await CheckInteropUnlockStateAsync();
            }
            catch
            {
            }

            return false;
        }

        public static async Task<bool> TryUninteropUnlockAsync()
        {
            IRegistryProvider _helper = App.MainRegistryHelper;
            try
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

                return !await CheckInteropUnlockStateAsync();
            }
            catch
            {
            }

            return false;
        }
    }
}
