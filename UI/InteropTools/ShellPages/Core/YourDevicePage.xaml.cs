using InteropTools.Classes;
using InteropTools.CorePages;

namespace InteropTools.ShellPages.Core
{
    public sealed partial class YourDevicePage
    {
        public string PageName => "Your Device";
        public PageGroup PageGroup => PageGroup.General;

        public YourDevicePage()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            DeviceInfo deviceInfo = DeviceInfo.Instance;
            DeviceName.Text = deviceInfo.FriendlyName;
            HardwareId.Text = deviceInfo.HardwareId;
            UUID.Text = deviceInfo.UUID;
            OSName.Text = deviceInfo.OperatingSystem;
            FirmwareVersion.Text = deviceInfo.SystemFirmwareVersion;
            HardwareVersion.Text = deviceInfo.SystemHardwareVersion;
            Manufacturer.Text = deviceInfo.SystemManufacturer;
            Model.Text = deviceInfo.SystemProductName;
            Sku.Text = deviceInfo.SystemSku;
            SystemVersion.Text = deviceInfo.SystemVersion;
            DeviceForm.Text = deviceInfo.DeviceForm;
            DeviceFamily.Text = deviceInfo.DeviceFamily.Replace(".", " ");
            DeviceFamilyVersion.Text = deviceInfo.DeviceFamilyVersion;
            CollectionLevel.Text = deviceInfo.CollectionLevel;

            try
            {
                ulong v = ulong.Parse(DeviceFamilyVersion.Text);
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = v & 0x000000000000FFFFL;
                DeviceFamilyVersion.Text = $"{v1}.{v2}.{v3}.{v4}";
            }

            catch
            {
                // ignored
            }
        }
    }
}