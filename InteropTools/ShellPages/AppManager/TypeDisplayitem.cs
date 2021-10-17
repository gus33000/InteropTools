namespace InteropTools.ShellPages.AppManager
{
    public class TypeDisplayitem
    {
        public PackageTypes? Type
        {
            get;
            set;
        }

        public string TypeName => Type == null ? Resources.TextResources.ApplicationManager_AllTypes : Type.ToString();
    }
}