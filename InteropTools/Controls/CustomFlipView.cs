namespace InteropTools.Controls
{
    public sealed class CustomFlipView : FlipView
    {
        public CustomFlipView()
        {
            DefaultStyleKey = typeof(CustomFlipView);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomFlipViewItem();
        }
    }
}