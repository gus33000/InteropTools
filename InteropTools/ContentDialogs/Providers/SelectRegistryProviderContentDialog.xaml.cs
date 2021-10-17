// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using InteropTools.Providers;
using Windows.UI.Xaml.Controls;
using static InteropTools.ContentDialogs.Providers.Viewmodel;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Providers
{
    public sealed partial class SelectRegistryProviderContentDialog : ContentDialog
    {
        private IRegProvider provider;

        public SelectRegistryProviderContentDialog()
        {
            InitializeComponent();
            Viewmodel dc = DataContext as Viewmodel;
            dc.RegPlugins.CollectionChanged += RegPlugins_CollectionChanged;
        }

        public async Task<IRegProvider> AskUserForProvider()
        {
            await ShowAsync();
            return provider;
        }

        private void Auto_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                extlist.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                IsPrimaryButtonEnabled = true;
            }
            catch
            {
            }
        }

        private void Auto_Unchecked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                extlist.Visibility = Windows.UI.Xaml.Visibility.Visible;
                IsPrimaryButtonEnabled = false;
            }
            catch
            {
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void extlist_ItemClick(object sender, ItemClickEventArgs e)
        {
            DisplayablePlugin plugin = e.ClickedItem as DisplayablePlugin;
            provider = new RegistryProvider(plugin.Plugin);
            Hide();
        }

        private void RegPlugins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Viewmodel dc = DataContext as Viewmodel;
            if (dc.RegPlugins.Count == 0)
            {
                NoneText.Visibility = Windows.UI.Xaml.Visibility.Visible;
                GetExtensions.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Auto.IsEnabled = false;
                Manual.IsEnabled = false;
                IsPrimaryButtonEnabled = false;
                IsSecondaryButtonEnabled = false;
            }
            else
            {
                NoneText.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                GetExtensions.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Auto.IsEnabled = true;
                Manual.IsEnabled = true;
                if (Auto.IsChecked ?? false)
                {
                    IsPrimaryButtonEnabled = true;
                }

                IsSecondaryButtonEnabled = true;
            }
        }
    }
}
