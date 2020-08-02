using System.IO;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel;

namespace InteropTools.Handlers
{
    public class VersionHandler
    {
        private string branch = "farewell_refactor";

        public string BuildString
        {
            get => GetBuildString();
        }

        public string Version
        {
            get => GetVersion();
        }

        private string GetBuildString()
        {
            var buildString = "";
            var assembly = GetType().GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream("InteropTools.Resources.BuildDate.txt");
            var data = new StreamReader(resource).ReadToEnd().Split('\n');
            var builddate = data.First().Replace("\r", "");
            var appver = Package.Current.Id.Version;
            var appverstr = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
            buildString = appverstr + " (" + branch + "(" + data.ElementAt(1).Replace("\r", "") + ")." + builddate + ")";
            return buildString;
        }

        private string GetVersion()
        {
            var appver = Package.Current.Id.Version;
            var appverstr = string.Format("{0}.{1}", appver.Major, appver.Minor);
            return appverstr;
        }
    }
}
