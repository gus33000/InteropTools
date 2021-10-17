// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class BreadCrumbControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(ObservableCollection<BreadCrumbItem>),
            typeof(BreadCrumbControl),
            new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        public BreadCrumbControl() => InitializeComponent();

        public delegate void ItemClickEvent(object sender, ItemClickEventArgs e);

        public event System.EventHandler<ItemClickEventArgs> OnItemClick;

        public ObservableCollection<BreadCrumbItem> ItemsSource
        {
            get => (ObservableCollection<BreadCrumbItem>)GetValue(ItemsSourceProperty);

            set => SetValue(ItemsSourceProperty, value);
        }

        private static double GetRealRenderedTextLength(string text)
        {
            TextBlock tb = new() {Text = text};
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return tb.DesiredSize.Width;
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BreadCrumbControl breadcrumbs = (BreadCrumbControl)d;

            if (breadcrumbs.ItemsSource == null)
            {
                return;
            }

            ObservableCollection<BreadCrumbItem> list = breadcrumbs.ItemsSource;
            BreadCrumbItem firstitem = list[0];

            if (list.Count - 1 != 0)
            {
                BreadCrumbItem lastitem = list[list.Count - 1];
                double firstsize = GetRealRenderedTextLength(firstitem.DisplayName);
                double lastsize = GetRealRenderedTextLength(lastitem.DisplayName);

                if (firstsize < 89)
                {
                    firstsize = 89;
                }

                if (lastsize < 89)
                {
                    lastsize = 89;
                }

                if (lastsize + firstsize + 2 * 89 > breadcrumbs.BreadCrumb.ActualWidth)
                {
                    List<BreadCrumbItem> newlist = new()
                    {
                        new BreadCrumbItem() {DisplayName = "...", ItemObject = true}, lastitem
                    };
                    List<BreadCrumbItem> tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;

                    if (tmplist != null && tmplist.Count == newlist.Count &&
                        tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1])
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
                else
                {
                    double sizenow = lastsize + firstsize;
                    List<BreadCrumbItem> newlist = new() {firstitem};
                    List<BreadCrumbItem> tmplist = new();
                    bool addplaceholder = false;

                    for (int i = list.Count - 2; i > 0; i--)
                    {
                        double length = GetRealRenderedTextLength(list[i].DisplayName);

                        if (length < 89)
                        {
                            length = 89;
                        }

                        sizenow += length;

                        if (sizenow + 89 + 2 * 89 > breadcrumbs.BreadCrumb.ActualWidth)
                        {
                            if (i != 1)
                            {
                                addplaceholder = true;
                                break;
                            }
                        }

                        tmplist.Add(list[i]);
                    }

                    if (addplaceholder)
                    {
                        tmplist.Add(new BreadCrumbItem() {DisplayName = "...", ItemObject = true});
                    }

                    for (int i = tmplist.Count - 1; i >= 0; i--)
                    {
                        newlist.Add(tmplist[i]);
                    }

                    newlist.Add(lastitem);
                    List<BreadCrumbItem> tmplist2 = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;

                    if (tmplist2 != null && tmplist2.Count == newlist.Count &&
                        tmplist2[tmplist2.Count - 1] == newlist[newlist.Count - 1])
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
            }
            else
            {
                List<BreadCrumbItem> newlist = new() {firstitem};
                List<BreadCrumbItem> tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;

                if (tmplist != null && tmplist.Count == newlist.Count &&
                    tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1])
                {
                    return;
                }

                breadcrumbs.BreadCrumb.ItemsSource = newlist;
            }
        }

        private void BreadCrumb_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if ((e.ClickedItem as BreadCrumbItem)?.ItemObject == null)
            {
                UpdateItemClick((e.ClickedItem as BreadCrumbItem)?.ItemObject);
            }
            else if ((e.ClickedItem as BreadCrumbItem)?.ItemObject.GetType() == typeof(bool))
            {
                // Display menu here
            }
            else
            {
                UpdateItemClick((e.ClickedItem as BreadCrumbItem)?.ItemObject);
            }
        }

        private void BreadCrumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BreadCrumbControl breadcrumbs = this;

            if (breadcrumbs.ItemsSource == null)
            {
                return;
            }

            ObservableCollection<BreadCrumbItem> list = breadcrumbs.ItemsSource;
            BreadCrumbItem firstitem = list[0];

            if (list.Count - 1 != 0)
            {
                BreadCrumbItem lastitem = list[list.Count - 1];
                double firstsize = GetRealRenderedTextLength(firstitem.DisplayName);
                double lastsize = GetRealRenderedTextLength(lastitem.DisplayName);

                if (firstsize < 89)
                {
                    firstsize = 89;
                }

                if (lastsize < 89)
                {
                    lastsize = 89;
                }

                if (lastsize + firstsize + 2 * 89 > breadcrumbs.BreadCrumb.ActualWidth)
                {
                    List<BreadCrumbItem> newlist = new()
                    {
                        new BreadCrumbItem() {DisplayName = "...", ItemObject = true}, lastitem
                    };
                    List<BreadCrumbItem> tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;

                    if (tmplist != null && tmplist.Count == newlist.Count &&
                        tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1])
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
                else
                {
                    double sizenow = lastsize + firstsize;
                    List<BreadCrumbItem> newlist = new() {firstitem};
                    List<BreadCrumbItem> tmplist = new();
                    bool addplaceholder = false;

                    for (int i = list.Count - 2; i > 0; i--)
                    {
                        double length = GetRealRenderedTextLength(list[i].DisplayName);

                        if (length < 89)
                        {
                            length = 89;
                        }

                        sizenow += length;

                        if (sizenow + 89 + 2 * 89 > breadcrumbs.BreadCrumb.ActualWidth)
                        {
                            if (i != 1)
                            {
                                addplaceholder = true;
                                break;
                            }
                        }

                        tmplist.Add(list[i]);
                    }

                    if (addplaceholder)
                    {
                        tmplist.Add(new BreadCrumbItem() {DisplayName = "...", ItemObject = true});
                    }

                    for (int i = tmplist.Count - 1; i >= 0; i--)
                    {
                        newlist.Add(tmplist[i]);
                    }

                    newlist.Add(lastitem);
                    List<BreadCrumbItem> tmplist2 = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;

                    if (tmplist2 != null && tmplist2.Count == newlist.Count &&
                        tmplist2[tmplist2.Count - 1] == newlist[newlist.Count - 1])
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
            }
            else
            {
                List<BreadCrumbItem> newlist = new() {firstitem};
                List<BreadCrumbItem> tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;

                if (tmplist != null && tmplist.Count == newlist.Count &&
                    tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1])
                {
                    return;
                }

                breadcrumbs.BreadCrumb.ItemsSource = newlist;
            }
        }

        private void ContentBorder_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            object a = ((FrameworkElement)e.OriginalSource).DataContext;

            if ((a as BreadCrumbItem)?.ItemObject != null &&
                (a as BreadCrumbItem)?.ItemObject.GetType() == typeof(bool))
            {
                // Display menu here
                ItemCollection list = BreadCrumb.Items;
                int index = list.IndexOf(a);
                List<BreadCrumbItem> displaylist = new();

                if (index == 0)
                {
                    int counter = 0;

                    foreach (BreadCrumbItem item in ItemsSource)
                    {
                        if (counter == ItemsSource.Count - 1)
                        {
                            break;
                        }

                        displaylist.Add(item);
                        counter++;
                    }
                }
                else
                {
                    int numbertoskip = list.Count - 2;

                    for (int i = 1; i <= ItemsSource.Count - 1 - numbertoskip; i++)
                    {
                        displaylist.Add(ItemsSource[i]);
                    }
                }

                ItemListView.ItemsSource = displaylist;
                FlyoutBase.ShowAttachedFlyout(BreadCrumb);
            }
        }

        private void TypeListView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            ItemFlyout.Hide();
            UpdateItemClick((e.ClickedItem as BreadCrumbItem)?.ItemObject);
        }

        private void UpdateItemClick(object item)
        {
            // Make sure someone is listening to event
            if (OnItemClick == null)
            {
                return;
            }

            ItemClickEventArgs args = new(item);
            OnItemClick(this, args);
        }

        public class BreadCrumbItem
        {
            public string DisplayName { get; set; }
            public object ItemObject { get; set; }
        }

        public class ItemClickEventArgs
        {
            public ItemClickEventArgs(object item) => ClickedItem = item;

            public object ClickedItem { get; internal set; }
        }
    }
}
