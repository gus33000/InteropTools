using InteropTools.Classes;
using InteropTools.Presentation;
using InteropTools.Providers;
using InteropTools.RemoteClasses.Client;
//using InteropToolsRegistryApp.Providers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Core
{
    public sealed partial class PickProviderContentDialog : ContentDialog
    {
        private readonly ObservableCollection<ProviderItem> _itemsList = new ObservableCollection<ProviderItem>();

        bool doNotClose = true;
        private RemoteAuthClient _client;

        private class ProviderItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Symbol { get; set; }
            public IRegistryProvider Provider { get; set; }
            public bool AllowsRegistryEditing { get; set; }
            public bool IsLocal { get; set; }
        }

        void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (doNotClose)
            {
                args.Cancel = true;
            }
        }
        
        public PickProviderContentDialog()
        {
            this.InitializeComponent();

            //if (int.Parse(DeviceInfo.Instance.SystemVersion.Split('.')[2]) >= 14393)
            //    Connecting.Children.Add(new TileControl() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, ImageSource = new Uri("ms-appx:///Assets/LoadingScreen/LoadingLogo.png"), IsAnimated = true });

            Loaded += DialogLoaded;
            
            Closing += ContentDialog_Closing;

            //#if !STORE
            CNativeRegistryProvider nativeprov2 = new CNativeRegistryProvider();
            _itemsList.Add(new ProviderItem { Title = nativeprov2.GetTitle(), Description = nativeprov2.GetDescription(), Provider = nativeprov2, Symbol = nativeprov2.GetSymbol(), AllowsRegistryEditing = nativeprov2.AllowsRegistryEditing(), IsLocal = nativeprov2.IsLocal() });
            WarningText.Visibility = Visibility.Collapsed;
            //#endif

            if (Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.AppExtensions.AppExtensionCatalog", "Open"))
            {
                LegacyBridgeRegistryProvider nativeprov = new LegacyBridgeRegistryProvider();
                _itemsList.Add(new ProviderItem { Title = nativeprov.GetTitle(), Description = nativeprov.GetDescription(), Provider = nativeprov, Symbol = nativeprov.GetSymbol(), AllowsRegistryEditing = nativeprov.AllowsRegistryEditing(), IsLocal = nativeprov.IsLocal() });
            } else
            {
                WarningText.Visibility = Visibility.Visible;
            }

            var sampleprov = new CSampleRegistryProvider();
            _itemsList.Add(new ProviderItem
            {
                Title = sampleprov.GetTitle(),
                Description = sampleprov.GetDescription(),
                Provider = sampleprov,
                Symbol = sampleprov.GetSymbol(),
                AllowsRegistryEditing = sampleprov.AllowsRegistryEditing(),
                IsLocal = sampleprov.IsLocal()
            });

            _itemsList.Add(new ProviderItem
            {
                Title = ResourceManager.Current.MainResourceMap.GetValue("Resources/Remote_Device", ResourceContext.GetForCurrentView()).ValueAsString,
                Description =
                ResourceManager.Current.MainResourceMap.GetValue("Resources/Connects_to_a_remote_device_which_has_remote_access_enabled__Level_of_access_is_subject_to_the_remote_device", ResourceContext.GetForCurrentView()).ValueAsString,
                Symbol = "",
                Provider = null,
                AllowsRegistryEditing = true,
                IsLocal = false
            });

            MainComboBox.ItemsSource = _itemsList;
            MainComboBox.SelectedIndex = 0;
        }
        
        private bool requireAuthAtStartUp
        {
            get
            {
                var applicationData = ApplicationData.Current;
                var localSettings = applicationData.LocalSettings;

                if ((localSettings.Values["requireAuthAtStartUp"] == null) || (localSettings.Values["requireAuthAtStartUp"].GetType() != typeof(bool)))
                {
                    localSettings.Values["requireAuthAtStartUp"] = true;
                }

                return (bool)localSettings.Values["requireAuthAtStartUp"];
            }

            set
            {
                var applicationData = ApplicationData.Current;
                var localSettings = applicationData.LocalSettings;
                localSettings.Values["requireAuthAtStartUp"] = value;
            }
        }

        private async Task<bool> AskCreds()
        {
            bool authorized = false;

            // Do we have capability to provide credentials from the device
            if (await KeyCredentialManager.IsSupportedAsync())
            {
                // Get credentials for current user and app
                KeyCredentialRetrievalResult result = await KeyCredentialManager.OpenAsync("MyAppCredentials");

                if (result.Credential != null)
                {
                    KeyCredentialOperationResult signResult =
                      await
                      result.Credential.RequestSignAsync(CryptographicBuffer.ConvertStringToBinary("LoginAuth",
                                                         BinaryStringEncoding.Utf8));

                    if (signResult.Status == KeyCredentialStatus.Success)
                    {
                        authorized = true;
                    }
                }

                // No previous saved credentials found
                else
                {
                    KeyCredentialRetrievalResult creationResult =
                      await
                      KeyCredentialManager.RequestCreateAsync("MyAppCredentials",
                          KeyCredentialCreationOption.ReplaceExisting);

                    if (creationResult.Status == KeyCredentialStatus.Success)
                    {
                        authorized = true;
                    }
                }
            }

            else
            {
                authorized = true;
            }

            return authorized;
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            PrimaryButtonText = "Use the selected provider";
            MainGridView.Opacity = 1;
            await HFadeInMainMenu.BeginAsync();
            MainGridView.Visibility = Visibility.Visible;
            ConnectDetails.Visibility = Visibility.Collapsed;
            Connecting.Visibility = Visibility.Collapsed;
            MainGridView.Opacity = 0;
            await SFadeInMainMenu.BeginAsync();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                _remoteip = hostname.Text;
                _remoteport = int.Parse(portnumber.Text);
            }

            catch
            {
                MainGridView.Opacity = 1;
                await HFadeInMainMenu.BeginAsync();
                MainGridView.Visibility = Visibility.Collapsed;
                ConnectDetails.Visibility = Visibility.Visible;
                Connecting.Visibility = Visibility.Collapsed;
                MainGridView.Opacity = 0;
                await SFadeInMainMenu.BeginAsync();
                return;
            }

            MainGridView.Opacity = 1;
            await HFadeInMainMenu.BeginAsync();
            ConnectDetails.Visibility = Visibility.Collapsed;
            MainGridView.Visibility = Visibility.Collapsed;
            Connecting.Visibility = Visibility.Visible;
            StatusText.Text = "";
            MainGridView.Opacity = 0;
            await SFadeInMainMenu.BeginAsync();
            _client = new RemoteAuthClient(_remoteip, _remoteport);
            _client.OnAuthentificated += Client_OnAuthentificated;
            _client.OnNotAuthentificated += Client_OnNotAuthentificated;
            _client.OnError += Client_OnError;
            _client.OnConnected += Client_OnConnected;
            _client.Connect();
        }


        private void Client_OnConnected()
        {
            StatusText.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Waiting_for_permission_from_the_remote_server", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        private void Client_OnError(string message)
        {
            StatusText.Text = "Connection failed:\n\n" + message;
            GoBackCreds();
        }
        
        private void DialogLoaded(object sender, RoutedEventArgs e)
        {
            new SettingsViewModel();
        }

        private async void GoBackCreds()
        {
            await Task.Delay(3000);
            MainGridView.Opacity = 1;
            await HFadeInMainMenu.BeginAsync();
            MainGridView.Visibility = Visibility.Collapsed;
            ConnectDetails.Visibility = Visibility.Visible;
            Connecting.Visibility = Visibility.Collapsed;
            MainGridView.Opacity = 0;
            await SFadeInMainMenu.BeginAsync();
        }


        private void Client_OnNotAuthentificated()
        {
            StatusText.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Connection_refused", ResourceContext.GetForCurrentView()).ValueAsString;
            GoBackCreds();
        }
        
        private string _remoteip = "";
        private int _remoteport;

        private void Client_OnAuthentificated()
        {
            StatusText.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Connection_accepted", ResourceContext.GetForCurrentView()).ValueAsString;
            var Helper = new CRemoteRegistryProvider(_remoteip, _remoteport);
            App.RegistryHelper = Helper;
            doNotClose = false;
            Hide();
        }

        private void MainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ProviderItem)e.AddedItems[0];
            SelectedProviderDesc.Text = selectedItem.Description;
        }
        
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var selectedItem = (ProviderItem)MainComboBox.SelectedItem;
            var tmpHelper = selectedItem.Provider;

            if (tmpHelper == null)
            {
                PrimaryButtonText = "";
                MainGridView.Opacity = 1;
                await HFadeInMainMenu.BeginAsync();
                MainGridView.Visibility = Visibility.Collapsed;
                ConnectDetails.Visibility = Visibility.Visible;
                Connecting.Visibility = Visibility.Collapsed;
                MainGridView.Opacity = 0;
                await SFadeInMainMenu.BeginAsync();
            }

            else
            {
                if (requireAuthAtStartUp)
                {
                    var result = await AskCreds();

                    if (result)
                    {
                        App.RegistryHelper = tmpHelper;
                        doNotClose = false;
                        Hide();
                    }
                }

                else
                {
                    App.RegistryHelper = tmpHelper;
                    doNotClose = false;
                    Hide();
                }
            }
        }
    }
}
