// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Text;

namespace RegistryHelper
{
    internal class Convert
    {
        private static readonly uint[] _lookup32 = CreateLookup32();

        private static string ByteArrayToHexViaLookup32(byte[] bytes)
        {
            try
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
            catch
            {
                return "";
            }
        }

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

        private static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < (hex.Length >> 1); ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
            }

            return arr;
        }

        private static int GetHexVal(char hex)
        {
            int val = hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static byte[] StringToRegBuffer(uint valtype, string data)
        {
            if (data.Length == 0)
                return null;

            switch (valtype)
            {
                case (uint)REG_VALUE_TYPE.REG_DWORD:
                    {
                        return data.Length == 0 ? new byte[0] : BitConverter.GetBytes(uint.Parse(data));
                    }
                case (uint)REG_VALUE_TYPE.REG_QWORD:
                    {
                        return data.Length == 0 ? new byte[0] : BitConverter.GetBytes(ulong.Parse(data));
                    }
                case (uint)REG_VALUE_TYPE.REG_MULTI_SZ:
                    {
                        return data.Length == 0 ? new byte[0] : Encoding.Unicode.GetBytes(string.Join("\0", data.Split('\n')) + "\0\0");
                    }
                case (uint)REG_VALUE_TYPE.REG_SZ:
                    {
                        return data.Length == 0 ? new byte[0] : Encoding.Unicode.GetBytes(data + '\0');
                    }
                case (uint)REG_VALUE_TYPE.REG_EXPAND_SZ:
                    {
                        return data.Length == 0 ? new byte[0] : Encoding.Unicode.GetBytes(data + '\0');
                    }
                default:
                    {
                        return StringToByteArrayFastest(data);
                    }
            }
        }

        public static string RegBufferToString(uint valtype, byte[] data)
        {
            if (data.Length == 0)
                return null;

            switch (valtype)
            {
                case (uint)REG_VALUE_TYPE.REG_DWORD:
                    {
                        return data.Length == 0 ? "" : BitConverter.ToUInt32(data, 0).ToString();
                    }
                case (uint)REG_VALUE_TYPE.REG_QWORD:
                    {
                        return data.Length == 0 ? "" : BitConverter.ToUInt64(data, 0).ToString();
                    }
                case (uint)REG_VALUE_TYPE.REG_MULTI_SZ:
                    {
                        string strNullTerminated = Encoding.Unicode.GetString(data);
                        if (strNullTerminated.Substring(strNullTerminated.Length - 2) == "\0\0")
                        {
                            // The REG_MULTI_SZ is properly terminated.
                            // Remove the array terminator, and the final string terminator.
                            strNullTerminated = strNullTerminated.Substring(0, strNullTerminated.Length - 2);
                        }
                        else if (strNullTerminated.Substring(strNullTerminated.Length - 1) == "\0")
                        {
                            // The REG_MULTI_SZ is improperly terminated (only one terminator).
                            // Remove it.
                            strNullTerminated = strNullTerminated.Substring(0, strNullTerminated.Length - 1);
                        }
                        // Split by null terminator.
                        return string.Join("\n", strNullTerminated.Split('\0'));
                    }
                case (uint)REG_VALUE_TYPE.REG_SZ:
                    {
                        return Encoding.Unicode.GetString(data).TrimEnd('\0');
                    }
                case (uint)REG_VALUE_TYPE.REG_EXPAND_SZ:
                    {
                        return Encoding.Unicode.GetString(data).TrimEnd('\0');
                    }
                default:
                    {
                        return ByteArrayToHexViaLookup32(data);
                    }
            }
        }
    }
}
