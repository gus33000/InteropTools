// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using InteropTools.Providers;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Registry
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TweaksPage : Page
    {
        private readonly IRegistryProvider _helper;

        public TweaksPage()
        {
            InitializeComponent();
            _helper = App.RegistryHelper;
            Refresh();
        }

        private void AddBoolTweak(Func<bool> Check, Action<bool> Apply, string Title, string Description)
        {
            TweakBoolControl control = new(Check, Apply, Title, Description);
            BoolTweaks.Children.Add(control);
        }

        private void AddInputTweak(Func<string> Check, Action<string> Apply, string Title, string Description)
        {
            TweakInputControl control = new(Check, Apply, Title, Description);
            BoolTweaks.Children.Add(control);
        }

        private HelperErrorCodes GetKeyValue(RegHives hive, string key, string valuename, RegTypes type,
            out RegTypes valtype, out string value)
        {
            GetKeyValueReturn res = AsyncHelpers.RunSync(() => _helper.GetKeyValue(hive, key, valuename, type));
            valtype = res.regtype;
            value = res.regvalue;

            return res.returncode;
        }

        private void Refresh()
        {
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Docking",
                        "EnabledForTest", RegTypes.REG_DWORD, out RegTypes BrightSliderType,
                        out string BrightSliderValue);

                    return BrightSliderValue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Docking",
                            "EnabledForTest", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Docking",
                            "EnabledForTest", RegTypes.REG_DWORD, "0");
                    }
                },
                "Force Continuum via Miracast on unsupported devices",
                "Allows you to use continuum on an unsupported device, requires MS_DOCKING to be installed.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\OEM\\NOKIA\\Display\\ColorAndLight",
                        "UserSettingNoBrightnessSettings", RegTypes.REG_DWORD, out RegTypes BrightSliderType,
                        out string BrightSliderValue);

                    return BrightSliderValue == "0";
                },
                async state =>
                {
                    if (state)
                    {
                        AsyncHelpers.RunSync(() => _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\OEM\\NOKIA\\Display\\ColorAndLight",
                            "UserSettingNoBrightnessSettings", RegTypes.REG_DWORD, "0"));
                    }
                    else
                    {
                        AsyncHelpers.RunSync(() => _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\OEM\\NOKIA\\Display\\ColorAndLight",
                            "UserSettingNoBrightnessSettings", RegTypes.REG_DWORD, "1"));
                        HelperErrorCodes result = AsyncHelpers.RunSync(() =>
                            _helper.DeleteValue(RegHives.HKEY_LOCAL_MACHINE,
                                "SOFTWARE\\OEM\\NOKIA\\Display\\ColorAndLight", "UserSettingNoBrightnessSettings"));

                        if (result == HelperErrorCodes.Failed)
                        {
                            await
                                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                    async () =>
                                    {
                                        await
                                            new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
                                                "In order to turn off that option, we need to delete a specific value in the registry, sometimes deleting a value doesn't work as expected. If you really wish to get back manual states for brightness, you'll sadly need to do a hard reset. This option will still appear as off here but it might not revert in settings.",
                                                "Failed to turn off Brightness slider");
                                    });
                        }
                    }
                },
                "Enable brightness slider (Might not be reversible) (Reboot required)",
                "Allows you to dynamically specify your brightness intensity in the settings app");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "BCD00000001\\Objects\\{7ea2e1ac-2e61-4728-aaa3-896d9d0a9f0e}\\Elements\\26000512", "Element",
                        RegTypes.REG_BINARY, out RegTypes regtype, out string regvalue);

                    return regvalue == "01";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "BCD00000001\\Objects\\{7ea2e1ac-2e61-4728-aaa3-896d9d0a9f0e}\\Elements\\26000512",
                            "Element", RegTypes.REG_BINARY, "01");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "BCD00000001\\Objects\\{7ea2e1ac-2e61-4728-aaa3-896d9d0a9f0e}\\Elements\\26000512",
                            "Element", RegTypes.REG_BINARY, "00");
                    }
                },
                "Enable Offline Charging (Reboot required)",
                "Allows you to charge the phone without the OS running");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Services\KeepWiFiOnSvc",
                        "Start", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "2";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SYSTEM\CurrentControlSet\Services\KeepWiFiOnSvc", "Start", RegTypes.REG_DWORD, "2");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SYSTEM\CurrentControlSet\Services\KeepWiFiOnSvc", "Start", RegTypes.REG_DWORD, "4");
                    }
                },
                "Keep Wifi Service on (Reboot required)",
                "Allows the Wifi Service to run under the lock screen");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\Microsoft\WCMSvc\WifiNetworkManager\Config", "EnableStaticIP", RegTypes.REG_DWORD,
                        out RegTypes regtype, out string regvalue);

                    if (regvalue != "1")
                    {
                        return false;
                    }

                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SYSTEM\ControlSet001\services\WiFiConnSvc\Parameters\Config", "EnableStaticIP",
                        RegTypes.REG_DWORD, out regtype, out regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SOFTWARE\Microsoft\WCMSvc\WifiNetworkManager\Config", "EnableStaticIP",
                            RegTypes.REG_DWORD,
                            "1");
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SYSTEM\ControlSet001\services\WiFiConnSvc\Parameters\Config", "EnableStaticIP",
                            RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SOFTWARE\Microsoft\WCMSvc\WifiNetworkManager\Config", "EnableStaticIP",
                            RegTypes.REG_DWORD,
                            "0");
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SYSTEM\ControlSet001\services\WiFiConnSvc\Parameters\Config", "EnableStaticIP",
                            RegTypes.REG_DWORD, "0");
                    }
                },
                "Allow static network settings (Reboot required)",
                "Allows you to specify static network settings under the Wifi settings page");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Settings\Lock", "DisableNever",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Settings\Lock",
                            "DisableNever", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Settings\Lock",
                            "DisableNever", RegTypes.REG_DWORD, "1");
                    }
                },
                "Allow never lock timeout option (Reboot required)",
                "Allows you to specify 'never' as an option for the screen timeout under the lock screen settings");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\Microsoft\EventSounds\Sounds\WiFiConnected", "Disabled", RegTypes.REG_DWORD,
                        out RegTypes regtype, out string regvalue);

                    if (regvalue != "0")
                    {
                        return false;
                    }

                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\Microsoft\EventSounds\Sounds\WiFiDisconnected", "Disabled", RegTypes.REG_DWORD,
                        out regtype, out regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SOFTWARE\Microsoft\EventSounds\Sounds\WiFiConnected", "Disabled", RegTypes.REG_DWORD,
                            "0");
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SOFTWARE\Microsoft\EventSounds\Sounds\WiFiDisconnected", "Disabled", RegTypes.REG_DWORD,
                            "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SOFTWARE\Microsoft\EventSounds\Sounds\WiFiConnected", "Disabled", RegTypes.REG_DWORD,
                            "1");
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            @"SOFTWARE\Microsoft\EventSounds\Sounds\WiFiDisconnected", "Disabled", RegTypes.REG_DWORD,
                            "1");
                    }
                },
                "Allow Wifi Sounds (Reboot required)",
                "Allows you to specify under the Sounds setting page if you want Wifi Status sounds");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Photos\OEM", "ShutterSoundUnlocked",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                }, state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Photos\OEM",
                            "ShutterSoundUnlocked", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Photos\OEM",
                            "ShutterSoundUnlocked", RegTypes.REG_DWORD, "0");
                    }
                }, "Allow Camera Sounds (Reboot required)",
                "Allows you to specify under the Sounds setting page if you want Camera sounds");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SYSTEM\ControlSet001\Services\NlpmService",
                        "ImagePath", RegTypes.REG_EXPAND_SZ, out RegTypes regtype, out string regvalue);

                    return regvalue.IndexOf(@"C:\windows\System32\OEMServiceHost.exe -k NsgGlance",
                        StringComparison.OrdinalIgnoreCase) >= 0;
                }, state =>
                {
                    _helper.AddKey(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance");
                    _helper.AddKey(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService", "PluginPath", RegTypes.REG_SZ,
                        "\\Data\\SharedData\\OEM\\Public\\NsgGlance_NlpmServiceImpl_4.1.12.4.dll");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService", "Path", RegTypes.REG_SZ,
                        "C:\\Data\\SharedData\\OEM\\Public\\NsgGlance_NlpmService_4.1.12.4.dll");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService", "Version", RegTypes.REG_SZ,
                        "4.1.12.4");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService", "PluginVersion",
                        RegTypes.REG_SZ,
                        "4.1.12.4");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService", "Enabled", RegTypes.REG_DWORD,
                        "1");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService", "UsingBeta",
                        RegTypes.REG_DWORD,
                        "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        @"SOFTWARE\OEM\Nokia\NokiaSvcHost\Plugins\NsgGlance\NlpmService", "UseBeta", RegTypes.REG_DWORD,
                        "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "AlwaysOnInCharger",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "AppGraphicTimeout",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "BSSwitchOffTimeout",
                        RegTypes.REG_DWORD, "30");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm",
                        "ClockAndIndicatorsCustomColor", RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "DarkMode",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "DarkModeElements",
                        RegTypes.REG_DWORD, "15");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "DarkModeEnd",
                        RegTypes.REG_DWORD, "420");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "DarkModeOverrideColor",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "DarkModeStart",
                        RegTypes.REG_DWORD, "1320");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "DarkModeThreshold",
                        RegTypes.REG_DWORD, "20000");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "DoubleTapEnabled",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "Enabled",
                        RegTypes.REG_DWORD,
                        "1");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "MinimizeIcon",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "Mode",
                        RegTypes.REG_DWORD,
                        "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "MoveClock",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "NormalModeElements",
                        RegTypes.REG_DWORD, "31");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "SwipeEnabled",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "SwitchOffTimeout",
                        RegTypes.REG_DWORD, "15");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "PanelType",
                        RegTypes.REG_DWORD, "1");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "ShowDetailedAppStatus",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm",
                        "ShowSystemNotifications",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "FontFile",
                        RegTypes.REG_SZ,
                        "\\Data\\SharedData\\OEM\\Public\\lpmFonts_4.1.12.4\\lpmFont_wxga.bin");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "AppGraphicGestures",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "SingleTapWakeup",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "EnablePublicSDK",
                        RegTypes.REG_DWORD, "0");
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\OEM\Nokia\lpm", "SupportedTouchEvents",
                        RegTypes.REG_DWORD, "0");

                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SYSTEM\ControlSet001\Services\NlpmService",
                            "ImagePath", RegTypes.REG_EXPAND_SZ,
                            @"C:\windows\System32\OEMServiceHost.exe -k NsgGlance");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SYSTEM\ControlSet001\Services\NlpmService",
                            "ImagePath", RegTypes.REG_EXPAND_SZ, @"C:\windows\System32\OEMServiceHost.exe -k NsgExtA");
                    }
                }, "Enable Old Glance Screen (RS1) (Glance enabled devices only) (Reboot required)",
                "Allows you to re-enable the old glance screen with the widget and battery icon support");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\Power", "TtmEnabled",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                }, state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\Power",
                            "TtmEnabled", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"System\CurrentControlSet\Control\Power",
                            "TtmEnabled", RegTypes.REG_DWORD, "0");
                    }
                }, "Continuum - Independent Monitor Idle",
                "This will enable the user to continue using the second screen even when the phone goes to idle.");
            AddBoolTweak(() =>
            {
                GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\FingerKB\Options",
                    "ForceLargeScreenDevice", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                return regvalue == "1";
            }, state =>
            {
                if (state)
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\FingerKB\Options",
                        "ForceLargeScreenDevice", RegTypes.REG_DWORD, "1");
                }
                else
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\FingerKB\Options",
                        "ForceLargeScreenDevice", RegTypes.REG_DWORD, "0");
                }
            }, "Simulate large screen - Keyboard", "Keyboards will behave as if the device had a large screen.");
            AddBoolTweak(() =>
            {
                GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Data Sense", "ShowPayPerByte",
                    RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                return regvalue == "1";
            }, state =>
            {
                if (state)
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Data Sense", "ShowPayPerByte",
                        RegTypes.REG_DWORD, "1");
                }
                else
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Data Sense", "ShowPayPerByte",
                        RegTypes.REG_DWORD, "0");
                }
            }, "Enable PayPerByte plan", "When enabled, Data Sense will support PayPerByte plan type.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Data Sense", "DisableSystemBucket",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                }, state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Data Sense",
                            "DisableSystemBucket", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Data Sense",
                            "DisableSystemBucket", RegTypes.REG_DWORD, "0");
                    }
                }, "Skip System bucketing",
                "By default Data Sense will combine all system apps and services into a single bucket. Skip this will allow viewing data usage by individual system apps and services.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Windows Phone\\LanguageCPL",
                        "CPLMode", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Windows Phone\\LanguageCPL", "CPLMode", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Windows Phone\\LanguageCPL", "CPLMode", RegTypes.REG_DWORD, "0");
                    }
                },
                "Toggle Language CPL mode",
                "ON (1) to enable 'Modern' Language CPL with a user profile language list, OFF (0) to show Apollo-style Language CPL with a single display language.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\ControlSet001\\Services\\BrokerInfrastructure\\Parameters", "EnergyBudgetDisabled",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "System\\ControlSet001\\Services\\BrokerInfrastructure\\Parameters", "EnergyBudgetDisabled",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "System\\ControlSet001\\Services\\BrokerInfrastructure\\Parameters", "EnergyBudgetDisabled",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "Budget Control",
                "IsDisabled?");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                        "Debug Endpoint", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                            "Debug Endpoint", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                            "Debug Endpoint", RegTypes.REG_DWORD, "1");
                    }
                },
                "Debug Endpoint",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                        "EnableWebBrowserBorder", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                            "EnableWebBrowserBorder", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                            "EnableWebBrowserBorder", RegTypes.REG_DWORD, "1");
                    }
                },
                "Yellow Border",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                        "EnableWebBrowserLogger", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                            "EnableWebBrowserLogger", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                            "EnableWebBrowserLogger", RegTypes.REG_DWORD, "1");
                    }
                },
                "Write Debug Logs",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                        "EnableDefaultDocModeForLegacyApp", RegTypes.REG_DWORD, out RegTypes regtype,
                        out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                            "EnableDefaultDocModeForLegacyApp", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\TaskHost",
                            "EnableDefaultDocModeForLegacyApp", RegTypes.REG_DWORD, "1");
                    }
                },
                "Default Doc Mode for legacy app",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                        "DisableEntityExtraction", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                            "DisableEntityExtraction", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                            "DisableEntityExtraction", RegTypes.REG_DWORD, "1");
                    }
                },
                "Disable Entity Extraction",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                        "TouchEvents", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                            "TouchEvents", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\Debug",
                            "TouchEvents", RegTypes.REG_DWORD, "1");
                    }
                },
                "Touch events",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "SOFTWARE\\Microsoft\\Internet Explorer\\IntelliForms", "Enabled", RegTypes.REG_DWORD,
                        out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\IntelliForms", "Enabled", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Internet Explorer\\IntelliForms", "Enabled", RegTypes.REG_DWORD, "1");
                    }
                },
                "Intelliforms",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_DISABLE_MULTITHREADED_WEBVIEW",
                        "*", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "MT";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_DISABLE_MULTITHREADED_WEBVIEW",
                            "*", RegTypes.REG_DWORD, "MT");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_DISABLE_MULTITHREADED_WEBVIEW",
                            "*", RegTypes.REG_DWORD, "MT");
                    }
                },
                "Disable Multi-threaded WebView",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_ENABLE_MULTITHREADED_WEBVIEW_FOR_VERSION_6_3",
                        "*", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "MT";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_ENABLE_MULTITHREADED_WEBVIEW_FOR_VERSION_6_3",
                            "*", RegTypes.REG_DWORD, "MT");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_ENABLE_MULTITHREADED_WEBVIEW_FOR_VERSION_6_3",
                            "*", RegTypes.REG_DWORD, "MT");
                    }
                },
                "Enable MT WebView for v6.3",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Internet Explorer\\Main",
                        "DisableEdgeEngine", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "Edge";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Internet Explorer\\Main",
                            "DisableEdgeEngine", RegTypes.REG_DWORD, "Edge");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Internet Explorer\\Main",
                            "DisableEdgeEngine", RegTypes.REG_DWORD, "Edge");
                    }
                },
                "Edge Engine",
                "Toggle this to disable the Edge HTML engine in IE. This only takes effect for new tabs.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Email\\OemSettings\\DarkTheme",
                        "Disabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Email\\OemSettings\\DarkTheme", "Disabled", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Email\\OemSettings\\DarkTheme", "Disabled", RegTypes.REG_DWORD, "1");
                    }
                },
                "Toggle OEM Dark Theme Reg Key",
                "When enabled(the default), email will theme with the system; so when enabled, and the system theme is Dark, email will be Dark. However, Email's Read and Compose UI will remain overridden to Light to ensure that the reading and typing experience with html based email is not distorted.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Contacts\\Sim",
                        "EnableSIMAddressBookAndExport", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Contacts\\Sim",
                            "EnableSIMAddressBookAndExport", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Contacts\\Sim",
                            "EnableSIMAddressBookAndExport", RegTypes.REG_DWORD, "0");
                    }
                },
                "Toggle SIM Address Book Reg Key",
                "When enabled: -contacts from the SIM card will be displayed in the People Hub - users can create contacts directly on the SIM - users can export contacts to the SIM from the People Settings page");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Contacts\\Settings",
                        "FeaturesEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Contacts\\Settings",
                            "FeaturesEnabled", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Contacts\\Settings",
                            "FeaturesEnabled", RegTypes.REG_DWORD, "0");
                    }
                },
                "Enable Do Not Disturb",
                "When enabled: -Untested Contacts Features are disabled.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\BackgroundModel\\Policy", "Enable",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\BackgroundModel\\Policy", "Enable",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\BackgroundModel\\Policy", "Enable",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "Toggle Background Policy",
                "When disabled, the background policy will not be enforced thus allowing skype to run in the background without being terminated. When enabled(the default), background policy is enforced thus potentially terminating skype background if it exceeds CPU quota.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\ActiveSync",
                        "DisablePIIStripping", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\ActiveSync",
                            "DisablePIIStripping", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\ActiveSync",
                            "DisablePIIStripping", RegTypes.REG_DWORD, "0");
                    }
                },
                "Toggle PII Stripping",
                "Enable / disable PII Stripping in Sync Logs. (Default: Enabled)");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                        "EnableMediaLogDisplay", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "EnableMediaLogDisplay", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "EnableMediaLogDisplay", RegTypes.REG_DWORD, "1");
                    }
                },
                "Media Info Overlay Display",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications", "Server", RegTypes.REG_DWORD,
                        out RegTypes regtype, out string regvalue);

                    return regvalue == "winphone.wns.windows.com";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications", "Server",
                            RegTypes.REG_DWORD, "winphone.wns.windows.com");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications", "Server",
                            RegTypes.REG_DWORD, "bay.wns.windows.com");
                    }
                },
                "Select WNS endpoint for selfhosting, reboot required for the change to take effect",
                "Production causes Client (notifsvc) to connect to WNS PROD endpoint. DogFood causes Client (notifsvc) to connect to WNS DF endpoint");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\PhoneShareHost",
                        "EnableDataPackageHost", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\PhoneShareHost",
                            "EnableDataPackageHost", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\PhoneShareHost",
                            "EnableDataPackageHost", RegTypes.REG_DWORD, "1");
                    }
                },
                "Enable DataPackageHost",
                "Enable or disable DataPackageHost for sharing between modern apps. If no value is set DataPackageHost is enabled. When disabled registry value is 0. When enabled registry value is 1.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                        "DisableYUVComposition", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "YUV";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "DisableYUVComposition", RegTypes.REG_DWORD, "YUV");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "DisableYUVComposition", RegTypes.REG_DWORD, "YUV");
                    }
                },
                "Disable Silverlight YUV Composition",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                        "DisableDCompForVideo", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "YUV";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "DisableDCompForVideo", RegTypes.REG_DWORD, "YUV");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "DisableDCompForVideo", RegTypes.REG_DWORD, "YUV");
                    }
                },
                "Disable Silverlight SVR for Video",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                        "EnableFramerateCounter", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "YUV";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "EnableFramerateCounter", RegTypes.REG_DWORD, "YUV");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Silverlight",
                            "EnableFramerateCounter", RegTypes.REG_DWORD, "YUV");
                    }
                },
                "Silverlight Frame Rate Counter",
                "");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Input\\TIPC", "EnableTestMode",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Input\\TIPC",
                            "EnableTestMode", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Input\\TIPC",
                            "EnableTestMode", RegTypes.REG_DWORD, "1");
                    }
                },
                "Toggle Test Mode",
                "This will turn on test mode for TIPC to assist in e2e validation.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Input\\TIPC", "MaxDataAge",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Input\\TIPC",
                            "MaxDataAge",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Input\\TIPC",
                            "MaxDataAge",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "Clear MaxDataAge",
                "Setting this to test will set the value to 1 and upload to COSMOS with no wait time. Default is 24 hrs");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\TestHooks",
                        "InstallAppsToSDTestHook", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\TestHooks",
                            "InstallAppsToSDTestHook", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\TestHooks",
                            "InstallAppsToSDTestHook", RegTypes.REG_DWORD, "1");
                    }
                },
                "Always install apps on SD Card",
                "Enable or disable apps always install on SD Card");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\MusicVideoHub",
                        "MusicAppRedirectorEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\MusicVideoHub",
                            "MusicAppRedirectorEnabled", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\MusicVideoHub",
                            "MusicAppRedirectorEnabled", RegTypes.REG_DWORD, "1");
                    }
                },
                "Enable Music app redirection",
                "When the Music app is launched, redirect to the Jupiter Music app if installed");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Data Sense",
                        "DisableSystemBucket", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Data Sense",
                            "DisableSystemBucket", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Data Sense",
                            "DisableSystemBucket", RegTypes.REG_DWORD, "not");
                    }
                },
                "Skip System bucketing",
                "By default Data Sense will combine all system apps and services into a single bucket.Skip this will allow viewing data usage by individual system apps and services.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Data Sense", "ShowPayPerByte",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Data Sense",
                            "ShowPayPerByte", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Data Sense",
                            "ShowPayPerByte", RegTypes.REG_DWORD, "not");
                    }
                },
                "Enable PayPerByte plan",
                "When enabled, Data Sense will support PayPerByte plan type.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\SystemTray",
                        "DataActivity", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\SystemTray",
                            "DataActivity", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\OEM\\SystemTray",
                            "DataActivity", RegTypes.REG_DWORD, "not");
                    }
                },
                "System Tray Network Usage",
                "When enabled, System Tray will show arrows indicating network usage.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "SYSTEM\\ControlSet001\\Control\\WMI\\Autologger\\WiFiSession\\{5CA18737-22AC-4050-85BC-B8DBB9F7D986}",
                        "EnableLevel", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "4";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SYSTEM\\ControlSet001\\Control\\WMI\\Autologger\\WiFiSession\\{5CA18737-22AC-4050-85BC-B8DBB9F7D986}",
                            "EnableLevel", RegTypes.REG_DWORD, "4");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "SYSTEM\\ControlSet001\\Control\\WMI\\Autologger\\WiFiSession\\{5CA18737-22AC-4050-85BC-B8DBB9F7D986}",
                            "EnableLevel", RegTypes.REG_DWORD, "5");
                    }
                },
                "Logging Level",
                "!!!REQUIRES REBOOT TO APPLY CHANGES !!! Sets the WiFi Network Manager WPP logging level.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Office Mobile\\OneNote Mobile",
                        "SyncFileBackupEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Office Mobile\\OneNote Mobile", "SyncFileBackupEnabled",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Office Mobile\\OneNote Mobile", "SyncFileBackupEnabled",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "Sync file backup",
                "Once enabled everytime you sync any OneNote file, a backup of the file will be created to help debug sync issues. Do the following to collect files and send them: (1) Enable this registry if not enabled already (2) Cleanup old backed up files (3) Run your scenario (4) Email your backed up files (5) (Optional) Unset registry and delete old files.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Search\\ClientConfiguration",
                        "UseCustomServer", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Search\\ClientConfiguration",
                            "UseCustomServer", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Search\\ClientConfiguration",
                            "UseCustomServer", RegTypes.REG_DWORD, "1");
                    }
                },
                "Toggle Custom Server",
                "Toggles whether to override the config service endpoint with a custom option");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration", "MOSServerName",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "hybrid.api.here.com";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration", "MOSServerName",
                            RegTypes.REG_DWORD, "hybrid.api.here.com");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration", "MOSServerName",
                            RegTypes.REG_DWORD, "Integration");
                    }
                },
                "Server Configuration",
                "Choose the map data server. The name will be set on the initialization of MOS.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration", "DisableFrameRateCap",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "hybrid.api.here.com";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "DisableFrameRateCap", RegTypes.REG_DWORD, "hybrid.api.here.com");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "DisableFrameRateCap", RegTypes.REG_DWORD, "Integration");
                    }
                },
                "Frame Rate Cap",
                "Set whether the frame rate should be capped to 30 fps on devices with high-resolution screens. The default is to cap.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration", "FusionSensorCapable",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "hybrid.api.here.com";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "FusionSensorCapable", RegTypes.REG_DWORD, "hybrid.api.here.com");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "FusionSensorCapable", RegTypes.REG_DWORD, "Integration");
                    }
                },
                "Fusion Sensor Capability",
                "Set whether unblock/block the fusion sensor capability so that the sensor based features can be supported/unsupported. The default is unblock. Used for testing.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration", "ShowDebugPanel",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "hybrid.api.here.com";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "ShowDebugPanel",
                            RegTypes.REG_DWORD, "hybrid.api.here.com");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "ShowDebugPanel",
                            RegTypes.REG_DWORD, "Integration");
                    }
                },
                "Debug Panel Visibility",
                "Set whether to show the debug panel in map app or not. Default setting is invisible. Used for testing.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                        "ShowRenderStatistics", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "hybrid.api.here.com";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "ShowRenderStatistics", RegTypes.REG_DWORD, "hybrid.api.here.com");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration",
                            "ShowRenderStatistics", RegTypes.REG_DWORD, "Integration");
                    }
                },
                "Render Statistics Visibility",
                "Set whether to show render statistics in the map app or not. Default setting is invisible. Used for testing.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Dev\\SapiSvr",
                        "EnableVoiceAgents", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Dev\\SapiSvr",
                            "EnableVoiceAgents", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Dev\\SapiSvr",
                            "EnableVoiceAgents", RegTypes.REG_DWORD, "1");
                    }
                },
                "Enable Blue Speech Experience",
                "Enable Blue Speech Experience");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Dev\\SapiSvr",
                        "EnableAllVoiceAgents", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Dev\\SapiSvr",
                            "EnableAllVoiceAgents", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Dev\\SapiSvr",
                            "EnableAllVoiceAgents", RegTypes.REG_DWORD, "1");
                    }
                },
                "Enable Dev Voice Agents",
                "Enable Dev Voice Agents");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Test",
                        "EnableSpeechRecoServiceAudioSegmentation", RegTypes.REG_DWORD, out RegTypes regtype,
                        out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Test",
                            "EnableSpeechRecoServiceAudioSegmentation", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Speech\\Test",
                            "EnableSpeechRecoServiceAudioSegmentation", RegTypes.REG_DWORD, "1");
                    }
                },
                "Enable SRS audio segmentation",
                "Temporary setting: Enable the Speech Recognition Service (SRS) to handle audio segmentation for continuous dictation");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\ContextualAwareness\\InterestExtraction\\", "PIIStripping",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\ContextualAwareness\\InterestExtraction\\", "PIIStripping",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\ContextualAwareness\\InterestExtraction\\", "PIIStripping",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "PII Stripping",
                "Enable/Disable PII stripping in the logged traces.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\ContextualAwareness\\InterestExtraction\\", "ZCortanaAppSupport",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\ContextualAwareness\\InterestExtraction\\", "ZCortanaAppSupport",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\ContextualAwareness\\InterestExtraction\\", "ZCortanaAppSupport",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "zCortanaApp Support",
                "Enable/Disable support for interest extraction through zCortanaApp.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Start",
                        "PinOnInstallComplete", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Start",
                            "PinOnInstallComplete", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Start",
                            "PinOnInstallComplete", RegTypes.REG_DWORD, "0");
                    }
                },
                "Pin On Install Complete",
                "Enable/Disable automatic pinning when install completes.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\ActionCenter",
                        "XamlExperienceEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\ActionCenter",
                            "XamlExperienceEnabled", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\ActionCenter",
                            "XamlExperienceEnabled", RegTypes.REG_DWORD, "0");
                    }
                },
                "Experience Type",
                "This switch provides the ability to toggle between Splash or Xaml AC.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\LockAppSettings",
                        "LockLayer", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\LockAppSettings",
                            "LockLayer", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\LockAppSettings",
                            "LockLayer", RegTypes.REG_DWORD, "0");
                    }
                },
                "Enable Lock Layer",
                "This will launch the lock app into a new view container.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\LockAppSettings",
                        "LivingImage", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\LockAppSettings",
                            "LivingImage", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\LockAppSettings",
                            "LivingImage", RegTypes.REG_DWORD, "0");
                    }
                },
                "Enable Living Image",
                "This will allow you to set living images on your lock screen.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\CurrentControlSet\\Control\\Power",
                        "TtmEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\CurrentControlSet\\Control\\Power",
                            "TtmEnabled", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\CurrentControlSet\\Control\\Power",
                            "TtmEnabled", RegTypes.REG_DWORD, "0");
                    }
                },
                "Continuum - Independent Monitor Idle",
                "This will enable the user to continue using the second screen even when the phone goes to idle. Your phone will reboot automatically for the change to take effect.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Windows Media Foundation\\Platform\\SAR", "OffloadDisable",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Windows Media Foundation\\Platform\\SAR", "OffloadDisable",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "Software\\Microsoft\\Windows Media Foundation\\Platform\\SAR", "OffloadDisable",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "Toggle SAR HW Offload Disabled Reg Key",
                "When enabled, the SAR(Streaming Audio Renderer) will attempt to use HW offload to decode media when available in the underlying audio stack for a given type(falling back to using a decoder in the pipeline if not available). When disabled, the SAR will instead always use only the decoders available as MFTs.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\Playback",
                        "DisableHardwareMFTUse", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\Playback",
                            "DisableHardwareMFTUse", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\Playback",
                            "DisableHardwareMFTUse", RegTypes.REG_DWORD, "1");
                    }
                },
                "Toggle Zune Disable Hardware MFT Use Reg Key",
                "When enabled, ZMQ will attempt to use Hardware MFT to decode media when available(falling back to using a software decoder in the pipeline if not available). When disabled, ZMQ will instead use only Software decoders when using MFTs.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\XAML",
                        "ReleaseGraphicsDeviceOnSuspend", RegTypes.REG_DWORD, out RegTypes regtype,
                        out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\XAML",
                            "ReleaseGraphicsDeviceOnSuspend", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\XAML",
                            "ReleaseGraphicsDeviceOnSuspend", RegTypes.REG_DWORD, "0");
                    }
                },
                "Drop Graphics Device On Suspend",
                "ReleaseGraphicsDeviceOnSuspend");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\AgHost", "Enabled",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\AgHost", "Enabled",
                            RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\AgHost", "Enabled",
                            RegTypes.REG_DWORD, "1");
                    }
                },
                "Run SL apps in Modern Context",
                "This will run Silverlight apps on CoreApplication. Currently, apps must be installed via APPX and enabling this will break Jupiter apps.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\TaskHost",
                        "AllowUIAutomationForLegacyApps", RegTypes.REG_DWORD, out RegTypes regtype,
                        out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\TaskHost",
                            "AllowUIAutomationForLegacyApps", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\TaskHost",
                            "AllowUIAutomationForLegacyApps", RegTypes.REG_DWORD, "1");
                    }
                },
                "Turn on accessibility for legacy SL apps",
                "Only SL 8.1 or above apps are accessible. This will allow testing accessibility on existing apps.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\PackageManager",
                        "EverythingIsLightUp", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\PackageManager",
                            "EverythingIsLightUp", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\PackageManager",
                            "EverythingIsLightUp", RegTypes.REG_DWORD, "1");
                    }
                },
                "All XAPs are SL 8.1 XAPs",
                "This will treat SL 8.0 XAPs as SL 8.1 XAPs. It'll trigger functionality that will created SL 8.1 manifest on the fly so the application is deployed/installed as such (SL 8.1 one).");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\PackageManager",
                        "ForcedLightupWhiteListEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\PackageManager",
                            "ForcedLightupWhiteListEnabled", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\PackageManager",
                            "ForcedLightupWhiteListEnabled", RegTypes.REG_DWORD, "1");
                    }
                },
                "Whitelist of forced SL 8.1 XAPs",
                @"This will turn on whitelist of forced SL 8.1 XAPs, and all apps defined in whitelist will be treated as SL 8.1 XAPs. It'll trigger functionality that will created SL 8.1 manifest on the fly so the application is deployed/installed as such (SL 8.1 one). The whitelist is defined at C:\Windows\System32\ForcedLightupAppWhiteList.xml.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration\\DataLoader",
                        "UseOdvsIntData", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "INT";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration\\DataLoader",
                            "UseOdvsIntData", RegTypes.REG_DWORD, "INT");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Maps\\Configuration\\DataLoader",
                            "UseOdvsIntData", RegTypes.REG_DWORD, "PROD");
                    }
                },
                "Endpoints Configuration",
                "Set whether to use INT or PROD endpoints. Default is PROD.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "System\\Assistant\\ClientConfiguration\\HoseProfile", "UseConfig", RegTypes.REG_DWORD,
                        out RegTypes regtype, out string regvalue);

                    return regvalue == "registry";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "System\\Assistant\\ClientConfiguration\\HoseProfile", "UseConfig", RegTypes.REG_DWORD,
                            "registry");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                            "System\\Assistant\\ClientConfiguration\\HoseProfile", "UseConfig", RegTypes.REG_DWORD,
                            "Configuration");
                    }
                },
                "Select EndPoint source",
                "Select EndPoint and CookieDomain source");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\OEM\\Nokia\\Touch\\WakeupGesture",
                        "Enabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\OEM\\Nokia\\Touch\\WakeupGesture",
                            "Enabled", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\OEM\\Nokia\\Touch\\WakeupGesture",
                            "Enabled", RegTypes.REG_DWORD, "0");
                    }
                },
                "Enable Double Tap to Wake",
                "This will allow the phone screen to wake on double tap.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\Settings", "WakeupGestureSupported",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\Settings",
                            "WakeupGestureSupported", RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\Settings",
                            "WakeupGestureSupported", RegTypes.REG_DWORD, "0");
                    }
                },
                "Enable Double Tap to Wake Supported",
                "Sets flag on device that double tap to wake is supported by hardware.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                        "ForceWideLayout", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "ForceWideLayout", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "ForceWideLayout", RegTypes.REG_DWORD, "1");
                    }
                },
                "Force Layouts to Wide",
                "Force all layouts to use wide versions(will clip on some devices).");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "ShowCommaOnChat", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                            "ShowCommaOnChat", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                            "ShowCommaOnChat", RegTypes.REG_DWORD, "1");
                    }
                },
                "Prefer comma key on Chat",
                "Use comma instead of Emoji button in Chat contexts. (IS_CHAT).");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                        "DockedKeyboardEnable", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "DockedKeyboardEnable", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "DockedKeyboardEnable", RegTypes.REG_DWORD, "1");
                    }
                },
                "Toggle the docked keyboard feature",
                "Enable / Disable docked keyboards.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretDisabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                            "CaretDisabled", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                            "CaretDisabled", RegTypes.REG_DWORD, "1");
                    }
                },
                "Caret Stick Enabled/Disabled",
                "Enable the caret stick for use on the touch keyboard.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretRightHand", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                            "CaretRightHand", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                            "CaretRightHand", RegTypes.REG_DWORD, "1");
                    }
                },
                "Caret Stick Right/Left Handed",
                "Choose caret stick location based on the dominant hand of the user.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\SearchMaps\\Assist\\Persona", "Hiding",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "1";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\SearchMaps\\Assist\\Persona",
                            "Hiding",
                            RegTypes.REG_DWORD, "1");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\SearchMaps\\Assist\\Persona",
                            "Hiding",
                            RegTypes.REG_DWORD, "0");
                    }
                },
                "toggle visibility",
                "Temporarily disable the creation of the persona controls in the UI in order to work around stability issues.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                        "MultiSizeLogicEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "MultiSizeLogicEnabled", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "MultiSizeLogicEnabled", RegTypes.REG_DWORD, "1");
                    }
                },
                "Keyboard sizing logic",
                "Toggle the logic to size and position the keyboard.");
            AddBoolTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                        "DragAnchorKeyEnabled", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);

                    return regvalue == "0";
                },
                state =>
                {
                    if (state)
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "DragAnchorKeyEnabled", RegTypes.REG_DWORD, "0");
                    }
                    else
                    {
                        _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\StigRegKey\\FingerKB",
                            "DragAnchorKeyEnabled", RegTypes.REG_DWORD, "1");
                    }
                },
                "Enable/Disable DragAnchorKey related features",
                "Manually enable/disable DragAnchorKey related features (vertical and horizontal drags).");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_MOBILE_KEEP_DECODED_IMAGE_THRESHOLD_BYTES",
                        "*", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_MOBILE_KEEP_DECODED_IMAGE_THRESHOLD_BYTES",
                        "*", RegTypes.REG_DWORD, state);
                },
                "Keep Decoded Image Threshold Size",
                "Sets the decoded image threshold size in bytes. Images decoded to this size or less will be kept in code fully decoded (512MB device = 32KB (32768), 1GB device = 64KB (65536))");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_MOBILE_OVERSIZED_IMAGE_SCALING_THRESHOLD_BYTES",
                        "*", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_MOBILE_OVERSIZED_IMAGE_SCALING_THRESHOLD_BYTES",
                        "*", RegTypes.REG_DWORD, state);
                },
                "Oversized Scaling Threshold Size",
                "Sets maximum image size in bytes before forced scaling occurs. Images larger will be scaled to this size. (512MB device = 8MB (8388608), 1GB device = 32MB (33554432))");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_MOBILE_DISPOSABLE_RESOURCE_CACHE_THRESHOLD_BYTES",
                        "*", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_MOBILE_DISPOSABLE_RESOURCE_CACHE_THRESHOLD_BYTES",
                        "*", RegTypes.REG_DWORD, state);
                },
                "Disposable Resource Threshold Size",
                "Sets the disposable resource cache cap in bytes. (Set to 0 to turn off disposable resource cache.) Items will be evicted in LRU manner when the cap is reached. (512MB device = 48MB (50331648), 1GB device = 96MB (100663296))");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Top",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Top",
                        RegTypes.REG_DWORD, state);
                },
                "Top Edge Threshold",
                "Specify the max distance in pixels from the top edge at which a contact may be considered as an edge gesture candidate. Enter distance in pixels");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Bottom",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Bottom",
                        RegTypes.REG_DWORD, state);
                },
                "Bottom Edge Threshold",
                "Specify the max distance in pixels from the bottom edge at which a contact may be considered as an edge gesture candidate. Enter distance in pixels");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Left",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Left",
                        RegTypes.REG_DWORD, state);
                },
                "Left Edge Threshold",
                "Specify the max distance in pixels from the left edge at which a contact may be considered as an edge gesture candidate. Enter distance in pixels");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Right",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Touch\\EdgeGestureThresholds", "Right",
                        RegTypes.REG_DWORD, state);
                },
                "Right Edge Threshold",
                "Specify the max distance in pixels from the right edge at which a contact may be considered as an edge gesture candidate. Enter distance in pixels");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Wallet\\Settings",
                        "MinimumPinLength", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Wallet\\Settings",
                        "MinimumPinLength", RegTypes.REG_DWORD, state);
                },
                "PIN Lock Min",
                "Adjusts the minimum length of the PIN lock Enter the new minimum size");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Wallet\\Services\\WLID",
                        "FakeLiveId", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Wallet\\Services\\WLID",
                        "FakeLiveId", RegTypes.REG_SZ, state);
                },
                "Fake Live ID",
                "Set whether the wallet should fake believe that a Live ID has been configured Enter the fake live id email (ex test@microsoft.com)");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Wallet\\Services\\WLID",
                        "FakeUserName", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Wallet\\Services\\WLID",
                        "FakeUserName", RegTypes.REG_SZ, state);
                },
                "Fake Live ID User Name",
                "Set the user name of the Fake Livd Id Enter the fake user name (ex 'Joe Smith')");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "DoubleTapTimeThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "DoubleTapTimeThreshold", RegTypes.REG_DWORD, state);
                },
                "DoubleTapTimeThreshold",
                "DoubleTapTimeThreshold(default: 300) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndLongHoldTimeThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndLongHoldTimeThreshold", RegTypes.REG_DWORD, state);
                },
                "TapAndLongHoldTimeThreshold",
                "TapAndLongHoldTimeThreshold(default: 1000) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndHoldStartTimeThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndHoldStartTimeThreshold", RegTypes.REG_DWORD, state);
                },
                "TapAndHoldStartTimeThreshold",
                "TapAndHoldStartTimeThreshold(default: 700) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndShortHoldTimeThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndShortHoldTimeThreshold", RegTypes.REG_DWORD, state);
                },
                "TapAndShortHoldTimeThreshold",
                "TapAndShortHoldTimeThreshold(default: 600) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndHoldDurationThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Input",
                        "TapAndHoldDurationThreshold", RegTypes.REG_DWORD, state);
                },
                "TapAndHoldDurationThreshold",
                "TapAndHoldDurationThreshold(default: 300) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Dictation",
                        "DictationSilenceAutoStopTimeout", RegTypes.REG_DWORD, out RegTypes regtype,
                        out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Dictation",
                        "DictationSilenceAutoStopTimeout", RegTypes.REG_DWORD, state);
                },
                "Silence Auto Stop Timeout",
                "Value, stored as an milliseconds 0-20000 (0-20 seconds), for non-form filling fields, that represents the interval for the dictation to auto stop Enter time interval in milliseconds, e.g. 1500 for 1.5 second, 0 for default");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Dictation",
                        "FormFillingDictationSilenceAutoStopTimeout", RegTypes.REG_DWORD, out RegTypes regtype,
                        out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Dictation",
                        "FormFillingDictationSilenceAutoStopTimeout", RegTypes.REG_DWORD, state);
                },
                "Form Filling Silence Timeout",
                "Value, stored as an milliseconds 0-20000 (0-20 seconds), for form filling fields, that represents the interval for the dictation to auto stop Enter time interval in milliseconds, e.g. 1500 for 1.5 second, 0 for default");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Messaging\\GlobalSettings",
                        "PduDumpPath", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Messaging\\GlobalSettings",
                        "PduDumpPath", RegTypes.REG_SZ, state);
                },
                "[Test Hook] Dump MMS PDU",
                "Dump MMS PDU to a user defined path Enter the path on device.Please make sure MMS Transport has access right to the folder.");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Phone\\Settings",
                        "VoLTEAudioQualityString", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Phone\\Settings",
                        "VoLTEAudioQualityString", RegTypes.REG_SZ, state);
                },
                "Set VoLTE Audio Quality String",
                "Set VoLTE Audio Quality String. Enter VoLTE Audio Quality String");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "SYSTEM\\ControlSet001\\Control\\WMI\\Autologger\\NetworkTrace", "FileName", RegTypes.REG_SZ,
                        out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE,
                        "SYSTEM\\ControlSet001\\Control\\WMI\\Autologger\\NetworkTrace", "FileName", RegTypes.REG_SZ,
                        state);
                },
                "Set NetworkTrace Name",
                "Set NetworkTrace name (*.etl). File will be stored in c:\\data\\systemdata\\etw\\ Enter Trace name");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration\\Custom",
                        "ProactivePageUrl", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration\\Custom",
                        "ProactivePageUrl", RegTypes.REG_SZ, state);
                },
                "Custom Proctive Base Page",
                "Specify a custom PROACTIVE base page endpoint. You must also select \"CUSTOM\"in the 'Select SnR Environment' menu. URL should not contain any query parameters. Use FQDNs for all host names. Example: http://dev.redmond.corp.microsoft.com/proactive;");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration\\Custom",
                        "ReactiveServerPrefix", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration\\Custom",
                        "ReactiveServerPrefix", RegTypes.REG_SZ, state);
                },
                "Custom Reactive Server Prefix",
                "Specify a custom REACTIVE server prefix. You must also select \"CUSTOM\"in the 'Select SnR Environment' menu. URL should not contain any query parameters. Use FQDNs for all host names. Example: http://dev.redmond.corp.microsoft.com");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration\\Custom",
                        "CookieDomain", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration\\Custom",
                        "CookieDomain", RegTypes.REG_SZ, state);
                },
                "Custom Cookie Domain",
                "Specify a custom Cookie Domain. You must also select \"CUSTOM\"in the 'Select SnR Environment' menu. This should match whatever domain you're using in your CUSTOM proactive/reactive endpoints. Use FQDNs for all host names. Example: http://dev.redmond.corp.microsoft.com");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration",
                        "CustomProactiveQueryParams", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration",
                        "CustomProactiveQueryParams", RegTypes.REG_SZ, state);
                },
                "Proactive Query Parameters",
                "Specify custom/debug query parameters to be appended to /proactive HTML requests. Value should start with a '?' Example: ? flight = assistant & debug = true");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration",
                        "CustomReactiveQueryParams_Cat2", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration",
                        "CustomReactiveQueryParams_Cat2", RegTypes.REG_SZ, state);
                },
                "Reactive Cat2 Query Parameters",
                "Specify custom/debug query parameters to be appended to CAT2 /Reactive HTML requests. Value should start with an '&' Example: &flight = assistant & debug = true");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration",
                        "CustomReactiveQueryParams_Cat3", RegTypes.REG_SZ, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "System\\Assistant\\ClientConfiguration",
                        "CustomReactiveQueryParams_Cat3", RegTypes.REG_SZ, state);
                },
                "Reactive Cat3 Query Parameters",
                "Specify custom/debug query parameters to be appended to CAT3 /Reactive HTML requests. Value should start with an '&' Example: &flight = assistant & debug = true");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Start", "DropAreaDelta",
                        RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Start",
                        "DropAreaDelta",
                        RegTypes.REG_DWORD, state);
                },
                "PFI Drop Area Delta",
                "Value, stored as an int 0-100, that represents a percent (0.00-1.00) to use as drop area for folder creation on Start Enter Delta (0-100)");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "RippleTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "RippleTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover Ripple Timer Interval",
                "Specify the time interval(ms) for the Hover Ripple animation in Start. (Default: 1) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MarqueeTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MarqueeTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover Marquee Timer Interval",
                "Specify the time interval(ms) for the Hover Marquee Timer. (Default: 200) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "PopTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "PopTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover Pop Timer Interval",
                "Specify the time interval(ms) for the Hover Pop animation in Start. (Default: 5) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "GrowTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "GrowTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover Grow Timer Interval",
                "Specify the time interval(ms) for the Hover Grow animation in Start. (Default: 800) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "TileFlyoutIdleTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "TileFlyoutIdleTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover TileFlyout Idle Timer Interval",
                "Specify the time interval(ms) after hovering outside of subtiles to go idle in TileFlyout. (Default: 1200) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "TileFlyoutEndTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "TileFlyoutEndTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover TileFlyout End Timer Interval",
                "Specify the time interval(ms) after hovering stops to go idle in TileFlyout. (Default: 800) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "TileFlyoutSubtileFocusChangeTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype,
                        out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "TileFlyoutSubtileFocusChangeTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover TileFlyout Subtile Focus Change Timer Interval",
                "Specify the time interval(ms) to delay subtile focus changes in TileFlyout. (Default: 200) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "CancelTileFlyoutTimerInterval", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "CancelTileFlyoutTimerInterval", RegTypes.REG_DWORD, state);
                },
                "Hover Cancel TileFlyout Timer Interval",
                "Specify the time interval(ms) after hovering stops to cancel TileFlyout in Start. (Default: 2000) Enter time interval in milliseconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetMaxAltitudeThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetMaxAltitudeThreshold", RegTypes.REG_DWORD, state);
                },
                "Magnet Max Altitude Threshold",
                "The maximum altitude for the magnetic tilt effect. (Default: 1000) Enter altitude");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetMinAltitudeThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetMinAltitudeThreshold", RegTypes.REG_DWORD, state);
                },
                "Magnet Min Altitude Threshold",
                "The minimum altitude for the magnetic tilt effect. (Default: 5) Enter altitude");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetTiltThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetTiltThreshold", RegTypes.REG_DWORD, state);
                },
                "Magnet Tilt Threshold",
                "The radial distance from the centre of the tile(in pixels on a 480x800 screen) to start the tilt effect in magnetic tilt. (Default: 250) Enter distance");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetPopoutThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetPopoutThreshold", RegTypes.REG_DWORD, state);
                },
                "Magnet Popout Threshold",
                "The radial distance from the centre of the tile(in pixels on a 480x800 screen) to start the popout effect in magnetic tilt. (Default: 250) Enter distance");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetFadeThreshold", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetFadeThreshold", RegTypes.REG_DWORD, state);
                },
                "Magnet Fade Threshold",
                "The radial distance from the centre of the tile(in pixels on a 480x800 screen) to start the fade effect in magnetic tilt. (Default: 200) Enter distance");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetTiltPercent", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetTiltPercent", RegTypes.REG_DWORD, state);
                },
                "Magnet Tilt Percent",
                "The magnitude(as a percentage) of the tilt effect in magnetic tilt. (Default: 30) Enter magnitude");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetPopoutDistance", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetPopoutDistance", RegTypes.REG_DWORD, state);
                },
                "Magnet Popout Distance",
                "The distance of the popout effect in magnetic tilt. (Default: 80) Enter distance");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetFadePercent", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Shell\\Start",
                        "MagnetFadePercent", RegTypes.REG_DWORD, state);
                },
                "Magnet Fade Percent",
                "The magnitude(as a percentage) of the fade effect in magnetic tilt. (Default: 60) Enter magnitude");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Lock",
                        "PolicyIdleTimeout", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Shell\\Lock",
                        "PolicyIdleTimeout", RegTypes.REG_DWORD, state);
                },
                "Set Lock Screen Idle Timeout Policy",
                "This will set the policy value for the lock screen idle timeout. Enter time interval in seconds");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretCenterX_Percentage", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretCenterX_Percentage", RegTypes.REG_DWORD, state);
                },
                "Caret Stick X Coordinate",
                "Choose the caret stick location as a percentage offset relative to the left side of the keyboard. (Note: this offset will be evaluated relative to the right side of the keyboard for left handed users.) Enter the caret stick x location (0-100). Debug menu will not check the validity of the entered number.");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretCenterY_Percentage", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretCenterY_Percentage", RegTypes.REG_DWORD, state);
                },
                "Caret Stick Y Coordinate",
                "Choose the caret stick location as a percentage offset relative to the top of the keyboard. Enter the caret stick y location (0-100). Debug menu will not check the validity of the entered number.");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretInputWidth_Percentage", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretInputWidth_Percentage", RegTypes.REG_DWORD, state);
                },
                "Caret Stick Input Region Width",
                "Choose a caret input region width as a percentage relative of the overall keyboard width. Caret Stick Input Region Width (0-100). Debug menu will not check the validity of the entered number.");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretInputHeight_Percentage", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "CaretInputHeight_Percentage", RegTypes.REG_DWORD, state);
                },
                "Caret Stick Input Region Height",
                "Choose a caret input region height as a percentage relative of the overall keyboard height. Caret Stick Input Region Height (0-100). Debug menu will not check the validity of the entered number.");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "MinDistSqCaret", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "MinDistSqCaret", RegTypes.REG_DWORD, state);
                },
                "Required Distance Change",
                "Enter a minimum squared distance (mm^2) between the start position of a touch and the current touch location to allow movement of the cursor. Enter a minimum squared distance change (mm^2) to allow cursor movement. Debug menu will not check the validity of the entered number.");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "MinTimeCaret", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "MinTimeCaret", RegTypes.REG_DWORD, state);
                },
                "Minimum Time to Start",
                "Enter the minimum time (ms) to allow cursor movement. (Note: too little of time may affect flick gesture.) Enter the minimum time (ms) required to allow cursor movement. Debug menu will not check the validity of the entered number.");
            AddInputTweak(() =>
                {
                    GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "DisableTimeCaret", RegTypes.REG_DWORD, out RegTypes regtype, out string regvalue);
                    return regvalue;
                },
                state =>
                {
                    _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\FingerKB\\Options",
                        "DisableTimeCaret", RegTypes.REG_DWORD, state);
                },
                "Caret Disable Time",
                "Enter the amount of time (ms) after a keypress to disable caret input. Enter the amount of time (ms) for disabling the caret after a keypress. Debug menu will not check the validity of the entered number.");
        }
    }
}
