// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Intense.UI
{
    /// <summary>
    /// Describes customizations to the non-client area of the current window.
    /// </summary>
    public class WindowChrome : DependencyObject, IApplicationViewEventSink
    {
        /// <summary>
        /// Identifies the AutoUpdateMargin dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoUpdateMarginProperty = DependencyProperty.Register("AutoUpdateMargin", typeof(bool), typeof(WindowChrome), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the Chrome attached property.
        /// </summary>
        public static readonly DependencyProperty ChromeProperty = DependencyProperty.RegisterAttached("Chrome", typeof(WindowChrome), typeof(WindowChrome), new PropertyMetadata(null, OnChromeChanged));

        /// <summary>
        /// Identifies the Margin dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register("Margin", typeof(Thickness), typeof(WindowChrome), new PropertyMetadata(null, OnMarginChanged));

        /// <summary>
        /// Identifies the StatusBarBackgroundColor dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusBarBackgroundColorProperty = DependencyProperty.Register("StatusBarBackgroundColor", typeof(Color), typeof(WindowChrome), new PropertyMetadata(null, OnStatusBarBackgroundColorChanged));

        /// <summary>
        /// Identifies the StatusBarForegroundColor dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusBarForegroundColorProperty = DependencyProperty.Register("StatusBarForegroundColor", typeof(Color), typeof(WindowChrome), new PropertyMetadata(null, OnStatusBarForegroundColorChanged));

        private bool initialized;
        private FrameworkElement target;

        /// <summary>
        /// Gets or sets a value indicating whether the margin of the target framework element is automatically updated when the visible bounds changes.
        /// </summary>
        public bool AutoUpdateMargin
        {
            get => (bool)GetValue(AutoUpdateMarginProperty);
            set => SetValue(AutoUpdateMarginProperty, value);
        }

        /// <summary>
        /// Gets the window margin.
        /// </summary>
        public Thickness Margin
        {
            get => (Thickness)GetValue(MarginProperty);
            private set => SetValue(MarginProperty, value);
        }

        /// <summary>
        /// Gets or set the status bar background color.
        /// </summary>
        public Color StatusBarBackgroundColor
        {
            get => (Color)GetValue(StatusBarBackgroundColorProperty);
            set => SetValue(StatusBarBackgroundColorProperty, value);
        }

        /// <summary>
        /// Gets or set the status bar foreground color.
        /// </summary>
        public Color StatusBarForegroundColor
        {
            get => (Color)GetValue(StatusBarForegroundColorProperty);
            set => SetValue(StatusBarForegroundColorProperty, value);
        }

        /// <summary>
        /// Retrieves the <see cref="WindowChrome"/> attached to specified dependency object instance.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static WindowChrome GetChrome(DependencyObject o)
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            return (WindowChrome)o.GetValue(ChromeProperty);
        }

        /// <summary>
        /// Attaches a <see cref="WindowChrome"/> to specified dependency object.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="chrome"></param>
        public static void SetChrome(DependencyObject o, WindowChrome chrome)
        {
            o.SetValue(ChromeProperty, chrome);
        }

        void IApplicationViewEventSink.OnConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
        }

        void IApplicationViewEventSink.OnVisibleBoundsChanged(ApplicationView sender, object args)
        {
            CalculateMargin();
        }

        private static void OnChromeChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            // assign dependency object of type FrameworkElement to the chrome instance
            // this works as long as the chrome instance is not shared among dependency objects
            WindowChrome chrome = (WindowChrome)args.NewValue;
            chrome?.SetTarget(o as FrameworkElement);

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            SolidColorBrush transparentColorBrush = new() { Opacity = 0 };
            Color transparentColor = transparentColorBrush.Color;
            titlebar.BackgroundColor = transparentColor;
            titlebar.ButtonBackgroundColor = transparentColor;
            titlebar.ButtonInactiveBackgroundColor = transparentColor;
            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush solidColorBrush)
            {
                titlebar.ButtonForegroundColor = solidColorBrush.Color;
                titlebar.ButtonInactiveForegroundColor = solidColorBrush.Color;
            }

            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush colorBrush)
            {
                titlebar.ForegroundColor = colorBrush.Color;
            }

            Color hovercolor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color ?? default;
            hovercolor.A = 32;

            titlebar.ButtonHoverBackgroundColor = hovercolor;
            titlebar.ButtonHoverForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;

            hovercolor.A = 64;

            titlebar.ButtonPressedBackgroundColor = hovercolor;
            titlebar.ButtonPressedForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;
        }

        private static void OnMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ((WindowChrome)o).ApplyMarginToTarget();
        }

        private static void OnStatusBarBackgroundColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ((WindowChrome)o).SetStatusBarBackground();
        }

        private static void OnStatusBarForegroundColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ((WindowChrome)o).SetStatusBarForeground();
        }

        private static bool TryGetStatusBar(out StatusBar statusBar)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                statusBar = StatusBar.GetForCurrentView();
                return true;
            }
            statusBar = null;
            return false;
        }

        private void ApplyMarginToTarget()
        {
            if (AutoUpdateMargin && target != null)
            {
                target.Margin = Margin;
            }
        }

        private void CalculateMargin()
        {
            ApplicationView appView = ApplicationView.GetForCurrentView();
            Windows.Foundation.Rect visibleBounds = appView.VisibleBounds;
            Windows.Foundation.Rect wndBounds = Window.Current.Bounds;

            if (visibleBounds != wndBounds)
            {
                double left = Math.Ceiling(visibleBounds.Left - wndBounds.Left);
                double top = Math.Ceiling(visibleBounds.Top - wndBounds.Top);
                double right = Math.Ceiling(wndBounds.Right - visibleBounds.Right);
                double bottom = Math.Ceiling(wndBounds.Bottom - visibleBounds.Bottom);

                Margin = new Thickness(left, top, right, bottom);
            }
            else
            {
                Margin = new Thickness();
            }
        }

        private void InitializeChrome()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;

            SetStatusBarBackground();
            SetStatusBarForeground();

            ApplicationView.GetForCurrentView().RegisterEventSink(this);

            CalculateMargin();
            ApplyMarginToTarget();
        }

        private void SetStatusBarBackground()
        {
            if (!initialized)
            {
                return;
            }
            if (TryGetStatusBar(out StatusBar statusBar))
            {
                // infer opacity from alpha channel of the color
                statusBar.BackgroundOpacity = StatusBarBackgroundColor.A / 255d;
                statusBar.BackgroundColor = StatusBarBackgroundColor;
            }

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            SolidColorBrush transparentColorBrush = new() { Opacity = 0 };
            Color transparentColor = transparentColorBrush.Color;
            titlebar.BackgroundColor = transparentColor;
            titlebar.ButtonBackgroundColor = transparentColor;
            titlebar.ButtonInactiveBackgroundColor = transparentColor;
            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush solidColorBrush)
            {
                titlebar.ButtonForegroundColor = solidColorBrush.Color;
                titlebar.ButtonInactiveForegroundColor = solidColorBrush.Color;
            }

            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush colorBrush)
            {
                titlebar.ForegroundColor = colorBrush.Color;
            }

            Color hovercolor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color ?? default;
            hovercolor.A = 32;

            titlebar.ButtonHoverBackgroundColor = hovercolor;
            titlebar.ButtonHoverForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;

            hovercolor.A = 64;

            titlebar.ButtonPressedBackgroundColor = hovercolor;
            titlebar.ButtonPressedForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;
        }

        private void SetStatusBarForeground()
        {
            if (!initialized)
            {
                return;
            }
            if (TryGetStatusBar(out StatusBar statusBar))
            {
                statusBar.ForegroundColor = StatusBarForegroundColor;
            }

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            SolidColorBrush transparentColorBrush = new() { Opacity = 0 };
            Color transparentColor = transparentColorBrush.Color;
            titlebar.BackgroundColor = transparentColor;
            titlebar.ButtonBackgroundColor = transparentColor;
            titlebar.ButtonInactiveBackgroundColor = transparentColor;
            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush solidColorBrush)
            {
                titlebar.ButtonForegroundColor = solidColorBrush.Color;
                titlebar.ButtonInactiveForegroundColor = solidColorBrush.Color;
            }

            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush colorBrush)
            {
                titlebar.ForegroundColor = colorBrush.Color;
            }

            Color hovercolor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color ?? default;
            hovercolor.A = 32;

            titlebar.ButtonHoverBackgroundColor = hovercolor;
            titlebar.ButtonHoverForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;

            hovercolor.A = 64;

            titlebar.ButtonPressedBackgroundColor = hovercolor;
            titlebar.ButtonPressedForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;
        }

        private void SetTarget(FrameworkElement target)
        {
            this.target = target;
            InitializeChrome();
            ApplyMarginToTarget();
        }
    }
}