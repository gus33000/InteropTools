using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
    public sealed partial class MountHiveContentDialog : ContentDialog
    {
        public MountHiveContentDialog()
        {
            InitializeComponent();
        }

        private bool inUser;
        private string FilePath;

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await App.MainRegistryHelper.LoadHive(FilePath, NewName.Text, inUser);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public async Task MountHive(string FilePath, bool inUser)
        {
            this.inUser = inUser;
            this.FilePath = FilePath;
            await ShowAsync();
        }
    }
}
