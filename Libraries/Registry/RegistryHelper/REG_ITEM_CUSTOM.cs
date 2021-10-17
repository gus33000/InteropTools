namespace RegistryHelper
{
    public sealed class REG_ITEM_CUSTOM
    {
        public string DataAsString { get; internal set; }
        public REG_HIVES Hive { get; internal set; }
        public string Key { get; internal set; }
        public string Name { get; internal set; }
        public REG_TYPE Type { get; internal set; }
        public uint? ValueType { get; internal set; }
    }
}