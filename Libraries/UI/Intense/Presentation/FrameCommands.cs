using Intense.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Intense.Presentation
{
    /// <summary>
    /// Provides a set of commands for navigating a frame.
    /// </summary>
    public class FrameCommands
        : DependencyObject, IFrameNavigationEventSink
    {
        /// <summary>
        /// Identifies the Frame dependency property.
        /// </summary>
        public static DependencyProperty FrameProperty = DependencyProperty.Register("Frame", typeof(Frame), typeof(FrameCommands), new PropertyMetadata(null, OnFrameChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameCommands"/> class.
        /// </summary>
        public FrameCommands()
        {
            GoBackCommand = new RelayCommand(o => GoBack(), o => CanGoBack());
            GoForwardCommand = new RelayCommand(o => GoForward(), o => CanGoForward());
            GoHomeCommand = new RelayCommand(o => GoHome(), o => CanGoHome());
        }

        /// <summary>
        /// Gets or sets the target frame where the commands operate on.
        /// </summary>
        public Frame Frame
        {
            get => (Frame)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }

        /// <summary>
        /// The command for navigating to the most recent item in back navigation history.
        /// </summary>
        public Command GoBackCommand { get; }

        /// <summary>
        /// The command for navigating to the most recent item in forward navigation history
        /// </summary>
        public Command GoForwardCommand { get; }

        /// <summary>
        /// The command for navigating back to the first element in the frame's navigation stack
        /// </summary>
        public Command GoHomeCommand { get; }

        void IFrameNavigationEventSink.OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateCommandStates();
        }

        void IFrameNavigationEventSink.OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        void IFrameNavigationEventSink.OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
        }

        void IFrameNavigationEventSink.OnNavigationStopped(object sender, NavigationEventArgs e)
        {
        }

        private static void OnFrameChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ((FrameCommands)o).OnFrameChanged((Frame)args.OldValue, (Frame)args.NewValue);
        }

        private bool CanGoBack()
        {
            return Frame?.CanGoBack ?? false;
        }

        private bool CanGoForward()
        {
            return Frame?.CanGoForward ?? false;
        }

        private bool CanGoHome()
        {
            return CanGoBack();
        }

        private void GoBack()
        {
            if (CanGoBack())
            {
                Frame.GoBack();
            }
        }

        private void GoForward()
        {
            if (CanGoForward())
            {
                Frame.GoForward();
            }
        }

        private void GoHome()
        {
            if (CanGoHome())
            {
                while (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
        }

        private void OnFrameChanged(Frame oldFrame, Frame newFrame)
        {
            newFrame?.RegisterEventSink(this);

            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            GoBackCommand.OnCanExecuteChanged();
            GoForwardCommand.OnCanExecuteChanged();
            GoHomeCommand.OnCanExecuteChanged();
        }
    }
}