using InteropTools.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Private
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class WindowsInsiderProgramPage : Page
	{
		public List<Train> TrainList;

		private bool initialized = false;

		public WindowsInsiderProgramPage()
		{
			this.InitializeComponent();
			this._helper = App.RegistryHelper;
			TrainList = GetTrainList();
			RingSelector.ItemsSource = TrainList;
            Init();
		}

        public async void Init()
        {
            await Refresh();
            initialized = true;
        }

        public async Task Refresh()
		{
			var enabled = await IsITDefaultTrainManager();
			SetUIEnabledState(enabled);
			await RefreshTrainListSelected();
			TrainManagerToggle.IsOn = enabled;
			RegTypes regtype;
			String regvalue;
			var result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Settings", "UseWU", RegTypes.REG_DWORD); regtype = result.regtype; regvalue = result.regvalue;
			UseWUtxt.Text = "True";

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				UseWUtxt.Text = "Unknown";
			}

			if (regtype != RegTypes.REG_DWORD)
			{
				UseWUtxt.Text = "Unknown";
			}

			if (regvalue != "1")
			{
				UseWUtxt.Text = "False";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Settings", "GuidOfCategoryToScan", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;
			CatIDtxt.Text = regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				CatIDtxt.Text = "Unknown";
			}

			if (regtype != RegTypes.REG_SZ)
			{
				CatIDtxt.Text = "Unknown";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "__THRESHOLD_FLIGHTING__", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;
            THFlighttxt.Text = "True";

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				THFlighttxt.Text = "False";
			}

			if (regtype != RegTypes.REG_SZ)
			{
				THFlighttxt.Text = "Unknown";
			}

			if (regvalue != "")
			{
				THFlighttxt.Text = "Unknown";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "__OSVERSION_GE__", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;
			OSGEtxt.Text = regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				OSGEtxt.Text = "Unknown";
			}

			if (regtype != RegTypes.REG_SZ)
			{
				OSGEtxt.Text = "Unknown";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\USS\Configuration", "DetectionDllUrl1", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;
			Detect1txt.Text = regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				Detect1txt.Text = "Unknown";
			}

			if (regtype != RegTypes.REG_SZ)
			{
				Detect1txt.Text = "Unknown";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\USS\Configuration", "DetectionDllUrl2", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;
			Detect2txt.Text = regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				Detect2txt.Text = "Unknown";
			}

			if (regtype != RegTypes.REG_SZ)
			{
				Detect2txt.Text = "Unknown";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "TestTarget", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;
			TestTrgttxt.Text = regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				TestTrgttxt.Text = "Unknown";
			}

			if (regtype != RegTypes.REG_SZ)
			{
				TestTrgttxt.Text = "Unknown";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "UpdateTrainName", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;
			TrainNametxt.Text = regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				TrainNametxt.Text = "Unknown";
			}

			if (regtype != RegTypes.REG_SZ)
			{
				TrainNametxt.Text = "Unknown";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Orchestrator", "EnableUUPScan", RegTypes.REG_DWORD); regtype = result.regtype; regvalue = result.regvalue;
			UUPScanEnabledtxt.Text = "True";

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				UUPScanEnabledtxt.Text = "False";
			}

			if (regtype != RegTypes.REG_DWORD)
			{
				UUPScanEnabledtxt.Text = "Unknown";
			}

			if (regvalue != "1")
			{
				UUPScanEnabledtxt.Text = "False";
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\WindowsUpdate", "SupportsUUP", RegTypes.REG_DWORD); regtype = result.regtype; regvalue = result.regvalue;
			SupportsUUPtxt.Text = "True";

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				SupportsUUPtxt.Text = "False";
			}

			if (regtype != RegTypes.REG_DWORD)
			{
				SupportsUUPtxt.Text = "Unknown";
			}

			if (regvalue != "1")
			{
				SupportsUUPtxt.Text = "False";
			}
		}

		public class Train
		{
			public string Title { get; set; }
			public string Description { get; set; }
			public string TestTarget { get; set; }
			public string UpdateTrainName { get; set; }
		}

		private async Task RefreshTrainListSelected()
		{
			var isFlightEnabled = false;
			var isInProd = false;
			RegTypes regtype;
			string regvalue;
			var result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "TestTarget", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;

			if (regvalue == "00000000-0000-0000-0000-000000000000")
			{
				isInProd = true;
				RingDescription.Text = "Go back to the pleb tier.";
			}

			else
			{
				foreach (var item in TrainList)
				{
					if (item.TestTarget == regvalue)
					{
						RingSelector.SelectedIndex = TrainList.IndexOf(item);
						RingDescription.Text = item.Description;
						isFlightEnabled = true;
					}
				}
			}

			if (!isFlightEnabled)
			{
				NonProduction.Visibility = Visibility.Collapsed;
				Production.Visibility = Visibility.Visible;

				if (!isInProd)
				{
					await ShowUnknownGUIDMessageBox(regvalue);
				}
			}

			else
			{
				NonProduction.Visibility = Visibility.Visible;
				Production.Visibility = Visibility.Collapsed;
			}
		}

		private async Task ShowUnknownGUIDMessageBox(string s)
		{
			await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog("GUID: " + s, "Unknown GUID detected");
		}

		private readonly IRegistryProvider _helper;

		public void SetUIEnabledState(bool Enabled)
		{
			if (Enabled)
			{
				MainPanel.Opacity = 1;
				MainPanel.IsHitTestVisible = true;
				MainPanel.IsDoubleTapEnabled = true;
				MainPanel.IsHoldingEnabled = true;
				MainPanel.IsRightTapEnabled = true;
				MainPanel.IsTapEnabled = true;
			}

			else
			{
				MainPanel.Opacity = 0.5;
				MainPanel.IsHitTestVisible = false;
				MainPanel.IsDoubleTapEnabled = false;
				MainPanel.IsHoldingEnabled = false;
				MainPanel.IsRightTapEnabled = false;
				MainPanel.IsTapEnabled = false;
			}
		}

		public List<Train> GetTrainList()
		{
			List<Train> _trainList = new List<Train>();
			_trainList.Add(new Train
			{
				Title = "Canary",
				Description =
				"Best for Microsoft employees who enjoy testing daily builds, with a high risk to their devices in order to identify issues, provide suggestions and ideas to make Windows software and devices great before a wider group of people gets them.",
				TestTarget = "931e9127-67e2-4ff2-b3bc-3cbc802ebdf0",
				UpdateTrainName = "Canary"
			});
			_trainList.Add(new Train
			{
				Title = "Selfhost",
				Description =
				"Best for Microsoft employees who enjoy testing future Insider release candidates, with a medium risk to their devices in order to identify issues, provide suggestions and ideas to make Windows software and devices great.",
				TestTarget = "5a62fe89-66ed-4f5b-952a-f26ab61dd86e",
				UpdateTrainName = "Selfhost"
			});
			_trainList.Add(new Train
			{
				Title = "Insider Fast",
				Description =
				"Best for Insiders who enjoy being the first to get access to builds and feature updates, with some risk to their devices in order to identify issues, and provide suggestions and ideas to make Windows software and devices great.",
				TestTarget = "3d73e726-9ce9-4207-832e-2b973e5a10d5",
				UpdateTrainName = "Insider Fast"
			});
			_trainList.Add(new Train
			{
				Title = "Insider Slow",
				Description =
				"Best for Insiders who enjoy getting early access to builds and feature updates, with less risk to their devices, and still want to provide feedback to make Windows software and devices great.",
				TestTarget = "f5154171-f09e-442a-8790-09eff8782248",
				UpdateTrainName = "Insider Slow"
			});
			_trainList.Add(new Train
			{
				Title = "Microsoft",
				Description =
				"Best for Microsoft employees who wants to run the latest and greatest updates for the Current Branch, Microsoft applications, and drivers, with no risk to their device, and still want to provide feedback to make Windows devices great.",
				TestTarget = "8c923c83-3e2c-4522-8e51-27e963ca3f6a",
				UpdateTrainName = "Microsoft"
			});
			_trainList.Add(new Train
			{
				Title = "Servicing Development",
				Description =
				"Best for Microsoft employees to get early access to builds for the Current Branch before any other ring.",
				TestTarget = "2540dc30-86fa-4059-82ca-e7488ab37fcf",
				UpdateTrainName = "Servicing Development"
			});
			_trainList.Add(new Train
			{
				Title = "Insider Release Preview",
				Description =
				"Best for Insiders who enjoy getting early access to updates for the Current Branch, Microsoft applications, and drivers, with minimal risk to their devices, and still want to provide feedback to make Windows devices great.",
				TestTarget = "e0350cb6-88df-40cc-a885-8b2a8afd0bbb",
				UpdateTrainName = "Insider Release Preview"
			});
			_trainList.Add(new Train
			{
				Title = "Mobile Operator Trial",
				Description =
				"Best for Mobile operators to review future updates for the Current Branch, with minimal risk to their devices, in order to check if the update meets their criteria for public distribution.",
				TestTarget = "ad250f25-34dc-4ae2-a23b-851835191b0c",
				UpdateTrainName = "Mobile Operator Trial"
			});
			return _trainList;
		}

		public async Task<bool> SetSystemTrain(Train train)
		{
			var result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "TestTarget", RegTypes.REG_SZ, train.TestTarget);

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "UpdateTrainName", RegTypes.REG_SZ, train.UpdateTrainName);

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> SetUpITAsTrainManager()
		{
			var result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Settings", "UseWU", RegTypes.REG_DWORD, "1");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Settings", "GuidOfCategoryToScan", RegTypes.REG_SZ,
			                             "1064927b-d5a2-461c-b23a-41540d02a686");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "__THRESHOLD_FLIGHTING__", RegTypes.REG_SZ, "");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "__OSVERSION_GE__", RegTypes.REG_SZ, "8.15.12458.8");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "TestTarget", RegTypes.REG_SZ,
			                             "00000000-0000-0000-0000-000000000000");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "UpdateTrainName", RegTypes.REG_SZ, "Production");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\USS\Configuration", "DetectionDllUrl1", RegTypes.REG_SZ,
			                             "http://fe2.update.microsoft.com/WM10/MicrosoftUpdate/Selfupdate/%u_ussdetection.dll");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\USS\Configuration", "DetectionDllUrl2", RegTypes.REG_SZ,
			                             "http://ds.download.windowsupdate.com/WM10/MicrosoftUpdate/Selfupdate/%u_ussdetection.dll");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "ThresholdFlightsDisabled", RegTypes.REG_DWORD, "1");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Orchestrator", "EnableUUPScan", RegTypes.REG_DWORD, "0");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\WindowsUpdate", "SupportsUUP", RegTypes.REG_DWORD, "0");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> UnsetITAsDefaultTrainManager()
		{
			var result = await _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "ThresholdFlightsDisabled", RegTypes.REG_DWORD, "0");

			if (result != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> IsITDefaultTrainManager()
		{
			RegTypes regtype;
			String regvalue;
			var result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Settings", "UseWU", RegTypes.REG_DWORD); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_DWORD) || (regvalue != "1"))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Settings", "GuidOfCategoryToScan", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_SZ) || (regvalue != "1064927b-d5a2-461c-b23a-41540d02a686"))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "__THRESHOLD_FLIGHTING__", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_SZ) || (regvalue != ""))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\Agent\Protocol", "__OSVERSION_GE__", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_SZ) || (regvalue != "8.15.12458.8"))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\USS\Configuration", "DetectionDllUrl1", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_SZ) || (regvalue != "http://fe2.update.microsoft.com/WM10/MicrosoftUpdate/Selfupdate/%u_ussdetection.dll"))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceUpdate\USS\Configuration", "DetectionDllUrl2", RegTypes.REG_SZ); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_SZ) || (regvalue != "http://ds.download.windowsupdate.com/WM10/MicrosoftUpdate/Selfupdate/%u_ussdetection.dll"))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability", "ThresholdFlightsDisabled", RegTypes.REG_DWORD); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_DWORD) || (regvalue != "1"))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Orchestrator", "EnableUUPScan", RegTypes.REG_DWORD); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_DWORD) || (regvalue != "0"))
			{
				return false;
			}

			result = await _helper.GetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\WindowsUpdate", "SupportsUUP", RegTypes.REG_DWORD); regtype = result.regtype; regvalue = result.regvalue;

			if (result.returncode != HelperErrorCodes.SUCCESS)
			{
				return false;
			}

			if ((regtype != RegTypes.REG_DWORD) || (regvalue != "0"))
			{
				return false;
			}

			return true;
		}

		private async void TrainManagerToggle_Toggled(Object sender, RoutedEventArgs e)
		{
			if (initialized)
			{
				initialized = false;

				if (TrainManagerToggle.IsOn)
				{
					await SetUpITAsTrainManager();
				}

				else
				{
					await UnsetITAsDefaultTrainManager();
				}

				await Refresh();
				initialized = true;
			}
		}


		private async void RingSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (initialized)
			{
				initialized = false;
				var selectedItem = (Train)RingSelector.SelectedItem;
				await SetSystemTrain(selectedItem);
				await Refresh();
				initialized = true;
			}
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var WIS = new Train
			{
				Title = "Insider Slow",
				Description =
				"Best for Insiders who enjoy getting early access to builds and feature updates, with less risk to their devices, and still want to provide feedback to make Windows software and devices great.",
				TestTarget = "f5154171-f09e-442a-8790-09eff8782248",
				UpdateTrainName = "Insider Slow"
			};

			if (initialized)
			{
				initialized = false;
				await SetSystemTrain(WIS);
				await Refresh();
				initialized = true;
			}
		}

		private async void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var Prod = new Train
			{
				Title = "Production",
				Description = "Go back to the pleb tier.",
				TestTarget = "00000000-0000-0000-0000-000000000000",
				UpdateTrainName = "Production"
			};

			if (initialized)
			{
				initialized = false;
				await SetSystemTrain(Prod);
				await Refresh();
				initialized = true;
			}
		}
	}
}
