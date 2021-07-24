using InteropTools.ContentDialogs.AppManager;
using InteropTools.CorePages;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Management.Deployment;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.AppManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppProductPage : Page
    {

        public class Item
        {
            public string DisplayName { get; set; }
            public string Description { get; set; }

            public string FullName { get; set; }
        }


        private readonly ObservableCollection<Item> ItemsList = new();

        private Package _package;

        public AppProductPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string pfn = e.Parameter as string;
            GatherInfos(pfn);
        }

        public async void GatherInfos(string fullname)
        {
            PackageManager PackageMan = new();

            try
            {
                Package package = PackageMan.FindPackageForUser("", fullname);
                _package = package;

                try
                {
                    Author.Text = package.Id.Author;

                    if (Author.Text.Trim() == "")
                    {
                        AuthorHeader.Visibility = Visibility.Collapsed;
                        Author.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    AuthorHeader.Visibility = Visibility.Collapsed;
                    Author.Visibility = Visibility.Collapsed;
                }

                try
                {
                    string arch = ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;

                    switch (package.Id.Architecture)
                    {
                        case ProcessorArchitecture.Arm:
                            {
                                arch = "ARM";
                                break;
                            }

                        case ProcessorArchitecture.Neutral:
                            {
                                arch = ResourceManager.Current.MainResourceMap.GetValue("Resources/Neutral", ResourceContext.GetForCurrentView()).ValueAsString;
                                break;
                            }

                        case ProcessorArchitecture.Unknown:
                            {
                                arch = ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;
                                break;
                            }

                        case ProcessorArchitecture.X64:
                            {
                                arch = "x64";
                                break;
                            }

                        case ProcessorArchitecture.X86:
                            {
                                arch = "x86";
                                break;
                            }
                    }

                    Architecture.Text = arch;

                    if (Architecture.Text.Trim() == "")
                    {
                        ArchitectureHeader.Visibility = Visibility.Collapsed;
                        Architecture.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    ArchitectureHeader.Visibility = Visibility.Collapsed;
                    Architecture.Visibility = Visibility.Collapsed;
                }

                try
                {
                    FamilyName.Text = package.Id.FamilyName;
                    AppTitle.Text = package.Id.FamilyName;

                    if (FamilyName.Text.Trim() == "")
                    {
                        FamilyNameHeader.Visibility = Visibility.Collapsed;
                        FamilyName.Visibility = Visibility.Collapsed;
                        OpenStore.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    FamilyNameHeader.Visibility = Visibility.Collapsed;
                    FamilyName.Visibility = Visibility.Collapsed;
                }

                try
                {
                    FullName.Text = package.Id.FullName;

                    if (FullName.Text.Trim() == "")
                    {
                        FullNameHeader.Visibility = Visibility.Collapsed;
                        FullName.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    FullNameHeader.Visibility = Visibility.Collapsed;
                    FullName.Visibility = Visibility.Collapsed;
                }

                try
                {
                    PName.Text = package.Id.Name;

                    if (PName.Text.Trim() == "")
                    {
                        NameHeader.Visibility = Visibility.Collapsed;
                        PName.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    NameHeader.Visibility = Visibility.Collapsed;
                    PName.Visibility = Visibility.Collapsed;
                }

                try
                {
                    ProductID.Text = package.Id.ProductId;

                    if (ProductID.Text.Trim() == "")
                    {
                        ProductIDHeader.Visibility = Visibility.Collapsed;
                        ProductID.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    ProductIDHeader.Visibility = Visibility.Collapsed;
                    ProductID.Visibility = Visibility.Collapsed;
                }

                try
                {
                    Publisher.Text = package.Id.Publisher;

                    if (Publisher.Text.Trim() == "")
                    {
                        PublisherHeader.Visibility = Visibility.Collapsed;
                        Publisher.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    PublisherHeader.Visibility = Visibility.Collapsed;
                    Publisher.Visibility = Visibility.Collapsed;
                }

                try
                {
                    PublisherID.Text = package.Id.PublisherId;

                    if (PublisherID.Text.Trim() == "")
                    {
                        PublisherIDHeader.Visibility = Visibility.Collapsed;
                        PublisherID.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    PublisherIDHeader.Visibility = Visibility.Collapsed;
                    PublisherID.Visibility = Visibility.Collapsed;
                }

                try
                {
                    ResourceID.Text = package.Id.ResourceId;

                    if (ResourceID.Text.Trim() == "")
                    {
                        ResourceIDHeader.Visibility = Visibility.Collapsed;
                        ResourceID.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    ResourceIDHeader.Visibility = Visibility.Collapsed;
                    ResourceID.Visibility = Visibility.Collapsed;
                }

                try
                {
                    Version.Text = package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
                                   package.Id.Version.Build + "." + package.Id.Version.Revision;

                    if (Version.Text.Trim() == "...")
                    {
                        VersionHeader.Visibility = Visibility.Collapsed;
                        Version.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    VersionHeader.Visibility = Visibility.Collapsed;
                    Version.Visibility = Visibility.Collapsed;
                }

                try
                {
                    Description.Text = package.Description;

                    if (Description.Text.Trim() == "")
                    {
                        DescriptionHeader.Visibility = Visibility.Collapsed;
                        Description.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    DescriptionHeader.Visibility = Visibility.Collapsed;
                    Description.Visibility = Visibility.Collapsed;
                }

                try
                {
                    DisplayName.Text = package.DisplayName;

                    if (DisplayName.Text.Trim() == "")
                    {
                        DisplayNameHeader.Visibility = Visibility.Collapsed;
                        DisplayName.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    DisplayNameHeader.Visibility = Visibility.Collapsed;
                    DisplayName.Visibility = Visibility.Collapsed;
                }

                try
                {
                    InstalledDate.Text = package.InstalledDate.LocalDateTime.ToString();

                    if (InstalledDate.Text.Trim() == "")
                    {
                        InstalledDateHeader.Visibility = Visibility.Collapsed;
                        InstalledDate.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    InstalledDateHeader.Visibility = Visibility.Collapsed;
                    InstalledDate.Visibility = Visibility.Collapsed;
                }

                try
                {
                    InstalledLocation.Text = package.InstalledLocation.Path;

                    if (InstalledLocation.Text.Trim() == "")
                    {
                        InstalledLocationHeader.Visibility = Visibility.Collapsed;
                        InstalledLocation.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    InstalledLocationHeader.Visibility = Visibility.Collapsed;
                    InstalledLocation.Visibility = Visibility.Collapsed;
                }

                try
                {
                    IsBundle.Text = package.IsBundle.ToString();

                    if (IsBundle.Text.Trim() == "")
                    {
                        IsBundleHeader.Visibility = Visibility.Collapsed;
                        IsBundle.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    IsBundleHeader.Visibility = Visibility.Collapsed;
                    IsBundle.Visibility = Visibility.Collapsed;
                }

                try
                {
                    IsDevelopmentMode.Text = package.IsDevelopmentMode.ToString();

                    if (IsDevelopmentMode.Text.Trim() == "")
                    {
                        IsDevelopmentModeHeader.Visibility = Visibility.Collapsed;
                        IsDevelopmentMode.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    IsDevelopmentModeHeader.Visibility = Visibility.Collapsed;
                    IsDevelopmentMode.Visibility = Visibility.Collapsed;
                }

                try
                {
                    IsFramework.Text = package.IsFramework.ToString();

                    if (IsFramework.Text.Trim() == "")
                    {
                        IsFrameworkHeader.Visibility = Visibility.Collapsed;
                        IsFramework.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    IsFrameworkHeader.Visibility = Visibility.Collapsed;
                    IsFramework.Visibility = Visibility.Collapsed;
                }

                try
                {
                    IsResourcePackage.Text = package.IsResourcePackage.ToString();

                    if (IsResourcePackage.Text.Trim() == "")
                    {
                        IsResourcePackageHeader.Visibility = Visibility.Collapsed;
                        IsResourcePackage.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    IsResourcePackageHeader.Visibility = Visibility.Collapsed;
                    IsResourcePackage.Visibility = Visibility.Collapsed;
                }

                try
                {
                    Logo.Text = package.Logo.PathAndQuery;

                    if (Logo.Text.Trim() == "")
                    {
                        LogoHeader.Visibility = Visibility.Collapsed;
                        Logo.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    LogoHeader.Visibility = Visibility.Collapsed;
                    Logo.Visibility = Visibility.Collapsed;
                }

                try
                {
                    PublisherDisplayName.Text = package.PublisherDisplayName;

                    if (PublisherDisplayName.Text.Trim() == "")
                    {
                        PublisherDisplayNameHeader.Visibility = Visibility.Collapsed;
                        PublisherDisplayName.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    PublisherDisplayNameHeader.Visibility = Visibility.Collapsed;
                    PublisherDisplayName.Visibility = Visibility.Collapsed;
                }

                try
                {
                    string statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Ok", ResourceContext.GetForCurrentView()).ValueAsString;

                    if (package.Status.DataOffline)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Data_Offline", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                        if (package.Status.DependencyIssue)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Dependency_Issue", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                            if (package.Status.DeploymentInProgress)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Deployment_In_Progress", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                if (package.Status.Disabled)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Disabled", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                    if (package.Status.LicenseIssue)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/License_Issue", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                        if (package.Status.Modified)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Modified", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                            if (package.Status.NeedsRemediation)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Needs_Remediation", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                                if (package.Status.NotAvailable)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Not_Available", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                                    if (package.Status.PackageOffline)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Package_Offline", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                                        if (package.Status.Servicing)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Servicing", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                                                            if (package.Status.Tampered)
                    {
                        statustext = ResourceManager.Current.MainResourceMap.GetValue("Resources/Tampered", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    Status.Text = statustext;

                    if (Status.Text.Trim() == "")
                    {
                        StatusHeader.Visibility = Visibility.Collapsed;
                        Status.Visibility = Visibility.Collapsed;
                    }
                }

                catch
                {
                    StatusHeader.Visibility = Visibility.Collapsed;
                    Status.Visibility = Visibility.Collapsed;
                }

                AppDesc.Text = Architecture.Text + " " + Version.Text;

                try
                {
                    if (ItemsList.Count != 0)
                    {
                        for (int i = ItemsList.Count - 1; i >= 0; i--)
                        {
                            ItemsList.RemoveAt(i);
                        }
                    }

                    foreach (Package dependency in package.Dependencies)
                    {
                        DepTitle.Visibility = Visibility.Visible;
                        string arch = ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;

                        switch (dependency.Id.Architecture)
                        {
                            case ProcessorArchitecture.Arm:
                                {
                                    arch = "ARM";
                                    break;
                                }

                            case ProcessorArchitecture.Neutral:
                                {
                                    arch = ResourceManager.Current.MainResourceMap.GetValue("Resources/Neutral", ResourceContext.GetForCurrentView()).ValueAsString;
                                    break;
                                }

                            case ProcessorArchitecture.Unknown:
                                {
                                    arch = ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;
                                    break;
                                }

                            case ProcessorArchitecture.X64:
                                {
                                    arch = "x64";
                                    break;
                                }

                            case ProcessorArchitecture.X86:
                                {
                                    arch = "x86";
                                    break;
                                }
                        }

                        ItemsList.Add(new Item
                        {
                            DisplayName = dependency.Id.FamilyName,
                            FullName = dependency.Id.FullName,
                            Description =
                            arch + " " + dependency.Id.Version.Major + "." + dependency.Id.Version.Minor + "." +
                            dependency.Id.Version.Build + "." + dependency.Id.Version.Revision
                        });
                    }

                    DependenciesList.ItemsSource = ItemsList;
                }

                catch (Exception caughtEx)
                {
                    await
                    new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(ResourceManager.Current.MainResourceMap.GetValue("Resources/Exception_Thrown__",
                        ResourceContext.GetForCurrentView()).ValueAsString + caughtEx.HResult + " " + caughtEx.Message);
                }

                string displayname = "";
                string description = "";
                dynamic logo = "";

                try
                {
                    System.Collections.Generic.IReadOnlyList<Windows.ApplicationModel.Core.AppListEntry> AppEntries = await package.GetAppListEntriesAsync();

                    foreach (Windows.ApplicationModel.Core.AppListEntry AppEntry in AppEntries)
                    {
                        try
                        {
                            displayname = AppEntry.DisplayInfo.DisplayName;
                            AppTitle.Text = displayname;

                            if (DisplayName.Text.Trim() == "")
                            {
                                DisplayName.Text = displayname;
                                DisplayName.Visibility = Visibility.Visible;
                                DisplayNameHeader.Visibility = Visibility.Visible;
                            }
                        }

                        catch
                        {
                            DisplayName.Visibility = Visibility.Collapsed;
                            DisplayNameHeader.Visibility = Visibility.Collapsed;
                        }

                        try
                        {
                            description = AppEntry.DisplayInfo.Description;
                            AppDesc.Text = description;

                            if (Description.Text.Trim() == "")
                            {
                                Description.Text = description;
                                Description.Visibility = Visibility.Visible;
                                DescriptionHeader.Visibility = Visibility.Visible;
                            }
                        }

                        catch
                        {
                            Description.Visibility = Visibility.Collapsed;
                            DescriptionHeader.Visibility = Visibility.Collapsed;
                        }

                        try
                        {
                            Size logosize = new()
                            {
                                Height = 175,
                                Width = 175
                            };
                            Windows.Storage.Streams.RandomAccessStreamReference applogo = AppEntry.DisplayInfo.GetLogo(logosize);
                            BitmapImage bitmapImage = new();
                            Windows.Storage.Streams.IRandomAccessStreamWithContentType ras = await applogo.OpenReadAsync();
                            bitmapImage.SetSource(ras);
                            logo = bitmapImage;
                            AppLogo.Source = logo;
                            //Create a transform to get a 1x1 image
                            BitmapTransform myTransform = new() { ScaledHeight = 1, ScaledWidth = 1 };
                            BitmapDecoder dec = await BitmapDecoder.CreateAsync(await applogo.OpenReadAsync());
                            PixelDataProvider data = await dec.GetPixelDataAsync(BitmapPixelFormat.Rgba8,
                                                                   BitmapAlphaMode.Ignore,
                                                                   myTransform,
                                                                   ExifOrientationMode.IgnoreExifOrientation,
                                                                   ColorManagementMode.DoNotColorManage);
                            byte[] bytes = data.DetachPixelData();
                            Color myDominantColor = Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);

                            if ((myDominantColor != Colors.Black) && (myDominantColor != Colors.White))
                            {
                                HeaderPanel.Fill = new SolidColorBrush(myDominantColor);
                                HeaderPanel2.Fill = new SolidColorBrush(myDominantColor);
                            }
                        }

                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
            catch
            {
                (App.AppContent as Shell).RootFrame.GoBack();
            }
        }

        private async void DependenciesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Hide();
            Item SelectedItem = (Item)e.ClickedItem;
            await new AppPackageContentDialog(SelectedItem.FullName).ShowAsync();
            //await ShowAsync();
        }

        private async void Value_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock SelectedItem = (TextBlock)e.OriginalSource;
            DataPackage dataPackage = new() { RequestedOperation = DataPackageOperation.Copy };
            dataPackage.SetText(SelectedItem.Text);
            Clipboard.SetContent(dataPackage);
            //this.Hide();
            await
            new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(SelectedItem.Text + "\nThe above value was copied to your clipboard", "Package Info");
            // await this.ShowAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?PFN=" + _package.Id.FamilyName));
            }

            catch
            {
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Collections.Generic.IReadOnlyList<Windows.ApplicationModel.Core.AppListEntry> results = await _package.GetAppListEntriesAsync();
                await results[0].LaunchAsync();
            }

            catch
            {
            }
        }
    }
}
