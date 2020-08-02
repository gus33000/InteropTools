using System;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using InteropTools.Providers;
using Windows.ApplicationModel.Resources.Core;
using InteropTools.CorePages;
using Windows.UI.Xaml;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
	public sealed partial class AddKeyContentDialog : ContentDialog
	{
		private readonly IRegistryProvider helper;
		private RegHives hive = RegHives.HKEY_LOCAL_MACHINE;
		private string keylocation = "";
		private string keyname = "";

		public AddKeyContentDialog(RegHives hive, string keylocation, string keyname)
		{
			InitializeComponent();
			helper = App.MainRegistryHelper;
			this.hive = hive;
			this.keylocation = keylocation;
			this.keyname = keyname;
			KeyLocationPathInputBox.Text = keylocation;
			KeyNameInputBox.Text = keyname;

			switch (GetRegistryHiveName(this.hive).ToUpper())
			{
				case "HKEY_CURRENT_CONFIG":
					{
						HiveSelector.SelectedIndex = 0;
						break;
					}

				case "HKEY_CLASSES_ROOT":
					{
						HiveSelector.SelectedIndex = 1;
						break;
					}

				case "HKEY_CURRENT_USER":
					{
						HiveSelector.SelectedIndex = 2;
						break;
					}

				case "HKEY_CURRENT_USER_LOCAL_SETTINGS":
					{
						HiveSelector.SelectedIndex = 3;
						break;
					}

				case "HKEY_DYN_DATA":
				case "HKEY_DYNAMIC_DATA":
					{
						HiveSelector.SelectedIndex = 4;
						break;
					}

				case "HKEY_LOCAL_MACHINE":
					{
						HiveSelector.SelectedIndex = 5;
						break;
					}

				case "HKEY_PERFORMANCE_DATA":
					{
						HiveSelector.SelectedIndex = 6;
						break;
					}

				case "HKEY_USERS":
					{
						HiveSelector.SelectedIndex = 7;
						break;
					}
			}
		}

		private string GetRegistryHiveName(RegHives hive)
		{
			return Enum.GetName(typeof(RegHives), hive);
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			keyname = KeyNameInputBox.Text;
			keylocation = KeyLocationPathInputBox.Text;
			hive = GetSelectedHive();
			RunInThreadPool(async () =>
			{
				var status = await helper.AddKey(hive, keylocation + "\\" + keyname);

				if (status == HelperErrorCodes.FAILED)
				{
					RunInUIThread(() => { ShowKeyUnableToAddMessageBox(); });
				}
			});
		}

		private RegHives GetSelectedHive()
		{
			var hive = RegHives.HKEY_LOCAL_MACHINE;
			var selectedhiveindex = HiveSelector.SelectedIndex;

			switch (selectedhiveindex)
			{
				case 0:
					{
						hive = RegHives.HKEY_CURRENT_CONFIG;
						break;
					}

				case 1:
					{
						hive = RegHives.HKEY_CLASSES_ROOT;
						break;
					}

				case 2:
					{
						hive = RegHives.HKEY_CURRENT_USER;
						break;
					}

				case 3:
					{
						hive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS;
						break;
					}

				case 4:
					{
						hive = RegHives.HKEY_DYN_DATA;
						break;
					}

				case 5:
					{
						hive = RegHives.HKEY_LOCAL_MACHINE;
						break;
					}

				case 6:
					{
						hive = RegHives.HKEY_PERFORMANCE_DATA;
						break;
					}

				case 7:
					{
						hive = RegHives.HKEY_USERS;
						break;
					}
			}

			return hive;
		}

		private async void ShowKeyUnableToAddMessageBox()
		{
			await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
			  ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_add_the_specified_key__no_changes_to_the_phone_registry_were_made_", ResourceContext.GetForCurrentView()).ValueAsString,
			  ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
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