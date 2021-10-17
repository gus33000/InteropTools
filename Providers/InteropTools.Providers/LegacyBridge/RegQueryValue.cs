namespace InteropTools.Providers
{
    public class RegQueryValue
    {
        public RegTypes regtype { get; set; }
        public string regvalue { get; set; }
        public HelperErrorCodes returncode { get; set; }
    }
}