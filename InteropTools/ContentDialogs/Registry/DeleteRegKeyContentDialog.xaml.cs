using InteropTools.Providers;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
    public sealed partial class DeleteRegKeyContentDialog : ContentDialog
    {
        private readonly IRegistryProvider helper;
        private readonly RegHives hive;
        private readonly string key;

        public DeleteRegKeyContentDialog(RegHives hive, string key)
        {
            InitializeComponent();
            helper = App.MainRegistryHelper;
            this.hive = hive;
            this.key = key;
            Title = ResourceManager.Current.MainResourceMap.GetValue("Resources/Do_you_really_want_to_delete_that_key_", ResourceContext.GetForCurrentView()).ValueAsString;
            Description.Text = "Anything under " + key + " will be deleted for ever and you won't be able to recover.";
            PrimaryButtonText = ResourceManager.Current.MainResourceMap.GetValue("Resources/Delete_the_key", ResourceContext.GetForCurrentView()).ValueAsString;
            SecondaryButtonText = ResourceManager.Current.MainResourceMap.GetValue("Resources/Keep_the_key", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        private static async void ShowKeyUnableToDeleteMessageBox()
        {
            await new Core.MessageDialogContentDialog().ShowMessageDialog(
              ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_delete_the_specified_key__no_changes_to_the_phone_registry_were_made_", ResourceContext.GetForCurrentView()).ValueAsString,
              ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (RecurseCheckBox.IsChecked == true)
            {
                HelperErrorCodes status = await helper.DeleteKey(hive, key, true);

                if (status == HelperErrorCodes.FAILED)
                {
                    ShowKeyUnableToDeleteMessageBox();
                }
            }
            else
            {
                HelperErrorCodes status = await helper.DeleteKey(hive, key, false);

                if (status == HelperErrorCodes.FAILED)
                {
                    ShowKeyUnableToDeleteMessageBox();
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}