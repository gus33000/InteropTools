using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace InteropTools.Providers
{
    public class CCMDProvider : IRegistryProvider
    {
        private IRegistryProvider natprov;

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.AddKey(hive, key);
        }

        public bool AllowsRegistryEditing()
        {
            return true;
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.DeleteKey(hive, key, recursive);
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.DeleteValue(hive, key, keyvalue);
        }

        public bool DoesFileExists(string path)
        {
            return App.SshClient.RunCommand("if EXIST \"" + path + "\" echo True").Execute().Contains("True");
        }

        public string GetAppInstallationPath()
        {
            return Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        }

        public string GetDescription()
        {
            return "Provides SYSTEM registry access through the command line";
        }

        public string GetFriendlyName()
        {
            return App.SshClient.ConnectionInfo.Host == "127.0.0.1" ? "This device" : App.SshClient.ConnectionInfo.Host;
        }

        public string GetHostName()
        {
            return App.SshClient.ConnectionInfo.Host;
        }

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.GetKeyLastModifiedTime(hive, key);
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.GetKeyStatus(hive, key);
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.GetKeyValue(hive, key, keyvalue, type);
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            /*var hivename = hive.ToString();
			var querystr = hivename + @"\" + key;

			if (key == "")
			{
			    querystr = hivename;
			}

			if (string.IsNullOrEmpty(keyvalue))
			{
			    var output = App.SshClient.RunCommand("%SystemRoot%\\system32\\reg.exe query \"" + querystr + "\" /ve").Execute();
			    if (output.ToUpper().Contains('\n') && (output.ToUpper().Split('\n').Count() != 0))
			    {
			        foreach (var line in output.Split('\n'))
			        {
			            if (!line.Contains("End of search:"))
			            {
			                var str = line.Replace("\r", "").Trim();
			                if (str != "")
			                {
			                    if (!str.ToUpper().Contains(querystr.ToUpper()))
			                    {
			                        var temparray = str.Split(new[] { "    " }, StringSplitOptions.None);
			                        var valuename = temparray[0];
			                        regtype = (RegTypes)Enum.Parse(typeof(RegTypes), temparray[1]);
			                        var valuedata = "";
			                        if (temparray.Length == 3)
			                        {
			                            valuedata = temparray[2];
			                            if (regtype == RegTypes.REG_DWORD)
			                            {
			                                valuedata = int.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
			                            }

			                            if (regtype == RegTypes.REG_QWORD)
			                            {
			                                valuedata = long.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
			                            }

			                            if (regtype == RegTypes.REG_MULTI_SZ)
			                            {
			                                valuedata = valuedata.Replace(@"\0", "\n");
			                            }

			                            if ((valuename == "(Default)") && (valuedata == "(value not set)"))
			                            {
			                                valuedata = "";
			                            }

			                            if (valuename == "(Default)")
			                            {
			                                valuename = "";
			                            }

			                            regvalue = valuedata;
			                            return HelperErrorCodes.SUCCESS;
			                        }
			                    }
			                }
			            }
			        }
			    }
			}
			else
			{
			    var output = App.SshClient.RunCommand("%SystemRoot%\\system32\\reg.exe query \"" + querystr + "\" /v " + keyvalue + " /t " + type.ToString()).Execute();
			    if (output.ToUpper().Contains('\n') && (output.ToUpper().Split('\n').Count() != 0) && (!output.Contains("End of search: 0 match(es) found.")))
			    {
			        foreach (var line in output.Split('\n'))
			        {
			            if (!line.Contains("End of search:"))
			            {
			                var str = line.Replace("\r", "").Trim();
			                if (str != "")
			                {
			                    if (!str.ToUpper().Contains(querystr.ToUpper()))
			                    {
			                        var temparray = str.Split(new[] { "    " }, StringSplitOptions.None);
			                        var valuename = temparray[0];
			                        regtype = (RegTypes)Enum.Parse(typeof(RegTypes), temparray[1]);
			                        var valuedata = "";
			                        if (temparray.Length == 3)
			                        {
			                            valuedata = temparray[2];
			                            if (regtype == RegTypes.REG_DWORD)
			                            {
			                                valuedata = int.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
			                            }

			                            if (regtype == RegTypes.REG_QWORD)
			                            {
			                                valuedata = long.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
			                            }

			                            if (regtype == RegTypes.REG_MULTI_SZ)
			                            {
			                                valuedata = valuedata.Replace(@"\0", "\n");
			                            }

			                            if ((valuename == "(Default)") && (valuedata == "(value not set)"))
			                            {
			                                valuedata = "";
			                            }

			                            if (valuename == "(Default)")
			                            {
			                                valuename = "";
			                            }

			                            regvalue = valuedata;
			                            return HelperErrorCodes.SUCCESS;
			                        }
			                    }
			                }
			            }
			        }
			    }
			}

			regtype = RegTypes.REG_ERROR;
			regvalue = "";
			return HelperErrorCodes.FAILED;*/
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.GetKeyValue(hive, key, keyvalue, type);
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryHives()
        {
            List<RegistryItem> itemList = new List<RegistryItem>
            {
                new RegistryItem
                {
                    Name = "HKEY_CLASSES_ROOT (HKCR)",
                    Hive = RegHives.HKEY_CLASSES_ROOT,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_CURRENT_CONFIG (HKCC)",
                    Hive = RegHives.HKEY_CURRENT_CONFIG,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_CURRENT_USER (HKCU)",
                    Hive = RegHives.HKEY_CURRENT_USER,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS (HKCULS)",
                    Hive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_DYN_DATA (HKDD)",
                    Hive = RegHives.HKEY_DYN_DATA,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_LOCAL_MACHINE (HKLM)",
                    Hive = RegHives.HKEY_LOCAL_MACHINE,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_PERFORMANCE_DATA (HKPD)",
                    Hive = RegHives.HKEY_PERFORMANCE_DATA,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_USERS (HKU)",
                    Hive = RegHives.HKEY_USERS,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                }
            };
            return itemList;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            List<RegistryItemCustom> itemList = new List<RegistryItemCustom>
            {
                new RegistryItemCustom
                {
                    Name = "HKEY_CLASSES_ROOT (HKCR)",
                    Hive = RegHives.HKEY_CLASSES_ROOT,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_CONFIG (HKCC)",
                    Hive = RegHives.HKEY_CURRENT_CONFIG,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_USER (HKCU)",
                    Hive = RegHives.HKEY_CURRENT_USER,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS (HKCULS)",
                    Hive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_DYN_DATA (HKDD)",
                    Hive = RegHives.HKEY_DYN_DATA,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_LOCAL_MACHINE (HKLM)",
                    Hive = RegHives.HKEY_LOCAL_MACHINE,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_PERFORMANCE_DATA (HKPD)",
                    Hive = RegHives.HKEY_PERFORMANCE_DATA,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_USERS (HKU)",
                    Hive = RegHives.HKEY_USERS,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                }
            };
            return itemList;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key)
        {
            Renci.SshNet.SshClient Client = App.SshClient;
            List<RegistryItem> ItemsList = new List<RegistryItem>();
            string hivename = hive.ToString();
            string querystr = hivename + @"\" + key;

            if (key == "")
            {
                querystr = hivename;
            }

            string output = Client.RunCommand("%SystemRoot%\\system32\\reg.exe query \"" + querystr + "\"").Execute();

            foreach (string line in output.Split('\n'))
            {
                string str = line.Replace("\r", "").Trim();

                if (str != "")
                {
                    if (str.ToUpper().Contains(querystr.ToUpper()))
                    {
                        if (str.ToUpper() != querystr.ToUpper())
                        {
                            string[] temparray = str.Split('\\');
                            ItemsList.Add(new RegistryItem
                            {
                                Hive = hive,
                                Key = key,
                                Name = temparray.Last(),
                                Type = RegistryItemType.KEY,
                                Value = null,
                                ValueType = RegTypes.REG_ERROR
                            });
                        }
                    }

                    else
                    {
                        string[] temparray = str.Split(new[] { "    " }, StringSplitOptions.None);
                        string valuename = temparray[0];
                        RegTypes regtype = (RegTypes)Enum.Parse(typeof(RegTypes), temparray[1]);
                        string valuedata = "";

                        if (temparray.Length == 3)
                        {
                            valuedata = temparray[2];

                            if (regtype == RegTypes.REG_DWORD)
                            {
                                valuedata = int.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                            }

                            if (regtype == RegTypes.REG_QWORD)
                            {
                                valuedata = long.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                            }

                            if (regtype == RegTypes.REG_MULTI_SZ)
                            {
                                valuedata = valuedata.Replace(@"\0", "\n");
                            }

                            if ((valuename == "(Default)") && (valuedata == "(value not set)"))
                            {
                                valuedata = "";
                            }

                            if (valuename == "(Default)")
                            {
                                valuename = "";
                            }

                            ItemsList.Add(new RegistryItem
                            {
                                Hive = hive,
                                Key = key,
                                Name = valuename,
                                Type = RegistryItemType.VALUE,
                                Value = valuedata,
                                ValueType = regtype
                            });
                        }
                    }
                }
            }

            output = App.SshClient.RunCommand("%SystemRoot%\\system32\\reg.exe query \"" + querystr + "\" /ve").Execute();

            if (output.ToUpper().Contains('\n') && (output.ToUpper().Split('\n').Count() != 0))
            {
                foreach (string line in output.Split('\n'))
                {
                    if (!line.Contains("END OF SEARCH:"))
                    {
                        string str = line.Replace("\r", "").Trim();

                        if (str != "")
                        {
                            if (!str.ToUpper().Contains(querystr.ToUpper()))
                            {
                                string[] temparray = str.Split(new[] { "    " }, StringSplitOptions.None);
                                string valuename = temparray[0];
                                RegTypes regtype = (RegTypes)Enum.Parse(typeof(RegTypes), temparray[1]);
                                string valuedata = "";

                                if (temparray.Length == 3)
                                {
                                    valuedata = temparray[2];

                                    if (regtype == RegTypes.REG_DWORD)
                                    {
                                        valuedata = int.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                                    }

                                    if (regtype == RegTypes.REG_QWORD)
                                    {
                                        valuedata = long.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                                    }

                                    if (regtype == RegTypes.REG_MULTI_SZ)
                                    {
                                        valuedata = valuedata.Replace(@"\0", "\n");
                                    }

                                    if ((valuename == "(Default)") && (valuedata == "(value not set)"))
                                    {
                                        valuedata = "";
                                    }

                                    if (valuename == "(Default)")
                                    {
                                        valuename = "";
                                    }

                                    ItemsList.Add(new RegistryItem
                                    {
                                        Hive = hive,
                                        Key = key,
                                        Name = valuename,
                                        Type = RegistryItemType.VALUE,
                                        Value = valuedata,
                                        ValueType = regtype
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return ItemsList;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            Renci.SshNet.SshClient Client = App.SshClient;
            List<RegistryItemCustom> ItemsList = new List<RegistryItemCustom>();
            string hivename = hive.ToString();
            string querystr = hivename + @"\" + key;

            if (key == "")
            {
                querystr = hivename;
            }

            string output = Client.RunCommand("%SystemRoot%\\system32\\reg.exe query \"" + querystr + "\"").Execute();

            foreach (string line in output.Split('\n'))
            {
                string str = line.Replace("\r", "").Trim();

                if (str != "")
                {
                    if (str.ToUpper().Contains(querystr.ToUpper()))
                    {
                        if (str.ToUpper() != querystr.ToUpper())
                        {
                            string[] temparray = str.Split('\\');
                            ItemsList.Add(new RegistryItemCustom
                            {
                                Hive = hive,
                                Key = key,
                                Name = temparray.Last(),
                                Type = RegistryItemType.KEY,
                                Value = null
                            });
                        }
                    }

                    else
                    {
                        string[] temparray = str.Split(new[] { "    " }, StringSplitOptions.None);
                        string valuename = temparray[0];
                        uint regtype = uint.Parse(temparray[1]);
                        string valuedata = "";

                        if (temparray.Length == 3)
                        {
                            valuedata = temparray[2];

                            if (regtype == 4)
                            {
                                valuedata = int.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                            }

                            if (regtype == 11)
                            {
                                valuedata = long.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                            }

                            if (regtype == 7)
                            {
                                valuedata = valuedata.Replace(@"\0", "\n");
                            }

                            if ((valuename == "(Default)") && (valuedata == "(value not set)"))
                            {
                                valuedata = "";
                            }

                            if (valuename == "(Default)")
                            {
                                valuename = "";
                            }

                            ItemsList.Add(new RegistryItemCustom
                            {
                                Hive = hive,
                                Key = key,
                                Name = valuename,
                                Type = RegistryItemType.VALUE,
                                Value = valuedata,
                                ValueType = regtype
                            });
                        }
                    }
                }
            }

            output = App.SshClient.RunCommand("%SystemRoot%\\system32\\reg.exe query \"" + querystr + "\" /ve").Execute();

            if (output.ToUpper().Contains('\n') && (output.ToUpper().Split('\n').Count() != 0))
            {
                foreach (string line in output.Split('\n'))
                {
                    if (!line.Contains("END OF SEARCH:"))
                    {
                        string str = line.Replace("\r", "").Trim();

                        if (str != "")
                        {
                            if (!str.ToUpper().Contains(querystr.ToUpper()))
                            {
                                string[] temparray = str.Split(new[] { "    " }, StringSplitOptions.None);
                                string valuename = temparray[0];
                                uint regtype = uint.Parse(temparray[1]);
                                string valuedata = "";

                                if (temparray.Length == 3)
                                {
                                    valuedata = temparray[2];

                                    if (regtype == 4)
                                    {
                                        valuedata = int.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                                    }

                                    if (regtype == 11)
                                    {
                                        valuedata = long.Parse(valuedata.Remove(0, 2), NumberStyles.HexNumber).ToString();
                                    }

                                    if (regtype == 7)
                                    {
                                        valuedata = valuedata.Replace(@"\0", "\n");
                                    }

                                    if ((valuename == "(Default)") && (valuedata == "(value not set)"))
                                    {
                                        valuedata = "";
                                    }

                                    if (valuename == "(Default)")
                                    {
                                        valuename = "";
                                    }

                                    ItemsList.Add(new RegistryItemCustom
                                    {
                                        Hive = hive,
                                        Key = key,
                                        Name = valuename,
                                        Type = RegistryItemType.VALUE,
                                        Value = valuedata,
                                        ValueType = regtype
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return ItemsList;
        }

        public string GetSymbol()
        {
            return "";
        }

        public string GetTitle()
        {
            return "Command Line Provider";
        }

        public bool IsLocal()
        {
            return App.SshClient.ConnectionInfo.Host == "127.0.0.1";
        }

        public Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            throw new NotImplementedException();
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.RenameKey(hive, key, newname);
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.SetKeyValue(hive, key, keyvalue, type, data);
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            if (natprov == null)
            {
                natprov = new LegacyBridgeRegistryProvider();
            }

            return await natprov.SetKeyValue(hive, key, keyvalue, type, data);
        }

        public Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            throw new NotImplementedException();
        }
    }
}