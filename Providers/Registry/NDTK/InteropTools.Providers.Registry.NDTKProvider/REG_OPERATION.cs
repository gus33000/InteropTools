namespace InteropTools.Providers.Registry.NDTKProvider
{
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
}