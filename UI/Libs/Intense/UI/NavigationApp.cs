using Intense.Presentation;
using Intense.Resources;
using Intense.UI.Controls;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Intense.UI
{
    /// <summary>
    /// Provides the base implementation of a navigation app.
    /// </summary>
    public class NavigationApp
        : Application
    {
        /// <summary>
        /// Gets or sets the root of the navigation structure of the application.
        /// </summary>
        public NavigationItem NavigationStructure { get; set; }

        /// <summary>
        /// Occurs when the application is launched.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);


            if (!(Window.Current.Content is Frame frame))
            {
                frame = new Frame
                {

                    // assign navigation root style
                    Style = (Style)Resources["NavigationRootFrameStyle"]
                };

                frame.Navigated += OnFrameNavigated;
                frame.NavigationFailed += OnFrameNavigationFailed;

                // add frame commands to global resources
                Resources.Add("FrameCommands", new FrameCommands { Frame = frame });

                Window.Current.Content = frame;

                // listen for backbutton requests
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
                UpdateBackButtonVisibility();
            }

            if (frame.Content == null)
            {
                // navigate to the master page providing the navigation structure
                frame.Navigate(typeof(MasterNavigationPage), NavigationStructure);
            }
            Window.Current.Activate();
        }

        private void UpdateBackButtonVisibility()
        {
            AppViewBackButtonVisibility visibility = AppViewBackButtonVisibility.Collapsed;
            Frame frame = (Frame)Window.Current.Content;
            if (frame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame frame = (Frame)Window.Current.Content;
            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButtonVisibility();
        }

        private void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception(ResourceHelper.GetString("LoadPageFailed", e.SourcePageType.FullName));
        }
    }
}
