using InteropTools.CorePages;
using InteropTools.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Registry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotificationLEDPage : Page
    {
        public string PageName => "Notification LED";
        public PageGroup PageGroup => PageGroup.Tweaks;

        /*public ObservableRangeCollection<DevicePortal.Device> devicelist = new ObservableRangeCollection<DevicePortal.Device>();
		public ObservableRangeCollection<object> devicelist2 = new ObservableRangeCollection<object>();*/

        public DeviceWatcher watcher = null;

        public ObservableCollection<DeviceInformationDisplay> ResultCollection
        {
            get;
            private set;
        }

        /*public class DisplayLEDItem
		{
		    public Color ColorBrush { get; set; }
		    public DevicePortal.Device Device { get; set; }

		    public DisplayLEDItem(DevicePortal.Device device)
		    {
		        Device = device;

		        var random = new Random();
		        random.Next(0x1000000);
		        var colorcode = String.Format("{0:X6}", random.Next(0x1000000));

		        var a = byte.Parse("FF", NumberStyles.HexNumber);
		        var r = byte.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber);
		        var g = byte.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber);
		        var b = byte.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber);

		        ColorBrush = Color.FromArgb(a, r, g, b);
		    }
		}*/

        private readonly IRegistryProvider _helper;

        public NotificationLEDPage()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            ResultCollection = new ObservableCollection<DeviceInformationDisplay>();
            DeviceGridView.ItemsSource = ResultCollection;
            Initialize();
        }

        public async void Initialize()
        {
            Providers.RegTypes regtype;
            string regvalue;
            GetKeyValueReturn ret = await _helper.GetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Intensity", Providers.RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (string.IsNullOrEmpty(regvalue))
            {
                await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Intensity", Providers.RegTypes.REG_DWORD, "100");
                ret = await _helper.GetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Intensity", Providers.RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;
            }

            try
            {
                IntensitySlider.Value = int.Parse(regvalue);
            }

            catch
            {
            }

            ret = await _helper.GetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Period", Providers.RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;

            if (string.IsNullOrEmpty(regvalue))
            {
                await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Period", Providers.RegTypes.REG_DWORD, "2000");
                ret = await _helper.GetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Period", Providers.RegTypes.REG_DWORD); regtype = ret.regtype; regvalue = ret.regvalue;
            }

            try
            {
                PeriodTextBox.Text = regvalue;
            }

            catch
            {
            }

            watcher = DeviceInformation.CreateWatcher("", null, DeviceInformationKind.Device);
            watcher.Added += Watcher_Added;
            watcher.Removed += Watcher_Removed;
            watcher.Updated += Watcher_Updated;
            watcher.Start();
        }

        private async void Watcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                // Find the corresponding updated DeviceInformation in the collection and pass the update object
                // to the Update method of the existing DeviceInformation. This automatically updates the object
                // for us.
                foreach (DeviceInformationDisplay deviceInfoDisp in ResultCollection)
                {
                    if (deviceInfoDisp.Id == args.Id)
                    {
                        deviceInfoDisp.Update(args);
                        break;
                    }
                }
            });
        }

        private async void Watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                // Find the corresponding DeviceInformation in the collection and remove it
                foreach (DeviceInformationDisplay deviceInfoDisp in ResultCollection)
                {
                    if (deviceInfoDisp.Id == args.Id)
                    {
                        ResultCollection.Remove(deviceInfoDisp);
                        break;
                    }
                }
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (watcher != null)
            {
                if (DeviceWatcherStatus.Started == watcher.Status ||
                    DeviceWatcherStatus.EnumerationCompleted == watcher.Status)
                {
                    watcher.Stop();
                }
            }
        }

        private async void Watcher_Added(Windows.Devices.Enumeration.DeviceWatcher sender, Windows.Devices.Enumeration.DeviceInformation args)
        {
            // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                /*Debug.WriteLine("-----------------------------------------");

				Debug.WriteLine(args.Name);

				foreach (var prop in args.Properties)
				{
				    Debug.WriteLine(prop.Key + ": " + prop.Value);
				}

				Debug.WriteLine("-----------------------------------------");*/
                if (args.Name.ToLower().Contains("hwnled"))
                {
                    ResultCollection.Add(new DeviceInformationDisplay(args));
                }
            });
        }

        private async Task RunInUIThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => { function(); });
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }

        public class DeviceInformationDisplay : INotifyPropertyChanged
        {
            private DeviceInformation deviceInfo;

            public DeviceInformationDisplay(DeviceInformation deviceInfoIn)
            {
                deviceInfo = deviceInfoIn;
                UpdateGlyphBitmapImage();
            }

            public DeviceInformationKind Kind => deviceInfo.Kind;

            public string Id => deviceInfo.Id;

            public string Name => deviceInfo.Name;

            public BitmapImage GlyphBitmapImage
            {
                get;
                private set;
            }

            public bool CanPair => deviceInfo.Pairing.CanPair;

            public bool IsPaired => deviceInfo.Pairing.IsPaired;

            public IReadOnlyDictionary<string, object> Properties => deviceInfo.Properties;

            public DeviceInformation DeviceInformation
            {
                get => deviceInfo;

                private set => deviceInfo = value;
            }

            public void Update(DeviceInformationUpdate deviceInfoUpdate)
            {
                deviceInfo.Update(deviceInfoUpdate);
                OnPropertyChanged("Kind");
                OnPropertyChanged("Id");
                OnPropertyChanged("Name");
                OnPropertyChanged("DeviceInformation");
                OnPropertyChanged("CanPair");
                OnPropertyChanged("IsPaired");
                UpdateGlyphBitmapImage();
            }

            private async void UpdateGlyphBitmapImage()
            {
                DeviceThumbnail deviceThumbnail = await deviceInfo.GetGlyphThumbnailAsync();
                BitmapImage glyphBitmapImage = new();
                await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
                GlyphBitmapImage = glyphBitmapImage;
                OnPropertyChanged("GlyphBitmapImage");
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        private async void DeviceGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Id.Text = ((sender as GridView).SelectedItem as DeviceInformationDisplay).Id;
            await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "HardwareId", Providers.RegTypes.REG_SZ, string.Join(@"\",
                                ((sender as GridView).SelectedItem as DeviceInformationDisplay).Id.Split('\\').ToList().Take(2)));
            await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "InstanceId", Providers.RegTypes.REG_DWORD,
                                ((sender as GridView).SelectedItem as DeviceInformationDisplay).Id.Split('\\').ToList().Last());
            await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "LedHwAvailable", Providers.RegTypes.REG_DWORD, "1");
            await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Dutycycle", Providers.RegTypes.REG_DWORD, "60");
            await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Cyclecount", Providers.RegTypes.REG_DWORD, uint.MaxValue.ToString());
        }

        private async void IntensitySlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            await _helper.SetKeyValue(Providers.RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Shell\Nocontrol\LedAlert", "Intensity", Providers.RegTypes.REG_DWORD, e.NewValue.ToString());
        }
    }
}
