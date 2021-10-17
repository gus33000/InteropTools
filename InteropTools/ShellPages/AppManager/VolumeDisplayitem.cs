// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.Management.Deployment;
using Windows.UI.Xaml;

namespace InteropTools.ShellPages.AppManager
{
    public class VolumeDisplayitem
    {
        public string _MountPoint = InteropTools.Resources.TextResources.ApplicationManager_MountPoint;
        public string _Name = InteropTools.Resources.TextResources.ApplicationManager_Name;
        public string _Offline = InteropTools.Resources.TextResources.ApplicationManager_Offline;
        public string _PackageStore = InteropTools.Resources.TextResources.ApplicationManager_PackageStore;
        public string _SupportsHardLinks = InteropTools.Resources.TextResources.ApplicationManager_SupportsHardLinks;
        public string _SystemVolume = InteropTools.Resources.TextResources.ApplicationManager_SystemVolume;
        public Visibility AllVisibility => Volume == null ? Visibility.Visible : Visibility.Collapsed;

        public PackageVolume Volume
        {
            get;
            set;
        }

        public Visibility VolumeVisibility => Volume != null ? Visibility.Visible : Visibility.Collapsed;
    }
}
