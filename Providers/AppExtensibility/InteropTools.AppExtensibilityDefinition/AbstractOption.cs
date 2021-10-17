using System.Runtime.Serialization;

namespace InteropTools.AppExtensibilityDefinition
{
    [DataContract]
    public class AbstractOption
    {
        public AbstractOption(string name, string description)
        {
            Name = name;
            Description = description;
        }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public string Name { get; private set; }
    }
}