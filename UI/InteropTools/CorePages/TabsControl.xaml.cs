using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static InteropTools.App;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.CorePages
{
	internal sealed partial class TabsControl : UserControl
	{
		public TabsControl()
		{
			this.InitializeComponent();
			(Window.Current.Content as CoreFrame).OnCurrentContentChanged += TabsControl_OnCurrentContentChanged;
			TabsListView.ItemsSource = Sessions.Select(x => new DisplayItem(x));
			Sessions.CollectionChanged += Sessions_CollectionChanged;
			SizeChanged += TabsControl_SizeChanged;
		}

		private void TabsControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			TabsListView.MaxWidth = e.NewSize.Width - 2 * (32 + 4);
		}

		private void TabsControl_OnCurrentContentChanged(object sender)
		{
			TabsListView.ItemsSource = Sessions.Select(x => new DisplayItem(x));
		}

		private void Sessions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			TabsListView.ItemsSource = Sessions.Select(x => new DisplayItem(x));
		}

        public class DisplayItem
        {
            public DisplayItem(Session session)
            {
                this.session = session;
            }

            public string Symbol
            {
                get
                {
                    return session.Helper.GetSymbol();
                }
            }

            public string DisplayName
            {
                get
                {
                    return session.Helper.GetFriendlyName() + " (" + session.CreationDate + ")";
                }
            }

            public string Description
            {
                get
                {
                    return session.Helper.GetTitle() + ResourceManager.Current.MainResourceMap.GetValue("Resources/_connected_to_", ResourceContext.GetForCurrentView()).ValueAsString + session.Helper.GetHostName();
                }
            }

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

            public Session session { get; internal set; }
        }

        public bool IsOpen = false;

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

		private void CreateNewTab_Click(object sender, RoutedEventArgs e)
		{
			AddNewSession("");
		}


		public delegate void OpenStateChangedEvent(object sender);

		public event OpenStateChangedEvent OnOpenStateChanged;

		private void UpdateOpenStateChanged()
		{
			// Make sure someone is listening to event
			if (OnOpenStateChanged == null)
			{
				return;
			}

			OnOpenStateChanged(this);
		}

		private void TabsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			SwitchSession((e.ClickedItem as DisplayItem).session);
		}
	}
}
