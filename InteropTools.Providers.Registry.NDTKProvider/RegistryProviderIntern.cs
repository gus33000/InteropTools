/*++

Copyright (c) 2016  Interop Tools Development Team
Copyright (c) 2017  Gustave M.

Module Name:

    Plugin.cs

Abstract:

    This module implements the NDTK Registry Provider Plugin.

Author:

    Gustave M.     (gus33000)       20-Mar-2017

Revision History:

    Gustave M. (gus33000) 20-Mar-2017

        Initial Implementation.

--*/

using InteropTools.Providers.Registry.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace InteropTools.Providers.Registry.NDTKProvider
{

    public sealed class RegistryProvider : IBackgroundTask
    {
        private readonly IBackgroundTask internalTask = new RegistryProviderIntern();
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            internalTask.Run(taskInstance);
        }
    }

    internal class RegistryProviderIntern : RegistryProvidersWithOptions
    {
        // Define your provider class here
        private readonly IRegProvider provider = new NDTKRegProvider();

        protected override async Task<string> ExecuteAsync(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken)//, Options options
        {
            //var revereseOptions = new RegistryProviderOptions(options);

            string[] arr = input.Split(new string[] { "_" }, StringSplitOptions.None);

            string operation = Encoding.UTF8.GetString(Convert.FromBase64String(arr.First()));
            Enum.TryParse(operation, true, out REG_OPERATION operationenum);

            List<List<string>> returnvalue = new List<List<string>>();
            List<string> returnvalue2 = new List<string>();

            if (provider.IsSupported(operationenum))
            {
                switch (operationenum)
                {
                    case REG_OPERATION.RegAddKey:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);
                            REG_STATUS ret = provider.RegAddKey(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))));

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegDeleteKey:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);

                            bool.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), out bool recurse);

                            REG_STATUS ret = provider.RegDeleteKey(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), recurse);

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegDeleteValue:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);

                            REG_STATUS ret = provider.RegDeleteValue(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(3))));

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegEnumKey:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);

                            IReadOnlyList<REG_ITEM> items;

                            REG_STATUS ret;

                            if (string.IsNullOrEmpty(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1)))))
                            {
                                ret = provider.RegEnumKey(null, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), out items);
                            }
                            else
                            {
                                ret = provider.RegEnumKey(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), out items);
                            }

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);

                            foreach (REG_ITEM item in items)
                            {
                                List<string> itemlist = new List<string>();
                                if (item.Data == null)
                                {
                                    itemlist.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("")));
                                }
                                else
                                {
                                    itemlist.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(BitConverter.ToString(item.Data))));
                                }

                                itemlist.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Hive.HasValue ? item.Hive.Value.ToString() : "")));
                                itemlist.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Key == null ? "" : item.Key)));
                                itemlist.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Name == null ? "" : item.Name)));
                                itemlist.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Type.HasValue ? item.Type.Value.ToString() : "")));
                                itemlist.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(item.ValueType.HasValue ? item.ValueType.Value.ToString() : "")));
                                returnvalue.Add(itemlist);
                            }
                            break;
                        }
                    case REG_OPERATION.RegQueryKeyLastModifiedTime:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);

                            REG_STATUS ret = provider.RegQueryKeyLastModifiedTime(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), out long lastmodified);

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(lastmodified.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegQueryKeyStatus:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);

                            REG_KEY_STATUS ret = provider.RegQueryKeyStatus(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))));

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegQueryValue:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);


                            uint.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(4))), out uint valuetype);

                            REG_STATUS ret = provider.RegQueryValue(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(3))), valuetype, out uint outvaltype, out byte[] data);

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));
                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(outvaltype.ToString())));
                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(BitConverter.ToString(data))));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegRenameKey:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);

                            REG_STATUS ret = provider.RegRenameKey(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(3))));

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegSetValue:
                        {
                            Enum.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out REG_HIVES hive);

                            uint.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(4))), out uint valuetype);

                            string[] tempAry = Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(5))).Split('-');
                            byte[] buffer = new byte[tempAry.Length];
                            for (int i = 0; i < tempAry.Length; i++)
                            {
                                buffer[i] = Convert.ToByte(tempAry[i], 16);
                            }

                            REG_STATUS ret = provider.RegSetValue(hive, Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(3))), valuetype, buffer);

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegLoadHive:
                        {
                            bool.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(3))), out bool inuser);

                            REG_STATUS ret = provider.RegLoadHive(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), inuser);

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                    case REG_OPERATION.RegUnloadHive:
                        {
                            bool.TryParse(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), out bool inuser);

                            REG_STATUS ret = provider.RegUnloadHive(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), inuser);

                            returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(ret.ToString())));

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                }
            }
            else
            {
                returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(REG_STATUS.NOT_SUPPORTED.ToString())));

                returnvalue.Add(returnvalue2);
            }

            string returnstr = "";

            foreach (List<string> str in returnvalue)
            {
                string str2 = string.Join(" ", str);
                if (string.IsNullOrEmpty(returnstr))
                {
                    returnstr = str2;
                }
                else
                {
                    returnstr += "_" + str2;
                }
            }

            return returnstr;
        }


        protected override Task<Options> GetOptions()
        {
            return Task.FromResult<Options>(new RegistryProviderOptions());
        }

        protected override Guid GetOptionsGuid()
        {
            return RegistryProviderOptions.ID;
        }
    }
}
