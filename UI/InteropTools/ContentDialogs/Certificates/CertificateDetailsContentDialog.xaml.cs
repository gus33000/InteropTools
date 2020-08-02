using InteropTools.CorePages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources.Core;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Certificates
{
	public sealed partial class CertificateDetailsContentDialog : ContentDialog
	{
		private readonly Certificate cert;
		private readonly ObservableCollection<Certificate> certlist = new ObservableCollection<Certificate>();

		private readonly ObservableCollection<DisplayCertificate> chainCertList =
		  new ObservableCollection<DisplayCertificate>();

		public CertificateDetailsContentDialog(Certificate cert, ObservableCollection<Certificate> certlist)
		{
			InitializeComponent();
			this.cert = cert;
			this.certlist = certlist;
			ChainListView.ItemsSource = chainCertList;
			Refresh();
		}

		private async void Refresh()
		{
			Subject.Text = cert.Subject;
			FriendlyName.Text = cert.FriendlyName;

			try
			{
				SName.Text = cert.Subject;

				if (SName.Text.Trim() == "")
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

			/*try
			{
			    SubjectAlternativeName.Text = cert.SubjectAlternativeName;
			    if (SubjectAlternativeName.Text.Trim() == "")
			    {
			        SubjectAlternativeNameHeader.Visibility = Visibility.Collapsed;
			        SubjectAlternativeName.Visibility = Visibility.Collapsed;
			    }
			}
			catch
			{
			    SubjectAlternativeNameHeader.Visibility = Visibility.Collapsed;
			    SubjectAlternativeName.Visibility = Visibility.Collapsed;
			}*/

			try
			{
				FName.Text = cert.FriendlyName;

				if (FName.Text.Trim() == "")
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

				if (Issuer.Text.Trim() == "")
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

				if (StoreName.Text.Trim() == "")
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

				if (KeyStorageProviderName.Text.Trim() == "")
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

				if (ValidFrom.Text.Trim() == "")
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

				if (ValidTo.Text.Trim() == "")
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

				if (SerialNumber.Text.Trim() == "")
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

				if (KeyAlgorithmName.Text.Trim() == "")
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

				if (SignatureAlgorithmName.Text.Trim() == "")
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

				if (SignatureHashAlgorithmName.Text.Trim() == "")
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

				if (HasPrivateKey.Text.Trim() == "")
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

				if (IsPerUser.Text.Trim() == "")
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

				if (IsSecurityDeviceBound.Text.Trim() == "")
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

				if (IsStronglyProtected.Text.Trim() == "")
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

			var result = await cert.BuildChainAsync(this.certlist);
			var validation = result.Validate();
			//Debug.WriteLine(validation.ToString());
			var chain = result.GetCertificates(true);
			var certlist = new List<Certificate>();

			foreach (var cert2 in chain)
			{
				certlist.Insert(0, cert2);
			}

			var counter = 0;
			var padding = 0;

			foreach (var cert2 in certlist)
			{
				var dispcert = new DisplayCertificate(cert2);
				dispcert.Index = chainCertList.Count().ToString();
				padding = counter * 16;
				dispcert.Padding = new GridLength(padding);
				counter++;
				chainCertList.Add(dispcert);
			}
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ChainListView_ItemClick(object sender, ItemClickEventArgs e)
		{
		}

		private async void Value_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var SelectedItem = (TextBlock) e.OriginalSource;
			var dataPackage = new DataPackage {RequestedOperation = DataPackageOperation.Copy};
			dataPackage.SetText(SelectedItem.Text);
			Clipboard.SetContent(dataPackage);
			Hide();
			await
			new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(SelectedItem.Text + "\nThe above value was copied to your clipboard", "Certificate Info");
			await ShowAsync();
		}

		private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
		}

		public class DisplayCertificate
		{
			public DisplayCertificate(Certificate cert)
			{
				this.cert = cert;
			}

			public string Subject
			{
				get {
					return cert.Subject;
				}
			}

			public string Issuer
			{
				get {
					return cert.Issuer;
				}
			}

			public string ValidTo
			{
				get {
					return cert.ValidTo.ToString();
				}
			}

			public string HasPrivateKey
			{
				get {
					return cert.HasPrivateKey ? ResourceManager.Current.MainResourceMap.GetValue("Resources/Yes",
					       ResourceContext.GetForCurrentView()).ValueAsString : ResourceManager.Current.MainResourceMap.GetValue("Resources/No", ResourceContext.GetForCurrentView()).ValueAsString;
				}
			}

			public string IsSecurityDeviceBound
			{
				get {
					return cert.IsSecurityDeviceBound ? ResourceManager.Current.MainResourceMap.GetValue("Resources/Yes",
					       ResourceContext.GetForCurrentView()).ValueAsString : ResourceManager.Current.MainResourceMap.GetValue("Resources/No", ResourceContext.GetForCurrentView()).ValueAsString;
				}
			}

			public string FriendlyName
			{
				get {
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

			public GridLength Padding { get; internal set; }

			public Certificate cert { get; }
		}
	}
}