using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace InteropTools.Providers.OSReboot.Definition
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

    public abstract class Options : IReadOnlyList<AbstractOption>
    {
        private AbstractOption[] settings;
        public int Count => Settings.Length;
        public abstract Guid OptionsIdentifier { get; }

        public AbstractOption[] Settings
        {
            get
            {
                settings = settings ?? GetSettings();
                return settings;
            }
        }

        public AbstractOption this[int index] => Settings[index];

        public IEnumerator<AbstractOption> GetEnumerator()
        {
            return Settings.Where(f => f != null).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Settings.GetEnumerator();
        }

        protected abstract AbstractOption[] GetSettings();
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

    [DataContract]
    [KnownType(typeof(IntOption))]
    [KnownType(typeof(StringOption))]
    public class TransfareOptions : IReadOnlyList<AbstractOption>
    {
        internal Guid? optionsIdentifyer;

        private Options options;

        private AbstractOption[] settings;

        public int Count => Options.Count;

        internal Options Options
        {
            get
            {
                return options ??= new OptionsImpl(settings ?? throw new InvalidOperationException(), optionsIdentifyer ?? throw new InvalidOperationException());
            }
            set => options = value;
        }

        [DataMember]
        internal Guid OptionsIdentifyer
        {
            get
            {
                return optionsIdentifyer ??= Options.OptionsIdentifier;
            }
            set => optionsIdentifyer = value;
        }

        [DataMember]
        internal AbstractOption[] Settings
        {
            get
            {
                return settings ??= options?.Settings ?? throw new InvalidOperationException();
            }
            set => settings = value;
        }

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
}