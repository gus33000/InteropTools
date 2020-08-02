
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using InteropTools.Providers;
using InteropTools.CorePages;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Registry
{
	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class KeyboardCarretPage
    {
        public string PageName => "Keyboard Settings";
        public PageGroup PageGroup => PageGroup.Tweaks;

        private readonly IRegistryProvider _helper;

		private readonly bool _initialized = false;

		private decimal _offsetXPercentage;
		private decimal _offsetYPercentage;

		public KeyboardCarretPage()
		{
			InitializeComponent();
			_helper = App.MainRegistryHelper;
            Refresh();
		}

        private async void Refresh()
		{
			if (_initialized)
			{
				return;
			}

			//Initialized = true;

			try
			{
				RegTypes regtype;
				string regvalue;
				var ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\FingerKB\Options",
				                    "CaretCenterX_Percentage", RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;
                _offsetXPercentage = decimal.Parse(regvalue) / 100m;
                ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\FingerKB\Options",
				                    "CaretCenterY_Percentage", RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;
                _offsetYPercentage = decimal.Parse(regvalue) / 100m;
                ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\FingerKB\Options",
				                    "CaretInputWidth_Percentage", RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;
                var XPercentage = decimal.Parse(regvalue) / 100m;
                ret = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"Software\Microsoft\FingerKB\Options",
				                    "CaretInputHeight_Percentage", RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;
                var YPercentage = decimal.Parse(regvalue) / 100m;
				var OffsetX = _offsetXPercentage * long.Parse(FakeKeyb.ActualWidth.ToString().Split('.').First());
				var OffsetY = (1m - _offsetYPercentage) * long.Parse(FakeKeyb.ActualHeight.ToString().Split('.').First());
				var PxX = XPercentage * decimal.Parse(FakeKeyb.ActualWidth.ToString());
				var PxY = (1m - YPercentage) * decimal.Parse(FakeKeyb.ActualHeight.ToString());
				Canvas.SetLeft(Carret, double.Parse(PxX.ToString()));
				Canvas.SetTop(Carret, double.Parse((PxY - OffsetY).ToString()));
			}

			catch
			{
			}
		}

		private void x_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			Canvas.SetLeft(Carret, e.NewValue / 100 * FakeKeyb.ActualWidth);
		}

		private void y_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			Canvas.SetTop(Carret, (100 - e.NewValue) / 100 * FakeKeyb.ActualHeight);
		}
	}
}