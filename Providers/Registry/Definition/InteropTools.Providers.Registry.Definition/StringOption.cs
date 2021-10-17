using System.Runtime.Serialization;

namespace InteropTools.Providers.Registry.Definition
{
    [DataContract]
    public class StringOption : AbstractOption
    {
        public StringOption(string name, string description) : base(name, description)
        {
        }

        [DataMember]
        public string Value { get; set; }
    }
}