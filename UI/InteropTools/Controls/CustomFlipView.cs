using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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

    public sealed class CustomFlipViewItem : FlipViewItem
    {
        public CustomFlipViewItem()
        {
            DefaultStyleKey = typeof(CustomFlipViewItem);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Left || e.Key == VirtualKey.Right || e.Key == VirtualKey.Up || e.Key == VirtualKey.Down)
            {
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
    }
}
