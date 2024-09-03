using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PhotoshoppedUUPCLI.UUP;
using PhotoshoppedUUPCLI.UUP.FE3.Requests;
using PhotoshoppedUUPCLI.UUP.FE3.Responses;
using PhotoshoppedUUPCLI;
using Windows.Web.Http;
using Windows.Storage;
using System.Diagnostics;
using GetAppxPackages.WU;
using System.Reflection;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Gofly
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RingPage : Page
    {
        public RingPage()
        {
            this.InitializeComponent();
            CheckAllPossibleRings();
        }

        public async Task CheckAllPossibleRings()
        {
            // We have to check:
            // W/O Skip ahead:
            //  Desktop RP
            //  Desktop MSIT
            //  Desktop WIS
            //  Desktop WIF
            //  IoTCore RP
            //  IoTCore MSIT
            //  IoTCore WIS
            //  IoTCore WIF
            //  IoTCore OSG
            //  IoTCore Canary
            // Mobile RP
            // Mobile WIS
            // Mobile WIF

            var token = await new StoreProjectApp.ContentDialogs.MBILoginContentDialog().GetToken();


            var pcprodb = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "Retail", "en-US", "Branch"));
            BrClientProd.Text = pcprodb.BuildString;

            var pcrpb = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "RP", "en-US", "Branch"));
            BrClientIRP.Text = pcrpb.BuildString;

            var pcmsitb = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "MSIT", "en-US", "Branch"));
            BrClientMSIT.Text = pcmsitb.BuildString;

            var pcwisb = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIS", "en-US", "Branch"));
            BrClientWIS.Text = pcwisb.BuildString;

            var pcwifb = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIF", "en-US", "Branch"));
            BrClientWIF.Text = pcwifb.BuildString;


            var pcprod = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "Retail", "en-US", "Active"));
            ActiveClientProd.Text = pcprod.BuildString;

            var pcrp = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "RP", "en-US", "Active"));
            ActiveClientIRP.Text = pcrp.BuildString;

            var pcmsit = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "MSIT", "en-US", "Active"));
            ActiveClientMSIT.Text = pcmsit.BuildString;

            var pcwis = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "WIS", "en-US", "Active"));
            ActiveClientWIS.Text = pcwis.BuildString;

            var pcwif = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "WIF", "en-US", "Active"));
            ActiveClientWIF.Text = pcwif.BuildString;


            var pcprods = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "Retail", "en-US", "Skip"));
            SkipClientProd.Text = pcprods.BuildString;

            var pcrps = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "RP", "en-US", "Skip"));
            SkipClientIRP.Text = pcrps.BuildString;

            var pcmsits = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "MSIT", "en-US", "Skip"));
            SkipClientMSIT.Text = pcmsits.BuildString;

            var pcwiss = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "WIS", "en-US", "Skip"));
            SkipClientWIS.Text = pcwiss.BuildString;

            var pcwifs = await CheckCurrentBuild(token, new AttributeData(OSType.Client, Architecture.amd64, PhoneReleaseType.Test, "EXTERNAL", "WIF", "en-US", "Skip"));
            SkipClientWIF.Text = pcwifs.BuildString;


            var iotprodb = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "Retail", "en-US", "Branch"));
            BrIoTProd.Text = iotprodb.BuildString;

            var iotrpb = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "RP", "en-US", "Branch"));
            BrIoTIRP.Text = iotrpb.BuildString;

            var iotmsitb = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "MSIT", "en-US", "Branch"));
            BrIoTMSIT.Text = iotmsitb.BuildString;

            var iotwisb = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIS", "en-US", "Branch"));
            BrIoTWIS.Text = iotwisb.BuildString;

            var iotwifb = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIF", "en-US", "Branch"));
            BrIoTWIF.Text = iotwifb.BuildString;

            var iotosgb = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "OSG", "en-US", "Branch"));
            BrIoTSELF.Text = iotosgb.BuildString;

            var iotcanaryb = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "Canary", "en-US", "Branch"));
            BrIoTCANARY.Text = iotcanaryb.BuildString;

            var iotprod = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "Retail", "en-US", "Active"));
            ActiveIoTProd.Text = iotprod.BuildString;

            var iotrp = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "RP", "en-US", "Active"));
            ActiveIoTIRP.Text = iotrp.BuildString;

            var iotmsit = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "MSIT", "en-US", "Active"));
            ActiveIoTMSIT.Text = iotmsit.BuildString;

            var iotwis = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIS", "en-US", "Active"));
            ActiveIoTWIS.Text = iotwis.BuildString;

            var iotwif = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIF", "en-US", "Active"));
            ActiveIoTWIF.Text = iotwif.BuildString;

            var iotosg = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "OSG", "en-US", "Active"));
            ActiveIoTSELF.Text = iotosg.BuildString;

            var iotcanary = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "Canary", "en-US", "Active"));
            ActiveIoTCANARY.Text = iotcanary.BuildString;


            var iotprods = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "Retail", "en-US", "Skip"));
            SkipIoTProd.Text = iotprods.BuildString;

            var iotrps = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "RP", "en-US", "Skip"));
            SkipIoTIRP.Text = iotrps.BuildString;

            var iotmsits = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "MSIT", "en-US", "Skip"));
            SkipIoTMSIT.Text = iotmsits.BuildString;

            var iotwiss = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIS", "en-US", "Skip"));
            SkipIoTWIS.Text = iotwiss.BuildString;

            var iotwifs = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "WIF", "en-US", "Skip"));
            SkipIoTWIF.Text = iotwifs.BuildString;

            var iotosgs = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "OSG", "en-US", "Skip"));
            SkipIoTSELF.Text = iotosgs.BuildString;

            var iotcanarys = await CheckCurrentBuild(token, new AttributeData(OSType.IoTCore, Architecture.amd64, PhoneReleaseType.Test, "RS_PRERELEASE", "Canary", "en-US", "Skip"));
            SkipIoTCANARY.Text = iotcanarys.BuildString;


            var mobprodb = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "RS_PRERELEASE", "Retail", "en-US", "Branch"));
            BrMobileProd.Text = mobprodb.BuildString;

            var mobrpb = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "RS_PRERELEASE", "RP", "en-US", "Branch"));
            BrMobileIRP.Text = mobrpb.BuildString;

            var mobmsitb = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "RS_PRERELEASE", "MSIT", "en-US", "Branch"));
            BrMobileMSIT.Text = mobmsitb.BuildString;

            var mobwisb = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "RS_PRERELEASE", "WIS", "en-US", "Branch"));
            BrMobileWIS.Text = mobwisb.BuildString;

            var mobwifb = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "RS_PRERELEASE", "WIF", "en-US", "Branch"));
            BrMobileWIF.Text = mobwifb.BuildString;


            var mobprod = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "Retail", "en-US", "Active"));
            ActiveMobileProd.Text = mobprod.BuildString;

            var mobrp = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "RP", "en-US", "Active"));
            ActiveMobileIRP.Text = mobrp.BuildString;

            var mobmsit = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "MSIT", "en-US", "Active"));
            ActiveMobileMSIT.Text = mobmsit.BuildString;

            var mobwis = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "WIS", "en-US", "Active"));
            ActiveMobileWIS.Text = mobwis.BuildString;

            var mobwif = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "WIF", "en-US", "Active"));
            ActiveMobileWIF.Text = mobwif.BuildString;


            var mobprods = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "Retail", "en-US", "Skip"));
            SkipMobileProd.Text = mobprods.BuildString;

            var mobrps = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "RP", "en-US", "Skip"));
            SkipMobileIRP.Text = mobrps.BuildString;

            var mobmsits = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "MSIT", "en-US", "Skip"));
            SkipMobileMSIT.Text = mobmsits.BuildString;

            var mobwiss = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "WIS", "en-US", "Skip"));
            SkipMobileWIS.Text = mobwiss.BuildString;

            var mobwifs = await CheckCurrentBuild(token, new AttributeData(OSType.Mobile, Architecture.arm, PhoneReleaseType.Production, "EXTERNAL", "WIF", "en-US", "Skip"));
            SkipMobileWIF.Text = mobwifs.BuildString;
        }

        public class CheckBuildResult
        {
            public string BuildStr { get; set; }
            public string BuildString { get { return BuildStr + " - " + Meta;  } }
            public string Meta { get; set; }
        }

        public async static Task<CheckBuildResult> CheckCurrentBuild(string token, AttributeData attributedata)
        {
            var result = new CheckBuildResult();
            try
            {
                var updates = await SyncUpdates(true, attributedata, token);

                foreach (var update in updates)
                {
                    if (update.Files.Any(x => x.FileName.EndsWith("Deployment.cab")))
                    {
                        // Call FE3 to get the list of files in the update we're dealing with right now
                        var request = await RequestHandler.GetExtendedUpdateInfo2RequestAsync(update.UpdateID, update.RevisionNumber, attributedata, token);

                        // Parse the response.
                        var inforesponse = await GetResponseSecured(request);
                        var inforesp = ResponseHandler.GetExtendedUpdateInfo2Response(inforesponse);

                        var filetoget = update.Files.First(x => x.FileName.EndsWith("Deployment.cab"));

                        if (inforesp.GetExtendedUpdateInfo2Result.FileLocations.Any(x => x.FileDigest == filetoget.Digest))
                        {
                            var url = inforesp.GetExtendedUpdateInfo2Result.FileLocations.First(x => x.FileDigest == filetoget.Digest).Url;
                            await WebUtils.DownloadAsync(new Uri(url), ApplicationData.Current.LocalFolder.Path + @"\Deployment.cab");

                            var bytes = System.IO.File.ReadAllBytes(ApplicationData.Current.LocalFolder.Path + @"\Deployment.cab");

                            int length;
                            byte[] outdata;
                            CabExtract.ExtractFile(bytes, "UpdateAgent.dll", out outdata, out length);

                            var verstr = System.Text.Encoding.Unicode.GetString(HexToBytes(ToHex(outdata).Split(new string[] { "460069006c006500560065007200730069006f006e0000000000" }, StringSplitOptions.None).Last().Split(new string[] { "000000" }, StringSplitOptions.None).First() + "00"));

                            System.IO.File.Delete(ApplicationData.Current.LocalFolder.Path + @"\Deployment.cab");

                            result.BuildStr = verstr;
                            result.Meta = update.FlightMeta;
                            return result;
                        }
                    }
                }
            }
            catch
            {

            }
            result.BuildStr = "N/A";
            result.Meta = "N/A";
            return result;
        }

        public static string ToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

        public static byte[] HexToBytes(string str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }

        public static async Task<List<SyncUpdateValues>> SyncUpdates(bool fastmode, AttributeData attribdata, string token)
        {

            bool done = false;

            List<SyncUpdatesResponseClasses.Update> osupdates = new List<SyncUpdatesResponseClasses.Update>();
            List<SyncUpdatesResponseClasses.Update> locosupdates = new List<SyncUpdatesResponseClasses.Update>();
            List<SyncUpdatesResponseClasses.UpdateInfo> otherupdates = new List<SyncUpdatesResponseClasses.UpdateInfo>();

            List<string> cachedupdatesids = new List<string>();

            while (!done)
            {
                string cachedstr = "";

                foreach (var id in cachedupdatesids)
                    cachedstr += "<int>" + id + "</int>";

                var cookiedata = await GetCookie();
                var request = await RequestHandler.GetSyncUpdatesRequestAsync(cookiedata, attribdata, cachedstr, token);

                var syncupdatesresponse = await GetResponse(request);
                var getsyncupdatesresp = ResponseHandler.GetSyncUpdatesResponse(syncupdatesresponse);

                List<SyncUpdatesResponseClasses.Update> updates = getsyncupdatesresp.SyncUpdatesResult.ExtendedUpdateInfo.Updates.Update;

                if (updates.Count != 0)
                {
                    foreach (var update in updates)
                    {
                        if (update.Xml.Contains("OSInstaller"))
                        {
                            osupdates.Add(update);
                        }

                        cachedupdatesids.Add(update.ID);
                    }

                    foreach (var update in updates)
                    {
                        foreach (var update2 in osupdates)
                            if (update.ID == update2.ID && update.Xml != update2.Xml)
                                locosupdates.Add(update);

                        foreach (var update2 in getsyncupdatesresp.SyncUpdatesResult.NewUpdates.UpdateInfo)
                        {
                            if (update2.ID == update.ID)
                                otherupdates.Add(update2);
                        }
                    }
                }
                else
                {
                    done = true;
                }

                if (fastmode)
                    done = fastmode;
            }

            List<SyncUpdateValues> updatelist = new List<SyncUpdateValues>();

            foreach (var update in osupdates)
            {
                var result = new SyncUpdateValues();

                foreach (var update2 in locosupdates)
                {
                    if (update2.ID == update.ID)
                    {
                        XmlSerializer serializer2 = new XmlSerializer(typeof(XmlResponseClasses.Xml));

                        using (StringReader textReader = new StringReader("<Xml>" + update2.Xml + "</Xml>"))
                        {
                            var xml = ((XmlResponseClasses.Xml)serializer2.Deserialize(textReader));

                            foreach (var prop in xml.LocalizedProperties)
                            {
                                result.UpdateTitle = prop.Title;
                                result.UpdateDescription = prop.Description;
                            }
                        }
                    }
                }
                
                List<PhotoshoppedUUPCLI.UUP.FileInfo> filelist = new List<PhotoshoppedUUPCLI.UUP.FileInfo>();

                XmlSerializer serializer = new XmlSerializer(typeof(XmlResponseClasses.Xml));

                using (StringReader textReader2 = new StringReader("<Xml>" + update.Xml + "</Xml>"))
                {
                    var xml2 = ((XmlResponseClasses.Xml)serializer.Deserialize(textReader2));

                    foreach (var file2 in xml2.Files.File)
                    {
                        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
                        long bytes = Math.Abs(uint.Parse(file2.Size));
                        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                        var Size = (Math.Sign(uint.Parse(file2.Size)) * num).ToString() + suf[place] + " (" + file2.Size + " Bytes)";

                        filelist.Add(new PhotoshoppedUUPCLI.UUP.FileInfo() { Digest = file2.Digest, FileName = file2.FileName, LastModified = file2.Modified, Size = file2.Size, LocalizedSize = Size, LocalizedLastModified = DateTime.Parse(file2.Modified).ToString() });
                    }
                }

                foreach (var update2 in otherupdates)
                {
                    if (update2.ID == update.ID)
                    {
                        XmlSerializer serializer2 = new XmlSerializer(typeof(XmlResponseClasses.Xml));

                        using (StringReader textReader = new StringReader("<Xml>" + update2.Xml + "</Xml>"))
                        {
                            var xml = ((XmlResponseClasses.Xml)serializer2.Deserialize(textReader));

                            foreach (var prop in xml.UpdateIdentity)
                            {
                                result.UpdateID = prop.UpdateID;
                                result.RevisionNumber = prop.RevisionNumber;
                                break;
                            }
                        }

                        result.FlightMeta = update2.Deployment.FlightId + " (" + update2.Deployment.LastChangeTime + ")";
                    }
                }
                result.Attributes = attribdata;
                result.DownloadFullOnly = true;
                result.Files = filelist;
                updatelist.Add(result);
            }

            return updatelist;
        }

        public async static Task<string> GetResponse(string body)
        {
            var httprequest = CreateWebRequest("https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx");

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(body);

            httprequest.Headers["ContentLength"] = byte1.Length.ToString();

            Stream newStream = await httprequest.GetRequestStreamAsync();
            newStream.Write(byte1, 0, byte1.Length);

            string soapResult;
            WebResponse webResponse = await httprequest.GetResponseAsync();
            using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
            {
                soapResult = rd.ReadToEnd();
            }

            return soapResult;
        }


        public async static Task<CookieData> GetCookie()
        {
            // Looks like my GetCookie code broke again, let's hardcode the token, it expires in 2045 anyway.

            /*var request = RequestHandler.GetCookieRequest();
            var cookie2response = await GetResponse(request);
            var getcookieresp = ResponseHandler.GetCookieResponse(cookie2response);*/

            // See you in 2045 with the world falling off because of this
            //return new CookieData("2045-01-27T14:47:10Z", "");

            var getcookieresp = await new Parser().GetCookieAsync();

            return new CookieData(getcookieresp.Expiration, getcookieresp.EncryptedData);
        }

        public async static Task<string> GetResponseSecured(string body)
        {
            var httprequest = CreateWebRequest("https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx/Secured");

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(body);

            httprequest.Headers["ContentLength"] = byte1.Length.ToString();

            Stream newStream = await httprequest.GetRequestStreamAsync();
            newStream.Write(byte1, 0, byte1.Length);

            string soapResult;
            WebResponse webResponse = await httprequest.GetResponseAsync();
            using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
            {
                soapResult = rd.ReadToEnd();
            }

            return soapResult;
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Proxy = null;
            webRequest.Headers["MS-CV"] = "HX52syBFzEGlzh94.0.1.0.0.2.1";
            webRequest.Headers["UserAgent"] = "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/1.56";
            webRequest.ContentType = "application/soap+xml; charset=utf-8";
            webRequest.Accept = "xpress";
            webRequest.Method = "POST";
            return webRequest;
        }
    }
}
