using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Registry.Definition.TransfareOptions

namespace InteropTools.Providers
{
    public class RegistryProvider : IRegProvider
    {
        //private RegPlugin p;
        private readonly string pid;

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
                        return BitConverter.GetBytes(ulong.Parse(val));
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
                        byte[] buffer = StringToByteArrayFastest(val);

                        return buffer;
                    }
            }
        }

        public static byte[] StringToByteArrayFastest(string hex)
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

        public async Task<REG_STATUS> RegAddKey(REG_HIVES hive, string key)
        {
            try
            {
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegAddKey")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst[0], true, out REG_STATUS status);

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
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegDeleteKey")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(recursive.ToString()))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

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
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegDeleteValue")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(name))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

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
            RegEnumKey ret = new();
            try
            {
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegEnumKey")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.HasValue ? hive.Value.ToString() : "")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

                List<string>[] retitems = retarr.Skip(1).ToArray();

                List<REG_ITEM> lstitems = new();

                foreach (List<string> item in retitems)
                {
                    REG_ITEM itm = new();

                    if (!string.IsNullOrEmpty(item[0]))
                    {
                        string[] retbuf = item[0].Split('-');

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

                    Enum.TryParse(item.ElementAt(1), true, out REG_HIVES hiv);
                    itm.Hive = hiv;

                    itm.Key = item.ElementAt(2);
                    itm.Name = item.ElementAt(3);

                    Enum.TryParse(item.ElementAt(4), true, out REG_TYPE typ);
                    itm.Type = typ;

                    uint.TryParse(item.ElementAt(5), out uint valtyp);
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
            RegQueryKeyLastModifiedTime ret = new();
            try
            {
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegQueryKeyLastModifiedTime")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

                long.TryParse(lst2.ElementAt(1), out long lastmodified);

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
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegQueryKeyStatus")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_KEY_STATUS status);

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
            RegQueryValue2 ret = new();
            try
            {
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegQueryValue")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(regvalue)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(valtype.ToString()))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

                uint.TryParse(lst2.ElementAt(1), out uint outvaltype);

                string[] retbuf = lst2.ElementAt(2).Split('-');

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
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);
                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegRenameKey")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(newname))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

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
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegSetValue")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hive.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(key)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(regvalue)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(valtype.ToString())),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(BitConverter.ToString(data)))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

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
            RegQueryValue rtn = new();
            RegQueryValue2 ret = await RegQueryValue1(hive, key, regvalue, (uint)valtype);
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
            RegQueryValue1 rtn = new();
            RegQueryValue2 ret = await RegQueryValue1(hive, key, regvalue, valtype);
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
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegLoadHive")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(hivepath)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(mountedname)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(InUser.ToString()))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

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
                AppPlugin.PluginList.PluginList<string, string, double> regpluginlist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);
                RegPlugin p = regpluginlist.Plugins.First(x => x.UniqueId == pid);

                //var o = await p.PrototypeOptions;
                List<string> lst = new()
                {
                    Convert.ToBase64String(Encoding.UTF8.GetBytes("RegUnloadHive")),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(mountedname)),
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(InUser.ToString()))
                };

                string args = string.Join("_", lst);
                string result = await p.ExecuteAsync(args);//, o);
                List<List<string>> retarr = new();

                foreach (string arg in result.Split(new string[] { "_" }, StringSplitOptions.None))
                {
                    List<string> newlst = new();
                    foreach (string arg2 in arg.Split(new string[] { " " }, StringSplitOptions.None))
                    {
                        newlst.Add(Encoding.UTF8.GetString(Convert.FromBase64String(arg2)));
                    }

                    retarr.Add(newlst);
                }

                List<string> lst2 = retarr[0];

                string str = lst2[0];

                Enum.TryParse(lst2[0], true, out REG_STATUS status);

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
