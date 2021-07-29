namespace InteropTools.Providers.Registry.RegistryRTProvider
{
    internal enum REG_HIVES
    {
        HKEY_CLASSES_ROOT,
        HKEY_CURRENT_USER,
        HKEY_LOCAL_MACHINE,
        HKEY_USERS,
        HKEY_PERFORMANCE_DATA,
        HKEY_CURRENT_CONFIG,
        HKEY_DYN_DATA,
        HKEY_CURRENT_USER_LOCAL_SETTINGS
    }

    internal enum REG_KEY_STATUS
    {
        FOUND,
        NOT_FOUND,
        ACCESS_DENIED,
        UNKNOWN,
    }

    internal enum REG_OPERATION
    {
        RegQueryKeyLastModifiedTime,
        RegAddKey,
        RegDeleteKey,
        RegDeleteValue,
        RegRenameKey,
        RegQueryKeyStatus,
        RegQueryValue,
        RegSetValue,
        RegEnumKey,
        RegLoadHive,
        RegUnloadHive
    }

    internal enum REG_STATUS
    {
        SUCCESS,
        FAILED,
        ACCESS_DENIED,
        NOT_SUPPORTED,
    }

    internal enum REG_TYPE
    {
        HIVE,
        KEY,
        VALUE
    }

    internal enum REG_VALUE_TYPE
    {
        REG_NONE,
        REG_SZ,
        REG_EXPAND_SZ,
        REG_BINARY,
        REG_DWORD,
        REG_DWORD_BIG_ENDIAN,
        REG_LINK,
        REG_MULTI_SZ,
        REG_RESOURCE_LIST,
        REG_FULL_RESOURCE_DESCRIPTOR,
        REG_RESOURCE_REQUIREMENTS_LIST,
        REG_QWORD
    }

    internal sealed class REG_ITEM
    {
        public byte[] Data { get; internal set; }
        public REG_HIVES? Hive { get; internal set; }
        public string Key { get; internal set; }
        public string Name { get; internal set; }
        public REG_TYPE? Type { get; internal set; }
        public uint? ValueType { get; internal set; }
    }
}