/*++

Copyright (c) 2016  Interop Tools Development Team
Copyright (c) 2017  Gustave M.

Module Name:

    NDTKRegProvider.cs

Abstract:

    This module implements the NDTK Registry Provider Interface.

Author:

    Gustave M.     (gus33000)       20-Mar-2017

Revision History:

    Gustave M. (gus33000) 20-Mar-2017

        Initial Implementation.

--*/

using ndtklib;
using System;
using System.Collections.Generic;

namespace InteropTools.Providers.Registry.NDTKProvider
{
    internal class NDTKRegProvider : IRegProvider
    {
        private NRPC _nrpc;

        private static readonly Dictionary<REG_HIVES, uint> _ndtkhives = new Dictionary<REG_HIVES, uint>
        {
            { REG_HIVES.HKEY_CLASSES_ROOT, 0 },
            { REG_HIVES.HKEY_CURRENT_USER, 2 },
            { REG_HIVES.HKEY_LOCAL_MACHINE, 1 },
            { REG_HIVES.HKEY_USERS, 4 },
            { REG_HIVES.HKEY_PERFORMANCE_DATA, 5 },
            { REG_HIVES.HKEY_CURRENT_CONFIG, 3 },
            { REG_HIVES.HKEY_DYN_DATA, 6 },
            { REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS, 7 }
        };

        private static readonly Dictionary<REG_VALUE_TYPE, uint> _ndtkvaltypes = new Dictionary<REG_VALUE_TYPE, uint>
        {
            { REG_VALUE_TYPE.REG_NONE , 0 },
            { REG_VALUE_TYPE.REG_SZ , 1 },
            { REG_VALUE_TYPE.REG_EXPAND_SZ , 2 },
            { REG_VALUE_TYPE.REG_BINARY , 3 },
            { REG_VALUE_TYPE.REG_DWORD , 4 },
            { REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN , 5 },
            { REG_VALUE_TYPE.REG_LINK , 6 },
            { REG_VALUE_TYPE.REG_MULTI_SZ , 7 },
            { REG_VALUE_TYPE.REG_RESOURCE_LIST , 8 },
            { REG_VALUE_TYPE.REG_FULL_RESOURCE_DESCRIPTOR , 9 },
            { REG_VALUE_TYPE.REG_RESOURCE_REQUIREMENTS_LIST , 10 },
            { REG_VALUE_TYPE.REG_QWORD , 11 }
        };

        public bool IsSupported(REG_OPERATION operation)
        {
            if (_nrpc == null)
            {
                try
                {
                    _nrpc = new NRPC();
                    uint ret = _nrpc.Initialize();
                }
                catch
                {
                    _nrpc = null;
                    return false;
                }
            }

            switch (operation)
            {
                case REG_OPERATION.RegAddKey:
                case REG_OPERATION.RegDeleteKey:
                case REG_OPERATION.RegDeleteValue:
                case REG_OPERATION.RegEnumKey:
                case REG_OPERATION.RegQueryKeyLastModifiedTime:
                case REG_OPERATION.RegQueryKeyStatus:
                case REG_OPERATION.RegRenameKey:
                    {
                        return false;
                    }
            }
            return true;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data)
        {
            if (!IsSupported(REG_OPERATION.RegQueryValue))
            {
                throw new NotImplementedException();
            }

            try
            {
                int length = 0;
                byte[] buffer;
                uint returncode;
                uint dataType = valtype;

                do
                {
                    length += 1;
                    buffer = new byte[length];
                    try
                    {
                        returncode = _nrpc.RegQueryValue(_ndtkhives[hive], key, regvalue, dataType, buffer);
                    }
                    catch (Exception e)
                    {
                        returncode = (uint)e.HResult;
                        if (returncode != 0x800700ea)
                        {
                            // rethrow if not ERROR_MORE_DATA
                            data = new byte[0];
                            outvaltype = 0;
                            return REG_STATUS.FAILED;
                        }
                    }
                    // throw if an error occured that's not ERROR_MORE_DATA
                    if ((returncode != 0) && (returncode != 0x800700ea))
                    {
                        data = new byte[0];
                        outvaltype = 0;
                        return REG_STATUS.FAILED;
                    }
                } while (returncode == 0x800700ea);

                data = buffer;
                outvaltype = dataType;

                return REG_STATUS.SUCCESS;
            }
            catch
            {
                data = new byte[0];
                outvaltype = 0;
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data)
        {
            if (!IsSupported(REG_OPERATION.RegSetValue))
            {
                throw new NotImplementedException();
            }

            try
            {
                try
                {
                    uint returncode = _nrpc.RegSetValue(_ndtkhives[hive], key, regvalue, valtype, data);
                    if (returncode != 0)
                    {
                        return REG_STATUS.FAILED;
                    }
                }
                catch
                {

                }
                return REG_STATUS.SUCCESS;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegAddKey(REG_HIVES hive, string key)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegDeleteKey(REG_HIVES hive, string key, bool recursive)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, string key, string name)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM> items)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, string key, out long lastmodified)
        {
            throw new NotImplementedException();
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegRenameKey(REG_HIVES hive, string key, string newname)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegLoadHive(string hivepath, string mountedname, bool InUser)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegUnloadHive(string mountedname, bool InUser)
        {
            throw new NotImplementedException();
        }
    }
}
