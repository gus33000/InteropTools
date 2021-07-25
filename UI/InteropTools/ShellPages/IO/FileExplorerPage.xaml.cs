using InteropTools.ShellPages.Registry;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.IO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileExplorerPage : Page
    {
        public FileExplorerPage()
        {
            InitializeComponent();
            this.Loaded += RegistryBrowserPage_Loaded;
            Unloaded += RegistryBrowserPage_Unloaded;
        }

        private void RegistryBrowserPage_Loaded(Object sender, RoutedEventArgs e)
        {
            Breadcrumbbar.OnItemClick += Breadcrumbbar_OnItemClick;

            SystemNavigationManager.GetForCurrentView().BackRequested += RegistryBrowserPage_BackRequested;
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;
        }

        private void Breadcrumbbar_OnItemClick(Object sender, BreadCrumbControl.ItemClickEventArgs e)
        {

        }

        private void RegistryBrowserPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= RegistryBrowserPage_BackRequested;
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
        }

        private void RegistryBrowserPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
        }

        private void PathInput_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void KeyActionButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void JumpToButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void AddKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (BrowserCtrl._currentRegItem == null)
            {
                return;
            }
        }

        private void AddValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (BrowserCtrl._currentRegItem == null)
            {
                return;
            }
        }

        private static async void ShowKeyUnableToAddMessageBox()
        {
            await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
                Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources /We_couldn_t_add_the_specified_key__no_changes_to_the_phone_registry_were_made_", ResourceContext.GetForCurrentView()).ValueAsString, Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private static async void ShowKeyMessageBox(string s)
        {
            await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(s + "\nThe above path was copied to your clipboard", Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/Current_Key", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private static async void RunInUiThread(Action function)
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { function(); });
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }

        private void BrowserControl_OnCurrentItemChanged(object sender,
            BrowserControl.CurrentItemChangedEventArgs e)
        {
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            BrowserCtrl.SortByType = !BrowserCtrl.SortByType;
            BrowserCtrl.RefreshListView();
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
