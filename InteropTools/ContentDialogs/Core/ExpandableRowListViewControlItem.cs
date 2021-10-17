// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace InteropTools.ContentDialogs.Core
{
    [TemplatePart(Name = "gridRowHeader", Type = typeof(Grid))]
    [ContentProperty(Name = "Content")]
    public sealed class ExpandableRowListViewControlItem : ContentControl
    {
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(ExpandableRowListViewControlItem), new PropertyMetadata(false, IsExpanded_OnChanged));
        public static readonly DependencyProperty RowHeaderProperty = DependencyProperty.Register(nameof(RowHeader), typeof(object), typeof(ExpandableRowListViewControlItem), null);
        private const string VISUALSTATES_COLLAPSED = "Collapsed";
        private const string VISUALSTATES_EXPANDED = "Expanded";

        public ExpandableRowListViewControlItem()
            : base()
        {
            DefaultStyleKey = typeof(ExpandableRowListViewControlItem);
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public object RowHeader
        {
            get => GetValue(RowHeaderProperty);
            set => SetValue(RowHeaderProperty, value);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsExpanded)
            {
                VisualStateManager.GoToState(this, VISUALSTATES_EXPANDED, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VISUALSTATES_COLLAPSED, false);
            }

            Grid gridRowHeader = (Grid)GetTemplateChild("gridRowHeader");
            if (gridRowHeader != null)
            {
                gridRowHeader.Tapped += (sender, e) =>
                {
                    // Toggle expanded state
                    IsExpanded = !IsExpanded;
                };
            }
        }

        private static void IsExpanded_OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpandableRowListViewControlItem item = (ExpandableRowListViewControlItem)d;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;

            if (oldValue == newValue)
            {
                return;
            }

            if (newValue)
            {
                VisualStateManager.GoToState(item, VISUALSTATES_EXPANDED, true);
            }
            else
            {
                VisualStateManager.GoToState(item, VISUALSTATES_COLLAPSED, true);
            }
        }
    }
}