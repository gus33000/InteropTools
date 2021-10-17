// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InteropTools.Providers.Applications.Definition
{
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