using Intense.Presentation;
using Windows.UI.Xaml;

namespace InteropTools.Presentation
{
	/// <summary>
	/// Represents a displayable theme.
	/// </summary>
	public class DisplayableTheme
		: Displayable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DisplayableTheme"/>.
		/// </summary>
		/// <param name="displayName"></param>
		/// <param name="theme"></param>
		public DisplayableTheme(string displayName, ApplicationTheme? theme)
		{
			this.DisplayName = displayName;
			this.Theme = theme;
		}

		/// <summary>
		/// Gets the them.
		/// </summary>
		public ApplicationTheme? Theme { get; }
	}
}