using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Intense.THemes
{
    public class TextBoxBehaviors
    {
        public static bool GetIsUppercaseOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsUppercaseOnlyProperty);
        }

        public static void SetIsUppercaseOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(IsUppercaseOnlyProperty, value);
        }

        public static readonly DependencyProperty IsUppercaseOnlyProperty = DependencyProperty.RegisterAttached("IsUppercaseOnly", typeof(bool), typeof(TextBoxBehaviors), new PropertyMetadata(false, OnIsUpercaseOnlyChanged));

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