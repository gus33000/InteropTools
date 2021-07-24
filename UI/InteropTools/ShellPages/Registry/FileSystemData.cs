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
                    /*var key = (string.IsNullOrEmpty(RegItem.Key)) ? RegItem.Name : (RegItem.Key + @"\" + RegItem.Name);
					var result = App.MainRegistryHelper.GetKeyLastModifiedTime(RegItem.Hive, key, out lastModified);

					if (result == HelperErrorCodes.SUCCESS)
					{
						return name + " (" + lastModified.ToString() + ")";
					}*/
                }

                return name;
            }
        }

        private readonly string name;

        public bool IsFolder => RegItem?.Type == RegistryItemType.KEY;

        public bool IsHive => RegItem?.Type == RegistryItemType.HIVE;

        public bool IsNothing => !(IsHive || IsFolder);

        public bool HasMore =>
                    /*if (RegItem != null)
{
var key = RegItem.Key;

if (RegItem.Type == RegistryItemType.KEY)
{
if ((key == "") || (key == null))
{
    key = RegItem.Name;
}

else
{
    key += @"\" + RegItem.Name;
}
}

if (key == null)
{
key = "";
}

var items = App.MainRegistryHelper.GetRegistryItems2(RegItem.Hive, key);

foreach (var item in items)
if (item.Type == RegistryItemType.KEY)
{ return true; }

return false;
}

else
{*/
                    true;//}

        public RegistryItemCustom RegItem { get; set; }
    }
}
