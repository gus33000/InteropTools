using System.Runtime.Serialization;

namespace InteropTools.AppExtensibilityDefinition
{
    [DataContract]
    public class IntOption : AbstractOption
    {
        public IntOption(string name, string description, int min, int max) : base(name, description)
        {
            Min = min;
            Max = max;
        }

        [DataMember]
        public int Max { get; private set; }

        [DataMember]
        public int Min { get; private set; }

        [DataMember]
        public int Value { get; set; }
    }
}