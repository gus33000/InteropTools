using InteropTools.Providers;

namespace InteropTools.ShellPages.Registry
{
    public class FileSystemData
    {
        public FileSystemData(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                if (IsFolder)
                {
                }

                return name;
            }
        }

        private readonly string name;

        public bool IsFolder => RegItem?.Type == RegistryItemType.KEY;

        public bool IsHive => RegItem?.Type == RegistryItemType.HIVE;

        public bool IsNothing => !(IsHive || IsFolder);

        public bool HasMore => true;

        public RegistryItemCustom RegItem { get; set; }
    }
}
