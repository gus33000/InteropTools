using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace InteropTools.Controls
{
	public sealed class CustomFlipView : FlipView
	{
		public CustomFlipView()
		{
			this.DefaultStyleKey = typeof(CustomFlipView);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new CustomFlipViewItem();
		}
	}

	public sealed class CustomFlipViewItem : FlipViewItem
	{
		public CustomFlipViewItem()
		{
			this.DefaultStyleKey = typeof(CustomFlipViewItem);
		}


		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Left || e.Key == VirtualKey.Right || e.Key == VirtualKey.Up || e.Key == VirtualKey.Down)
			{
				e.Handled = true;
			}

			base.OnKeyDown(e);
		}
	}
}
