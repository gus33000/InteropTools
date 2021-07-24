using InteropTools.Providers;
using System;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class YourWindowsBuildPage
    {
        private readonly IRegistryProvider helper;

        public YourWindowsBuildPage()
        {
            InitializeComponent();
            helper = App.MainRegistryHelper;
            Refresh();
        }

        private void Refresh()
        {
            RunInThreadPool(async () =>
            {
                string compiledate;
                GetKeyValueReturn ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\Versions",
                  "TimeStamp",
                  RegTypes.REG_SZ
                );
                compiledate = ret.regvalue;
                string sourceosversion;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\Setup\BuildUpdate",
                  "SourceOSVersion",
                  RegTypes.REG_SZ
                );
                sourceosversion = ret.regvalue;

                try
                {
                    ulong v = ulong.Parse(sourceosversion);
                    ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                    ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                    ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                    ulong v4 = v & 0x000000000000FFFFL;
                    sourceosversion = $"{v1}.{v2}.{v3}.{v4}";
                }

                catch
                {
                    sourceosversion = "";
                }

                string builder;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\Versions",
                  "Builder",
                  RegTypes.REG_SZ
                );
                builder = ret.regvalue;
                string osproductpfn;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\CurrentControlSet\Control\ProductOptions",
                  "OSProductPfn",
                  RegTypes.REG_SZ
                );
                osproductpfn = ret.regvalue;
                string productsuite;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\CurrentControlSet\Control\ProductOptions",
                  "ProductSuite",
                  RegTypes.REG_MULTI_SZ
                );
                productsuite = ret.regvalue;
                string producttype;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\CurrentControlSet\Control\ProductOptions",
                  "ProductType",
                  RegTypes.REG_SZ
                );
                producttype = ret.regvalue;
                string componentizedbuild;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\CurrentControlSet\Control\Windows",
                  "ComponentizedBuild",
                  RegTypes.REG_DWORD
                );
                componentizedbuild = ret.regvalue;
                string csdbuildnumber;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\CurrentControlSet\Control\Windows",
                  "CSDBuildNumber",
                  RegTypes.REG_DWORD
                );
                csdbuildnumber = ret.regvalue;
                string csdreleasetype;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\CurrentControlSet\Control\Windows",
                  "CSDReleaseType",
                  RegTypes.REG_DWORD
                );
                csdreleasetype = ret.regvalue;
                string csdversion;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SYSTEM\CurrentControlSet\Control\Windows",
                  "CSDVersion",
                  RegTypes.REG_DWORD
                );
                csdversion = ret.regvalue;
                string productid;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\DefaultProductKey",
                  "ProductId",
                  RegTypes.REG_SZ
                );
                productid = ret.regvalue;
                string buildbranch;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "BuildBranch",
                  RegTypes.REG_SZ
                );
                buildbranch = ret.regvalue;
                string buildguid;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "BuildGUID",
                  RegTypes.REG_SZ
                );
                buildguid = ret.regvalue;
                string buildlab;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "BuildLab",
                  RegTypes.REG_SZ
                );
                buildlab = ret.regvalue;
                string buildlabex;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "BuildLabEx",
                  RegTypes.REG_SZ
                );
                buildlabex = ret.regvalue;
                string currentbuildnumber;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "CurrentBuildNumber",
                  RegTypes.REG_SZ
                );
                currentbuildnumber = ret.regvalue;
                string major;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  "Software\\Microsoft\\Windows NT\\CurrentVersion",
                  "CurrentMajorVersionNumber",
                  RegTypes.REG_DWORD
                );
                major = ret.regvalue;
                string minor;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  "Software\\Microsoft\\Windows NT\\CurrentVersion",
                  "CurrentMinorVersionNumber",
                  RegTypes.REG_DWORD
                );
                minor = ret.regvalue;
                string currenttype;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "CurrentType",
                  RegTypes.REG_SZ
                );
                currenttype = ret.regvalue;
                string editionid;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "EditionID",
                  RegTypes.REG_SZ
                );
                editionid = ret.regvalue;
                string installationtype;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "InstallationType",
                  RegTypes.REG_SZ
                );
                installationtype = ret.regvalue;
                string productname;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "ProductName",
                  RegTypes.REG_SZ
                );
                productname = ret.regvalue;
                string releaseid;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "ReleaseId",
                  RegTypes.REG_SZ
                );
                releaseid = ret.regvalue;
                string systemroot;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "SystemRoot",
                  RegTypes.REG_SZ
                );
                systemroot = ret.regvalue;
                string ubr;
                ret = await helper.GetKeyValue(
                  RegHives.HKEY_LOCAL_MACHINE,
                  @"Software\Microsoft\Windows NT\CurrentVersion",
                  "UBR",
                  RegTypes.REG_DWORD
                );
                ubr = ret.regvalue;
                RunInUIThread(() =>
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
                    {
                        BackgroundHeroImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/th2.jpg"));
                    }

                    else
                        if (buildbranch.ToLower().Contains("rs1"))
                    {
                        BackgroundHeroImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/rs.jpg"));
                    }

                    else
                            if (buildbranch.ToLower().Contains("rs"))
                    {
                        BackgroundHeroImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/rs2.jpg"));
                    }
                });
            });
        }

        private async void RunInUIThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => { function(); });
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }
    }
}