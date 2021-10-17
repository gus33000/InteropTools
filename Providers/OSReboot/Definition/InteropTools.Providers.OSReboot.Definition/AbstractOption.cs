// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

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
}