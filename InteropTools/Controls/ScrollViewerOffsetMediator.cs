// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace InteropTools.Controls
{
    /// <summary>
    /// Mediator that forwards Offset property changes on to a ScrollViewer
    /// instance to enable the animation of Horizontal/VerticalOffset.
    /// </summary>
    public class ScrollViewerOffsetMediator : FrameworkElement
    {
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(null, OnHorizontalOffsetChanged));

        public static readonly DependencyProperty ScrollableHeightMultiplierProperty =
            DependencyProperty.Register(
                "ScrollableHeightMultiplier",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(null, OnScrollableHeightMultiplierChanged));

        public static readonly DependencyProperty ScrollableWidthMultiplierProperty =
            DependencyProperty.Register(
                "ScrollableWidthMultiplier",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(null, OnScrollableWidthMultiplierChanged));

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register(
                "ScrollViewer",
                typeof(ScrollViewer),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(null, OnScrollViewerChanged));

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(
                "VerticalOffset",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(null, OnVerticalOffsetChanged));

        /// <summary>
        /// HorizontalOffset property to forward to the ScrollViewer.
        /// </summary>
        public double HorizontalOffset
        {
            get => (double)GetValue(HorizontalOffsetProperty);
            set => SetValue(HorizontalOffsetProperty, value);
        }

        /// <summary>
        /// Multiplier for ScrollableHeight property to forward to the ScrollViewer.
        /// </summary>
        /// <remarks>
        /// 0.0 means "scrolled to top"; 1.0 means "scrolled to bottom".
        /// </remarks>
        public double ScrollableHeightMultiplier
        {
            get => (double)GetValue(ScrollableHeightMultiplierProperty);
            set => SetValue(ScrollableHeightMultiplierProperty, value);
        }

        /// <summary>
        /// Multiplier for ScrollableWidth property to forward to the ScrollViewer.
        /// </summary>
        /// <remarks>
        /// 0.0 means "scrolled to left"; 1.0 means "scrolled to right".
        /// </remarks>
        public double ScrollableWidthMultiplier
        {
            get => (double)GetValue(ScrollableWidthMultiplierProperty);
            set => SetValue(ScrollableWidthMultiplierProperty, value);
        }

        /// <summary>
        /// ScrollViewer instance to forward Offset changes on to.
        /// </summary>
        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        /// <summary>
        /// VerticalOffset property to forward to the ScrollViewer.
        /// </summary>
        public double VerticalOffset
        {
            get => (double)GetValue(VerticalOffsetProperty);
            set => SetValue(VerticalOffsetProperty, value);
        }

        public static void OnHorizontalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerOffsetMediator mediator = (ScrollViewerOffsetMediator)o;
            mediator.ScrollViewer?.ScrollToHorizontalOffset((double)e.NewValue);
        }

        public static void OnScrollableHeightMultiplierChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerOffsetMediator mediator = (ScrollViewerOffsetMediator)o;
            ScrollViewer scrollViewer = mediator.ScrollViewer;
            scrollViewer?.ScrollToVerticalOffset((double)e.NewValue * scrollViewer.ScrollableHeight);
        }

        public static void OnScrollableWidthMultiplierChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerOffsetMediator mediator = (ScrollViewerOffsetMediator)o;
            ScrollViewer scrollViewer = mediator.ScrollViewer;
            scrollViewer?.ScrollToHorizontalOffset((double)e.NewValue * scrollViewer.ScrollableWidth);
        }

        public static void OnVerticalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerOffsetMediator mediator = (ScrollViewerOffsetMediator)o;
            mediator.ScrollViewer?.ScrollToVerticalOffset((double)e.NewValue);
        }

        private static void OnScrollViewerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerOffsetMediator mediator = (ScrollViewerOffsetMediator)o;
            ScrollViewer scrollViewer = (ScrollViewer)e.NewValue;
            scrollViewer?.ScrollToVerticalOffset(mediator.VerticalOffset);
        }
    }
}