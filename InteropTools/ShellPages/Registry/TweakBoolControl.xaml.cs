using System;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class TweakBoolControl : UserControl
    {
        private readonly Action<bool> _apply;
        private readonly Func<bool> _check;

        private bool _initialized;

        public TweakBoolControl(Func<bool> check, Action<bool> apply, string title, string description)
        {
            InitializeComponent();
            TitleBox.Text = title;
            DescBox.Text = description;
            _apply = apply;
            _check = check;
            RunInThreadPool(DoChecks);
        }

        private void DoChecks()
        {
            _initialized = false;
            bool result = _check.Invoke();
            RunInUIThread(() =>
            {
                MainSwitch.IsOn = result;
                _initialized = true;
            });
        }

        private void MainSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            bool state = MainSwitch.IsOn;
            RunInThreadPool(() =>
            {
                _apply(state);
                DoChecks();
            });
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async void RunInUIThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }
    }
}