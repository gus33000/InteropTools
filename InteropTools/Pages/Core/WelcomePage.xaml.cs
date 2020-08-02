using InteropTools.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.Pages.Core
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();
            SizeChanged += WelcomePage_SizeChanged;

            if (Window.Current.Bounds.Width >= 720)
            {
                this.SetExtended(HeaderBackground, false, true, true, false);
            }
            else
            {
                this.SetExtended(HeaderBackground, false, false, false, false);
            }
        }
        
        private void WelcomePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width >= 720)
            {
                this.SetExtended(HeaderBackground, false, true, true, false);
            }
            else
            {
                this.SetExtended(HeaderBackground, false, false, false, false);
            }
        }
    }
}
