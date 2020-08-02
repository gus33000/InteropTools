using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestMountHive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Go_Click(object sender, RoutedEventArgs e)
        {
            /*var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Unspecified;

            picker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var helper = new RegistryRT.Registry();
                helper.InitNTDLLEntryPoints();

                Debug.WriteLine(file.Path);

                var ret = helper.LoadHive(file.Path, "HelloWorld", 0);
                await new MessageDialog(ret.ToString()).ShowAsync();
            }*/

            var helper = new RegistryRT.Registry();
            helper.InitNTDLLEntryPoints();
            
            var ret = helper.UnloadHive("HelloWorld", 0);
        }
    }
}
