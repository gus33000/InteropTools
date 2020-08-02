using Windows.System.Profile;
using Windows.UI.Xaml.Controls;

namespace InteropTools.Controls.Blurring.Acrylic
{
	public sealed partial class AcrylicMedium : UserControl
	{
		public AcrylicMedium()
		{
			this.InitializeComponent();
            /*
			if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop" && Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateHostBackdropBrush"))
			{
				MainGrid.Children.Add(new HostBackDrop() { Opacity = 0.25, BlurAmount = 1 });
			}*/
		}
	}
}
