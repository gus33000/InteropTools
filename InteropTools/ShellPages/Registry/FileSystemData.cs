using InteropTools.Providers;

namespace InteropTools.ShellPages.Registry
{
    public class FileSystemData
    {
        private readonly string name;

        public FileSystemData(string name)
        {
            this.name = name;
        }

        public bool HasMore => true;

        public bool IsFolder => RegItem?.Type == RegistryItemType.KEY;

        public bool IsHive => RegItem?.Type == RegistryItemType.HIVE;

        public bool IsNothing => !(IsHive || IsFolder);

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

        public RegistryItemCustom RegItem { get; set; }
    }
}