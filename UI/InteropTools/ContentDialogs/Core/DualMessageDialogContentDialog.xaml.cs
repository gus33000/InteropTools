using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Core
{
    public sealed partial class DualMessageDialogContentDialog : ContentDialog
    {
        private bool Reply;

        public DualMessageDialogContentDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Reply = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Reply = false;
        }

        public async Task<bool> ShowDualMessageDialog(string Title, string Description, string PrimaryButtonText = "Yes", string SecondaryButtonText = "No")
        {
            this.Title = Title;
            this.Description.Text = Description;
            this.PrimaryButtonText = PrimaryButtonText;
            this.SecondaryButtonText = SecondaryButtonText;
            await ShowAsync();
            return Reply;
        }
    }
}
