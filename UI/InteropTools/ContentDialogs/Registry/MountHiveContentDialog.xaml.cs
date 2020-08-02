using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
    public sealed partial class MountHiveContentDialog : ContentDialog
    {
        public MountHiveContentDialog()
        {
            this.InitializeComponent();
        }

        bool inUser;
        string FilePath;

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
