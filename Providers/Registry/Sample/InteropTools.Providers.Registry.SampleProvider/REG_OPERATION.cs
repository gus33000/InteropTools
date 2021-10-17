namespace InteropTools.Providers.Registry.SampleProvider
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