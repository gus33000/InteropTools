// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace InteropTools.Controls
{
    public sealed class FlipViewIndicator : ListBox
    {
        /// <summary>
        /// Identifies the <see cref="FlipView"/> dependency property
        /// </summary>
        public static readonly DependencyProperty FlipViewProperty =
            DependencyProperty.Register("FlipView", typeof(FlipView), typeof(FlipViewIndicator), new PropertyMetadata(null, (depobj, args) =>
            {
                FlipViewIndicator fvi = (FlipViewIndicator)depobj;
                FlipView fv = (FlipView)args.NewValue;

                // this is a special case where ItemsSource is set in code
                // and the associated FlipView's ItemsSource may not be available yet
                // if it isn't available, let's listen for SelectionChanged
                fv.SelectionChanged += (s, e) => fvi.ItemsSource = fv.Items;

                fvi.ItemsSource = fv.Items;

                // create the element binding source
                Binding eb = new()
                {
                    Mode = BindingMode.TwoWay,
                    Source = fv,
                    Path = new PropertyPath("SelectedItem")
                };

                // set the element binding to change selection when the FlipView changes
                fvi.SetBinding(SelectedItemProperty, eb);
            }));

        /// <summary>
        /// Initializes a new instance of the <see cref="FlipViewIndicator"/> class.
        /// </summary>
        public FlipViewIndicator()
        {
            DefaultStyleKey = typeof(FlipViewIndicator);
        }

        /// <summary>
        /// Gets or sets the flip view.
        /// </summary>
        public FlipView FlipView
        {
            get => (FlipView)GetValue(FlipViewProperty);
            set => SetValue(FlipViewProperty, value);
        }
    }
}