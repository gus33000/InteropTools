using Intense.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Intense.Presentation
{
    /// <summary>
    /// A set of commands for managing the current application view.
    /// </summary>
    public class ApplicationViewCommands
        : IWindowEventSink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationViewCommands"/> class.
        /// </summary>
        public ApplicationViewCommands()
        {
            var view = ApplicationView.GetForCurrentView();

            this.EnterFullScreenModeCommand = new RelayCommand(o => view.TryEnterFullScreenMode(), o => !view.IsFullScreenMode);
            this.ExitFullScreenModeCommand = new RelayCommand(o => view.ExitFullScreenMode(), o => view.IsFullScreenMode);

            Window.Current.RegisterEventSink(this);
        }

        void IWindowEventSink.OnActivated(object sender, WindowActivatedEventArgs e)
        {
        }

        void IWindowEventSink.OnClosed(object sender, CoreWindowEventArgs e)
        {
        }

        void IWindowEventSink.OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.EnterFullScreenModeCommand.OnCanExecuteChanged();
            this.ExitFullScreenModeCommand.OnCanExecuteChanged();
        }

        void IWindowEventSink.OnVisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
        }

        /// <summary>
        /// The command for entering full screen mode.
        /// </summary>
        public Command EnterFullScreenModeCommand { get; }
        /// <summary>
        /// The command for exiting full screen mode.
        /// </summary>
        public Command ExitFullScreenModeCommand { get; }
    }
}
