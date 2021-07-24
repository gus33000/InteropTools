using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.Controls.Pivots
{
    public sealed partial class TabHeader : UserControl
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(TabHeader), null);

        public string Glyph
        {
            get => GetValue(GlyphProperty) as string;

            set => SetValue(GlyphProperty, value);
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TabHeader), null);

        public string Label
        {
            get => GetValue(LabelProperty) as string;

            set => SetValue(LabelProperty, value);
        }

        public TabHeader()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}