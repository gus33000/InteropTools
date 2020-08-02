using System;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteropTools.Handlers
{
    public static class PageExtensions
    {
        public static void SetExtended(this Page pg, FrameworkElement control, bool left = false, bool top = false, bool right = false, bool bottom = false)
        {
            var page = (Window.Current.Content as FrameworkElement);
            var margin = new Thickness(0, 0, 0, 0);

            var addedheight = (GetCurrentDisplaySize().Height - page.ActualHeight) / 2;
            var addedwidth = (GetCurrentDisplaySize().Width - page.ActualWidth) / 2;

            if (left)
                margin.Left = -addedwidth;
            if (top)
                margin.Top = -addedheight;
            if (right)
                margin.Right = -addedwidth;
            if (bottom)
                margin.Bottom = -addedheight;

            control.Margin = margin;
        }

        public static void SetShrinked(this Page pg, FrameworkElement control, bool left = false, bool top = false, bool right = false, bool bottom = false)
        {
            var page = (Window.Current.Content as FrameworkElement);
            var margin = new Thickness(0, 0, 0, 0);

            var addedheight = (GetCurrentDisplaySize().Height - page.ActualHeight) / 2;
            var addedwidth = (GetCurrentDisplaySize().Width - page.ActualWidth) / 2;

            if (left)
                margin.Left = addedwidth;
            if (top)
                margin.Top = addedheight;
            if (right)
                margin.Right = addedwidth;
            if (bottom)
                margin.Bottom = addedheight;

            control.Margin = margin;
        }
        
        public static async void RunInUIThread(this Page pg, Action function)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { function(); });
        }

        public static async void RunInThreadPool(this Page pg, Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }

        private static Size GetCurrentDisplaySize()
        {
            var displayInformation = DisplayInformation.GetForCurrentView();
            TypeInfo t = typeof(DisplayInformation).GetTypeInfo();
            var props = t.DeclaredProperties.Where(x => x.Name.StartsWith("Screen") && x.Name.EndsWith("InRawPixels")).ToArray();
            var w = props.Where(x => x.Name.Contains("Width")).First().GetValue(displayInformation);
            var h = props.Where(x => x.Name.Contains("Height")).First().GetValue(displayInformation);
            var size = new Size(Convert.ToDouble(w), Convert.ToDouble(h));
            switch (displayInformation.CurrentOrientation)
            {
                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    size = new Size(Math.Max(size.Width, size.Height), Math.Min(size.Width, size.Height));
                    break;
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    size = new Size(Math.Min(size.Width, size.Height), Math.Max(size.Width, size.Height));
                    break;
            }
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            size = new Size(size.Width * scaleFactor, size.Height * scaleFactor);
            return size;
        }
    }
}
