// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InteropTools.Providers.Registry.SampleProvider.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage() => InitializeComponent();

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e) =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("interoptools:"));
    }
}
