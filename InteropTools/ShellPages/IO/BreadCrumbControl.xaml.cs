using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.ShellPages.IO
{
    public sealed partial class BreadCrumbControl : UserControl
    {
        public class BreadCrumbItem
        {
            public string DisplayName { get; set; }
            public object ItemObject { get; set; }
        }

        public ObservableCollection<BreadCrumbItem> ItemsSource
        {
            get { return (ObservableCollection<BreadCrumbItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(ObservableCollection<BreadCrumbItem>),
            typeof(BreadCrumbControl),
            new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        private static double GetRealRenderedTextLength(string text)
        {
            var tb = new TextBlock { Text = text };
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            return tb.DesiredSize.Width;
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var breadcrumbs = (BreadCrumbControl)d;
            if (breadcrumbs.ItemsSource == null)
            {
                return;
            }

            var list = breadcrumbs.ItemsSource;

            var firstitem = list[0];

            if (list.Count - 1 != 0)
            {
                var lastitem = list[list.Count - 1];

                var firstsize = GetRealRenderedTextLength(firstitem.DisplayName);
                var lastsize = GetRealRenderedTextLength(lastitem.DisplayName);

                if (firstsize < 89)
                {
                    firstsize = 89;
                }

                if (lastsize < 89)
                {
                    lastsize = 89;
                }

                if (lastsize + firstsize + (2 * 89) > breadcrumbs.BreadCrumb.ActualWidth)
                {
                    var newlist = new List<BreadCrumbItem>();
                    newlist.Add(new BreadCrumbItem() { DisplayName = "...", ItemObject = true });
                    newlist.Add(lastitem);

                    var tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;
                    if ((tmplist != null) && (tmplist.Count == newlist.Count) && (tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1]))
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
                else
                {
                    var sizenow = lastsize + firstsize;
                    var newlist = new List<BreadCrumbItem>();
                    newlist.Add(firstitem);

                    var tmplist = new List<BreadCrumbItem>();

                    bool addplaceholder = false;

                    for (int i = list.Count - 2; i > 0; i--)
                    {
                        var length = GetRealRenderedTextLength(list[i].DisplayName);

                        if (length < 89)
                        {
                            length = 89;
                        }

                        sizenow += length;

                        if (sizenow + 89 + (2 * 89) > breadcrumbs.BreadCrumb.ActualWidth)
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
                        tmplist.Add(new BreadCrumbItem() { DisplayName = "...", ItemObject = true });
                    }

                    for (int i = tmplist.Count - 1; i >= 0; i--)
                    {
                        newlist.Add(tmplist[i]);
                    }

                    newlist.Add(lastitem);

                    var tmplist2 = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;
                    if ((tmplist2 != null) && (tmplist2.Count == newlist.Count) && (tmplist2[tmplist2.Count - 1] == newlist[newlist.Count - 1]))
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
            }
            else
            {
                var newlist = new List<BreadCrumbItem>();
                newlist.Add(firstitem);

                var tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;
                if ((tmplist != null) && (tmplist.Count == newlist.Count) && (tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1]))
                {
                    return;
                }

                breadcrumbs.BreadCrumb.ItemsSource = newlist;
            }
        }

        public BreadCrumbControl()
        {
            this.InitializeComponent();
        }

        private void BreadCrumb_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            var breadcrumbs = this;
            if (breadcrumbs.ItemsSource == null)
            {
                return;
            }

            var list = breadcrumbs.ItemsSource;

            var firstitem = list[0];

            if (list.Count - 1 != 0)
            {
                var lastitem = list[list.Count - 1];

                var firstsize = GetRealRenderedTextLength(firstitem.DisplayName);
                var lastsize = GetRealRenderedTextLength(lastitem.DisplayName);

                if (firstsize < 89)
                {
                    firstsize = 89;
                }

                if (lastsize < 89)
                {
                    lastsize = 89;
                }

                if (lastsize + firstsize + (2 * 89) > breadcrumbs.BreadCrumb.ActualWidth)
                {
                    var newlist = new List<BreadCrumbItem>();
                    newlist.Add(new BreadCrumbItem() { DisplayName = "...", ItemObject = true });
                    newlist.Add(lastitem);

                    var tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;
                    if ((tmplist != null) && (tmplist.Count == newlist.Count) && (tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1]))
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
                else
                {
                    var sizenow = lastsize + firstsize;
                    var newlist = new List<BreadCrumbItem>();
                    newlist.Add(firstitem);

                    var tmplist = new List<BreadCrumbItem>();

                    bool addplaceholder = false;

                    for (int i = list.Count - 2; i > 0; i--)
                    {
                        var length = GetRealRenderedTextLength(list[i].DisplayName);

                        if (length < 89)
                        {
                            length = 89;
                        }

                        sizenow += length;

                        if (sizenow + 89 + (2 * 89) > breadcrumbs.BreadCrumb.ActualWidth)
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
                        tmplist.Add(new BreadCrumbItem() { DisplayName = "...", ItemObject = true });
                    }

                    for (int i = tmplist.Count - 1; i >= 0; i--)
                    {
                        newlist.Add(tmplist[i]);
                    }

                    newlist.Add(lastitem);

                    var tmplist2 = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;
                    if ((tmplist2 != null) && (tmplist2.Count == newlist.Count) && (tmplist2[tmplist2.Count - 1] == newlist[newlist.Count - 1]))
                    {
                        return;
                    }

                    breadcrumbs.BreadCrumb.ItemsSource = newlist;
                }
            }
            else
            {
                var newlist = new List<BreadCrumbItem>();
                newlist.Add(firstitem);

                var tmplist = (List<BreadCrumbItem>)breadcrumbs.BreadCrumb.ItemsSource;
                if ((tmplist != null) && (tmplist.Count == newlist.Count) && (tmplist[tmplist.Count - 1] == newlist[newlist.Count - 1]))
                {
                    return;
                }

                breadcrumbs.BreadCrumb.ItemsSource = newlist;
            }
        }

        public delegate void ItemClickEvent(object sender, ItemClickEventArgs e);

        public event ItemClickEvent OnItemClick;

        private void UpdateItemClick(object item)
        {
            // Make sure someone is listening to event
            if (OnItemClick == null)
            {
                return;
            }

            var args = new ItemClickEventArgs(item);
            OnItemClick(this, args);
        }

        public class ItemClickEventArgs
        {
            public ItemClickEventArgs(object item)
            {
                this.ClickedItem = item;
            }

            public object ClickedItem { get; internal set; }
        }

        private void BreadCrumb_ItemClick(Object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if ((e.ClickedItem as BreadCrumbItem).ItemObject == null)
            {
                UpdateItemClick((e.ClickedItem as BreadCrumbItem).ItemObject);
            }
            else if ((e.ClickedItem as BreadCrumbItem).ItemObject.GetType() == typeof(bool))
            {
                // Display menu here
            }
            else
            {
                UpdateItemClick((e.ClickedItem as BreadCrumbItem).ItemObject);
            }
        }

        private void ContentBorder_Tapped(Object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var a = ((FrameworkElement)e.OriginalSource).DataContext;

            if (((a as BreadCrumbItem).ItemObject != null) && (a as BreadCrumbItem).ItemObject.GetType() == typeof(bool))
            {
                // Display menu here
                var list = BreadCrumb.Items;

                var index = list.IndexOf(a);

                var displaylist = new List<BreadCrumbItem>();

                if (index == 0)
                {
                    int counter = 0;
                    foreach (var item in ItemsSource)
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
                    var numbertoskip = list.Count - 2;

                    for (int i = 1; i <= ItemsSource.Count - 1 - numbertoskip; i++)
                    {
                        displaylist.Add(ItemsSource[i]);
                    }
                }

                ItemListView.ItemsSource = displaylist;
                FlyoutBase.ShowAttachedFlyout(BreadCrumb);
            }
        }

        private void TypeListView_ItemClick(Object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            ItemFlyout.Hide();
            UpdateItemClick((e.ClickedItem as BreadCrumbItem).ItemObject);
        }
    }
}