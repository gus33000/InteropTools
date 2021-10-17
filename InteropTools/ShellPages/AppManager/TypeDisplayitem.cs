// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.Management.Deployment;

namespace InteropTools.ShellPages.AppManager
{
    public class TypeDisplayitem
    {
        public PackageTypes? Type
        {
            get;
            set;
        }

        public string TypeName => Type == null ? InteropTools.Resources.TextResources.ApplicationManager_AllTypes : Type.ToString();
    }
}
