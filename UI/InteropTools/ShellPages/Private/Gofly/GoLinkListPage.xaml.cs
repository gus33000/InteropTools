using MyToolkit.Model;
using StoreProjectApp.Presentation;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StoreProjectApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GoLinkListPage : Page
    {
        public GoLinkListPage()
        {
            this.InitializeComponent();
            PauseButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            Model.PropertyChanged += (sender, args) =>
            {
                StartIDTextBox.Text = Model.GoLink[0].ID;
                if (args.IsProperty<GoLinkListPageModel>(m => m.Filter))
                {
                    DataGrid.SetFilter<Update>(p =>
                        p.ID.ToLower().Contains(Model.Filter.ToLower()) ||
                        p.Link.ToLower().Contains(Model.Filter.ToLower()));
                }
            };
        }




        public GoLinkListPageModel Model
        {
            get { return (GoLinkListPageModel)Resources["ViewModel"]; }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Model.DoIt(int.Parse(StartIDTextBox.Text), int.Parse(EndIDTextBox.Text));
            StartButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
        }

        private async void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            Model.pause = true;
            PauseButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            await Task.Delay(TimeSpan.FromSeconds(10));
            Model.pause = false;
        }
    }
}
