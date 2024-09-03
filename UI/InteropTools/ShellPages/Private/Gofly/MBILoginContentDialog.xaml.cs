using PhotoshoppedUUPCLI;
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

namespace StoreProjectApp.ContentDialogs
{
    public sealed partial class MBILoginContentDialog : ContentDialog
    {
        public MBILoginContentDialog()
        {
            this.InitializeComponent();
        }

        bool done = false;

        public async Task<string> GetToken()
        {
            string ret = "";

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["username"] as string != null && localSettings.Values["pass"] as string != null)
                ret = await TokenUtils.GetTokenForMSA(localSettings.Values["username"] as string, localSettings.Values["pass"] as string);
            else
            {
                await this.ShowAsync();
                if (done)
                {
                    ret = await TokenUtils.GetTokenForMSA(Username.Text, Password.Password);
                    if (Remember.IsChecked.Value)
                    {
                        localSettings.Values["username"] = Username.Text;
                        localSettings.Values["pass"] = Password.Password;
                    }
                }
            }
            return ret;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            done = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
