// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using static InteropTools.SessionManager;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Core
{
    internal sealed partial class SelectSessionContentDialog : ContentDialog
    {
        public SelectSessionContentDialog()
        {
            InitializeComponent();
            SessionList.ItemsSource = Sessions.Select(x => new DisplayItem(x));
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
            AddNewSession("");
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void SessionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Hide();
            SwitchSession(((DisplayItem)e.ClickedItem).session);
        }

        private async void Test()
        {
            //Capture the screen (this) and apply no transformation<>
            RenderTargetBitmap renderTargetBitmap = new();
            await renderTargetBitmap.RenderAsync(this, (int)ActualWidth, (int)ActualHeight);
        }

        public class DisplayItem
        {
            public DisplayItem(Session session) => this.session = session;

            public string Description => session.Helper.GetTitle() +
                                         ResourceManager.Current.MainResourceMap.GetValue("Resources/_connected_to_",
                                             ResourceContext.GetForCurrentView()).ValueAsString +
                                         session.Helper.GetHostName();

            public string DisplayName => session.Helper.GetFriendlyName() + " (" + session.CreationDate + ")";

            public object Preview
            {
                get
                {
                    if (Sessions[(int)CurrentSession] == session)
                    {
                        return null;
                    }

                    return session.Preview;
                }
            }

            public Visibility PreviewVisibility
            {
                get
                {
                    if (Sessions[(int)CurrentSession] == session)
                    {
                        return Visibility.Collapsed;
                    }

                    return Visibility.Visible;
                }
            }

            public Visibility SelectedVisibility
            {
                get
                {
                    if (Sessions[(int)CurrentSession] == session)
                    {
                        return Visibility.Visible;
                    }

                    return Visibility.Collapsed;
                }
            }

            public Session session { get; internal set; }
            public string Symbol => session.Helper.GetSymbol();
        }
    }
}
