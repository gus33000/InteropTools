using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace InteropTools.Classes
{
    public static class VersionHelper
    {
        public static string GetVersion()
        {
            PackageVersion version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public static string GetBuildString()
        {
            return $"{GetVersion()} ({GetBranch()}.{GetBuildDate()})";
        }

        public static string GetBranch()
        {
            Type myType = Type.GetType("InteropTools.ShellPages.Private.YourWindowsBuildPage");
            string additional = myType != null ? "/private" : "";
            return $"fbl_prerelease(gustavem){additional}";
        }

        public static string GetBuildDate()
        {
            Assembly assembly = Windows.UI.Xaml.Application.Current.GetType().GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("InteropTools.Resources.BuildDate.txt");
            return new StreamReader(resource).ReadLine().Replace("\r", "");
        }
    }
}
