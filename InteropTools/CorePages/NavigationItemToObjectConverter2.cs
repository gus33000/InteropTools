using Intense.Presentation;
using System;
using TreeViewControl;
using Windows.UI.Xaml.Data;

namespace InteropTools.CorePages
{
    /// <summary>
    /// Converts a <see cref="NavigationItem"/> instance to object and vice versa.
    /// </summary>
    public class NavigationItemToObjectConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new TreeNode2() { Data = new NavigationItemData() { NavigationItem = value as NavigationItem } };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value as TreeNode2)?.Data as NavigationItem;
        }
    }
}