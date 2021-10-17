// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Intense.THemes
{
    public class TextBoxBehaviors
    {
        public static readonly DependencyProperty IsUppercaseOnlyProperty =
            DependencyProperty.RegisterAttached("IsUppercaseOnly", typeof(bool), typeof(TextBoxBehaviors),
                new PropertyMetadata(false, OnIsUpercaseOnlyChanged));

        public static bool GetIsUppercaseOnly(DependencyObject obj) => (bool)obj.GetValue(IsUppercaseOnlyProperty);

        public static void SetIsUppercaseOnly(DependencyObject obj, bool value) =>
            obj.SetValue(IsUppercaseOnlyProperty, value);

        private static void OnIsUpercaseOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBlock textBox))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.Text = textBox.Text.ToUpper();
            }
        }
    }
}
