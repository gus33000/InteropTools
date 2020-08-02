using InteropTools.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static InteropTools.Pages.Certificates.CertificateDetailPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.Pages.Certificates
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CertificateManagerPage : Page
    {
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

        public CertificateManagerPage()
        {
            InitializeComponent();
            Refresh();
            
            SizeChanged += CertificateManagerPage_SizeChanged;

            this.SetExtended(HeaderBackground, false, true, true, false);

            var marg = HeaderBackground.Margin;
            marg.Left = ActualWidth - ((Window.Current.Content as Frame).Content as Shell).ActualWidth;
            HeaderBackground.Margin = marg;

            DetailPane.Content = new CertificateDetailPage();
        }

        private void CertificateManagerPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetExtended(HeaderBackground, false, true, true, false);

            var marg = HeaderBackground.Margin;
            marg.Left = ActualWidth - ((Window.Current.Content as Frame).Content as Shell).ActualWidth;
            HeaderBackground.Margin = marg;
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

            foreach (var certStore in _knownCertificateStores)
            {
                try
                {
                    var query = new CertificateQuery { StoreName = certStore };
                    var certificates = await CertificateStores.FindAllAsync(query);
                    var localMachineCerts = new List<Certificate>();
                    var currentUserCerts = new List<Certificate>();

                    foreach (var cert in certificates)
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
                        var lmpivot = new PivotItem { Header = GetFriendlyStoreName(certStore) };
                        LocalMachinePivot.Items.Add(lmpivot);
                        var listview = new ListView
                        {
                            ItemTemplate = CertListViewTemplate,
                            IsItemClickEnabled = true,
                            SelectionMode = ListViewSelectionMode.None
                        };
                        listview.ItemClick += Listview_ItemClick;
                        var certList = new ObservableCollection<DisplayCertificate>();
                        listview.ItemsSource = certList;
                        lmpivot.Content = listview;

                        foreach (var cert in localMachineCerts)
                        {
                            var dispCert = new DisplayCertificate(cert) { Index = _maincertList.Count.ToString() };
                            _maincertList.Add(cert);
                            certList.Add(dispCert);
                        }
                    }

                    if (currentUserCerts.Count == 0)
                    {
                        continue;
                    }

                    {
                        var cupivot = new PivotItem { Header = GetFriendlyStoreName(certStore) };
                        CurrentUserPivot.Items.Add(cupivot);
                        var listview = new ListView
                        {
                            ItemTemplate = CertListViewTemplate,
                            IsItemClickEnabled = true,
                            SelectionMode = ListViewSelectionMode.None
                        };
                        listview.ItemClick += Listview_ItemClick;
                        var certList = new ObservableCollection<DisplayCertificate>();
                        listview.ItemsSource = certList;
                        cupivot.Content = listview;

                        foreach (var cert in currentUserCerts)
                        {
                            var dispCert = new DisplayCertificate(cert) { Index = _maincertList.Count.ToString() };
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
                        return "Trusted Publishers";
                    }

                case "ClientAuthIssuer":
                    {
                        return "Client Authentication Issuer";
                    }

                case "Root":
                    {
                        return "Trusted Root Certification Authorities";
                    }

                case "CA":
                    {
                        return "Intermediate Certification Authorities";
                    }

                case "UserDS":
                    {
                        return "User Directory Store";
                    }

                case "REQUEST":
                    {
                        return "Certificate Enrollment Requests";
                    }

                case "AuthRoot":
                    {
                        return "Thrid Party Root Certification Authorities";
                    }

                case "TrustedPeople":
                    {
                        return "Trusted People";
                    }

                case "My":
                    {
                        return "Personal";
                    }

                case "SmartCardRoot":
                    {
                        return "Smart Card Trusted Roots";
                    }

                case "Trust":
                    {
                        return "Trusted Certificates";
                    }

                case "Disallowed":
                    {
                        return "Untrusted Certificates";
                    }

                case "TrustedDevices":
                    {
                        return "Trusted Devices";
                    }

                case "FlightRoot":
                    {
                        return "Preview Build Roots";
                    }

                default:
                    {
                        return storename;
                    }
            }
        }

        private void Listview_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedItem = (DisplayCertificate)e.ClickedItem;
            var cert = selectedItem.cert;
            
            DetailPane.Navigate(typeof(CertificateDetailPage), new CertificatePagePayload() { Certificate = cert, ChainCertificateList = _maincertList });
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var sndr = (StackPanel)sender;
            var index = ((TextBlock)sndr.Children[0]).Text;
            var selectedItem = _maincertList[int.Parse(index)];
            var flyout = new MenuFlyout { Placement = FlyoutPlacementMode.Top };
            var flyoutitem1 = new MenuFlyoutItem();
            flyoutitem1.Text = "Delete";
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
            
            public GridLength Padding { get; internal set; }

            public Visibility FriendlyNameVisibility
            {
                get
                {
                    return string.IsNullOrEmpty(cert.FriendlyName) ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            public string Subject
            {
                get
                {
                    return cert.Subject;
                }
            }

            public string Issuer
            {
                get
                {
                    return cert.Issuer;
                }
            }

            public string ValidTo
            {
                get
                {
                    return cert.ValidTo.ToString();
                }
            }

            public string HasPrivateKey
            {
                get
                {
                    return cert.HasPrivateKey ? "Yes" : "No";
                }
            }

            public string IsSecurityDeviceBound
            {
                get
                {
                    return cert.IsSecurityDeviceBound ? "Yes" : "No";
                }
            }

            public string FriendlyName
            {
                get
                {
                    return cert.FriendlyName;
                }
            }

            public string Status
            {
                get
                {
                    string result;

                    if ((cert.ValidFrom < DateTime.Now) && (cert.ValidTo > DateTime.Now))
                    {
                        result = "Okay";
                    }

                    else
                        if (cert.ValidFrom > DateTime.Now)
                    {
                        result = "Not yet valid";
                    }

                    else
                    {
                        result = "Expired";
                    }

                    return result;
                }
            }

            public string Index { get; internal set; }

            public Certificate cert { get; }
        }
    }
}
