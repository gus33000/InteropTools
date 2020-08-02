using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InteropTools.Providers.Registry.Definition
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
                if (this.optionsIdentifyer == null)
                    this.optionsIdentifyer = this.Options.OptionsIdentifier;
                return this.optionsIdentifyer.Value;
            }
            set
            {
                this.optionsIdentifyer = value;
            }
        }

        private Options options;
        internal Options Options
        {
            get
            {
                if (this.options == null)
                    this.options = new OptionsImpl(this.settings ?? throw new InvalidOperationException(), this.optionsIdentifyer ?? throw new InvalidOperationException());
                return this.options;
            }
            set
            {
                this.options = value;
            }
        }

        private AbstractOption[] settings;
        [DataMember]
        internal AbstractOption[] Settings
        {
            get
            {
                if (this.settings == null)
                    this.settings = this.options?.Settings ?? throw new InvalidOperationException();
                return this.settings;
            }
            set
            {
                this.settings = value;
            }
        }

        public int Count => this.Options.Count;

        public AbstractOption this[int index] => this.Options[index];




        public IEnumerator<AbstractOption> GetEnumerator() => this.Options.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.Options.GetEnumerator();

        private class OptionsImpl : Options
        {
            private readonly Guid guid;
            private readonly AbstractOption[] settings;
            public OptionsImpl(AbstractOption[] settings, Guid guid)
            {
                this.settings = settings;
                this.guid = guid;
            }

            public override Guid OptionsIdentifier => this.guid;

            protected override AbstractOption[] GetSettings() => this.settings;

        }
    }

    public abstract class Options : IReadOnlyList<AbstractOption>
    {
        public abstract Guid OptionsIdentifier { get; }

        public AbstractOption this[int index]
        {
            get
            {
                return this.Settings[index];
            }
        }

        public int Count => this.Settings.Length;

        private AbstractOption[] settings;

        public AbstractOption[] Settings
        {
            get
            {
                this.settings = this.settings ?? GetSettings();
                return this.settings;
            }
        }

        protected abstract AbstractOption[] GetSettings();

        public IEnumerator<AbstractOption> GetEnumerator()
        {
            return this.Settings.OfType<AbstractOption>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Settings.GetEnumerator();
        }
    }

    [DataContract]
    public class AbstractOption
    {
        public AbstractOption(string name, string description)
        {
            this.Name = name;
            this.Description = description;
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
            this.Min = min;
            this.Max = max;
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
