using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RegPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Registry.Definition.TransfareOptions
using System.Threading.Tasks;

namespace InteropTools.Providers
{
    public class RegistryProvider : IRegProvider
    {
        //private RegPlugin p;
        private string pid;

        public RegistryProvider(RegPlugin regplugin)
        {
            pid = regplugin.UniqueId;
        }

        public string RegBufferToString(uint valtype, byte[] data)
        {
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
                        return String.Join("\n", strNullTerminated.Split('\0'));
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


        public byte[] RegStringToBuffer(uint valtype, string val)
        {
            switch (valtype)
            {
                case (uint)REG_VALUE_TYPE.REG_DWORD:
                    {
                        return BitConverter.GetBytes(uint.Parse(val));
                    }
                case (uint)REG_VALUE_TYPE.REG_QWORD:
                    {
                        return BitConverter.GetBytes(UInt64.Parse(val));
                    }
                case (uint)REG_VALUE_TYPE.REG_MULTI_SZ:
                    {
                        return Encoding.Unicode.GetBytes(string.Join("\0", val.Replace("\r", "\0").Split('\n')) + "\0\0");
                    }
                case (uint)REG_VALUE_TYPE.REG_SZ:
                    {
                        return Encoding.Unicode.GetBytes(val + '\0');
                    }
                case (uint)REG_VALUE_TYPE.REG_EXPAND_SZ:
                    {
                        return Encoding.Unicode.GetBytes(val + '\0');
                    }
                default:
                    {
                        var buffer = StringToByteArrayFastest(val);

                        return buffer;
                    }
            }
        }


        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < (hex.Length >> 1); ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public async Task<REG_STATUS> RegAddKey(REG_HIVES hive, string key)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegAddKey")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_STATUS.NOT_SUPPORTED;
            }
        }

        public async Task<REG_STATUS> RegDeleteKey(REG_HIVES hive, string key, bool recursive)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegDeleteKey")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(recursive.ToString())));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_STATUS.NOT_SUPPORTED;
            }
        }

        public async Task<REG_STATUS> RegDeleteValue(REG_HIVES hive, string key, string name)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegDeleteValue")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(name)));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_STATUS.NOT_SUPPORTED;
            }
        }

        public async Task<RegEnumKey> RegEnumKey(REG_HIVES? hive, string key)
        {
            var ret = new RegEnumKey();
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegEnumKey")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.HasValue ? hive.Value.ToString() : "")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                var retitems = retarr.Skip(1).ToArray();

                List<REG_ITEM> lstitems = new List<REG_ITEM>();

                foreach (var item in retitems)
                {
                    REG_ITEM itm = new REG_ITEM();

                    if (!string.IsNullOrEmpty(item.First()))
                    {
                        var retbuf = item.First().Split('-');

                        byte[] buffer = new byte[retbuf.Length];
                        for (int i = 0; i < retbuf.Length; i++)
                        {
                            buffer[i] = Convert.ToByte(retbuf[i], 16);
                        }

                        itm.Data = buffer;
                    }
                    else
                    {
                        itm.Data = new byte[0];
                    }

                    REG_HIVES hiv;
                    Enum.TryParse(item.ElementAt(1), true, out hiv);
                    itm.Hive = hiv;

                    itm.Key = item.ElementAt(2);
                    itm.Name = item.ElementAt(3);

                    REG_TYPE typ;
                    Enum.TryParse(item.ElementAt(4), true, out typ);
                    itm.Type = typ;

                    uint valtyp;
                    uint.TryParse(item.ElementAt(5), out valtyp);
                    itm.ValueType = valtyp;

                    lstitems.Add(itm);
                }

                ret.items = lstitems;
                ret.returncode = status;

                regpluginlist.Dispose();
            }
            catch
            {
                ret.items = new List<REG_ITEM>();
                ret.returncode = REG_STATUS.NOT_SUPPORTED;
                return ret;
            }
            return ret;
        }

        public async Task<RegQueryKeyLastModifiedTime> RegQueryKeyLastModifiedTime(REG_HIVES hive, string key)
        {
            var ret = new RegQueryKeyLastModifiedTime();
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegQueryKeyLastModifiedTime")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                long lastmodified;
                long.TryParse(lst2.ElementAt(1), out lastmodified);

                ret.LastModified = lastmodified;
                ret.returncode = status;

                regpluginlist.Dispose();

                return ret;
            }
            catch
            {
                ret.LastModified = long.MinValue;
                ret.returncode = REG_STATUS.NOT_SUPPORTED;
                return ret;
            }
        }

        public async Task<REG_KEY_STATUS> RegQueryKeyStatus(REG_HIVES hive, string key)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegQueryKeyStatus")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_KEY_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_KEY_STATUS.UNKNOWN;
            }
        }

        public async Task<RegQueryValue2> RegQueryValue1(REG_HIVES hive, string key, string regvalue, uint valtype)
        {
            var ret = new RegQueryValue2();
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegQueryValue")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(regvalue)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(valtype.ToString())));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                uint outvaltype;
                uint.TryParse(lst2.ElementAt(1), out outvaltype);

                var retbuf = lst2.ElementAt(2).Split('-');

                if (!string.IsNullOrEmpty(lst2.ElementAt(2)))
                {
                    byte[] buffer = new byte[retbuf.Length];
                    for (int i = 0; i < retbuf.Length; i++)
                    {
                        buffer[i] = Convert.ToByte(retbuf[i], 16);
                    }
                    ret.regvalue = buffer;
                }
                else
                {
                    ret.regvalue = new byte[0];
                }

                ret.regtype = outvaltype;
                ret.returncode = status;

                regpluginlist.Dispose();

                return ret;
            }
            catch
            {
                ret.regtype = 0;
                ret.regvalue = new byte[0];
                ret.returncode = REG_STATUS.NOT_SUPPORTED;
                return ret;
            }
        }

        public async Task<REG_STATUS> RegRenameKey(REG_HIVES hive, string key, string newname)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegRenameKey")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(newname)));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_STATUS.NOT_SUPPORTED;
            }
        }

        public async Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegSetValue")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(regvalue)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(valtype.ToString())));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(BitConverter.ToString(data))));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_STATUS.NOT_SUPPORTED;
            }
        }

        public async Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, string data)
        {
            return await RegSetValue(hive, key, regvalue, (uint)valtype, RegStringToBuffer((uint)valtype, data));
        }

        public async Task<RegQueryValue> RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype)
        {
            var rtn = new RegQueryValue();
            var ret = await RegQueryValue1(hive, key, regvalue, (uint)valtype);
            rtn.regvalue = "";
            rtn.regtype = REG_VALUE_TYPE.REG_ERROR;
            if (ret.returncode == REG_STATUS.SUCCESS)
            {
                rtn.regvalue = RegBufferToString(ret.regtype, ret.regvalue);
                rtn.regtype = (REG_VALUE_TYPE)ret.regtype;
            }
            rtn.returncode = ret.returncode;
            return rtn;
        }

        public async Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, string data)
        {
            return await RegSetValue(hive, key, regvalue, valtype, RegStringToBuffer(valtype, data));
        }

        public async Task<RegQueryValue1> RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype)
        {
            var rtn = new RegQueryValue1();
            var ret = await RegQueryValue1(hive, key, regvalue, valtype);
            rtn.regvalue = "";
            rtn.regtype = 0;
            if (ret.returncode == REG_STATUS.SUCCESS)
            {
                rtn.regvalue = RegBufferToString(ret.regtype, ret.regvalue);
                rtn.regtype = ret.regtype;
            }
            rtn.returncode = ret.returncode;
            return rtn;
        }

        public async Task<REG_STATUS> RegLoadHive(string hivepath, string mountedname, bool InUser)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegLoadHive")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(hivepath)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(mountedname)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(InUser.ToString())));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_STATUS.NOT_SUPPORTED;
            }
        }

        public async Task<REG_STATUS> RegUnloadHive(string mountedname, bool InUser)
        {
            try
            {
                var regpluginlist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                var p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                var lst = new List<string>();

                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("RegUnloadHive")));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(mountedname)));
                lst.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(InUser.ToString())));

                var args = string.Join("_", lst);
                var result = await p.ExecuteAsync(args);//, o);
                var retarr = new List<List<string>>();

                foreach (var arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    var newlst = new List<string>();
                    foreach (var arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    retarr.Add(newlst);
                }

                var lst2 = retarr.First();

                var str = lst2.First();

                REG_STATUS status;

                Enum.TryParse(lst2.First(), true, out status);

                regpluginlist.Dispose();

                return status;
            }
            catch
            {
                return REG_STATUS.NOT_SUPPORTED;
            }
        }
    }
}
