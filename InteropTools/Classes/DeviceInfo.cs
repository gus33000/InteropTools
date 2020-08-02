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
			var deviceInformation = new EasClientDeviceInformation();
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
			var v = ulong.Parse(DeviceFamilyVersion);
			var v1 = (v & 0xFFFF000000000000L) >> 48;
			var v2 = (v & 0x0000FFFF00000000L) >> 32;
			var v3 = (v & 0x00000000FFFF0000L) >> 16;
			var v4 = v & 0x000000000000FFFFL;
			SystemVersion = $"{v1}.{v2}.{v3}.{v4}";

			try
			{
				CollectionLevel = PlatformDiagnosticsAndUsageDataSettings.CollectionLevel.ToString();
			}

			catch
			{
				CollectionLevel = "Unknown";
			}
		}

		public static DeviceInfo Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new DeviceInfo();
				}

				return _Instance;
			}
		}


		public string FriendlyName { get; private set; }
		public string HardwareId { get; private set; }
		public string UUID { get; private set; }
		public string OperatingSystem { get; private set; }
		public string SystemFirmwareVersion { get; private set; }
		public string SystemHardwareVersion { get; private set; }
		public string SystemManufacturer { get; private set; }
		public string SystemProductName { get; private set; }
		public string SystemSku { get; private set; }

		public string SystemVersion { get; private set; }

		public string DeviceForm { get; private set; }
		public string DeviceFamily { get; private set; }
		public string DeviceFamilyVersion { get; }
		public string CollectionLevel { get; private set; }

		private static string GetId()
		{
			if (ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
			{
				var token = HardwareIdentification.GetPackageSpecificToken(null);
				var hardwareId = token.Id;
				var dataReader = DataReader.FromBuffer(hardwareId);
				var bytes = new byte[hardwareId.Length];
				dataReader.ReadBytes(bytes);
				return BitConverter.ToString(bytes).Replace("-", "");
            }

			return "";
		}


        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        private static string ByteArrayToHexViaLookup32(byte[] bytes)
        {
            var lookup32 = _lookup32;
            var result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

    }
}