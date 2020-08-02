using InteropTools.Handlers;
using InteropTools.Presentation;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private string Settings = App.local.Settings;
        private string Settings_UseAuthAtStartup = App.local.Settings_UseAuthAtStartup;
        private string Settings_UseTimestamps = App.local.Settings_UseTimestamps;
        private string Settings_Personalization = App.local.Settings_Personalization;
        private string Settings_SelectYourTheme = App.local.Settings_SelectYourTheme;
        private string Settings_MDL2Theme = App.local.Settings_MDL2Theme;
        private string Settings_UseSystemAccentColor = App.local.Settings_UseSystemAccentColor;
        private string Settings_UseDefaultPalette = App.local.Settings_UseDefaultPalette;
        private string Settings_UseCustomColor = App.local.Settings_UseCustomColor;
        private string Settings_ChooseColor = App.local.Settings_ChooseColor;
        private string Settings_ChooseCustomColor = App.local.Settings_ChooseCustomColor;
        private string Settings_About = App.local.Settings_About;
        public string Settings_VisitIW = App.local.Settings_VisitIW;
        public string Settings_Twitter = App.local.Settings_Twitter;
        private string Settings_Credits = App.local.Settings_Credits;

        public SettingsPage()
        {
            this.InitializeComponent();
            this.ViewModel = new SettingsViewModel();

            try
            {
                ColorPicker.Color = this.ViewModel.SelectedBrush.Color;
            }
            catch
            {

            }

            Refresh();
        }
        
        public SettingsViewModel ViewModel { get; }

        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (!ViewModel.Brushes.Contains(new SolidColorBrush(args.NewColor)))
            {
                if (ViewModel.Brushes.Count == 48)
                    ViewModel.Brushes.Add(new SolidColorBrush(args.NewColor));
                ViewModel.Brushes[48] = new SolidColorBrush(args.NewColor);
            }
            ViewModel.SelectedBrush = new SolidColorBrush(args.NewColor);
        }
        
        private void Refresh()
        {
            var handler = new VersionHandler();
            
            VersionText.Text = handler.BuildString;
            AppTitle.Text = Package.Current.DisplayName;
            VersionShortText.Text = handler.Version;
        }
    }
}
