using InteropTools.Classes;
using InteropTools.Handlers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.Pages.Extras
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceInformationPage : Page
    {
        public DeviceInformationPage()
        {
            this.InitializeComponent();
            SizeChanged += DeviceInformationPage_SizeChanged;
            this.SetExtended(OSPivotGrid, false, false, true, true);
            this.SetShrinked(OSContent, false, false, true, true);
            this.SetExtended(HeaderBackground, false, true, true, false);

            var marg = HeaderBackground.Margin;
            marg.Left = ActualWidth - ((Window.Current.Content as Frame).Content as Shell).ActualWidth;
            HeaderBackground.Margin = marg;

            Loaded += DeviceInformationPage_Loaded;
        }

        private void DeviceInformationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DeviceInformationPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetExtended(OSPivotGrid, false, false, true, true);
            this.SetShrinked(OSContent, false, false, true, true);
            this.SetExtended(HeaderBackground, false, true, true, false);

            var marg = HeaderBackground.Margin;
            marg.Left = ActualWidth - ((Window.Current.Content as Frame).Content as Shell).ActualWidth;
            HeaderBackground.Margin = marg;
        }

        private void Refresh()
        {
            var deviceInfo = DeviceInfo.Instance;
            DeviceName.Text = deviceInfo.FriendlyName;
            HardwareId.Text = deviceInfo.HardwareId;
            UUID.Text = deviceInfo.UUID;
            OSName.Text = deviceInfo.OperatingSystem;
            FirmwareVersion.Text = deviceInfo.SystemFirmwareVersion;
            HardwareVersion.Text = deviceInfo.SystemHardwareVersion;
            Manufacturer.Text = deviceInfo.SystemManufacturer;
            Model.Text = deviceInfo.SystemProductName;
            Sku.Text = deviceInfo.SystemSku;
            SystemVersion.Text = deviceInfo.SystemVersion;
            DeviceForm.Text = deviceInfo.DeviceForm;
            DeviceFamily.Text = deviceInfo.DeviceFamily.Replace(".", " ");
            DeviceFamilyVersion.Text = deviceInfo.DeviceFamilyVersion;
            CollectionLevel.Text = deviceInfo.CollectionLevel;

            try
            {
                var v = ulong.Parse(DeviceFamilyVersion.Text);
                var v1 = (v & 0xFFFF000000000000L) >> 48;
                var v2 = (v & 0x0000FFFF00000000L) >> 32;
                var v3 = (v & 0x00000000FFFF0000L) >> 16;
                var v4 = v & 0x000000000000FFFFL;
                DeviceFamilyVersion.Text = $"{v1}.{v2}.{v3}.{v4}";
            }
            catch
            {
                
            }

            var helper = new RegistryHelper.CRegistryHelper();
            this.RunInThreadPool(() =>
            {
                RegistryHelper.REG_VALUE_TYPE outvaltype;
                string compiledate;
                string sourceosversion;
                string builder;
                string osproductpfn;
                string productsuite;
                string producttype;
                string componentizedbuild;
                string csdbuildnumber;
                string csdreleasetype;
                string csdversion;
                string productid;
                string buildbranch;
                string buildguid;
                string buildlab;
                string buildlabex;
                string currentbuildnumber;
                string major;
                string minor;
                string currenttype;
                string editionid;
                string installationtype;
                string productname;
                string releaseid;
                string systemroot;
                string ubr;
                
                string biosreleasedate;
                string biosversion;
                string systemmanufacturer;
                string systemproductname;

                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\Versions", "TimeStamp", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out compiledate);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\Setup\BuildUpdate", "SourceOSVersion", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out sourceosversion);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\Versions", "Builder", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out builder);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\ProductOptions", "OSProductPfn", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out osproductpfn);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\ProductOptions", "ProductSuite", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out productsuite);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\ProductOptions", "ProductType", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out producttype);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Windows", "ComponentizedBuild", RegistryHelper.REG_VALUE_TYPE.REG_DWORD, out outvaltype, out componentizedbuild);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Windows", "CSDBuildNumber", RegistryHelper.REG_VALUE_TYPE.REG_DWORD, out outvaltype, out csdbuildnumber);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Windows", "CSDReleaseType", RegistryHelper.REG_VALUE_TYPE.REG_DWORD, out outvaltype, out csdreleasetype);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Windows", "CSDVersion", RegistryHelper.REG_VALUE_TYPE.REG_DWORD, out outvaltype, out csdversion);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion\DefaultProductKey", "ProductId", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out productid);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "BuildBranch", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out buildbranch);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "BuildGUID", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out buildguid);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "BuildLab", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out buildlab);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "BuildLabEx", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out buildlabex);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out currentbuildnumber);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", RegistryHelper.REG_VALUE_TYPE.REG_DWORD, out outvaltype, out major);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber", RegistryHelper.REG_VALUE_TYPE.REG_DWORD, out outvaltype, out minor);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "CurrentType", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out currenttype);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "EditionID", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out editionid);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "InstallationType", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out installationtype);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "ProductName", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out productname);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "ReleaseId", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out releaseid);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "SystemRoot", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out systemroot);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"Software\Microsoft\Windows NT\CurrentVersion", "UBR", RegistryHelper.REG_VALUE_TYPE.REG_DWORD, out outvaltype, out ubr);

                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\SystemInformation", "BIOSReleaseDate", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out biosreleasedate);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\SystemInformation", "BIOSVersion", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out biosversion);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\SystemInformation", "SystemManufacturer", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out systemmanufacturer);
                helper.RegQueryValue(RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\SystemInformation", "SystemProductName", RegistryHelper.REG_VALUE_TYPE.REG_SZ, out outvaltype, out systemproductname);

                try
                {
                    var v = ulong.Parse(sourceosversion);
                    var v1 = (v & 0xFFFF000000000000L) >> 48;
                    var v2 = (v & 0x0000FFFF00000000L) >> 32;
                    var v3 = (v & 0x00000000FFFF0000L) >> 16;
                    var v4 = v & 0x000000000000FFFFL;
                    sourceosversion = $"{v1}.{v2}.{v3}.{v4}";
                }
                catch
                {
                    sourceosversion = "";
                }
                
                this.RunInUIThread(() =>
                {
                    SourceOSVersion.Text = sourceosversion;
                    Builder.Text = builder;
                    OSProductPfn.Text = osproductpfn;
                    ProductSuite.Text = productsuite;
                    ProductType.Text = productsuite;
                    ComponentizedBuild.Text = (componentizedbuild == "1").ToString();
                    CSDBuildNumber.Text = csdbuildnumber;
                    CSDReleaseType.Text = csdreleasetype;
                    CSDVersion.Text = csdversion;
                    ProductId.Text = productid;
                    BuildBranch.Text = buildbranch;
                    BuildGUID.Text = buildguid;
                    BuildLab.Text = buildlab;
                    BuildLabEx.Text = buildlabex;
                    CurrentBuildNumber.Text = currentbuildnumber;
                    CurrentMajorBuildNumber.Text = major;
                    CurrentMinorBuildNumber.Text = minor;
                    CurrentType.Text = currenttype;
                    EditionID.Text = editionid;
                    InstallationType.Text = installationtype;
                    ProductName.Text = productname;
                    ReleaseId.Text = releaseid;
                    SystemRoot.Text = systemroot;
                    UBR.Text = ubr;
                    OSbuild.Text = $"{major}.{minor}.{currentbuildnumber}.{ubr}";
                    CompileDate.Text = compiledate;

                    if (buildbranch.ToLower().Contains("th2"))
                        BackgroundHeroImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/th2.jpg"));
                    else if (buildbranch.ToLower().Contains("rs1"))
                        BackgroundHeroImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/rs.jpg"));
                    else if (buildbranch.ToLower().Contains("rs"))
                        BackgroundHeroImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/rs2.jpg"));
                    else if (buildbranch.ToLower().Contains("feature2"))
                        BackgroundHeroImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/f2.jpg"));

                    BIOSReleaseDate.Text = biosreleasedate;
                    BIOSVersion.Text = biosversion;
                    SystemManufacturer.Text = systemmanufacturer;
                    SystemProductName.Text = systemproductname;
                });
            });
        }
    }
}
