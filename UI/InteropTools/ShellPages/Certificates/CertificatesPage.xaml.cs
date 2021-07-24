using InteropTools.ContentDialogs.Certificates;
using InteropTools.CorePages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace InteropTools.ShellPages.Certificates
{
    public sealed partial class CertificatesPage : Page
    {
        public string PageName => "Certificates";
        public PageGroup PageGroup => PageGroup.General;

        private readonly List<string> _knownCertificateStores = new List<string>
        {
            "TrustedPublisher",
            "ClientAuthIssuer",
            "Root",
            "MSIEHistoryJournal",
            "CA",
            "UserDS",
            "REQUEST",
            "AuthRoot",
            "TrustedPeople",
            "Local NonRemovable Certificates",
            "My",
            "SmartCardRoot",
            "Trust",
            "Disallowed",
            "Remote Desktop",
            "Windows Web Management",
            "TrustedDevices",
            "Windows Live ID Token Issuer",
            "AAD Token Issuer",
            "FlightRoot",
            "Homegroup Machine Certificates"
        };

        private readonly ObservableCollection<Certificate> _maincertList = new ObservableCollection<Certificate>();

        public CertificatesPage()
        {
            InitializeComponent();
            Refresh();
        }

        private void CertificatesPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private async void Refresh()
        {
            if (LocalMachinePivot.Items == null)
            {
                return;
            }

            LocalMachinePivot.Items.Clear();

            if (CurrentUserPivot.Items == null)
            {
                return;
            }

            CurrentUserPivot.Items.Clear();

            foreach (string certStore in _knownCertificateStores)
            {
                try
                {
                    CertificateQuery query = new CertificateQuery { StoreName = certStore };
                    IReadOnlyList<Certificate> certificates = await CertificateStores.FindAllAsync(query);
                    List<Certificate> localMachineCerts = new List<Certificate>();
                    List<Certificate> currentUserCerts = new List<Certificate>();

                    foreach (Certificate cert in certificates)
                    {
                        try
                        {
                            if (cert.IsPerUser)
                            {
                                currentUserCerts.Add(cert);
                            }

                            else
                            {
                                localMachineCerts.Add(cert);
                            }
                        }

                        catch
                        {
                            localMachineCerts.Add(cert);
                        }
                    }

                    if (localMachineCerts.Count != 0)
                    {
                        PivotItem lmpivot = new PivotItem { Header = GetFriendlyStoreName(certStore) };
                        LocalMachinePivot.Items.Add(lmpivot);
                        ListView listview = new ListView
                        {
                            ItemTemplate = CertListViewTemplate,
                            IsItemClickEnabled = true,
                            SelectionMode = ListViewSelectionMode.None
                        };
                        listview.ItemClick += Listview_ItemClick;
                        ObservableCollection<DisplayCertificate> certList = new ObservableCollection<DisplayCertificate>();
                        listview.ItemsSource = certList;
                        lmpivot.Content = listview;

                        foreach (Certificate cert in localMachineCerts)
                        {
                            DisplayCertificate dispCert = new DisplayCertificate(cert) { Index = _maincertList.Count.ToString() };
                            _maincertList.Add(cert);
                            certList.Add(dispCert);
                        }
                    }

                    if (currentUserCerts.Count == 0)
                    {
                        continue;
                    }

                    {
                        PivotItem cupivot = new PivotItem { Header = GetFriendlyStoreName(certStore) };
                        CurrentUserPivot.Items.Add(cupivot);
                        ListView listview = new ListView
                        {
                            ItemTemplate = CertListViewTemplate,
                            IsItemClickEnabled = true,
                            SelectionMode = ListViewSelectionMode.None
                        };
                        listview.ItemClick += Listview_ItemClick;
                        ObservableCollection<DisplayCertificate> certList = new ObservableCollection<DisplayCertificate>();
                        listview.ItemsSource = certList;
                        cupivot.Content = listview;

                        foreach (Certificate cert in currentUserCerts)
                        {
                            DisplayCertificate dispCert = new DisplayCertificate(cert) { Index = _maincertList.Count.ToString() };
                            _maincertList.Add(cert);
                            certList.Add(dispCert);
                        }
                    }
                }

                catch
                {
                    // ignored
                }
            }
        }

        private string GetFriendlyStoreName(string storename)
        {
            switch (storename)
            {
                case "TrustedPublisher":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Trusted_Publishers", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "ClientAuthIssuer":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Client_Authentication_Issuer", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "Root":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Trusted_Root_Certification_Authorities", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "CA":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Intermediate_Certification_Authorities", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "UserDS":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/User_Directory_Store", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "REQUEST":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Certificate_Enrollment_Requests", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "AuthRoot":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Thrid_Party_Root_Certification_Authorities", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "TrustedPeople":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Trusted_People", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "My":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Personal", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "SmartCardRoot":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Smart_Card_Trusted_Roots", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "Trust":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Trusted_Certificates", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "Disallowed":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Untrusted_Certificates", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "TrustedDevices":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Trusted_Devices", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case "FlightRoot":
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Preview_Build_Roots", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                default:
                    {
                        return storename;
                    }
            }
        }

        private async void Listview_ItemClick(object sender, ItemClickEventArgs e)
        {
            DisplayCertificate selectedItem = (DisplayCertificate)e.ClickedItem;
            Certificate cert = selectedItem.cert;
            await new CertificateDetailsContentDialog(cert, _maincertList).ShowAsync();
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StackPanel sndr = (StackPanel)sender;
            string index = ((TextBlock)sndr.Children[0]).Text;
            Certificate selectedItem = _maincertList[int.Parse(index)];
            MenuFlyout flyout = new MenuFlyout { Placement = FlyoutPlacementMode.Top };
            MenuFlyoutItem flyoutitem1 = new MenuFlyoutItem
            {
                Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Delete", ResourceContext.GetForCurrentView()).ValueAsString
            };
            flyoutitem1.Click += async (sender_, e_) =>
            {
                try
                {
                    if (selectedItem.IsPerUser)
                    {
                        await
                        CertificateStores.GetUserStoreByName(selectedItem.StoreName)
                        .RequestDeleteAsync(selectedItem);
                    }

                    else
                    {
                        CertificateStores.GetStoreByName(selectedItem.StoreName).Delete(selectedItem);
                    }
                }

                catch
                {
                    // ignored
                }

                Refresh();
            };
            flyout.Items?.Add(flyoutitem1);
            flyout.ShowAt((StackPanel)sender, e.GetPosition((StackPanel)sender));
        }

        public class DisplayCertificate
        {
            public DisplayCertificate(Certificate cert)
            {
                this.cert = cert;
            }

            public string Subject => cert.Subject;

            public string Issuer => cert.Issuer;

            public string ValidTo => cert.ValidTo.ToString();

            public string HasPrivateKey => cert.HasPrivateKey ? ResourceManager.Current.MainResourceMap.GetValue("Resources/Yes",
                           ResourceContext.GetForCurrentView()).ValueAsString : ResourceManager.Current.MainResourceMap.GetValue("Resources/No", ResourceContext.GetForCurrentView()).ValueAsString;

            public string IsSecurityDeviceBound => cert.IsSecurityDeviceBound ? ResourceManager.Current.MainResourceMap.GetValue("Resources/Yes",
                           ResourceContext.GetForCurrentView()).ValueAsString : ResourceManager.Current.MainResourceMap.GetValue("Resources/No", ResourceContext.GetForCurrentView()).ValueAsString;

            public string FriendlyName => cert.FriendlyName;

            public string Status
            {
                get
                {
                    string result;

                    if ((cert.ValidFrom < DateTime.Now) && (cert.ValidTo > DateTime.Now))
                    {
                        result = ResourceManager.Current.MainResourceMap.GetValue("Resources/Okay", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                        if (cert.ValidFrom > DateTime.Now)
                    {
                        result = ResourceManager.Current.MainResourceMap.GetValue("Resources/Not_yet_valid", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    else
                    {
                        result = ResourceManager.Current.MainResourceMap.GetValue("Resources/Expired", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                    return result;
                }
            }

            public string Index { get; internal set; }

            public Certificate cert { get; }
        }
    }
}