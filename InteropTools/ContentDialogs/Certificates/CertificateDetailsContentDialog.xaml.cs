using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources.Core;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Certificates
{
    public sealed partial class CertificateDetailsContentDialog : ContentDialog
    {
        private readonly Certificate cert;
        private readonly ObservableCollection<Certificate> certlist = new();

        private readonly ObservableCollection<DisplayCertificate> chainCertList =
          new();

        public CertificateDetailsContentDialog(Certificate cert, ObservableCollection<Certificate> certlist)
        {
            InitializeComponent();
            this.cert = cert;
            this.certlist = certlist;
            ChainListView.ItemsSource = chainCertList;
            Refresh();
        }

        private void ChainListView_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void Refresh()
        {
            Subject.Text = cert.Subject;
            FriendlyName.Text = cert.FriendlyName;

            try
            {
                SName.Text = cert.Subject;

                if (SName.Text.Trim()?.Length == 0)
                {
                    SubjectHeader.Visibility = Visibility.Collapsed;
                    SName.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                SubjectHeader.Visibility = Visibility.Collapsed;
                SName.Visibility = Visibility.Collapsed;
            }

            try
            {
                FName.Text = cert.FriendlyName;

                if (FName.Text.Trim()?.Length == 0)
                {
                    FriendlyNameHeader.Visibility = Visibility.Collapsed;
                    FName.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                FriendlyNameHeader.Visibility = Visibility.Collapsed;
                FName.Visibility = Visibility.Collapsed;
            }

            try
            {
                Issuer.Text = cert.Issuer;

                if (Issuer.Text.Trim()?.Length == 0)
                {
                    IssuerHeader.Visibility = Visibility.Collapsed;
                    Issuer.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                IssuerHeader.Visibility = Visibility.Collapsed;
                Issuer.Visibility = Visibility.Collapsed;
            }

            try
            {
                StoreName.Text = cert.StoreName;

                if (StoreName.Text.Trim()?.Length == 0)
                {
                    StoreNameHeader.Visibility = Visibility.Collapsed;
                    StoreName.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                StoreNameHeader.Visibility = Visibility.Collapsed;
                StoreName.Visibility = Visibility.Collapsed;
            }

            try
            {
                KeyStorageProviderName.Text = cert.KeyStorageProviderName;

                if (KeyStorageProviderName.Text.Trim()?.Length == 0)
                {
                    KeyStorageProviderNameHeader.Visibility = Visibility.Collapsed;
                    KeyStorageProviderName.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                KeyStorageProviderNameHeader.Visibility = Visibility.Collapsed;
                KeyStorageProviderName.Visibility = Visibility.Collapsed;
            }

            try
            {
                ValidFrom.Text = cert.ValidFrom.ToString();

                if (ValidFrom.Text.Trim()?.Length == 0)
                {
                    ValidFromHeader.Visibility = Visibility.Collapsed;
                    ValidFrom.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                ValidFromHeader.Visibility = Visibility.Collapsed;
                ValidFrom.Visibility = Visibility.Collapsed;
            }

            try
            {
                ValidTo.Text = cert.ValidTo.ToString();

                if (ValidTo.Text.Trim()?.Length == 0)
                {
                    ValidToHeader.Visibility = Visibility.Collapsed;
                    ValidTo.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                ValidToHeader.Visibility = Visibility.Collapsed;
                ValidTo.Visibility = Visibility.Collapsed;
            }

            /*cert.KeyUsages;
			cert.EnhancedKeyUsages;*/

            try
            {
                SerialNumber.Text = BitConverter.ToString(cert.SerialNumber);

                if (SerialNumber.Text.Trim()?.Length == 0)
                {
                    SerialNumberHeader.Visibility = Visibility.Collapsed;
                    SerialNumber.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                SerialNumberHeader.Visibility = Visibility.Collapsed;
                SerialNumber.Visibility = Visibility.Collapsed;
            }

            try
            {
                KeyAlgorithmName.Text = cert.KeyAlgorithmName;

                if (KeyAlgorithmName.Text.Trim()?.Length == 0)
                {
                    KeyAlgorithmNameHeader.Visibility = Visibility.Collapsed;
                    KeyAlgorithmName.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                KeyAlgorithmNameHeader.Visibility = Visibility.Collapsed;
                KeyAlgorithmName.Visibility = Visibility.Collapsed;
            }

            try
            {
                SignatureAlgorithmName.Text = cert.SignatureAlgorithmName;

                if (SignatureAlgorithmName.Text.Trim()?.Length == 0)
                {
                    SignatureAlgorithmNameHeader.Visibility = Visibility.Collapsed;
                    SignatureAlgorithmName.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                SignatureAlgorithmNameHeader.Visibility = Visibility.Collapsed;
                SignatureAlgorithmName.Visibility = Visibility.Collapsed;
            }

            try
            {
                SignatureHashAlgorithmName.Text = cert.SignatureHashAlgorithmName;

                if (SignatureHashAlgorithmName.Text.Trim()?.Length == 0)
                {
                    SignatureHashAlgorithmNameHeader.Visibility = Visibility.Collapsed;
                    SignatureHashAlgorithmName.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                SignatureHashAlgorithmNameHeader.Visibility = Visibility.Collapsed;
                SignatureHashAlgorithmName.Visibility = Visibility.Collapsed;
            }

            try
            {
                HasPrivateKey.Text = cert.HasPrivateKey.ToString();

                if (HasPrivateKey.Text.Trim()?.Length == 0)
                {
                    HasPrivateKeyHeader.Visibility = Visibility.Collapsed;
                    HasPrivateKey.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                HasPrivateKeyHeader.Visibility = Visibility.Collapsed;
                HasPrivateKey.Visibility = Visibility.Collapsed;
            }

            try
            {
                IsPerUser.Text = cert.IsPerUser.ToString();

                if (IsPerUser.Text.Trim()?.Length == 0)
                {
                    IsPerUserHeader.Visibility = Visibility.Collapsed;
                    IsPerUser.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                IsPerUserHeader.Visibility = Visibility.Collapsed;
                IsPerUser.Visibility = Visibility.Collapsed;
            }

            try
            {
                IsSecurityDeviceBound.Text = cert.IsSecurityDeviceBound.ToString();

                if (IsSecurityDeviceBound.Text.Trim()?.Length == 0)
                {
                    IsSecurityDeviceBoundHeader.Visibility = Visibility.Collapsed;
                    IsSecurityDeviceBound.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                IsSecurityDeviceBoundHeader.Visibility = Visibility.Collapsed;
                IsSecurityDeviceBound.Visibility = Visibility.Collapsed;
            }

            try
            {
                IsStronglyProtected.Text = cert.IsStronglyProtected.ToString();

                if (IsStronglyProtected.Text.Trim()?.Length == 0)
                {
                    IsStronglyProtectedHeader.Visibility = Visibility.Collapsed;
                    IsStronglyProtected.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                IsStronglyProtectedHeader.Visibility = Visibility.Collapsed;
                IsStronglyProtected.Visibility = Visibility.Collapsed;
            }

            CertificateChain result = await cert.BuildChainAsync(this.certlist);
            _ = result.Validate();
            IReadOnlyList<Certificate> chain = result.GetCertificates(true);
            List<Certificate> certlist = new();

            foreach (Certificate cert2 in chain)
            {
                certlist.Insert(0, cert2);
            }

            int counter = 0;
            foreach (Certificate cert2 in certlist)
            {
                DisplayCertificate dispcert = new(cert2)
                {
                    Index = chainCertList.Count().ToString()
                };
                int padding = counter * 16;
                dispcert.Padding = new GridLength(padding);
                counter++;
                chainCertList.Add(dispcert);
            }
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
        }

        private async void Value_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock SelectedItem = (TextBlock)e.OriginalSource;
            DataPackage dataPackage = new() { RequestedOperation = DataPackageOperation.Copy };
            dataPackage.SetText(SelectedItem.Text);
            Clipboard.SetContent(dataPackage);
            Hide();
            await
            new Core.MessageDialogContentDialog().ShowMessageDialog(SelectedItem.Text + "\nThe above value was copied to your clipboard", "Certificate Info");
            await ShowAsync();
        }

        public class DisplayCertificate
        {
            public DisplayCertificate(Certificate cert)
            {
                this.cert = cert;
            }

            public Certificate cert { get; }
            public string FriendlyName => cert.FriendlyName;

            public string HasPrivateKey => cert.HasPrivateKey ? ResourceManager.Current.MainResourceMap.GetValue("Resources/Yes",
                           ResourceContext.GetForCurrentView()).ValueAsString : ResourceManager.Current.MainResourceMap.GetValue("Resources/No", ResourceContext.GetForCurrentView()).ValueAsString;

            public string Index { get; internal set; }

            public string IsSecurityDeviceBound => cert.IsSecurityDeviceBound ? ResourceManager.Current.MainResourceMap.GetValue("Resources/Yes",
                           ResourceContext.GetForCurrentView()).ValueAsString : ResourceManager.Current.MainResourceMap.GetValue("Resources/No", ResourceContext.GetForCurrentView()).ValueAsString;

            public string Issuer => cert.Issuer;
            public GridLength Padding { get; internal set; }

            public string Status
            {
                get
                {
                    if ((cert.ValidFrom < DateTime.Now) && (cert.ValidTo > DateTime.Now))
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Okay", ResourceContext.GetForCurrentView()).ValueAsString;
                    }
                    else
                        if (cert.ValidFrom > DateTime.Now)
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Not_yet_valid", ResourceContext.GetForCurrentView()).ValueAsString;
                    }
                    else
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Expired", ResourceContext.GetForCurrentView()).ValueAsString;
                    }
                }
            }

            public string Subject => cert.Subject;
            public string ValidTo => cert.ValidTo.ToString();
        }
    }
}