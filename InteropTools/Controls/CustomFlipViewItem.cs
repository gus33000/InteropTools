// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace InteropTools.Controls
{
    public sealed class CustomFlipViewItem : FlipViewItem
    {
        public CustomFlipViewItem() => DefaultStyleKey = typeof(CustomFlipViewItem);

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Left || e.Key == VirtualKey.Right || e.Key == VirtualKey.Up ||
                e.Key == VirtualKey.Down)
            {
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
    }
}
