using InteropTools.CorePages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Core
{
	public sealed partial class DualMessageDialogContentDialog : ContentDialog
	{
		private bool Reply = false;

		public DualMessageDialogContentDialog()
		{
			this.InitializeComponent();
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Reply = true;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Reply = false;
		}

		public async Task<bool> ShowDualMessageDialog(string Title, string Description, string PrimaryButtonText = "Yes", string SecondaryButtonText = "No")
		{
			this.Title = Title;
			this.Description.Text = Description;
			this.PrimaryButtonText = PrimaryButtonText;
			this.SecondaryButtonText = SecondaryButtonText;
			await this.ShowAsync();
			return Reply;
		}
	}
}
