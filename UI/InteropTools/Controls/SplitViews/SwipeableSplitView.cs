using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace InteropTools.Controls.SplitViews
{
    public sealed class SwipeableSplitView : SplitView
    {
        #region private variables

        private Grid _paneRoot;
        private Grid _overlayRoot;
        private Rectangle _panArea;
        private Rectangle _dismissLayer;
        private CompositeTransform _paneRootTransform;
        private CompositeTransform _panAreaTransform;
        private Storyboard _openSwipeablePane;
        private Storyboard _closeSwipeablePane;
        private Selector _menuHost;
        private readonly IList<SelectorItem> _menuItems = new List<SelectorItem>();
        private int _toBeSelectedIndex;
        private const double TOTAL_PANNING_DISTANCE = 160d;
        private double _distancePerItem;
        private double _startingDistance;

        #endregion

        public SwipeableSplitView()
        {
            DefaultStyleKey = typeof(SwipeableSplitView);
        }

        #region properties

        // safely subscribe/unsubscribe manipulation events here
        internal Grid PaneRoot
        {
            get => _paneRoot;

            set
            {
                if (_paneRoot != null)
                {
                    _paneRoot.Loaded -= OnPaneRootLoaded;
                    _paneRoot.ManipulationStarted -= OnManipulationStarted;
                    _paneRoot.ManipulationDelta -= OnManipulationDelta;
                    _paneRoot.ManipulationCompleted -= OnManipulationCompleted;
                }

                _paneRoot = value;

                if (_paneRoot != null)
                {
                    _paneRoot.Loaded += OnPaneRootLoaded;
                    _paneRoot.ManipulationStarted += OnManipulationStarted;
                    _paneRoot.ManipulationDelta += OnManipulationDelta;
                    _paneRoot.ManipulationCompleted += OnManipulationCompleted;
                }
            }
        }

        // safely subscribe/unsubscribe manipulation events here
        internal Rectangle PanArea
        {
            get => _panArea;

            set
            {
                if (_panArea != null)
                {
                    _panArea.ManipulationStarted -= OnManipulationStarted;
                    _panArea.ManipulationDelta -= OnManipulationDelta;
                    _panArea.ManipulationCompleted -= OnManipulationCompleted;
                    _panArea.Tapped -= OnDismissLayerTapped;
                }

                _panArea = value;

                if (_panArea != null)
                {
                    _panArea.ManipulationStarted += OnManipulationStarted;
                    _panArea.ManipulationDelta += OnManipulationDelta;
                    _panArea.ManipulationCompleted += OnManipulationCompleted;
                    _panArea.Tapped += OnDismissLayerTapped;
                }
            }
        }

        // safely subscribe/unsubscribe manipulation events here
        internal Rectangle DismissLayer
        {
            get => _dismissLayer;

            set
            {
                if (_dismissLayer != null)
                {
                    _dismissLayer.Tapped -= OnDismissLayerTapped;
                }

                _dismissLayer = value;

                if (_dismissLayer != null)
                {
                    _dismissLayer.Tapped += OnDismissLayerTapped;
                }
            }
        }

        // safely subscribe/unsubscribe animation completed events here
        internal Storyboard OpenSwipeablePaneAnimation
        {
            get => _openSwipeablePane;

            set
            {
                if (_openSwipeablePane != null)
                {
                    _openSwipeablePane.Completed -= OnOpenSwipeablePaneCompleted;
                }

                _openSwipeablePane = value;

                if (_openSwipeablePane != null)
                {
                    _openSwipeablePane.Completed += OnOpenSwipeablePaneCompleted;
                }
            }
        }

        // safely subscribe/unsubscribe animation completed events here
        internal Storyboard CloseSwipeablePaneAnimation
        {
            get => _closeSwipeablePane;

            set
            {
                if (_closeSwipeablePane != null)
                {
                    _closeSwipeablePane.Completed -= OnCloseSwipeablePaneCompleted;
                }

                _closeSwipeablePane = value;

                if (_closeSwipeablePane != null)
                {
                    _closeSwipeablePane.Completed += OnCloseSwipeablePaneCompleted;
                }
            }
        }

        public bool IsSwipeablePaneOpen
        {
            get => (bool)GetValue(IsSwipeablePaneOpenProperty);

            set => SetValue(IsSwipeablePaneOpenProperty, value);
        }

        public static readonly DependencyProperty IsSwipeablePaneOpenProperty =
          DependencyProperty.Register(nameof(IsSwipeablePaneOpen), typeof(bool), typeof(SwipeableSplitView), new PropertyMetadata(false, OnIsSwipeablePaneOpenChanged));

        public delegate void OnPaneOpenChanged(object sender);

        public event OnPaneOpenChanged PaneOpenChanged;

        private void TriggerPaneOpenChangedEvent()
        {
            // Make sure someone is listening to event
            if (PaneOpenChanged == null)
            {
                return;
            }

            PaneOpenChanged(this);
        }

        private static void OnIsSwipeablePaneOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwipeableSplitView splitView = (SwipeableSplitView)d;

            switch (splitView.DisplayMode)
            {
                case SplitViewDisplayMode.Inline:
                case SplitViewDisplayMode.CompactOverlay:
                case SplitViewDisplayMode.CompactInline:
                    splitView.IsPaneOpen = (bool)e.NewValue;
                    break;

                case SplitViewDisplayMode.Overlay:
                    if (splitView.OpenSwipeablePaneAnimation == null || splitView.CloseSwipeablePaneAnimation == null)
                    {
                        return;
                    }

                    if ((bool)e.NewValue)
                    {
                        splitView.OpenSwipeablePane();
                    }
                    else
                    {
                        splitView.CloseSwipeablePane();
                    }

                    break;
            }

            splitView.TriggerPaneOpenChangedEvent();
        }

        public double PanAreaInitialTranslateX
        {
            get => (double)GetValue(PanAreaInitialTranslateXProperty);

            set => SetValue(PanAreaInitialTranslateXProperty, value);
        }

        public static readonly DependencyProperty PanAreaInitialTranslateXProperty =
          DependencyProperty.Register(nameof(PanAreaInitialTranslateX), typeof(double), typeof(SwipeableSplitView), new PropertyMetadata(0d));

        public double PanAreaThreshold
        {
            get => (double)GetValue(PanAreaThresholdProperty);

            set => SetValue(PanAreaThresholdProperty, value);
        }

        public static readonly DependencyProperty PanAreaThresholdProperty =
          DependencyProperty.Register(nameof(PanAreaThreshold), typeof(double), typeof(SwipeableSplitView), new PropertyMetadata(36d));

        /// <summary>
        /// enabling this will allow users to select a menu item by panning up/down on the bottom area of the left pane,
        /// this could be particularly helpful when holding large phones since users don't need to stretch their fingers to
        /// reach the top part of the screen to select a different menu item.
        /// </summary>
        public bool IsPanSelectorEnabled
        {
            get => (bool)GetValue(IsPanSelectorEnabledProperty);

            set => SetValue(IsPanSelectorEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPanSelectorEnabledProperty =
          DependencyProperty.Register(nameof(IsPanSelectorEnabled), typeof(bool), typeof(SwipeableSplitView), new PropertyMetadata(true));

        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PaneRoot = GetTemplateChild<Grid>("PaneRoot");
            _overlayRoot = GetTemplateChild<Grid>("OverlayRoot");
            PanArea = GetTemplateChild<Rectangle>("PanArea");
            DismissLayer = GetTemplateChild<Rectangle>("DismissLayer");
            Grid rootGrid = _paneRoot.GetParent<Grid>();
            OpenSwipeablePaneAnimation = rootGrid.GetStoryboard("OpenSwipeablePane");
            CloseSwipeablePaneAnimation = rootGrid.GetStoryboard("CloseSwipeablePane");
            // initialization
            OnDisplayModeChanged(null, null);
            RegisterPropertyChangedCallback(DisplayModeProperty, OnDisplayModeChanged);

            // disable ScrollViewer as it will prevent finger from panning
            if (Pane is ListView || Pane is ListBox)
            {
                ScrollViewer.SetVerticalScrollMode(Pane, ScrollMode.Disabled);
            }
        }

        #region native property change handlers

        private void OnDisplayModeChanged(DependencyObject sender, DependencyProperty dp)
        {
            switch (DisplayMode)
            {
                case SplitViewDisplayMode.Inline:
                case SplitViewDisplayMode.CompactOverlay:
                case SplitViewDisplayMode.CompactInline:
                    PanAreaInitialTranslateX = 0d;
                    _overlayRoot.Visibility = Visibility.Collapsed;
                    break;

                case SplitViewDisplayMode.Overlay:
                    PanAreaInitialTranslateX = OpenPaneLength * -1;
                    _overlayRoot.Visibility = Visibility.Visible;
                    break;
            }

            ((CompositeTransform)_paneRoot.RenderTransform).TranslateX = PanAreaInitialTranslateX;
        }

        #endregion

        #region manipulation event handlers

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _panAreaTransform = PanArea.GetCompositeTransform();
            _paneRootTransform = PaneRoot.GetCompositeTransform();
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double x = _panAreaTransform.TranslateX + e.Delta.Translation.X;

            // keep the pan within the bountry
            if (x < PanAreaInitialTranslateX || x > 0)
            {
                return;
            }

            // while we are panning the PanArea on X axis, let's sync the PaneRoot's position X too
            _paneRootTransform.TranslateX = _panAreaTransform.TranslateX = x;

            if (sender == _paneRoot && IsPanSelectorEnabled)
            {
                // un-highlight everything first
                foreach (SelectorItem item in _menuItems)
                {
                    if (item != null)
                    {
                        VisualStateManager.GoToState(item, "Normal", true);
                    }
                }

                _toBeSelectedIndex = (int)Math.Round((e.Cumulative.Translation.Y + _startingDistance) / _distancePerItem, MidpointRounding.AwayFromZero);

                if (_toBeSelectedIndex < 0)
                {
                    _toBeSelectedIndex = 0;
                }
                else
                    if (_toBeSelectedIndex >= _menuItems.Count)
                {
                    _toBeSelectedIndex = _menuItems.Count - 1;
                }

                // highlight the item that's going to be selected
                SelectorItem itemContainer = _menuItems[_toBeSelectedIndex];

                if (itemContainer != null)
                {
                    VisualStateManager.GoToState(itemContainer, "PointerOver", true);
                }
            }
        }

        private async void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            double x = e.Velocities.Linear.X;

            // ignore a little bit velocity (+/-0.1)
            if (x <= -0.1)
            {
                CloseSwipeablePane();
            }
            else
                if (x > -0.1 && x < 0.1)
            {
                if (Math.Abs(_panAreaTransform.TranslateX) > Math.Abs(PanAreaInitialTranslateX) / 2)
                {
                    CloseSwipeablePane();
                }
                else
                {
                    OpenSwipeablePane();
                }
            }
            else
            {
                OpenSwipeablePane();
            }

            if (IsPanSelectorEnabled)
            {
                if (sender == _paneRoot)
                {
                    // if it's a flick, meaning the user wants to cancel the action, so we remove all the highlights;
                    // or it's intended to be a horizontal gesture, we also remove all the highlights
                    if (Math.Abs(e.Velocities.Linear.Y) >= 2 ||
                        Math.Abs(e.Cumulative.Translation.X) > Math.Abs(e.Cumulative.Translation.Y))
                    {
                        foreach (SelectorItem item in _menuItems)
                        {
                            if (item != null)
                            {
                                VisualStateManager.GoToState(item, "Normal", true);
                            }
                        }

                        return;
                    }

                    // un-highlight everything first
                    foreach (SelectorItem item in _menuItems)
                    {
                        if (item != null)
                        {
                            VisualStateManager.GoToState(item, "Unselected", true);
                        }
                    }

                    // highlight the item that's going to be selected
                    SelectorItem itemContainer = _menuItems[_toBeSelectedIndex];

                    if (itemContainer != null)
                    {
                        VisualStateManager.GoToState(itemContainer, "Selected", true);
                        // do a selection after a short delay to allow visual effect takes place first
                        await Task.Delay(250);
                        _menuHost.SelectedIndex = _toBeSelectedIndex;
                    }
                }
                else
                {
                    // recalculate the starting distance
                    _startingDistance = _distancePerItem * _menuHost.SelectedIndex;
                }
            }
        }

        #endregion

        #region DismissLayer tap event handlers

        private void OnDismissLayerTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseSwipeablePane();
        }

        #endregion

        #region animation completed event handlers

        private void OnOpenSwipeablePaneCompleted(object sender, object e)
        {
            DismissLayer.IsHitTestVisible = true;
        }

        private void OnCloseSwipeablePaneCompleted(object sender, object e)
        {
            DismissLayer.IsHitTestVisible = false;
        }

        #endregion

        #region loaded event handlers

        private void OnPaneRootLoaded(object sender, RoutedEventArgs e)
        {
            // fill the local menu items collection for later use
            if (IsPanSelectorEnabled)
            {
                Border border = (Border)PaneRoot.Children[0];
                _menuHost = border.AllChildren().Single(f => f.Name == "TopList") as Selector;

                if (_menuHost == null)
                {
                    throw new NullReferenceException("For the bottom panning to work, the Pane's Child needs to be of type Selector.");
                }

                //_menuHost = border.GetChild<Selector>("For the bottom panning to work, the Pane's Child needs to be of type Selector.");

                foreach (object item in _menuHost.Items)
                {
                    SelectorItem container = (SelectorItem)_menuHost.ContainerFromItem(item);
                    _menuItems.Add(container);
                }

                _distancePerItem = TOTAL_PANNING_DISTANCE / _menuItems.Count;
                // calculate the initial starting distance
                _startingDistance = _distancePerItem * _menuHost.SelectedIndex;
            }
        }

        #endregion

        #region private methods

        private void OpenSwipeablePane()
        {
            if (IsSwipeablePaneOpen)
            {
                OpenSwipeablePaneAnimation.Begin();
            }
            else
            {
                IsSwipeablePaneOpen = true;
            }
        }

        private void CloseSwipeablePane()
        {
            if (!IsSwipeablePaneOpen)
            {
                CloseSwipeablePaneAnimation.Begin();
            }
            else
            {
                IsSwipeablePaneOpen = false;
            }
        }

        private T GetTemplateChild<T>(string name, string message = null) where T : DependencyObject
        {
            if (GetTemplateChild(name) is not T child)
            {
                if (message == null)
                {
                    message = $"{name} should not be null! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return child;
        }

        #endregion
    }
}
