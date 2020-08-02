using Intense.Resources;
using Intense.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
            this.GoBackCommand = new RelayCommand(o => GoBack(), o => CanGoBack());
            this.GoForwardCommand = new RelayCommand(o => GoForward(), o => CanGoForward());
            this.GoHomeCommand = new RelayCommand(o => GoHome(), o => CanGoHome());
        }

        private static void OnFrameChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ((FrameCommands)o).OnFrameChanged((Frame)args.OldValue, (Frame)args.NewValue);
        }

        private void OnFrameChanged(Frame oldFrame, Frame newFrame)
        {
            if (newFrame != null) {
                newFrame.RegisterEventSink(this);
            }

            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            this.GoBackCommand.OnCanExecuteChanged();
            this.GoForwardCommand.OnCanExecuteChanged();
            this.GoHomeCommand.OnCanExecuteChanged();
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
        /// <summary>
        /// Gets or sets the target frame where the commands operate on.
        /// </summary>
        public Frame Frame
        {
            get { return (Frame)GetValue(FrameProperty); }
            set { SetValue(FrameProperty, value); }
        }

        private bool CanGoBack()
        {
            return this.Frame?.CanGoBack ?? false;
        }

        private void GoBack()
        {
            if (CanGoBack()) {
                this.Frame.GoBack();
            }
        }

        private bool CanGoForward()
        {
            return this.Frame?.CanGoForward ?? false;
        }

        private void GoForward()
        {
            if (CanGoForward()) {
                this.Frame.GoForward();
            }
        }

        private bool CanGoHome()
        {
            return CanGoBack();
        }

        private void GoHome()
        {
            if (CanGoHome()) {
                while (this.Frame.CanGoBack) {
                    this.Frame.GoBack();
                }
            }
        }

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
    }
}
