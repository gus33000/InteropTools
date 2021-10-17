namespace InteropTools.Providers
{
    public sealed class RegistryItem
    {
        public RegHives Hive { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public RegistryItemType Type { get; set; }
        public string Value { get; set; }
        public RegTypes ValueType { get; set; }
    }
}