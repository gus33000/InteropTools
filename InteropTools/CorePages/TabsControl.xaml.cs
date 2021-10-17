// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static InteropTools.SessionManager;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.CorePages
{
    internal sealed partial class TabsControl : UserControl
    {
        public bool IsOpen;

        public TabsControl()
        {
            InitializeComponent();
            (Window.Current.Content as CoreFrame).OnCurrentContentChanged += TabsControl_OnCurrentContentChanged;
            TabsListView.ItemsSource = Sessions.Select(x => new DisplayItem(x));
            Sessions.CollectionChanged += Sessions_CollectionChanged;
            SizeChanged += TabsControl_SizeChanged;
        }

        public delegate void OpenStateChangedEvent(object sender);

        public event OpenStateChangedEvent OnOpenStateChanged;

        private void CreateNewTab_Click(object sender, RoutedEventArgs e) => AddNewSession("");

        private void Sessions_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            TabsListView.ItemsSource = Sessions.Select(x => new DisplayItem(x));

        private void TabsControl_OnCurrentContentChanged(object sender) =>
            TabsListView.ItemsSource = Sessions.Select(x => new DisplayItem(x));

        private void TabsControl_SizeChanged(object sender, SizeChangedEventArgs e) =>
            TabsListView.MaxWidth = e.NewSize.Width - 2 * (32 + 4);

        private void TabsListView_ItemClick(object sender, ItemClickEventArgs e) =>
            SwitchSession((e.ClickedItem as DisplayItem)?.session);

        private void UpdateOpenStateChanged()
        {
            // Make sure someone is listening to event
            if (OnOpenStateChanged == null)
            {
                return;
            }

            OnOpenStateChanged(this);
        }

        private void ViewPreviews_Click(object sender, RoutedEventArgs e)
        {
            if (TabsListView.Height != 222)
            {
                IsOpen = true;
                TabsListView.Height = 222;
            }
            else
            {
                IsOpen = false;
                TabsListView.Height = 32;
            }

            UpdateOpenStateChanged();
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
