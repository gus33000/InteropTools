using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using InteropTools.ContentDialogs.Registry;
using InteropTools.ShellPages.Core;
using Shell = InteropTools.CorePages.Shell;
using InteropTools.CorePages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Registry
{
	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ImportRegFilePage
    {
        public string PageName => "Import Registry File";
        public PageGroup PageGroup => PageGroup.Registry;

        public ImportRegFilePage()
		{
			InitializeComponent();
			this.Loaded += ImportRegFilePage_Loaded;
		}

		private void ImportRegFilePage_Loaded(Object sender, RoutedEventArgs e)
		{
			OpenFile();
		}

		private static async void OpenFile()
		{
			var picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.ComputerFolder
			};
			picker.FileTypeFilter.Add(".reg");
			picker.FileTypeFilter.Add(".itreg");
			var file = await picker.PickSingleFileAsync();

			if (file != null)
			{
				await new ImportRegContentDialog(file).ShowAsync();
			}

			var shell = App.AppContent as Shell;
            try
            {
                shell.RootFrame.GoBack();
            }
            catch
            {
                return;
            }
        }
	}
}