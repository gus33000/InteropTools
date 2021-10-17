// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using InteropTools.ContentDialogs.Registry;
using InteropTools.CorePages;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Shell = InteropTools.CorePages.Shell;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Registry
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImportRegFilePage : Page
    {
        public ImportRegFilePage()
        {
            InitializeComponent();
            Loaded += ImportRegFilePage_Loaded;
        }

        public PageGroup PageGroup => PageGroup.Registry;
        public string PageName => "Import Registry File";

        private static async void OpenFile()
        {
            FileOpenPicker picker = new()
            {
                ViewMode = PickerViewMode.List, SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(".reg");
            picker.FileTypeFilter.Add(".itreg");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                await new ImportRegContentDialog(file).ShowAsync();
            }

            Shell shell = App.AppContent as Shell;
            try
            {
                shell.RootFrame.GoBack();
            }
            catch
            {
                return;
            }
        }

        private void ImportRegFilePage_Loaded(object sender, RoutedEventArgs e) => OpenFile();
    }
}
