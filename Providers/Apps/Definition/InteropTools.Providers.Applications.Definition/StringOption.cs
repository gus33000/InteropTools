// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Runtime.Serialization;

namespace InteropTools.Providers.Applications.Definition
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