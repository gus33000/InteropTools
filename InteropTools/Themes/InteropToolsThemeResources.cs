using InteropTools.Handlers;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace InteropTools.Themes
{
    public class InteropToolsThemeResources : ResourceDictionary
    {
        public InteropToolsThemeResources()
        {
            var settingshandler = new SettingsHandler();
            
            if (ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateHostBackdropBrush") && !settingshandler.useMDL2)
            {
                MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("ms-appx:///Themes/fluent.xaml") }
                );
            }
            else
            {
                MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("ms-appx:///Themes/mdl2.xaml") }
                );
            }
        }
    }
}
