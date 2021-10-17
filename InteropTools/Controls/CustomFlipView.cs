// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

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