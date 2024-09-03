using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using GetAppxPackages.WU.FE2;
using Windows.Data.Xml.Dom;

namespace GetAppxPackages.WU
{
    public class Parser_FE2
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
            public uint MaxDownloadSize { get; internal set; }
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


        public async Task<string> GetCookie()
        {
            Windows.Data.Xml.Dom.XmlDocument GetCookie = await SoapUtils_FE2.GetCookie();

            var result = Deserialize(GetCookie, typeof(CGetCookieResult.Envelope));
            CGetCookieResult.Envelope response = ((CGetCookieResult.Envelope)result);

            var cookie = String.Format("<Expiration>{0}</Expiration><EncryptedData>{1}</EncryptedData>", response.Body.GetCookieResponse.GetCookieResult.Expiration.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"), response.Body.GetCookieResponse.GetCookieResult.EncryptedData);

            return cookie;
        }


        public async Task<List<Update>> ScanUpdate(string id)
        {
            List<Update> uplist = new List<Update>();

            return uplist;
        }

        public async Task<List<Update>> ScanUpdates(string catids, string ContentType)
        {
            List<Update> uplist = new List<Update>();

            string Skipints = "";
            bool done = false;

            while (!done)
            {
                Windows.Data.Xml.Dom.XmlDocument SyncUpdates = await SoapUtils_FE2.SyncUpdatesAsync(await GetCookie(), "", "", Skipints);

                var result = Deserialize(SyncUpdates, typeof(CSyncUpdatesResult.Envelope));
                CSyncUpdatesResult.Envelope response = ((CSyncUpdatesResult.Envelope)result);

                //Console.WriteLine(SyncUpdates.InnerXml);

                var newupdates = response.Body.SyncUpdatesResponse.SyncUpdatesResult.NewUpdates;

                if (newupdates != null)
                {
                    foreach (var update in newupdates)
                    {
                        Skipints = Skipints + "<int>" + update.ID + "</int>";
                        try
                        {
                            Windows.Data.Xml.Dom.XmlDocument ExtendedUpdateInfo = await SoapUtils_FE2.GetExtendedUpdateInfoAsync(await GetCookie(), update.ID);
                            //Console.WriteLine(ExtendedUpdateInfo.InnerXml);
                            //Console.WriteLine();
                        } catch { }
                    }
                } else
                {
                    done = true;
                }
            }

            return uplist;
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
