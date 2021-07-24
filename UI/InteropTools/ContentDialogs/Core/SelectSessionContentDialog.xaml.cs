using System;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using static InteropTools.App;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Core
{
    internal sealed partial class SelectSessionContentDialog : ContentDialog
    {
        public SelectSessionContentDialog()
        {
            InitializeComponent();
            SessionList.ItemsSource = Sessions.Select(x => new DisplayItem(x));
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
            AddNewSession("");
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void SessionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Hide();
            SwitchSession(((DisplayItem)e.ClickedItem).session);
        }

        public class DisplayItem
        {
            public DisplayItem(Session session)
            {
                this.session = session;
            }

            public string Symbol => session.Helper.GetSymbol();

            public string DisplayName => session.Helper.GetFriendlyName() + " (" + session.CreationDate + ")";

            public string Description => session.Helper.GetTitle() + ResourceManager.Current.MainResourceMap.GetValue("Resources/_connected_to_", ResourceContext.GetForCurrentView()).ValueAsString + session.Helper.GetHostName();

            public object Preview
            {
                get
                {
                    if (Sessions[(int)CurrentSession] == session)
                    {
                        return null;
                    }

                    return session.Preview;
                }
            }

            public Visibility SelectedVisibility
            {
                get
                {
                    if (Sessions[(int)CurrentSession] == session)
                    {
                        return Visibility.Visible;
                    }

                    return Visibility.Collapsed;
                }
            }

            public Visibility PreviewVisibility
            {
                get
                {
                    if (Sessions[(int)CurrentSession] == session)
                    {
                        return Visibility.Collapsed;
                    }

                    return Visibility.Visible;
                }
            }

            public Session session { get; internal set; }
        }

        private async void Test()
        {
            //Capture the screen (this) and apply no transformation<>
            RenderTargetBitmap renderTargetBitmap = new();
            await renderTargetBitmap.RenderAsync(this, (int)ActualWidth, (int)ActualHeight);
        }
    }
}