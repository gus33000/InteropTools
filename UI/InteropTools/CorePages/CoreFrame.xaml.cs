using InteropTools.Providers;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.CorePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class CoreFrame
    {
        public CoreFrame()
        {
            InitializeComponent();
        }

        public IRegistryProvider provider { get; set; }

        public UIElement FrameContent
        {
            get => FramePanel.Content as UIElement;

            set
            {
                UpdateCurrentContentChanged();
                FramePanel.Content = value;
            }
        }

        public delegate void CurrentContentChangedEvent(object sender);

        public event CurrentContentChangedEvent OnCurrentContentChanged;

        private void UpdateCurrentContentChanged()
        {
            // Make sure someone is listening to event
            if (OnCurrentContentChanged == null)
            {
                return;
            }

            OnCurrentContentChanged(this);
        }
    }
}
