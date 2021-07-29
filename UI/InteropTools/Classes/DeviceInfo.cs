using System;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Metadata;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace InteropTools.Classes
{
    public sealed class DeviceInfo
    {
        private static DeviceInfo _Instance;

        private DeviceInfo()
        {
            HardwareId = GetId();
            EasClientDeviceInformation deviceInformation = new();
            FriendlyName = deviceInformation.FriendlyName;
            UUID = deviceInformation.Id.ToString();
            OperatingSystem = deviceInformation.OperatingSystem;
            SystemFirmwareVersion = deviceInformation.SystemFirmwareVersion;
            SystemHardwareVersion = deviceInformation.SystemHardwareVersion;
            SystemManufacturer = deviceInformation.SystemManufacturer;
            SystemProductName = deviceInformation.SystemProductName;
            SystemSku = deviceInformation.SystemSku;
            DeviceForm = AnalyticsInfo.DeviceForm;
            DeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            DeviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(DeviceFamilyVersion);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = v & 0x000000000000FFFFL;
            SystemVersion = $"{v1}.{v2}.{v3}.{v4}";

            try
            {
                CollectionLevel = PlatformDiagnosticsAndUsageDataSettings.CollectionLevel.ToString();
            }
            catch
            {
                CollectionLevel = ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;
            }
        }

        public static DeviceInfo Instance
        {
            get
            {
                return _Instance ??= new DeviceInfo();
            }
        }

        public string FriendlyName { get; }
        public string HardwareId { get; }
        public string UUID { get; }
        public string OperatingSystem { get; }
        public string SystemFirmwareVersion { get; }
        public string SystemHardwareVersion { get; }
        public string SystemManufacturer { get; }
        public string SystemProductName { get; }
        public string SystemSku { get; }

        public string SystemVersion { get; }

        public string DeviceForm { get; }
        public string DeviceFamily { get; }
        public string DeviceFamilyVersion { get; }
        public string CollectionLevel { get; }

        private static string GetId()
        {
            if (ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
            {
                HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
                IBuffer hardwareId = token.Id;
                DataReader dataReader = DataReader.FromBuffer(hardwareId);
                byte[] bytes = new byte[hardwareId.Length];
                dataReader.ReadBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "");
            }

            return "";
        }

        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            uint[] result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = s[0] + ((uint)s[1] << 16);
            }
            return result;
        }

        private static string ByteArrayToHexViaLookup32(byte[] bytes)
        {
            uint[] lookup32 = _lookup32;
            char[] result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                uint val = lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[(2 * i) + 1] = (char)(val >> 16);
            }
            return new string(result);
        }
    }
}