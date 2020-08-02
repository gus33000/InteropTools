using Intense.Presentation;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Intense.UI.Controls
{
    /// <summary>
    /// Represents the master-detail navigation page.
    /// </summary>
    public sealed partial class MasterDetailNavigationPage : NavigationPage
    {
        /// <summary>
        /// Identifies the master-detail layout state.
        /// </summary>
        public const string LayoutStateMasterDetail = "MasterDetailState";
        /// <summary>
        /// Identifies the detail layout state.
        /// </summary>
        public const string LayoutStateDetail = "DetailState";

        private NavigationItem lastSelectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDetailNavigationPage"/> class.
        /// </summary>
        public MasterDetailNavigationPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is no longer the current page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // make sure the content frame is cleared
            if (this.ContentFrame.SourcePageType != null && this.ContentFrame.SourcePageType != typeof(Page)) {
                this.ContentFrame.SourcePageType = typeof(Page);
            }
            this.ContentFrame.BackStack.Clear();
            this.ContentFrame.ForwardStack.Clear();
            this.lastSelectedItem = null;
        }

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnSelectedItemChanged(NavigationItem oldValue, NavigationItem newValue)
        {
            if (newValue == null || this.lastSelectedItem == newValue) {
                return;
            }
            this.lastSelectedItem = newValue;

            if (this.NavigationItem != newValue && ((!newValue.IsLeaf() || this.WindowState == WindowStateNarrow))) {
                // navigate to selected item
                base.OnSelectedItemChanged(oldValue, newValue);
            }
            else {
                // navigate to content and supply the PageParameter
                var pageType = newValue.PageType ?? typeof(Page);
                if (pageType != typeof(Page) || this.ContentFrame.SourcePageType != pageType) {
                    this.ContentFrame.Navigate(pageType, newValue.PageParameter);
                }
            }
        }

        /// <summary>
        /// Occurs when the navigation item has changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnNavigationItemChanged(NavigationItem oldValue, NavigationItem newValue)
        {
            base.OnNavigationItemChanged(oldValue, newValue);

            if (this.Frame == null || newValue == null) {
                this.SelectedItem = null;
                return;
            }

            string layoutStateName = LayoutStateMasterDetail;

            if (newValue.IsLeaf()) {
                // select navigation item itself
                this.SelectedItem = newValue;

                layoutStateName = LayoutStateDetail;
            }
            else if (this.SelectedItem == null) {
                // auto-select first child (if not set)
                this.SelectedItem = newValue.Items.FirstOrDefault();
            }

            VisualStateManager.GoToState(this, layoutStateName, false);
        }

        /// <summary>
        /// Occurs when the window state has changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnWindowStateChanged(string oldValue, string newValue)
        {
            if (this.Frame == null || this.NavigationItem == null) {
                return;
            }

            string layoutStateName = null;
            if (newValue == WindowStateWide) {
                layoutStateName = LayoutStateMasterDetail;

                if (this.NavigationItem.IsLeaf()) {
                    if (oldValue == WindowStateNarrow && IsMasterDetailCandidate(this.NavigationItem.Parent)) {
                        // if from narrow and leaf, auto-select parent
                        var navItem = this.NavigationItem;
                        this.NavigationItem = this.NavigationItem.Parent;
                        this.SelectedItem = navItem;

                        // remove narrow master from backstack
                        var entry = this.Frame.BackStack.FirstOrDefault(e => e.Parameter == navItem.Parent);
                        if (entry != null) {
                            this.Frame.BackStack.Remove(entry);
                        }

                        return;
                    }

                    // show leaf always in detail
                    layoutStateName = LayoutStateDetail;
                }
            }
            else if (newValue == WindowStateNarrow) {
                layoutStateName = LayoutStateDetail;

                // if from wide, auto-select to selected child
                if (oldValue == WindowStateWide && this.NavigationItem != this.SelectedItem && (this.SelectedItem?.IsLeaf() ?? false)) {
                    // auto-select child
                    var parent = this.NavigationItem;
                    this.NavigationItem = this.SelectedItem;

                    // add narrow master to backstack
                    this.Frame.BackStack.Add(new PageStackEntry(typeof(MasterNavigationPage), parent, null));

                    return;
                }
            }

            if (layoutStateName != null) {
                VisualStateManager.GoToState(this, layoutStateName, false);
            }
        }

        private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            // try sync selected item
            var item = this.NavigationItem.Items.FirstOrDefault(i => i.PageType == e.SourcePageType && i.PageParameter == e.Parameter);
            if (item != null) {
                this.SelectedItem = item;
            }
        }
    }
}
