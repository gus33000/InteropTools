using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace GetAppxPackages.WU
{
    class SoapUtils_FE2
    {
        public static async Task<Windows.Data.Xml.Dom.XmlDocument> GetCookie()
        {
            var _url = "https://fe2.update.microsoft.com/v6/ClientWebService/client.asmx";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetCookie";

            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE2//GetCookie.xml");
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            string soapResult;
            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                WebResponse webResponse = await webRequest.GetResponseAsync();
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }

                result.LoadXml(soapResult);
            }
            catch (Exception e)
            {
                //Console.WriteLine("[ERROR] " + e.Message);
            }
            return result;
        }

        public static async Task<Windows.Data.Xml.Dom.XmlDocument> SyncUpdatesAsync(string token, string CategoryIdentifierID, string skipintlist, string skipidlist)
        {
            var _url = "https://fe2.update.microsoft.com/v6/ClientWebService/client.asmx";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/SyncUpdates";

            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE2//SyncUpdates.xml", token, CategoryIdentifierID, skipintlist, skipidlist);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            string soapResult;
            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                WebResponse webResponse = await webRequest.GetResponseAsync();
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }

                result.LoadXml(soapResult);
            }
            catch (Exception e)
            {
                //Console.WriteLine("[ERROR] " + e.Message);
            }
            return result;
        }

        public static async Task<Windows.Data.Xml.Dom.XmlDocument> GetExtendedUpdateInfoAsync(string token, long revisionID)
        {
            var _url = "https://fe2.update.microsoft.com/v6/ClientWebService/client.asmx";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetExtendedUpdateInfo";

            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE2//GetExtendedUpdateInfo.xml", token, revisionID.ToString());
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            string soapResult;
            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                WebResponse webResponse = await webRequest.GetResponseAsync();
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }

                result.LoadXml(soapResult);
            }
            catch (Exception e)
            {
                //Console.WriteLine("[ERROR] " + e.Message);
                return null;
            }
            return result;
        }

        public static async Task<Windows.Data.Xml.Dom.XmlDocument> GetExtendedUpdateInfo2Async(string token, string UpdateID, string RevisionID)
        {
            var _url = "https://fe2.update.microsoft.com/v6/ClientWebService/client.asmx";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetExtendedUpdateInfo2";

            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE2//GetExtendedUpdateInfo2.xml", token, UpdateID, RevisionID);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            string soapResult;
            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                WebResponse webResponse = await webRequest.GetResponseAsync();
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }

                result.LoadXml(soapResult);
            }
            catch (Exception e)
            {
                //Console.WriteLine("[ERROR] " + e.Message);
            }
            return result;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Headers["SOAPAction"] = action;
            webRequest.Headers["MS-CV"] = "0";
            webRequest.Headers["User-Agent"] = "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/1.40";
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Accept = "xpress";
            webRequest.Method = "POST";
            return webRequest;
        }

        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var xmlfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + xmlpath, UriKind.Absolute));

            List<string> lines = new List<string>();
            using (var inputStream = await xmlfile.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                while (streamReader.Peek() >= 0)
                {
                    lines.Add(streamReader.ReadLine());
                }
            }

            string xmlcontent = string.Join("\n", lines);
            result = String.Format(xmlcontent);
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }


        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var xmlfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + xmlpath, UriKind.Absolute));

            List<string> lines = new List<string>();
            using (var inputStream = await xmlfile.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                while (streamReader.Peek() >= 0)
                {
                    lines.Add(streamReader.ReadLine());
                }
            }

            string xmlcontent = string.Join("\n", lines);
            result = String.Format(xmlcontent, token, InsertString);
            
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }

        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString, string InsertString2)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var xmlfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + xmlpath, UriKind.Absolute));

            List<string> lines = new List<string>();
            using (var inputStream = await xmlfile.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                while (streamReader.Peek() >= 0)
                {
                    lines.Add(streamReader.ReadLine());
                }
            }

            string xmlcontent = string.Join("\n", lines);
            result = String.Format(xmlcontent, token, InsertString, InsertString2);
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }

        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString, string InsertString2, string InsertString3)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var xmlfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + xmlpath, UriKind.Absolute));

            List<string> lines = new List<string>();
            using (var inputStream = await xmlfile.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                while (streamReader.Peek() >= 0)
                {
                    lines.Add(streamReader.ReadLine());
                }
            }

            string xmlcontent = string.Join("\n", lines);
            result = String.Format(xmlcontent, token, InsertString, InsertString2, InsertString3);
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }

        private async static void InsertSoapEnvelopeIntoWebRequest(Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = await webRequest.GetRequestStreamAsync())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(soapEnvelopeXml.GetXml());
                writer.Flush();
                stream.Position = 0;
            }
        }
    }
}
