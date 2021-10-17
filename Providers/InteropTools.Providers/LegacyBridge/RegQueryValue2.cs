namespace InteropTools.Providers
{
    public class RegQueryValue2
    {
        public uint regtype { get; set; }
        public byte[] regvalue { get; set; }
        public HelperErrorCodes returncode { get; set; }
    }
}