using System;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class TweakInputControl : UserControl
    {
        private readonly Action<string> _apply;
        private readonly Func<string> _check;

        private bool _initialized;

        public TweakInputControl(Func<string> check, Action<string> apply, string title, string description)
        {
            InitializeComponent();
            TitleBox.Text = title;
            DescBox.Text = description;
            _apply = apply;
            _check = check;
            RunInThreadPool(DoChecks);
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private static async void RunInUiThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private void DoChecks()
        {
            _initialized = false;
            string result = _check();
            RunInUiThread(() =>
            {
                InputBox.Text = result;
                _initialized = true;
            });
        }

        private void SetValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            string state = InputBox.Text;
            RunInThreadPool(() =>
            {
                _apply(state);
                DoChecks();
            });
        }
    }
}