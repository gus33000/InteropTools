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
        /// Identifies the detail layout state.
        /// </summary>
        public const string LayoutStateDetail = "DetailState";

        /// <summary>
        /// Identifies the master-detail layout state.
        /// </summary>
        public const string LayoutStateMasterDetail = "MasterDetailState";

        private NavigationItem lastSelectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDetailNavigationPage"/> class.
        /// </summary>
        public MasterDetailNavigationPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is no longer the current page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // make sure the content frame is cleared
            if (ContentFrame.SourcePageType != null && ContentFrame.SourcePageType != typeof(Page))
            {
                ContentFrame.SourcePageType = typeof(Page);
            }
            ContentFrame.BackStack.Clear();
            ContentFrame.ForwardStack.Clear();
            lastSelectedItem = null;
        }

        /// <summary>
        /// Occurs when the navigation item has changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnNavigationItemChanged(NavigationItem oldValue, NavigationItem newValue)
        {
            base.OnNavigationItemChanged(oldValue, newValue);

            if (Frame == null || newValue == null)
            {
                SelectedItem = null;
                return;
            }

            string layoutStateName = LayoutStateMasterDetail;

            if (newValue.IsLeaf())
            {
                // select navigation item itself
                SelectedItem = newValue;

                layoutStateName = LayoutStateDetail;
            }
            else if (SelectedItem == null)
            {
                // auto-select first child (if not set)
                SelectedItem = newValue.Items.FirstOrDefault();
            }

            VisualStateManager.GoToState(this, layoutStateName, false);
        }

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnSelectedItemChanged(NavigationItem oldValue, NavigationItem newValue)
        {
            if (newValue == null || lastSelectedItem == newValue)
            {
                return;
            }
            lastSelectedItem = newValue;

            if (NavigationItem != newValue && (!newValue.IsLeaf() || WindowState == WindowStateNarrow))
            {
                // navigate to selected item
                base.OnSelectedItemChanged(oldValue, newValue);
            }
            else
            {
                // navigate to content and supply the PageParameter
                System.Type pageType = newValue.PageType ?? typeof(Page);
                if (pageType != typeof(Page) || ContentFrame.SourcePageType != pageType)
                {
                    ContentFrame.Navigate(pageType, newValue.PageParameter);
                }
            }
        }

        /// <summary>
        /// Occurs when the window state has changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnWindowStateChanged(string oldValue, string newValue)
        {
            if (Frame == null || NavigationItem == null)
            {
                return;
            }

            string layoutStateName = null;
            if (newValue == WindowStateWide)
            {
                layoutStateName = LayoutStateMasterDetail;

                if (NavigationItem.IsLeaf())
                {
                    if (oldValue == WindowStateNarrow && IsMasterDetailCandidate(NavigationItem.Parent))
                    {
                        // if from narrow and leaf, auto-select parent
                        NavigationItem navItem = NavigationItem;
                        NavigationItem = NavigationItem.Parent;
                        SelectedItem = navItem;

                        // remove narrow master from backstack
                        PageStackEntry entry = Frame.BackStack.FirstOrDefault(e => e.Parameter == navItem.Parent);
                        if (entry != null)
                        {
                            Frame.BackStack.Remove(entry);
                        }

                        return;
                    }

                    // show leaf always in detail
                    layoutStateName = LayoutStateDetail;
                }
            }
            else if (newValue == WindowStateNarrow)
            {
                layoutStateName = LayoutStateDetail;

                // if from wide, auto-select to selected child
                if (oldValue == WindowStateWide && NavigationItem != SelectedItem && (SelectedItem?.IsLeaf() ?? false))
                {
                    // auto-select child
                    NavigationItem parent = NavigationItem;
                    NavigationItem = SelectedItem;

                    // add narrow master to backstack
                    Frame.BackStack.Add(new PageStackEntry(typeof(MasterNavigationPage), parent, null));

                    return;
                }
            }

            if (layoutStateName != null)
            {
                VisualStateManager.GoToState(this, layoutStateName, false);
            }
        }

        private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            // try sync selected item
            NavigationItem item = NavigationItem.Items.FirstOrDefault(i => i.PageType == e.SourcePageType && i.PageParameter == e.Parameter);
            if (item != null)
            {
                SelectedItem = item;
            }
        }
    }
}