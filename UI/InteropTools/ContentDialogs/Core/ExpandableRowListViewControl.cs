using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;


namespace InteropTools.ContentDialogs.Core
{
    [TemplatePart(Name = "listviewRows", Type = typeof(ListView))]
    [ContentProperty(Name = "Items")]
    public sealed class ExpandableRowListViewControl : ItemsControl
    {
        public ExpandableRowListViewControl()
            : base()
        {
            DefaultStyleKey = typeof(ExpandableRowListViewControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ListView listviewRows = (ListView)GetTemplateChild("listviewRows");
            if (listviewRows != null && ItemsSource == null)
            {
                while (Items.Count > 0)
                {
                    object item = Items[0];
                    Items.RemoveAt(0);  // This item cannot be in two different 'ItemCollection's
                    listviewRows.Items.Add(item);
                }
            }
        }
    }

    [TemplatePart(Name = "gridRowHeader", Type = typeof(Grid))]
    [ContentProperty(Name = "Content")]
    public sealed class ExpandableRowListViewControlItem : ContentControl
    {
        private static readonly string VISUALSTATES_EXPANDED = "Expanded";
        private static readonly string VISUALSTATES_COLLAPSED = "Collapsed";

        public ExpandableRowListViewControlItem()
            : base()
        {
            DefaultStyleKey = typeof(ExpandableRowListViewControlItem);
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

        public static readonly DependencyProperty RowHeaderProperty = DependencyProperty.Register(nameof(RowHeader), typeof(object), typeof(ExpandableRowListViewControlItem), null);
        public object RowHeader
        {
            get => GetValue(RowHeaderProperty);
            set => SetValue(RowHeaderProperty, value);
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(ExpandableRowListViewControlItem), new PropertyMetadata(false, IsExpanded_OnChanged));
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
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
