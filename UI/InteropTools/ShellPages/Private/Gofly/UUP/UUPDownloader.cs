using PhotoshoppedUUPCLI.UUP.FE3.Responses;
using PhotoshoppedUUPCLI.UUP.FE3.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace PhotoshoppedUUPCLI.UUP
{

    public enum PhoneReleaseType
    {
        Production,
        Test
    }

    public enum OSType
    {
        Client,
        Mobile,
        Server,
        ServerCore,
        IoTCore,
        HoloLens,
        Andromeda,
        Xbox
    }

    public enum Architecture
    {
        arm,
        arm64,
        amd64,
        x86,
        woa
    }

    /*public enum Platform
    {
        Mobile,
        Desktop,
        Analog,
        Team,
        IoTUAP,
        Xbox,
        Server
    }*/

    /*public enum DownloadOption
    {
        All,
        FullOnly,
        Custom,
        None
    }*/

    public class SyncUpdateValues
    {
        public string UpdateID { get; set; }
        public string RevisionNumber { get; set; }
        public AttributeData Attributes { get; set; }
        public string UpdateTitle { get; set; }
        public string UpdateDescription { get; set; }
        public List<FileInfo> Files { get; set; }
        public bool DownloadFullOnly { get; set; }
        public string FlightMeta { get; set; }
    }

    public class FileInfo
    {
        public string FileName { get; set; }
        public string Size { get; set; }
        public string LastModified { get; set; }
        public string Digest { get; set; }
        public string LocalizedSize { get; set; }
        public string LocalizedLastModified { get; set; }
    }

    public class AttributeData
    {
        public AttributeData(OSType ostype, Architecture arch, PhoneReleaseType phonereleasetype, string branch, string ring, string language, string flightcontent)
        {
            DeviceAttributes = $"App=WU;AppVer=10.0.14800.1000;AttrDataVer=21;BranchReadinessLevel=CB;FlightContent={flightcontent};FlightingBranchName={branch.ToUpper()};FlightRing={ring.ToUpper()};InstallLanguage={language};IsFlightingEnabled={(ring.ToLower() != "retail" ? "1" : "0")};OEMModel=RM-1085;OEMName_Uncleaned=RM-1085;OSSkuId=4;PhoneTargetingName=MICROSOFTMDG;OSVersion=10.0.14800.1000;ReleaseType={phonereleasetype};IsMsftOwned=1";

            //DeviceAttributes = $"App=WU;AppVer=10.0.14800.1000;AttrDataVer=21;BranchReadinessLevel=CB;CurrentBranch={branch.ToLower()};DeviceFamily=Windows.{platformname};FlightContent={flightcontent};FlightingBranchName={branch.ToUpper()};FlightRing={ring.ToUpper()};InstallationType={ostype.ToString().Replace("_", " ")};InstallLanguage={language};IsDeviceRetailDemo=0;IsFlightingEnabled={(ring.ToLower() != "retail" ? "1" : "0")};OEMModel=RM-1073;OEMName_Uncleaned=RM-1073;OSArchitecture={arch.ToString().ToUpper()};OSSkuId=4;OSUILocale={language};OSVersion=10.0.14800.1000;TelemetryLevel=2;UpdateManagementGroup=2;PhoneTargetingName=MICROSOFTMDG;ReleaseType={phonereleasetype};IsMsftOwned=1;";
            //DeviceAttributes = $"BranchReadinessLevel=CB;CurrentBranch={branch.ToLower()};OEMModel=RM-1073;FlightRing={ring.ToUpper()};AttrDataVer=9;InstallLanguage={language};OSUILocale={language};InstallationType={ostype.ToString().Replace("_", " ")};FlightingBranchName={branch.ToUpper()};FirmwareVersion=6.00;OSSkuId=4;App=WU;OEMName_Uncleaned=RM-1073;AppVer=10.0.14800.1000;OSArchitecture={arch.ToString().ToUpper()};UpdateManagementGroup=2;IsFlightingEnabled=1;IsDeviceRetailDemo=0;TelemetryLevel=3;DeferQualityUpdatePeriodInDays=0;DeferFeatureUpdatePeriodInDays=0;OSVersion=10.0.14800.1000;DeviceFamily=Windows.{platformname};ReleaseType={phonereleasetype};PhoneTargetingName=MICROSOFTMDG;FlightContent={flightcontent};";

            Products = $"PN={ostype.ToString().Replace("_", " ")}.OS.rs2.{arch.ToString().ToLower()}&amp;Branch={branch.ToLower()}&amp;V=10.0.14800.1000;";
            
            //Products = $"PN={ostype.ToString().Replace("_", " ")}.OS.rs2.{arch.ToString().ToLower()}&amp;Branch={branch.ToLower()}&amp;V=10.0.14800.1000;";
            this.arch = arch;
            this.ostype = ostype;
            this.language = language;
        }

        public Architecture arch
        {
            internal set;
            get;
        }

        public OSType ostype
        {
            internal set;
            get;
        }

        public string language
        {
            internal set;
            get;
        }

        public string DeviceAttributes
        {
            internal set;
            get;
        }
        public string Products
        {
            internal set;
            get;
        }
    }

    public class CookieData
    {
        public string Expiration { get; internal set; }
        public string EncryptedData { get; internal set; }

        public CookieData(string Expiration, string EncryptedData)
        {
            this.Expiration = Expiration;
            this.EncryptedData = EncryptedData;
        }
    }
}
