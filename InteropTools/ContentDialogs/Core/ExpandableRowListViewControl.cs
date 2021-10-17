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
}