using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools
{
    public sealed partial class TitlebarControl : UserControl
    {
        private string AppTitle = (!string.IsNullOrEmpty(ApplicationView.GetForCurrentView().Title) ? ApplicationView.GetForCurrentView().Title + " - " : "") + Package.Current.DisplayName;

        public TitlebarControl()
        {
            this.InitializeComponent();
            WindowTitle.Text = AppTitle;
            Window.Current.SetTitleBar(this);
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Height = CoreApplication.GetCurrentView().TitleBar.Height;
            var margin = CustomTitleBar.Margin;
            margin.Right = CoreApplication.GetCurrentView().TitleBar.SystemOverlayRightInset;
            CustomTitleBar.Margin = margin;
            CoreApplication.GetCurrentView().TitleBar.IsVisibleChanged += TitleBar_IsVisibleChanged;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            Height = sender.Height;
            var margin = CustomTitleBar.Margin;
            margin.Right = sender.SystemOverlayRightInset;
            CustomTitleBar.Margin = margin;
        }

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        public AppViewBackButtonVisibility BackButtonVisibility {
            get => SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility;
            set
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = value;
                switch (SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility)
                {
                    case AppViewBackButtonVisibility.Visible:
                        BackButtonBg.Visibility = Visibility.Visible;
                        Arrow.Visibility = Visibility.Collapsed;
                        break;
                    case AppViewBackButtonVisibility.Collapsed:
                        BackButtonBg.Visibility = Visibility.Collapsed;
                        Arrow.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        public void UpdateTitle()
        {
            AppTitle = (!string.IsNullOrEmpty(ApplicationView.GetForCurrentView().Title) ? ApplicationView.GetForCurrentView().Title + " - " : "") + Package.Current.DisplayName;
            WindowTitle.Text = AppTitle;
        }

        public string Title
        {
            get => ApplicationView.GetForCurrentView().Title;
            set
            {
                ApplicationView.GetForCurrentView().Title = value;
                AppTitle = (!string.IsNullOrEmpty(ApplicationView.GetForCurrentView().Title) ? ApplicationView.GetForCurrentView().Title + " - " : "") + Package.Current.DisplayName;
                WindowTitle.Text = AppTitle;
            }
        }
    }
}
