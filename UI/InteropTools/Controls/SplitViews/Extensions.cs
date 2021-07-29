using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace InteropTools.Controls.SplitViews
{
    public static class Extensions
    {
        public static List<Control> AllChildren(this DependencyObject parent)
        {
            List<Control> list = new();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject _Child = VisualTreeHelper.GetChild(parent, i);

                if (_Child is Control)
                {
                    list.Add(_Child as Control);
                }

                list.AddRange(AllChildren(_Child));
            }

            return list;
        }

        public static T GetChild<T>(this DependencyObject parentContainer, string controlName)
        {
            List<Control> childControls = AllChildren(parentContainer);
            T control = childControls.Where(f => f != null).Where(x => x.Name.Equals(controlName)).Cast<T>().First();
            return control;
        }

        public static T GetParent<T>(this FrameworkElement element, string message = null) where T : DependencyObject
        {
            if (element.Parent is not T parent)
            {
                if (message == null)
                {
                    message = "Parent element should not be null! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return parent;
        }

        public static T GetChild<T>(this Border element, string message = null) where T : DependencyObject
        {
            if (element.Child is not T child)
            {
                if (message == null)
                {
                    message = $"{nameof(Border)}'s child should not be null! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return child;
        }

        public static Storyboard GetStoryboard(this FrameworkElement element, string name, string message = null)
        {
            if (element.Resources[name] is not Storyboard storyboard)
            {
                if (message == null)
                {
                    message = $"Storyboard '{name}' cannot be found! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return storyboard;
        }

        public static CompositeTransform GetCompositeTransform(this FrameworkElement element, string message = null)
        {
            if (element.RenderTransform is not CompositeTransform transform)
            {
                if (message == null)
                {
                    message = $"{element.Name}'s RenderTransform should be a CompositeTransform! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return transform;
        }
    }
}
