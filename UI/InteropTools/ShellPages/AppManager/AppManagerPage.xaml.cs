using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using InteropTools.ContentDialogs.AppManager;
using Windows.ApplicationModel.Resources.Core;
using System.Globalization;
using InteropTools.Presentation;
using Windows.UI.Xaml.Data;
using System.Collections.Generic;
using InteropTools.CorePages;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.AppManager
{

	/*public class VolumeDisplayitem
	{

		public string _MountPoint = InteropTools.Resources.TextResources.ApplicationManager_MountPoint;
		public string _PackageStore = InteropTools.Resources.TextResources.ApplicationManager_PackageStore;
		public string _Name = InteropTools.Resources.TextResources.ApplicationManager_Name;
		public string _SystemVolume = InteropTools.Resources.TextResources.ApplicationManager_SystemVolume;
		public string _Offline = InteropTools.Resources.TextResources.ApplicationManager_Offline;
		public string _SupportsHardLinks = InteropTools.Resources.TextResources.ApplicationManager_SupportsHardLinks;

		public Visibility AllVisibility
		{
			get
			{
				return Volume == null ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public Visibility VolumeVisibility
		{
			get
			{
				return Volume != null ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public PackageVolume Volume
		{
			get;
			set;
		}
	}

	public class TypeDisplayitem
	{
		public string TypeName
		{
			get
			{
				return Type == null ? InteropTools.Resources.TextResources.ApplicationManager_AllTypes : Type.ToString();
			}
		}

		public PackageTypes? Type
		{
			get;
			set;
		}
	}*/

	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AppManagerPage
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

				if (DevMode.IsChecked != null && DevMode.IsChecked.Value)
				{
					depOptions = DeploymentOptions.DevelopmentMode;
				}

				else
					if (ForceApplicationShutdown.IsChecked != null && ForceApplicationShutdown.IsChecked.Value)
					{
						depOptions = DeploymentOptions.ForceApplicationShutdown;
					}

					else
						if (ForceTargetApplicationShutdown.IsChecked != null && ForceTargetApplicationShutdown.IsChecked.Value)
						{
							depOptions = DeploymentOptions.ForceTargetApplicationShutdown;
						}

						else
							if (InstallAllResources.IsChecked != null && InstallAllResources.IsChecked.Value)
							{
								depOptions = DeploymentOptions.InstallAllResources;
							}

							else
							{
								depOptions = DeploymentOptions.None;
							}

				var installTask = new PackageManager().AddPackageAsync(new Uri(MainPackagePath.Text), null, depOptions);
				await installTask.AsTask(new Progress<DeploymentProgress>(progress =>
				{
					InstallPackageProgress.Value = progress.percentage;
					InstallPackageStatus.Text = String.Format(InteropTools.Resources.TextResources.ApplicationManager_InstallInProgress, progress.percentage);
				}));

				switch (installTask.Status)
				{
					case AsyncStatus.Error:
						var deploymentResult = installTask.GetResults();
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
				InstallPackageStatus.Text = String.Format(InteropTools.Resources.TextResources.ApplicationManager_InstallError, "0x" + string.Format("{0:x}", caughtEx.HResult) + " " +
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
			var picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.ComputerFolder
			};
			picker.FileTypeFilter.Add(".appxbundle");
			picker.FileTypeFilter.Add(".appx");
			picker.FileTypeFilter.Add(".xap");
			var file = await picker.PickSingleFileAsync();

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

				if (DevMode.IsChecked != null && DevMode.IsChecked.Value)
				{
					depOptions = DeploymentOptions.DevelopmentMode;
				}

				else
					if (ForceApplicationShutdown.IsChecked != null && ForceApplicationShutdown.IsChecked.Value)
					{
						depOptions = DeploymentOptions.ForceApplicationShutdown;
					}

					else
						if (ForceTargetApplicationShutdown.IsChecked != null && ForceTargetApplicationShutdown.IsChecked.Value)
						{
							depOptions = DeploymentOptions.ForceTargetApplicationShutdown;
						}

						else
							if (InstallAllResources.IsChecked != null && InstallAllResources.IsChecked.Value)
							{
								depOptions = DeploymentOptions.InstallAllResources;
							}

							else
							{
								depOptions = DeploymentOptions.None;
							}

				var installTask = new PackageManager().RegisterPackageAsync(new Uri(MainUnpackedPackagePath.Text), null,
				    depOptions);
				await installTask.AsTask(new Progress<DeploymentProgress>(progress =>
				{
					RegisterPackageProgress.Value = progress.percentage;
					RegisterPackageStatus.Text = String.Format(InteropTools.Resources.TextResources.ApplicationManager_RegisterInProgress, progress.percentage);
				}));

				switch (installTask.Status)
				{
					case AsyncStatus.Error:
						var deploymentResult = installTask.GetResults();
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
				RegisterPackageStatus.Text = String.Format(InteropTools.Resources.TextResources.ApplicationManager_RegisterError, "0x" + string.Format("{0:x}", caughtEx.HResult) +
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
            var picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
			picker.FileTypeFilter.Add(".xml");
			var file = await picker.PickSingleFileAsync();

			if (file != null)
			{
				MainUnpackedPackagePath.Text = file.Path;
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
        /*
		private async void Refresh()
		{
			PackageListPanel.IsHitTestVisible = false;
			PackageListPanel.Opacity = 0.5;
			RunInThreadPool(async () => {
				try
				{
					_volumelist = new ObservableCollection<VolumeDisplayitem>();
					_typelist = new ObservableCollection<TypeDisplayitem>();
					_itemsList = new ObservableRangeCollection<Item>();
					_filteredItemsList = new ObservableRangeCollection<Item>();
					_itemsList.CollectionChanged += ItemsList_CollectionChanged;
					_filteredItemsList.CollectionChanged += _filteredItemsList_CollectionChanged;
					await RunInUiThread(() => {
						LoadingText.Text = "Fetching available system volumes...";
						LoadingStack.Visibility = Visibility.Visible;
					});
					var itemSource = AlphaKeyGroup<Item>.CreateGroups(_filteredItemsList, CultureInfo.InvariantCulture,
					                 s => s.DisplayName, true);
					await RunInUiThread(() => {
						((CollectionViewSource)Resources["AppsGroups"]).Source = itemSource;
					});
					var tmplist = new List<Item>();
					var vols = await new PackageManager().GetPackageVolumesAsync();
					_volumelist.Add(new VolumeDisplayitem());

					foreach (var vol in vols)
					{
						_volumelist.Add(new VolumeDisplayitem() { Volume = vol });
					}

					await RunInUiThread(() => {
						VolListView.ItemsSource = _volumelist;
						VolListView.SelectedIndex = 0;
					});
					await RunInUiThread(() => {
						LoadingText.Text = "Fetching available package types...";
						LoadingStack.Visibility = Visibility.Visible;
					});
					var pkgtypes = Enum.GetValues(typeof(PackageTypes)).Cast<PackageTypes>();
					_typelist.Add(new TypeDisplayitem());

					foreach (var type in pkgtypes)
					{
						_typelist.Add(new TypeDisplayitem() { Type = type });
					}

					await RunInUiThread(() => {
						TypeListView.ItemsSource = _typelist;
						TypeListView.SelectedIndex = 0;
					});
					await RunInUiThread(() => {
						LoadingText.Text = "Determining the number of packages present in the system...";
						LoadingStack.Visibility = Visibility.Visible;
					});
					int numofpkgs = 0;

					foreach (var vol in vols)
					{
						foreach (var type in pkgtypes)
						{
							var pkgs = vol.FindPackagesForUserWithPackageTypes("", type);
							numofpkgs += pkgs.Count();
						}
					}

					double count = 0;

					foreach (var vol in vols)
					{
						var applist = new ObservableRangeCollection<Package>();

						foreach (var type in pkgtypes)
						{
							var pkgs = vol.FindPackagesForUserWithPackageTypes("", type);

							foreach (var package in pkgs)
							{
								if (!_shouldContinue)
								{
									break;
								}

								count++;
								await RunInUiThread(() => {
									LoadingText.Text = String.Format("Fetching information for packages... ({0}%)", Math.Round(count / numofpkgs * 100, 0));
								});
								var arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchUnknown;

								switch (package.Id.Architecture)
								{
									case ProcessorArchitecture.Arm:
										{
											arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchARM;
											break;
										}

									case ProcessorArchitecture.Neutral:
										{
											arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchNeutral;
											break;
										}

									case ProcessorArchitecture.Unknown:
										{
											arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchUnknown;
											break;
										}

									case ProcessorArchitecture.X64:
										{
											arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchx64;
											break;
										}

									case ProcessorArchitecture.X86:
										{
											arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchx86;
											break;
										}
								}

								var displayname = package.Id.FamilyName;
								var description = arch + " " + package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
								                  package.Id.Version.Build + "." + package.Id.Version.Revision;
								dynamic logo = "";

								try
								{
									var appEntries = await package.GetAppListEntriesAsync();

									foreach (var appEntry in appEntries)
									{
										try
										{
											displayname = appEntry.DisplayInfo.DisplayName;
										}

										catch
										{
											// ignored
										}

										try
										{
											description = appEntry.DisplayInfo.Description + "\n" + arch + " " +
											              package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
											              package.Id.Version.Build + "." + package.Id.Version.Revision;
										}

										catch
										{
											// ignored
										}

										try
										{
											var logosize = new Size
											{
												Height = 56,
												Width = 56
											};
											var applogo = await appEntry.DisplayInfo.GetLogo(logosize).OpenReadAsync();
											await RunInUiThread(() => {
												var bitmapImage = new BitmapImage();
												bitmapImage.SetSource(applogo);
												logo = bitmapImage;
											});
										}

										catch
										{
											// ignored
										}

										break;
									}
								}

								catch
								{
									// ignored
								}

								if (string.IsNullOrEmpty(displayname.Trim()))
								{
									displayname = package.Id.FamilyName;
								}

								tmplist.Add(new Item
								{
									DisplayName = displayname,
									FullName = package.Id.FullName,
									Description = description,
									logo = logo,
									volume = vol,
									type = type
								});
							}
						}
					}

					await RunInUiThread(() => {
						_itemsList.AddRange(tmplist);
					});
				}

				catch (Exception caughtEx)
				{
					await RunInUiThread(async () => {
						await
						new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(String.Format(InteropTools.Resources.TextResources.ApplicationManager_PackageListError,
						    "0x" + string.Format("{0:x}", caughtEx.HResult) + " " +
						    caughtEx.Message + " " + caughtEx.StackTrace));
					});
				}

				finally
				{
					await RunInUiThread(() => {
						LoadingStack.Visibility = Visibility.Collapsed;
						PackageListPanel.IsHitTestVisible = true;
						PackageListPanel.Opacity = 1;
					});
				}
			});
		}
        */
		private async void BrowseUpdatePackageButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
			picker.FileTypeFilter.Add(".appxbundle");
			picker.FileTypeFilter.Add(".appx");
			picker.FileTypeFilter.Add(".xap");
			var file = await picker.PickSingleFileAsync();

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

				if (DevMode.IsChecked != null && DevMode.IsChecked.Value)
				{
					depOptions = DeploymentOptions.DevelopmentMode;
				}

				else
					if (ForceApplicationShutdown.IsChecked != null && ForceApplicationShutdown.IsChecked.Value)
					{
						depOptions = DeploymentOptions.ForceApplicationShutdown;
					}

					else
						if (ForceTargetApplicationShutdown.IsChecked != null && ForceTargetApplicationShutdown.IsChecked.Value)
						{
							depOptions = DeploymentOptions.ForceTargetApplicationShutdown;
						}

						else
							if (InstallAllResources.IsChecked != null && InstallAllResources.IsChecked.Value)
							{
								depOptions = DeploymentOptions.InstallAllResources;
							}

							else
							{
								depOptions = DeploymentOptions.None;
							}

				var installTask = new PackageManager().UpdatePackageAsync(new Uri(UpdatePackagePath.Text), null,
				    depOptions);
				await installTask.AsTask(new Progress<DeploymentProgress>(progress =>
				{
					UpdatePackageProgress.Value = progress.percentage;
					UpdatePackageStatus.Text = String.Format(InteropTools.Resources.TextResources.ApplicationManager_UpdateInProgress, progress.percentage);
				}));

				switch (installTask.Status)
				{
					case AsyncStatus.Error:
						var deploymentResult = installTask.GetResults();
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
				UpdatePackageStatus.Text = String.Format(InteropTools.Resources.TextResources.ApplicationManager_UpdateError, "0x" + string.Format("{0:x}", caughtEx.HResult) + " " + caughtEx.Message);
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