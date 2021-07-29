using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace InteropTools.Providers.Applications.Definition
{
    [DataContract]
    [KnownType(typeof(IntOption))]
    [KnownType(typeof(StringOption))]
    public class TransfareOptions : IReadOnlyList<AbstractOption>
    {
        internal Guid? optionsIdentifyer;
        [DataMember]
        internal Guid OptionsIdentifyer
        {
            get
            {
                return optionsIdentifyer ??= Options.OptionsIdentifier;
            }
            set => optionsIdentifyer = value;
        }

        private Options options;
        internal Options Options
        {
            get
            {
                return options ??= new OptionsImpl(settings ?? throw new InvalidOperationException(), optionsIdentifyer ?? throw new InvalidOperationException());
            }
            set => options = value;
        }

        private AbstractOption[] settings;
        [DataMember]
        internal AbstractOption[] Settings
        {
            get
            {
                return settings ??= options?.Settings ?? throw new InvalidOperationException();
            }
            set => settings = value;
        }

        public int Count => Options.Count;

        public AbstractOption this[int index] => Options[index];

        public IEnumerator<AbstractOption> GetEnumerator()
        {
            return Options.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Options.GetEnumerator();
        }

        private class OptionsImpl : Options
        {
            private readonly Guid guid;
            private readonly AbstractOption[] settings;
            public OptionsImpl(AbstractOption[] settings, Guid guid)
            {
                this.settings = settings;
                this.guid = guid;
            }

            public override Guid OptionsIdentifier => guid;

            protected override AbstractOption[] GetSettings()
            {
                return settings;
            }
        }
    }

    public abstract class Options : IReadOnlyList<AbstractOption>
    {
        public abstract Guid OptionsIdentifier { get; }

        public AbstractOption this[int index] => Settings[index];

        public int Count => Settings.Length;

        private AbstractOption[] settings;

        public AbstractOption[] Settings
        {
            get
            {
                settings = settings ?? GetSettings();
                return settings;
            }
        }

        protected abstract AbstractOption[] GetSettings();

        public IEnumerator<AbstractOption> GetEnumerator()
        {
            return Settings.Where(f => f != null).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Settings.GetEnumerator();
        }
    }

    [DataContract]
    public class AbstractOption
    {
        public AbstractOption(string name, string description)
        {
            Name = name;
            Description = description;
        }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Description { get; private set; }
    }

    [DataContract]
    public class IntOption : AbstractOption
    {
        public IntOption(string name, string description, int min, int max) : base(name, description)
        {
            Min = min;
            Max = max;
        }
        [DataMember]
        public int Min { get; private set; }
        [DataMember]
        public int Max { get; private set; }
        [DataMember]
        public int Value { get; set; }
    }

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
