using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using InteropTools.Providers;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;
using InteropTools.CorePages;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
	public sealed partial class RenameKeyContentDialog : ContentDialog
	{
		private readonly IRegistryProvider helper;
		private readonly RegHives hive = RegHives.HKEY_LOCAL_MACHINE;
		private readonly string key = "";

		public RenameKeyContentDialog(RegHives hive, string key)
		{
			InitializeComponent();
			helper = App.MainRegistryHelper;
			this.hive = hive;
			this.key = key;
			var currentkey = "";

			if (key.Contains("\\"))
			{
				currentkey = key.Split('\\').Last();
			}

			else
			{
				currentkey = key;
			}

			NewName.Text = currentkey;
		}
        
		private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			var result = await helper.RenameKey(hive, key, NewName.Text);
			RunInUIThread(() =>
			{
				switch (result)
				{
					case HelperErrorCodes.ACCESS_DENIED:
						{
							ShowAccessDeniedMessageBox();
							break;
						}

					case HelperErrorCodes.FAILED:
						{
							ShowFailedMessageBox();
							break;
						}
				}
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

		private async void ShowAccessDeniedMessageBox()
		{
			await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
			  ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_rename_the_specified_key_because_its_access_is_denied__no_changes_to_the_phone_registry_were_made",
			      ResourceContext.GetForCurrentView()).ValueAsString,
			  ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
		}

		private async void ShowFailedMessageBox()
		{
			await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
			  ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_rename_the_specified_key_due_to_an_unknown_error__no_changes_to_the_phone_registry_were_made",
			      ResourceContext.GetForCurrentView()).ValueAsString,
			  ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}