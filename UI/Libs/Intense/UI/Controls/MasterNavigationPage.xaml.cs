using Intense.Presentation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Intense.UI.Controls
{
    /// <summary>
    /// The master navigation page.
    /// </summary>
    public sealed partial class MasterNavigationPage : NavigationPage
    {
        /// <summary>
        /// Identifies the key for the basic navigation list view style.
        /// </summary>
        public const string BasicNavigationListViewStyleKey = "BasicNavigationListViewStyle";
        /// <summary>
        /// Identifies the key for the narrow navigation list view style.
        /// </summary>
        public const string NarrowNavigationListViewStyleKey = "NarrowNavigationListViewStyle";
        /// <summary>
        /// Identifies the key for the wide navigation list view style.
        /// </summary>
        public const string WideNavigationListViewStyleKey = "WideNavigationListViewStyle";

        /// <summary>
        /// Identifies the NavigationListViewStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationListViewStyleProperty = DependencyProperty.Register("NavigationListViewStyle", typeof(Style), typeof(MasterNavigationPage), null);

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterNavigationPage"/> class.
        /// </summary>
        public MasterNavigationPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the style of the navigation list view.
        /// </summary>
        public Style NavigationListViewStyle
        {
            get => (Style)GetValue(NavigationListViewStyleProperty);
            set => SetValue(NavigationListViewStyleProperty, value);
        }

        /// <summary>
        /// Occurs when the navigation item has changed.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnNavigationItemChanged(NavigationItem oldValue, NavigationItem newValue)
        {
            base.OnNavigationItemChanged(oldValue, newValue);

            SelectedItem = null;

            UpdateListViewStyle();
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

            // when state changes from narrow to wide and navigation item is a master-detail candidate, navigate to master-detail
            if (oldValue == WindowStateNarrow && newValue == WindowStateWide && IsMasterDetailCandidate(NavigationItem))
            {
                // navigate without transition
                Frame.Navigate(typeof(MasterDetailNavigationPage), NavigationItem, new SuppressNavigationTransitionInfo());

                // and clear the most recent backstack entry
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }
            else
            {
                UpdateListViewStyle();
            }
        }

        private void UpdateListViewStyle()
        {
            // update list view style
            string styleKey = WideNavigationListViewStyleKey;

            if (WindowState == WindowStateNarrow)
            {
                styleKey = BasicNavigationListViewStyleKey;
                if (NavigationItem != null && (NavigationItem.IsRoot() || NavigationItem.HasGrandchildren()))
                {
                    styleKey = NarrowNavigationListViewStyleKey;
                }
            }

            NavigationListViewStyle = (Style)Application.Current.Resources[styleKey];
        }
    }
}
