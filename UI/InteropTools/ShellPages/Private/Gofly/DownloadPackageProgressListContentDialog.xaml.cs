using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using static StoreProjectApp.ContentDialogs.DownloadPackageListContentDialog;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StoreProjectApp.ContentDialogs
{
    public sealed partial class DownloadPackageProgressListContentDialog : ContentDialog
    {

        private ObservableCollection<DownloadItem> DownloadItemList = new ObservableCollection<DownloadItem>();

        public class DownloadItem : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public string Symbol { get; set; }
            public string Type { get; set; }

            public string status { get; set; }

            public string Status
            {
                get { return status; }
                set
                {
                    if (status == value)
                        return;

                    status = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }

            private string progress { get; set; }

            public string Progress
            {
                get { return progress; }
                set
                {
                    if (progress == value)
                        return;

                    progress = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }


        public DownloadPackageProgressListContentDialog(IList<object> selectedItems, StorageFolder folder)
        {
            this.InitializeComponent();

            foreach (var item in selectedItems)
            {
                DownloadList.ItemsSource = DownloadItemList;
                DownloadItemList.Add(new DownloadItem { Name = ((Item)item).Name, Progress = "0", Symbol = ((Item)item).Symbol, Type = ((Item)item).Type, Status = "Idling" });
                Download(((Item)item).Uri, ((Item)item).Name, DownloadItemList.Count, folder);
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        DownloadOperation downloadOperation;
        CancellationTokenSource cancellationToken;
        Windows.Networking.BackgroundTransfer.BackgroundDownloader backgroundDownloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();

        public async void Download(string url, string localfilename, int progressindex, StorageFolder folder)
        {
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(localfilename, CreationCollisionOption.GenerateUniqueName);
                Uri durl = new Uri(url);
                downloadOperation = backgroundDownloader.CreateDownload(durl, file);

                Progress<DownloadOperation> progress = new Progress<DownloadOperation>((downloadOperation) =>
                {
                    int progress2 = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
                    DownloadItemList[progressindex - 1].Status = (String.Format("{0} of {1} kb. downloaded - {2}% complete.", downloadOperation.Progress.BytesReceived / 1024, downloadOperation.Progress.TotalBytesToReceive / 1024, progress2));

                    DownloadItemList[progressindex - 1].Progress = progress2.ToString();

                    switch (downloadOperation.Progress.Status)
                    {
                        case BackgroundTransferStatus.Running:
                            {
                                break;
                            }
                        case BackgroundTransferStatus.PausedByApplication:
                            {

                                break;
                            }
                        case BackgroundTransferStatus.PausedCostedNetwork:
                            {

                                break;
                            }
                        case BackgroundTransferStatus.PausedNoNetwork:
                            {

                                break;
                            }
                        case BackgroundTransferStatus.Error:
                            {
                                DownloadItemList[progressindex - 1].Status = "An error occured while downloading.";
                                break;
                            }
                    }
                    if (progress2 >= 100)
                    {
                        downloadOperation = null;
                    }
                });
                cancellationToken = new CancellationTokenSource();

                try
                {
                    DownloadItemList[progressindex - 1].Status = "Initializing...";
                    await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
                }
                catch (TaskCanceledException)
                {
                    downloadOperation.ResultFile.DeleteAsync();
                    downloadOperation = null;
                }
                catch (System.Exception)
                {
                    //403
                    downloadOperation.ResultFile.DeleteAsync();
                    downloadOperation = null;
                }
            }
        }
    }
}
