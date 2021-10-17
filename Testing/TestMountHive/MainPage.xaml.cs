﻿// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestMountHive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage() => InitializeComponent();

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
