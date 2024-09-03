using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GetAppxPackages.WU.FE3;
using System.Xml.Serialization;
using System.IO;
using StoreProjectApp.WU.FE3;
using Windows.Data.Xml.Dom;
using System.Diagnostics;

namespace GetAppxPackages.WU
{
    class Parser
    {
        public class Update
        {
            public string Title { get; internal set; }
            public string ID { get; internal set; }
            public string Description { get; internal set; }
            public List<UpdateFile> UpdateFiles { get; internal set; }
            public string UpdateID { get; internal set; }
            public string RevisionNumber { get; internal set; }

            public string BranchName { get; internal set; }
            public ushort BuildNumber { get; internal set; }
            public string ContentType { get; internal set; }
            public DateTime CreationDate { get; internal set; }
            public bool FromStoreService { get; internal set; }
            public bool IsAppxFramework { get; internal set; }
            public string LegacyMobileProductId { get; internal set; }
            public string LicensingPayloadId { get; internal set; }
            public string LicensingSalt { get; internal set; }
            public ulong MaxDownloadSize { get; internal set; }
            public byte MinDownloadSize { get; internal set; }
            public string PackageContentId { get; internal set; }
            public string PackageIdentityName { get; internal set; }
            public string Ring { get; internal set; }

            public bool IsAppxBundle { get; internal set; }
            public string ApplicabilityBlob { get; internal set; }
            public string PackageMoniker { get; internal set; }
            public string PackageType { get; internal set; }
        }

        public class UpdateFile
        {
            public string Digest { get; set; }
            public string DigestAlgorithm { get; set; }
            public string Url { get; set; }
            public string DecryptionKey { get; set; }
            public string FileName { get; set; }
            public string Modified { get; set; }
            public string Size { get; set; }
            public string SecurityData { get; set; }
        }

        public async Task<List<Update>> ScanUpdate(string token, string id)
        {
            List<Update> uplist = new List<Update>();

            Windows.Data.Xml.Dom.XmlDocument SyncUpdates = await SoapUtils.GetExtendedUpdateInfoAsync(token, long.Parse(id));

            var result = Deserialize(SyncUpdates, typeof(CGetExtendedUpdateInfoResponse.Envelope));
            CGetExtendedUpdateInfoResponse.Envelope response = ((CGetExtendedUpdateInfoResponse.Envelope)result);

            if (response.Body.GetExtendedUpdateInfoResponse.GetExtendedUpdateInfoResult != null)
            {
                var updates = response.Body.GetExtendedUpdateInfoResponse.GetExtendedUpdateInfoResult.Updates;

                // Grab updates which are OSFlights
                foreach (var update in updates)
                {
                    Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                    xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                    var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                    CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                    if (xmlresponse.ExtendedProperties != null)
                    {
                        Update up = new Update();
                        up.ID = update.ID.ToString();
                        uplist.Add(up);
                    }
                }

                // Grab details about updates being OSFlights
                foreach (var update in updates)
                {
                    Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                    xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                    var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                    CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                    if (xmlresponse.LocalizedProperties != null)
                    {
                        int counter = -1;
                        foreach (Update up in uplist)
                        {
                            counter++;
                            if (up.ID == update.ID.ToString())
                            {
                                uplist[counter].Title = xmlresponse.LocalizedProperties.Title;
                                uplist[counter].Description = xmlresponse.LocalizedProperties.Description;
                            }
                        }
                    }
                }

                // Grab files about updates being OSFlights
                foreach (var update in updates)
                {
                    Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                    xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                    var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                    CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                    if (xmlresponse.Files != null)
                    {
                        int counter = -1;
                        foreach (Update up in uplist)
                        {
                            counter++;
                            if (up.ID == update.ID.ToString())
                            {
                                List<UpdateFile> filelist = new List<UpdateFile>();
                                foreach (var file in xmlresponse.Files)
                                {
                                    UpdateFile upfile = new UpdateFile();
                                    upfile.FileName = file.FileName;
                                    upfile.Digest = file.Digest;
                                    upfile.DigestAlgorithm = file.DigestAlgorithm;

                                    string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
                                    long bytes = long.Parse(file.Size.ToString());
                                    int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                                    double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                                    upfile.Size = (Math.Sign(bytes) * num).ToString() + suf[place] + " (" + file.Size + " Bytes)";

                                    upfile.Modified = file.Modified.ToUniversalTime().ToString();
                                    filelist.Add(upfile);
                                }
                                filelist.Sort((x, y) => y.FileName.CompareTo(x.FileName));
                                uplist[counter].UpdateFiles = filelist;
                            }
                        }
                    }
                }

                // Grab details about app updates packages
                foreach (var update in updates)
                {
                    Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                    xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                    var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                    CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                    if (xmlresponse.ExtendedProperties != null)
                    {
                        int counter = -1;
                        foreach (Update up in uplist)
                        {
                            counter++;
                            if (up.ID == update.ID.ToString())
                            {
                                if (xmlresponse.ExtendedProperties.BranchName != null)
                                {
                                    uplist[counter].BranchName = xmlresponse.ExtendedProperties.BranchName;
                                }
                                if (xmlresponse.ExtendedProperties.BuildNumber != null)
                                {
                                    uplist[counter].BuildNumber = xmlresponse.ExtendedProperties.BuildNumber;
                                }
                                if (xmlresponse.ExtendedProperties.ContentType != null)
                                {
                                    uplist[counter].ContentType = xmlresponse.ExtendedProperties.ContentType;
                                }
                                if (xmlresponse.ExtendedProperties.CreationDate != null)
                                {
                                    uplist[counter].CreationDate = xmlresponse.ExtendedProperties.CreationDate;
                                }
                                if (xmlresponse.ExtendedProperties.FromStoreService != null)
                                {
                                    uplist[counter].FromStoreService = xmlresponse.ExtendedProperties.FromStoreService;
                                }
                                if (xmlresponse.ExtendedProperties.IsAppxFramework != null)
                                {
                                    uplist[counter].IsAppxFramework = xmlresponse.ExtendedProperties.IsAppxFramework;
                                }
                                if (xmlresponse.ExtendedProperties.LegacyMobileProductId != null)
                                {
                                    uplist[counter].LegacyMobileProductId = xmlresponse.ExtendedProperties.LegacyMobileProductId;
                                }
                                if (xmlresponse.ExtendedProperties.LicensingPayloadId != null)
                                {
                                    uplist[counter].LicensingPayloadId = xmlresponse.ExtendedProperties.LicensingPayloadId;
                                }
                                if (xmlresponse.ExtendedProperties.LicensingSalt != null)
                                {
                                    uplist[counter].LicensingSalt = xmlresponse.ExtendedProperties.LicensingSalt;
                                }
                                if (xmlresponse.ExtendedProperties.MaxDownloadSize != null)
                                {
                                    uplist[counter].MaxDownloadSize = xmlresponse.ExtendedProperties.MaxDownloadSize;
                                }
                                if (xmlresponse.ExtendedProperties.MinDownloadSize != null)
                                {
                                    uplist[counter].MinDownloadSize = xmlresponse.ExtendedProperties.MinDownloadSize;
                                }
                                if (xmlresponse.ExtendedProperties.PackageContentId != null)
                                {
                                    uplist[counter].PackageContentId = xmlresponse.ExtendedProperties.PackageContentId;
                                }
                                if (xmlresponse.ExtendedProperties.PackageIdentityName != null)
                                {
                                    uplist[counter].PackageIdentityName = xmlresponse.ExtendedProperties.PackageIdentityName;
                                }
                                if (xmlresponse.ExtendedProperties.Ring != null)
                                {
                                    uplist[counter].Ring = xmlresponse.ExtendedProperties.Ring;
                                }
                            }
                        }
                    }
                }

                /*var updateinfos = response.Body.SyncUpdatesResponse.SyncUpdatesResult.NewUpdates.UpdateInfo;

                foreach (var updateinfo in updateinfos)
                {
                    int counter = -1;
                    foreach (Update up in uplist)
                    {
                        counter++;
                        if (up.ID == updateinfo.ID)
                        {
                            Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                            xmldoc.LoadXml("<Xml>" + updateinfo.Xml + "</Xml>");
                            var xmlresult = Deserialize(xmldoc, typeof(CUpdateInfoXml.Xml));
                            CUpdateInfoXml.Xml xmlresponse = ((CUpdateInfoXml.Xml)xmlresult);
                            if (xmlresponse.UpdateIdentity != null)
                            {
                                uplist[counter].UpdateID = xmlresponse.UpdateIdentity.UpdateID;
                                if (xmlresponse.UpdateIdentity.RevisionNumber != null)
                                {
                                    uplist[counter].RevisionNumber = xmlresponse.UpdateIdentity.RevisionNumber.ToString();
                                }
                                else
                                {
                                    uplist[counter].RevisionNumber = "1";
                                }
                            }
                            if (xmlresponse.ApplicabilityRules != null)
                            {
                                if (xmlresponse.ApplicabilityRules.Metadata != null)
                                {
                                    var metadatas = xmlresponse.ApplicabilityRules.Metadata.AppxPackageMetadata.AppxMetadata;
                                    if (metadatas != null)
                                    {
                                        uplist[counter].ApplicabilityBlob = metadatas.ApplicabilityBlob;
                                        uplist[counter].IsAppxBundle = metadatas.IsAppxBundle;
                                        uplist[counter].PackageMoniker = metadatas.PackageMoniker;
                                        uplist[counter].PackageType = metadatas.PackageType;
                                    }
                                }
                            }
                        }
                    }
                }*/

                var filelocations = response.Body.GetExtendedUpdateInfoResponse.GetExtendedUpdateInfoResult.FileLocations;
                if (filelocations != null)
                {
                    foreach (var filelocation in filelocations)
                    {
                        int counter = -1;
                        foreach (Update up in uplist)
                        {
                            counter++;
                            int othercounter = -1;
                            foreach (var file in up.UpdateFiles)
                            {
                                othercounter++;
                                if (file.Digest == filelocation.FileDigest)
                                {
                                    uplist[counter].UpdateFiles[othercounter].Url = filelocation.Url;
                                }
                            }
                        }
                    }
                }

                var decryptiondata = response.Body.GetExtendedUpdateInfoResponse.GetExtendedUpdateInfoResult.FileDecryptionData;
                if (decryptiondata != null)
                {
                    int counter = -1;
                    foreach (Update up in uplist)
                    {
                        counter++;
                        int othercounter = -1;
                        foreach (var file in up.UpdateFiles)
                        {
                            othercounter++;
                            if (file.Digest == decryptiondata.FileDecryption.FileDigest)
                            {
                                uplist[counter].UpdateFiles[othercounter].DecryptionKey = decryptiondata.FileDecryption.DecryptionKey;
                                uplist[counter].UpdateFiles[othercounter].SecurityData = decryptiondata.FileDecryption.SecurityData.base64Binary;
                            }
                        }
                    }
                }
            }



            int maincounter = -1;
            foreach (Update up in uplist)
            {
                maincounter++;
                Windows.Data.Xml.Dom.XmlDocument GetExtendedUpdateInfo2 = await SoapUtils.GetExtendedUpdateInfo2Async(token, up.UpdateID, up.RevisionNumber, "", "");
                if (GetExtendedUpdateInfo2.GetXml() != "")
                {
                    var res = Deserialize(GetExtendedUpdateInfo2, typeof(CGetExtendedUpdateInfo2Response.Envelope));
                    CGetExtendedUpdateInfo2Response.Envelope resp = ((CGetExtendedUpdateInfo2Response.Envelope)res);

                    var filelocations = resp.Body.GetExtendedUpdateInfo2Response.GetExtendedUpdateInfo2Result.FileLocations;

                    if (filelocations != null)
                    {
                        foreach (var filelocation in filelocations)
                        {
                            int counter = -1;
                            foreach (var file in up.UpdateFiles)
                            {
                                counter++;
                                if (file.Digest == filelocation.FileDigest)
                                {
                                    uplist[maincounter].UpdateFiles[counter].Url = filelocation.Url;
                                }
                            }
                        }
                    }

                    var decryptiondata = resp.Body.GetExtendedUpdateInfo2Response.GetExtendedUpdateInfo2Result.FileDecryptionData;

                    if (decryptiondata != null)
                    {
                        int counter = -1;
                        foreach (var file in up.UpdateFiles)
                        {
                            counter++;
                            if (file.Digest == decryptiondata.FileDecryption.FileDigest)
                            {
                                uplist[maincounter].UpdateFiles[counter].DecryptionKey = decryptiondata.FileDecryption.DecryptionKey;
                                uplist[maincounter].UpdateFiles[counter].SecurityData = decryptiondata.FileDecryption.SecurityData.base64Binary;
                            }
                        }
                    }
                }
            }
            return uplist;
        }

        public async Task<List<Update>> ScanUpdates(string token, string catids, string ContentType, string ring, string content)
        {
            List<Update> uplist = new List<Update>();
            string SkipIds = "";
            string Skipints = "";
            bool done = false;

            while (!done)
            {
                Windows.Data.Xml.Dom.XmlDocument SyncUpdates = await SoapUtils.SyncUpdatesAsync(token, catids, Skipints, SkipIds, ring, content);
                var result = Deserialize(SyncUpdates, typeof(CSyncUpdatesResponse.Envelope));
                CSyncUpdatesResponse.Envelope response = ((CSyncUpdatesResponse.Envelope)result);

                if (response.Body.SyncUpdatesResponse.SyncUpdatesResult.ExtendedUpdateInfo != null)
                {
                    var updates = response.Body.SyncUpdatesResponse.SyncUpdatesResult.ExtendedUpdateInfo.Updates.Update;

                    // Grab updates which are OSFlights
                    foreach (var update in updates)
                    {
                        SkipIds = SkipIds + "<Id>" + update.ID + "</Id>";
                        Skipints = Skipints + "<int>" + update.ID + "</int>";

                        Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                        xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                        var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                        CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                        if (xmlresponse.ExtendedProperties != null)
                        {
                            if (ContentType != "")
                            {
                                if (xmlresponse.ExtendedProperties.ContentType == ContentType)
                                {
                                    Update up = new Update();
                                    up.ID = update.ID;
                                    uplist.Add(up);
                                }
                            }
                            else
                            {
                                Update up = new Update();
                                up.ID = update.ID;
                                uplist.Add(up);
                            }
                        }
                    }

                    // Grab details about updates being OSFlights
                    foreach (var update in updates)
                    {
                        Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                        xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                        var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                        CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                        if (xmlresponse.LocalizedProperties != null)
                        {
                            int counter = -1;
                            foreach (Update up in uplist)
                            {
                                counter++;
                                if (up.ID == update.ID)
                                {
                                    uplist[counter].Title = xmlresponse.LocalizedProperties.Title;
                                    uplist[counter].Description = xmlresponse.LocalizedProperties.Description;
                                    break;
                                }
                            }
                        }
                    }

                    // Grab files about updates being OSFlights
                    foreach (var update in updates)
                    {
                        Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                        xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                        var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                        CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                        if (xmlresponse.Files != null)
                        {
                            foreach (var file in xmlresponse.Files)
                            {
                                //Debug.WriteLine(file.FileName);
                            }

                            int counter = -1;
                            foreach (Update up in uplist)
                            {
                                counter++;
                                if (up.ID == update.ID)
                                {
                                    List<UpdateFile> filelist = new List<UpdateFile>();
                                    foreach (var file in xmlresponse.Files)
                                    {
                                        UpdateFile upfile = new UpdateFile();
                                        upfile.FileName = file.FileName;
                                        upfile.Digest = file.Digest;
                                        upfile.DigestAlgorithm = file.DigestAlgorithm;

                                        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
                                        long bytes = long.Parse(file.Size.ToString());
                                        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                                        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                                        upfile.Size = (Math.Sign(bytes) * num).ToString() + suf[place] + " (" + file.Size + " Bytes)";

                                        upfile.Modified = file.Modified.ToUniversalTime().ToString();
                                        filelist.Add(upfile);
                                    }
                                    filelist.Sort((x, y) => y.FileName.CompareTo(x.FileName));
                                    uplist[counter].UpdateFiles = filelist;
                                    break;
                                }
                            }
                        }
                    }

                    // Grab details about app updates packages
                    foreach (var update in updates)
                    {
                        Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                        xmldoc.LoadXml("<Xml>" + update.Xml + "</Xml>");
                        var xmlresult = Deserialize(xmldoc, typeof(CUpdateXml.Xml));
                        CUpdateXml.Xml xmlresponse = ((CUpdateXml.Xml)xmlresult);
                        if (xmlresponse.ExtendedProperties != null)
                        {
                            int counter = -1;
                            foreach (Update up in uplist)
                            {
                                counter++;
                                if (up.ID == update.ID)
                                {
                                    if (xmlresponse.ExtendedProperties.BranchName != null)
                                    {
                                        uplist[counter].BranchName = xmlresponse.ExtendedProperties.BranchName;
                                    }
                                    if (xmlresponse.ExtendedProperties.BuildNumber != null)
                                    {
                                        uplist[counter].BuildNumber = xmlresponse.ExtendedProperties.BuildNumber;
                                    }
                                    if (xmlresponse.ExtendedProperties.ContentType != null)
                                    {
                                        uplist[counter].ContentType = xmlresponse.ExtendedProperties.ContentType;
                                    }
                                    if (xmlresponse.ExtendedProperties.CreationDate != null)
                                    {
                                        uplist[counter].CreationDate = xmlresponse.ExtendedProperties.CreationDate;
                                    }
                                    if (xmlresponse.ExtendedProperties.FromStoreService != null)
                                    {
                                        uplist[counter].FromStoreService = xmlresponse.ExtendedProperties.FromStoreService;
                                    }
                                    if (xmlresponse.ExtendedProperties.IsAppxFramework != null)
                                    {
                                        uplist[counter].IsAppxFramework = xmlresponse.ExtendedProperties.IsAppxFramework;
                                    }
                                    if (xmlresponse.ExtendedProperties.LegacyMobileProductId != null)
                                    {
                                        uplist[counter].LegacyMobileProductId = xmlresponse.ExtendedProperties.LegacyMobileProductId;
                                    }
                                    if (xmlresponse.ExtendedProperties.LicensingPayloadId != null)
                                    {
                                        uplist[counter].LicensingPayloadId = xmlresponse.ExtendedProperties.LicensingPayloadId;
                                    }
                                    if (xmlresponse.ExtendedProperties.LicensingSalt != null)
                                    {
                                        uplist[counter].LicensingSalt = xmlresponse.ExtendedProperties.LicensingSalt;
                                    }
                                    if (xmlresponse.ExtendedProperties.MaxDownloadSize != null)
                                    {
                                        uplist[counter].MaxDownloadSize = xmlresponse.ExtendedProperties.MaxDownloadSize;
                                    }
                                    if (xmlresponse.ExtendedProperties.MinDownloadSize != null)
                                    {
                                        uplist[counter].MinDownloadSize = xmlresponse.ExtendedProperties.MinDownloadSize;
                                    }
                                    if (xmlresponse.ExtendedProperties.PackageContentId != null)
                                    {
                                        uplist[counter].PackageContentId = xmlresponse.ExtendedProperties.PackageContentId;
                                    }
                                    if (xmlresponse.ExtendedProperties.PackageIdentityName != null)
                                    {
                                        uplist[counter].PackageIdentityName = xmlresponse.ExtendedProperties.PackageIdentityName;
                                        //Debug.WriteLine(xmlresponse.ExtendedProperties.PackageIdentityName);
                                    }
                                    if (xmlresponse.ExtendedProperties.Ring != null)
                                    {
                                        uplist[counter].Ring = xmlresponse.ExtendedProperties.Ring;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    var updateinfos = response.Body.SyncUpdatesResponse.SyncUpdatesResult.NewUpdates.UpdateInfo;

                    foreach (var updateinfo in updateinfos)
                    {
                        int counter = -1;
                        foreach (Update up in uplist)
                        {
                            counter++;
                            if (up.ID == updateinfo.ID)
                            {
                                Windows.Data.Xml.Dom.XmlDocument xmldoc = new Windows.Data.Xml.Dom.XmlDocument();
                                xmldoc.LoadXml("<Xml>" + updateinfo.Xml + "</Xml>");
                                var xmlresult = Deserialize(xmldoc, typeof(CUpdateInfoXml.Xml));
                                CUpdateInfoXml.Xml xmlresponse = ((CUpdateInfoXml.Xml)xmlresult);
                                
                                if (xmlresponse.UpdateIdentity != null)
                                {
                                    uplist[counter].UpdateID = xmlresponse.UpdateIdentity.UpdateID;
                                    if (xmlresponse.UpdateIdentity.RevisionNumber != null)
                                    {
                                        uplist[counter].RevisionNumber = xmlresponse.UpdateIdentity.RevisionNumber.ToString();
                                    }
                                    else
                                    {
                                        uplist[counter].RevisionNumber = "1";
                                    }
                                }
                                if (xmlresponse.ApplicabilityRules != null)
                                {
                                    if (xmlresponse.ApplicabilityRules.Metadata != null)
                                    {
                                        var metadatas = xmlresponse.ApplicabilityRules.Metadata.AppxPackageMetadata.AppxMetadata;
                                        if (metadatas != null)
                                        {
                                            uplist[counter].ApplicabilityBlob = metadatas.ApplicabilityBlob;
                                            uplist[counter].IsAppxBundle = metadatas.IsAppxBundle;
                                            uplist[counter].PackageMoniker = metadatas.PackageMoniker;
                                            uplist[counter].PackageType = metadatas.PackageType;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }


                }
                else
                {
                    done = true;
                }
            }

            int maincounter = -1;
            foreach (Update up in uplist)
            {
                maincounter++;
                Windows.Data.Xml.Dom.XmlDocument GetExtendedUpdateInfo2 = await SoapUtils.GetExtendedUpdateInfo2Async(token, up.UpdateID, up.RevisionNumber, ring, content);
                if (GetExtendedUpdateInfo2.GetXml() != "")
                {
                    var result = Deserialize(GetExtendedUpdateInfo2, typeof(CGetExtendedUpdateInfo2Response.Envelope));
                    CGetExtendedUpdateInfo2Response.Envelope response = ((CGetExtendedUpdateInfo2Response.Envelope)result);

                    var filelocations = response.Body.GetExtendedUpdateInfo2Response.GetExtendedUpdateInfo2Result.FileLocations;

                    if (filelocations != null)
                    {
                        foreach (var filelocation in filelocations)
                        {
                            int counter = -1;
                            foreach (var file in up.UpdateFiles)
                            {
                                counter++;
                                if (file.Digest == filelocation.FileDigest)
                                {
                                    uplist[maincounter].UpdateFiles[counter].Url = filelocation.Url;
                                    break;
                                }
                            }
                        }
                    }

                    var decryptiondata = response.Body.GetExtendedUpdateInfo2Response.GetExtendedUpdateInfo2Result.FileDecryptionData;

                    if (decryptiondata != null)
                    {
                        int counter = -1;
                        foreach (var file in up.UpdateFiles)
                        {
                            counter++;
                            if (file.Digest == decryptiondata.FileDecryption.FileDigest)
                            {
                                uplist[maincounter].UpdateFiles[counter].DecryptionKey = decryptiondata.FileDecryption.DecryptionKey;
                                uplist[maincounter].UpdateFiles[counter].SecurityData = decryptiondata.FileDecryption.SecurityData.base64Binary;
                            }
                        }
                    }
                }
            }
            return uplist;
        }

        public async Task<CGetCookieResponse.GetCookieResult> GetCookieAsync()
        {

            Windows.Data.Xml.Dom.XmlDocument SyncUpdates = await SoapUtils.GetCookieAsync();
            var result = Deserialize(SyncUpdates, typeof(CGetCookieResponse.Envelope));
            CGetCookieResponse.Envelope response = ((CGetCookieResponse.Envelope)result);

            return response.Body.GetCookieResponse.GetCookieResult;

        }

        /// <summary>
        /// Deserializes an xml document back into an object
        /// </summary>
        /// <param name="xml">The xml data to deserialize</param>
        /// <param name="type">The type of the object being deserialized</param>
        /// <returns>A deserialized object</returns>
        public static object Deserialize(Windows.Data.Xml.Dom.XmlDocument xml, Type type)
        {
            XmlSerializer s = new XmlSerializer(type);
            string xmlString = xml.GetXml();
            byte[] buffer = ASCIIEncoding.UTF8.GetBytes(xmlString);
            MemoryStream ms = new MemoryStream(buffer);
            XmlReader reader = XmlReader.Create(ms);
            Exception caught = null;

            try
            {
                object o = s.Deserialize(reader);
                return o;
            }

            catch (Exception e)
            {
                caught = e;
            }
            finally
            {
                reader.Dispose();

                if (caught != null)
                    throw caught;
            }
            return null;
        }
    }
}
