using Intense.Presentation;
using InteropTools.Classes;
using InteropTools.CorePages;
using System;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Shell = InteropTools.CorePages.Shell;

namespace InteropTools.ShellPages.Core
{
    public sealed partial class WelcomePage : Page
    {
        public string PageName => "Welcome";
        public PageGroup PageGroup => PageGroup.Core;

        public WelcomePage()
        {
            InitializeComponent();
            Refresh();

            /*if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                this.FeedbackButtonList.Visibility = Visibility.Visible;
                this.FeedbackButtonList2.Visibility = Visibility.Visible;
            }*/

            Loaded += WelcomePage_Loaded;
            SizeChanged += WelcomePage_SizeChanged;

            OSVersion.Text = DeviceInfo.Instance.SystemVersion;
            DeviceFamily.Text = DeviceInfo.Instance.DeviceFamily.Replace(".", " ");
            AppVersion.Text = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build + "." + Package.Current.Id.Version.Revision;
        }

        private void WelcomePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Margin = new Thickness(0);
        }

        private void WelcomePage_Loaded(object sender, RoutedEventArgs e)
        {
            Margin = new Thickness(0);
        }

        private void Refresh()
        {
            Shell shell = (Shell)App.AppContent;
            Presentation.ShellViewModel vm = shell.ViewModel;
            GridView2.ItemsSource = ((Shell)App.AppContent).recentitems5max;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Type page = ((NavigationItem)e.ClickedItem).PageType;
            Shell shell = (Shell)App.AppContent;
            shell.RootFrame.Navigate(page);
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            NavigationItem item = (NavigationItem)((StackPanel)sender).DataContext;
            CreateTile(item);
        }

        private async void CreateTile(NavigationItem item)
        {
            string page = item.PageType.Name;
            /*var square150X150Logo = new Uri("ms-appx:///Assets/SecondaryTiles/" + page + "/150x150Logo.scale-400.png");
			var wide310X150Logo = new Uri("ms-appx:///Assets/SecondaryTiles/" + page + "/310X150Logo.scale-400.png");
			var square310X310Logo = new Uri("ms-appx:///Assets/SecondaryTiles/" + page + "/310x310Logo.scale-400.png");
			var square30X30Logo = new Uri("ms-appx:///Assets/SecondaryTiles/" + page + "/44x44Logo.scale-400.png");*/
            string tileActivationArguments = page;
            SecondaryTile secondaryTile = new SecondaryTile(page,
                                                  item.DisplayName,
                                                  tileActivationArguments,
                                                  null,
                                                  //square150X150Logo,
                                                  TileSize.Square150x150);
            /*secondaryTile.VisualElements.Wide310x150Logo = wide310X150Logo;
			secondaryTile.VisualElements.Square310x310Logo = square310X310Logo;
			secondaryTile.VisualElements.Square30x30Logo = square30X30Logo;*/
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
            secondaryTile.RoamingEnabled = false;
            await secondaryTile.RequestCreateAsync();
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            NavigationItem item = (NavigationItem)((Grid)sender).DataContext;
            CreateTile(item);
        }

        public class Item
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Symbol { get; set; }
            public Type Page { get; set; }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (GridView2.ItemsSource == ((Shell)App.AppContent).recentitems5max)
            {
                GridView2.ItemsSource = ((Shell)App.AppContent).recentitems;
                ViewMoreRecent.Content = "See less items";
            }

            else
            {
                GridView2.ItemsSource = ((Shell)App.AppContent).recentitems5max;
                ViewMoreRecent.Content = "See more items";
            }
        }

        private async void FeedbackButtonList_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await
            Launcher.LaunchUriAsync(
              new Uri(
                "http://forum.xda-developers.com/windows-10-mobile/windows-10-mobile-apps-and-games/app-interop-tools-versatile-registry-t3445271"));
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://insidewindows.net/category/interop-tools/"));
        }

        private async void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://insidewindows.net/category/interop-tools/"));
        }
    }
}