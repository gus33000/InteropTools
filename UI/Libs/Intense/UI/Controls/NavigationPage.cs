using Intense.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Intense.UI.Controls
{
    /// <summary>
    /// Represents the base implementation of a page featuring a list of navigation items.
    /// </summary>
    public abstract class NavigationPage
        : Page
    {
        /// <summary>
        /// Identifies the wide window state.
        /// </summary>
        public const string WindowStateWide = "WideState";
        /// <summary>
        /// Identifies the narrow window state.
        /// </summary>
        public const string WindowStateNarrow = "NarrowState";

        /// <summary>
        /// Identifies the NavigationItem dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationItemProperty = DependencyProperty.Register("NavigationItem", typeof(NavigationItem), typeof(NavigationPage), new PropertyMetadata(null, OnNavigationItemChanged));
        /// <summary>
        /// Identifies the RootItem dependency property.
        /// </summary>
        public static readonly DependencyProperty RootItemProperty = DependencyProperty.Register("RootItem", typeof(NavigationItem), typeof(NavigationPage), null);
        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(NavigationItem), typeof(NavigationPage), new PropertyMetadata(null, OnSelectedItemChanged));
        /// <summary>
        /// Identifies the WindowState dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowStateProperty = DependencyProperty.Register("WindowState", typeof(string), typeof(NavigationPage), new PropertyMetadata(null, OnWindowStateChanged));

        private static void OnNavigationItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var page = (NavigationPage)o;

            // delay invoke changed handler to ensure any dependent bindings are finalized

            // example of failure if not delay invoked in MasterDetailPage
            //  1) ListView.ItemsSource bound to NavigationItem.Items
            //  2) NavigationItem is updated
            //  3) in OnNavigationItemChanged SelectedItem is set
            //  4) ListView.ItemsSource binding is not finalized, Items is still empty
            //  5) two way SelectedItem binding result in SelectedItem to be result to null

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Delay(10).ContinueWith(t => {
                page.OnNavigationItemChanged((NavigationItem)args.OldValue, (NavigationItem)args.NewValue);
            }, scheduler);
        }

        /// <summary>
        /// Occurs when the navigation item has changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnNavigationItemChanged(NavigationItem oldValue, NavigationItem newValue)
        {
            // update rootitem
            this.RootItem = newValue?.GetAncestorsAndSelf().LastOrDefault();
        }

        private static void OnSelectedItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            // ignore if navigation item is null (two-way binding is invoked when NavigationItem is nullified when navigating away)
            var page = (NavigationPage)o;
            if (page.NavigationItem != null) {
                page.OnSelectedItemChanged((NavigationItem)args.OldValue, (NavigationItem)args.NewValue);
            }
        }

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnSelectedItemChanged(NavigationItem oldValue, NavigationItem newValue)
        {
            if (newValue != null) {
                // navigate to selected item
                var pageType = typeof(MasterNavigationPage);
                if (IsMasterDetailCandidate(newValue)) {
                    pageType = typeof(MasterDetailNavigationPage);
                }

                this.Frame?.Navigate(pageType, newValue);
            }
        }

        private static void OnWindowStateChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ((NavigationPage)o).OnWindowStateChanged((string)args.OldValue, (string)args.NewValue);
        }

        /// <summary>
        /// Occurs when the window state has changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnWindowStateChanged(string oldValue, string newValue)
        {
        }

        /// <summary>
        /// Determines whether specified item should be shown in the master-detail view, given the current state
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool IsMasterDetailCandidate(NavigationItem item)
        {
            return !item.IsRoot() && (item.IsLeaf() || (!item.HasGrandchildren() && this.WindowState == WindowStateWide));
        }

        /// <summary>
        /// Invoked when the <see cref="NavigationPage"/> is loaded and becomes the current source of a parent <see cref="Frame"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.NavigationItem = e.Parameter as NavigationItem;
        }

        /// <summary>
        /// Immediately invoked after the page is unloaded and is no longer the current source of a parent <see cref="Frame"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.NavigationItem = null;
        }

        /// <summary>
        /// Gets or sets the navigation item associated with this instance.
        /// </summary>
        public NavigationItem NavigationItem
        {
            get { return (NavigationItem)GetValue(NavigationItemProperty); }
            set { SetValue(NavigationItemProperty, value); }
        }

        /// <summary>
        /// Gets the root item.
        /// </summary>
        public NavigationItem RootItem
        {
            get { return (NavigationItem)GetValue(RootItemProperty); }
            private set { SetValue(RootItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected item associated with this instance.
        /// </summary>
        public NavigationItem SelectedItem
        {
            get { return (NavigationItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
        public string WindowState
        {
            get { return (string)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }
    }
}
