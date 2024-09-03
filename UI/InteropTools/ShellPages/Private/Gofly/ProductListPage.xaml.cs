using Flurl.Http;
using MyToolkit.Model;
using StoreProjectApp.ContentDialogs;
using StoreProjectApp.Presentation;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StoreProjectApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductListPage : Page
    {

        public ProductListPage()
        {
            this.InitializeComponent();
            Model.PropertyChanged += (sender, args) =>
            {
                if (args.IsProperty<ProductListPageModel>(m => m.Filter))
                {
                    DataGrid.SetFilter<Product>(p =>
                        p.ProductID.ToLower().Contains(Model.Filter.ToLower()) ||
                        p.Name.ToLower().Contains(Model.Filter.ToLower()) ||
                        p.Description.ToLower().Contains(Model.Filter.ToLower()));
                }
                Status.Text = string.Format("Loaded {0} Products.", Model.Product.Length);
                Ring.IsActive = false;
                DataGrid.OrderChanged += (sender2, args2) =>
                {
                    Status.Text = string.Format(
                        "Loaded {2} Products. Items are ordered by {0} ({1})",
                        DataGrid.SelectedColumn.OrderPropertyPath.Path,
                        (DataGrid.SelectedColumn.IsAscending ? "Ascending" : "Descending"),
                        Model.Product.Length
                    );
                };
            };
        }

        public ProductListPageModel Model
        {
            get { return (ProductListPageModel)Resources["ViewModel"]; }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DownloadPackageListContentDialog((Product)DataGrid.SelectedItem);
            await dialog.ShowAsync();
        }

        private async void OpeninStoreButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store://pdp/?ProductId={0}", ((Product)DataGrid.SelectedItem).ProductID)));
        }

        private async void CopyProdIDButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            dataPackage.SetText(((Product)DataGrid.SelectedItem).ProductID);
            Clipboard.SetContent(dataPackage);
            var dialog = new ContentDialog
            {
                Title = "We copied the following value to your clipboard.",
                Content = ((Product)DataGrid.SelectedItem).ProductID,
                PrimaryButtonText = "Ok"
            };
            await dialog.ShowAsync();
        }

        private async void DownloadNewButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, "", "Production", "Retail");
            
            await dialog.ShowAsync();
        }

        private async void DownloadNewWIFButton_Click(object sender, RoutedEventArgs e)
        {

            //var token = (await "http://pastebin.com/raw/ai8LteXW".GetStringAsync()).Replace("\r", "").Replace("\n", "");
            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();
            
            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, token, "WIF");

            await dialog.ShowAsync();
        }

        private async void DownloadNewWIFSButton_Click(object sender, RoutedEventArgs e)
        {

            //var token = (await "http://pastebin.com/raw/ai8LteXW".GetStringAsync()).Replace("\r", "").Replace("\n", "");
            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();

            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, token, "WIF", "Skip");

            await dialog.ShowAsync();
        }

        private async void DownloadNewIRPButton_Click(object sender, RoutedEventArgs e)
        {

            //var token = (await "http://pastebin.com/raw/ai8LteXW".GetStringAsync()).Replace("\r", "").Replace("\n", "");
            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();

            //var token = "";

            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, token, "RP");

            await dialog.ShowAsync();
        }

        private async void DownloadNewWISButton_Click(object sender, RoutedEventArgs e)
        {

            //var token = (await "http://pastebin.com/raw/ai8LteXW".GetStringAsync()).Replace("\r", "").Replace("\n", "");
            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();

            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, token, "WIS");

            await dialog.ShowAsync();
        }

        private async void DownloadNewSHButton_Click(object sender, RoutedEventArgs e)
        {

            //var token = (await "http://pastebin.com/raw/ai8LteXW".GetStringAsync()).Replace("\r", "").Replace("\n", "");
            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();

            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, token, "OSG", "Branch");

            await dialog.ShowAsync();
        }

        private async void DownloadNewCanaryButton_Click(object sender, RoutedEventArgs e)
        {

            //var token = (await "http://pastebin.com/raw/ai8LteXW".GetStringAsync()).Replace("\r", "").Replace("\n", "");
            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();

            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, token, "Canary", "Branch");

            await dialog.ShowAsync();
        }

        private async void DownloadNewMSITButton_Click(object sender, RoutedEventArgs e)
        {
            //var token = (await "http://pastebin.com/raw/ai8LteXW".GetStringAsync()).Replace("\r", "").Replace("\n", "");
            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();

            var dialog = new ContentDialogs.NEW.DownloadPackageListContentDialog((Product)DataGrid.SelectedItem, token, "MSIT");

            await dialog.ShowAsync();
        }
    }
}
