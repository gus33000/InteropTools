// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using InteropTools.CorePages;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Registry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class x50PlusDevicesNDTKUnlock : Page
    {
        public x50PlusDevicesNDTKUnlock() => InitializeComponent();

        public PageGroup PageGroup => PageGroup.Registry;
        public string PageName => "Lumia x50 Interop Unlock";
    }
}
