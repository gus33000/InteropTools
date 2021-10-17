// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Intense.UI;
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
            ApplicationView view = ApplicationView.GetForCurrentView();

            EnterFullScreenModeCommand =
                new RelayCommand(o => view.TryEnterFullScreenMode(), o => !view.IsFullScreenMode);
            ExitFullScreenModeCommand = new RelayCommand(o => view.ExitFullScreenMode(), o => view.IsFullScreenMode);

            Window.Current.RegisterEventSink(this);
        }

        /// <summary>
        /// The command for entering full screen mode.
        /// </summary>
        public Command EnterFullScreenModeCommand { get; }

        /// <summary>
        /// The command for exiting full screen mode.
        /// </summary>
        public Command ExitFullScreenModeCommand { get; }

        void IWindowEventSink.OnActivated(object sender, WindowActivatedEventArgs e)
        {
        }

        void IWindowEventSink.OnClosed(object sender, CoreWindowEventArgs e)
        {
        }

        void IWindowEventSink.OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            EnterFullScreenModeCommand.OnCanExecuteChanged();
            ExitFullScreenModeCommand.OnCanExecuteChanged();
        }

        void IWindowEventSink.OnVisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
        }
    }
}
