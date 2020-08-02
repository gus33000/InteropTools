using InteropTools.ContentDialogs.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            this.InitializeComponent();
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

            ValueSet result = new ValueSet();
            result["ReturnedData"] = "The returned result";
            _operation.ReportCompleted(result);
        }

        private void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            if (protocolForResultsArgs.Data.ContainsKey("TestData"))
            {
                string dataFromCaller = protocolForResultsArgs.Data["TestData"] as string;
            }

            ValueSet result = new ValueSet();
            result["ReturnedData"] = "The returned result";
            _operation.ReportCompleted(result);
        }
    }
}
