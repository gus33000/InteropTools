using InteropTools.CorePages;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage.Pickers;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.AppManager
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppManagerPage : Page
    {
        public string PageName => "Applications";
        public PageGroup PageGroup => PageGroup.General;

        public string _DeploymentOptions = InteropTools.Resources.TextResources.ApplicationManager_DeploymentOptions;
        public string _DeploymentMode = InteropTools.Resources.TextResources.ApplicationManager_DeploymentMode;
        public string _DeploymentModeDesc1 = InteropTools.Resources.TextResources.ApplicationManager_DeploymentModeDesc1;
        public string _DeploymentModeDesc2 = InteropTools.Resources.TextResources.ApplicationManager_DeploymentModeDesc2;
        public string _DeploymentModeDesc3 = InteropTools.Resources.TextResources.ApplicationManager_DeploymentModeDesc3;

        public string _ForceApplicationShutdown = InteropTools.Resources.TextResources.ApplicationManager_ForceApplicationShutdown;
        public string _ForceApplicationShutdownDesc = InteropTools.Resources.TextResources.ApplicationManager_ForceApplicationShutdownDesc;

        public string _ForceTargetApplicationShutdown = InteropTools.Resources.TextResources.ApplicationManager_ForceTargetApplicationShutdown;
        public string _ForceTargetApplicationShutdownDesc = InteropTools.Resources.TextResources.ApplicationManager_ForceTargetApplicationShutdownDesc;

        public string _InstallAllResources = InteropTools.Resources.TextResources.ApplicationManager_InstallAllResources;
        public string _InstallAllResourcesDesc1 = InteropTools.Resources.TextResources.ApplicationManager_InstallAllResourcesDesc1;
        public string _InstallAllResourcesDesc2 = InteropTools.Resources.TextResources.ApplicationManager_InstallAllResourcesDesc2;
        public string _InstallAllResourcesDesc3 = InteropTools.Resources.TextResources.ApplicationManager_InstallAllResourcesDesc3;

        public string _Install = InteropTools.Resources.TextResources.ApplicationManager_Install;
        public string _InstallDesc = InteropTools.Resources.TextResources.ApplicationManager_InstallDesc;
        public string _InstallPlaceHolderPath = InteropTools.Resources.TextResources.ApplicationManager_InstallPlaceHolderPath;
        public string _InstallStatusReady = InteropTools.Resources.TextResources.ApplicationManager_InstallStatusReady;
        public string _InstallBrowseButton = InteropTools.Resources.TextResources.ApplicationManager_InstallBrowseButton;
        public string _InstallPrimaryButton = InteropTools.Resources.TextResources.ApplicationManager_InstallPrimaryButton;

        public string _Register = InteropTools.Resources.TextResources.ApplicationManager_Register;
        public string _RegisterDesc = InteropTools.Resources.TextResources.ApplicationManager_RegisterDesc;
        public string _RegisterPlaceHolderPath = InteropTools.Resources.TextResources.ApplicationManager_RegisterPlaceHolderPath;
        public string _RegisterStatusReady = InteropTools.Resources.TextResources.ApplicationManager_RegisterStatusReady;
        public string _RegisterBrowseButton = InteropTools.Resources.TextResources.ApplicationManager_RegisterBrowseButton;
        public string _RegisterPrimaryButton = InteropTools.Resources.TextResources.ApplicationManager_RegisterPrimaryButton;

        public string _Update = InteropTools.Resources.TextResources.ApplicationManager_Update;
        public string _UpdateDesc = InteropTools.Resources.TextResources.ApplicationManager_UpdateDesc;
        public string _UpdatePlaceHolderPath = InteropTools.Resources.TextResources.ApplicationManager_UpdatePlaceHolderPath;
        public string _UpdateStatusReady = InteropTools.Resources.TextResources.ApplicationManager_UpdateStatusReady;
        public string _UpdateBrowseButton = InteropTools.Resources.TextResources.ApplicationManager_UpdateBrowseButton;
        public string _UpdatePrimaryButton = InteropTools.Resources.TextResources.ApplicationManager_UpdatePrimaryButton;

        public string _PackageList = InteropTools.Resources.TextResources.ApplicationManager_PackageList;

        public string _None = InteropTools.Resources.TextResources.ApplicationManager_None;
        public string _NoneDesc = InteropTools.Resources.TextResources.ApplicationManager_NoneDesc;

        public AppManagerPage()
        {
            InitializeComponent();
        }

        private async void InstallButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                BrowseMainPackageButton.IsEnabled = false;
                InstallButton.IsEnabled = false;
                MainPackagePath.IsEnabled = false;
                InstallPackageProgress.Value = 0;
                DeploymentOptions depOptions;

                if (DevMode.IsChecked == true)
                {
                    depOptions = DeploymentOptions.DevelopmentMode;
                }
                else
                    if (ForceApplicationShutdown.IsChecked == true)
                {
                    depOptions = DeploymentOptions.ForceApplicationShutdown;
                }
                else
                        if (ForceTargetApplicationShutdown.IsChecked == true)
                {
                    depOptions = DeploymentOptions.ForceTargetApplicationShutdown;
                }
                else
                            if (InstallAllResources.IsChecked == true)
                {
                    depOptions = DeploymentOptions.InstallAllResources;
                }
                else
                {
                    depOptions = DeploymentOptions.None;
                }

                IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installTask = new PackageManager().AddPackageAsync(new Uri(MainPackagePath.Text), null, depOptions);
                await installTask.AsTask(new Progress<DeploymentProgress>(progress =>
                {
                    InstallPackageProgress.Value = progress.percentage;
                    InstallPackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_InstallInProgress, progress.percentage);
                }));

                switch (installTask.Status)
                {
                    case AsyncStatus.Error:
                        DeploymentResult deploymentResult = installTask.GetResults();
                        InstallPackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_InstallError, deploymentResult.ErrorText);
                        break;

                    case AsyncStatus.Canceled:
                        InstallPackageStatus.Text = InteropTools.Resources.TextResources.ApplicationManager_InstallCanceled;
                        break;

                    case AsyncStatus.Completed:
                        InstallPackageStatus.Text = InteropTools.Resources.TextResources.ApplicationManager_InstallSucceeded;
                        break;
                }
            }
            catch (Exception caughtEx)
            {
                InstallPackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_InstallError, "0x" + string.Format("{0:x}", caughtEx.HResult) + " " +
                                            caughtEx.Message);
            }
            finally
            {
                BrowseMainPackageButton.IsEnabled = true;
                InstallButton.IsEnabled = true;
                MainPackagePath.IsEnabled = true;
            }
        }

        private async void BrowseMainPackageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker picker = new()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(".appxbundle");
            picker.FileTypeFilter.Add(".appx");
            picker.FileTypeFilter.Add(".xap");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                MainPackagePath.Text = file.Path;
            }
        }

        private async void RegisterUnpackedButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                BrowseMainUnpackedPackageButton.IsEnabled = false;
                RegisterUnpackedButton.IsEnabled = false;
                MainUnpackedPackagePath.IsEnabled = false;
                RegisterPackageProgress.Value = 0;
                DeploymentOptions depOptions;

                if (DevMode.IsChecked == true)
                {
                    depOptions = DeploymentOptions.DevelopmentMode;
                }
                else
                    if (ForceApplicationShutdown.IsChecked == true)
                {
                    depOptions = DeploymentOptions.ForceApplicationShutdown;
                }
                else
                        if (ForceTargetApplicationShutdown.IsChecked == true)
                {
                    depOptions = DeploymentOptions.ForceTargetApplicationShutdown;
                }
                else
                            if (InstallAllResources.IsChecked == true)
                {
                    depOptions = DeploymentOptions.InstallAllResources;
                }
                else
                {
                    depOptions = DeploymentOptions.None;
                }

                IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installTask = new PackageManager().RegisterPackageAsync(new Uri(MainUnpackedPackagePath.Text), null,
                    depOptions);
                await installTask.AsTask(new Progress<DeploymentProgress>(progress =>
                {
                    RegisterPackageProgress.Value = progress.percentage;
                    RegisterPackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_RegisterInProgress, progress.percentage);
                }));

                switch (installTask.Status)
                {
                    case AsyncStatus.Error:
                        DeploymentResult deploymentResult = installTask.GetResults();
                        RegisterPackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_RegisterError, deploymentResult.ErrorText);
                        break;

                    case AsyncStatus.Canceled:
                        RegisterPackageStatus.Text = InteropTools.Resources.TextResources.ApplicationManager_RegisterCanceled;
                        break;

                    case AsyncStatus.Completed:
                        RegisterPackageStatus.Text = InteropTools.Resources.TextResources.ApplicationManager_RegisterSucceeded;
                        break;
                }
            }
            catch (Exception caughtEx)
            {
                RegisterPackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_RegisterError, "0x" + string.Format("{0:x}", caughtEx.HResult) +
                                             " " + caughtEx.Message);
            }
            finally
            {
                BrowseMainUnpackedPackageButton.IsEnabled = true;
                RegisterUnpackedButton.IsEnabled = true;
                MainUnpackedPackagePath.IsEnabled = true;
            }
        }

        private async void BrowseMainUnpackedPackageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker picker = new()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(".xml");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                MainUnpackedPackagePath.Text = file.Path;
            }
        }

        private static async Task RunInUiThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async void BrowseUpdatePackageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker picker = new()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(".appxbundle");
            picker.FileTypeFilter.Add(".appx");
            picker.FileTypeFilter.Add(".xap");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                UpdatePackagePath.Text = file.Path;
            }
        }

        private async void UpdateButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                BrowseUpdatePackageButton.IsEnabled = false;
                UpdateButton.IsEnabled = false;
                UpdatePackagePath.IsEnabled = false;
                UpdatePackageProgress.Value = 0;
                DeploymentOptions depOptions;

                if (DevMode.IsChecked == true)
                {
                    depOptions = DeploymentOptions.DevelopmentMode;
                }
                else
                    if (ForceApplicationShutdown.IsChecked == true)
                {
                    depOptions = DeploymentOptions.ForceApplicationShutdown;
                }
                else
                        if (ForceTargetApplicationShutdown.IsChecked == true)
                {
                    depOptions = DeploymentOptions.ForceTargetApplicationShutdown;
                }
                else
                            if (InstallAllResources.IsChecked == true)
                {
                    depOptions = DeploymentOptions.InstallAllResources;
                }
                else
                {
                    depOptions = DeploymentOptions.None;
                }

                IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installTask = new PackageManager().UpdatePackageAsync(new Uri(UpdatePackagePath.Text), null,
                    depOptions);
                await installTask.AsTask(new Progress<DeploymentProgress>(progress =>
                {
                    UpdatePackageProgress.Value = progress.percentage;
                    UpdatePackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_UpdateInProgress, progress.percentage);
                }));

                switch (installTask.Status)
                {
                    case AsyncStatus.Error:
                        DeploymentResult deploymentResult = installTask.GetResults();
                        UpdatePackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_UpdateError, deploymentResult.ErrorText);
                        break;

                    case AsyncStatus.Canceled:
                        UpdatePackageStatus.Text = InteropTools.Resources.TextResources.ApplicationManager_UpdateCanceled;
                        break;

                    case AsyncStatus.Completed:
                        UpdatePackageStatus.Text = InteropTools.Resources.TextResources.ApplicationManager_UpdateSucceeded;
                        break;
                }
            }
            catch (Exception caughtEx)
            {
                UpdatePackageStatus.Text = string.Format(InteropTools.Resources.TextResources.ApplicationManager_UpdateError, "0x" + string.Format("{0:x}", caughtEx.HResult) + " " + caughtEx.Message);
            }
            finally
            {
                BrowseUpdatePackageButton.IsEnabled = true;
                UpdateButton.IsEnabled = true;
                UpdatePackagePath.IsEnabled = true;
            }
        }
    }
}