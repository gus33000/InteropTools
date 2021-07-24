using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.CorePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchedForResultsPage : Page
    {
        private Windows.System.ProtocolForResultsOperation _operation = null;
        private ProtocolForResultsActivatedEventArgs protocolForResultsArgs = null;

        public class ApplicationAccess
        {
            public string PFN { get; set; }
            public List<string> AllowedRegAPIs { get; set; }
            public string Token { get; set; }
            public string ProviderName { get; set; }
        }

        public LaunchedForResultsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            protocolForResultsArgs = e.Parameter as ProtocolForResultsActivatedEventArgs;
            // Set the ProtocolForResultsOperation field.
            _operation = protocolForResultsArgs.ProtocolForResultsOperation;

            Title1.Text = "To access the following priviledged APIs, " + protocolForResultsArgs.CallerPackageFamilyName + " needs your permission in order to prevent unwanted modifications to your device.";
            Title2.Text = protocolForResultsArgs.CallerPackageFamilyName + " wants to access the following APIs";
        }

        private void AllowButton_Click(object sender, RoutedEventArgs e)
        {
            if (protocolForResultsArgs.Data.ContainsKey("TestData"))
            {
                string dataFromCaller = protocolForResultsArgs.Data["TestData"] as string;
            }

            ValueSet result = new ValueSet
            {
                ["ReturnedData"] = "The returned result"
            };
            _operation.ReportCompleted(result);
        }

        private void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            if (protocolForResultsArgs.Data.ContainsKey("TestData"))
            {
                string dataFromCaller = protocolForResultsArgs.Data["TestData"] as string;
            }

            ValueSet result = new ValueSet
            {
                ["ReturnedData"] = "The returned result"
            };
            _operation.ReportCompleted(result);
        }
    }
}
