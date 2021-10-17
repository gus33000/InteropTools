// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InteropTools.Providers.Registry.Definition
{
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

        public IEnumerator<AbstractOption> GetEnumerator() => Settings.Where(f => f != null).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Settings.GetEnumerator();

        protected abstract AbstractOption[] GetSettings();
    }
}
