using Flurl.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.Storage;
//using System.Diagnostics;

namespace GetAppxPackages.WU
{
    class SoapUtils
    {

        public static async Task<Windows.Data.Xml.Dom.XmlDocument> SyncUpdatesAsync(string token, string CategoryIdentifierID, string skipintlist, string skipidlist, string ring, string contentt)
        {
            var _url = "https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/SyncUpdates";

            if (token != "")
            {
                token = String.Format("<TicketType Name=\"MSA\" Version=\"1.0\" Policy=\"MBI_SSL\"><User>{0}</User></TicketType>", token);
            }

            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE3//SyncUpdates.xml", token, skipintlist, skipidlist, CategoryIdentifierID, ring, contentt);

            var postop = _url.WithHeader("MS-CV", "0").WithHeader("SOAPAction", _action).WithHeader("User-agent", "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/1.40").WithHeader("Method", "POST");

            var content = new StringContent(soapEnvelopeXml.GetXml(), System.Text.Encoding.UTF8, "application/soap+xml");

            var postopresult = await postop.SendAsync(HttpMethod.Post, content);

            //Debug.WriteLine(await postopresult.Content.ReadAsStringAsync());

            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();

            result.LoadXml(await postopresult.Content.ReadAsStringAsync());
            return result;
        }

        private static async Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString, string InsertString2, string InsertString3, string ring, string content)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var cookie = await new Parser().GetCookieAsync();

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
            result = String.Format(xmlcontent, cookie.Expiration, cookie.EncryptedData, token, InsertString, InsertString2, InsertString3, ring, content);
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }

        private static async Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString, string InsertString2, string ring, string content)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var cookie = await new Parser().GetCookieAsync();

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
            result = String.Format(xmlcontent, cookie.Expiration, cookie.EncryptedData, token, InsertString, InsertString2, ring, content);
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }

        public static async Task<Windows.Data.Xml.Dom.XmlDocument> GetCookieAsync()
        {
            var _url = "https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetCookie";
            
            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE3//GetCookie.xml");

            var postop = _url.WithHeader("MS-CV", "0").WithHeader("SOAPAction", _action).WithHeader("User-agent", "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/1.40").WithHeader("Method", "POST");

            var content = new StringContent(soapEnvelopeXml.GetXml(), System.Text.Encoding.UTF8, "application/soap+xml");

            var postopresult = await postop.SendAsync(HttpMethod.Post, content);

            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();

            result.LoadXml(await postopresult.Content.ReadAsStringAsync());
            return result;
        }

        public static async Task<Windows.Data.Xml.Dom.XmlDocument> GetExtendedUpdateInfoAsync(string token, long revisionID)
        {
            var _url = "https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetExtendedUpateInfo";


            if (token != "")
            {
                token = String.Format("<TicketType Name=\"MSA\" Version=\"1.0\" Policy=\"MBI_SSL\"><User>{0}</User></TicketType>", token);
            }

            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE3//GetExtendedUpdateInfo.xml", token, revisionID.ToString());
            var postop = _url.WithHeader("MS-CV", "0").WithHeader("SOAPAction", _action).WithHeader("User-agent", "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/1.40").WithHeader("Method", "POST");

            var content = new StringContent(soapEnvelopeXml.GetXml(), System.Text.Encoding.UTF8, "application/soap+xml");

            var postopresult = await postop.SendAsync(HttpMethod.Post, content);

            //Debug.WriteLine(await postopresult.Content.ReadAsStringAsync());

            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();

            result.LoadXml(await postopresult.Content.ReadAsStringAsync());
            return result;
        }

        public static async Task<Windows.Data.Xml.Dom.XmlDocument> GetExtendedUpdateInfo2Async(string token, string UpdateID, string RevisionID, string ring, string contentt)
        {
            var _url = "https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx/secured";
            var _action = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetExtendedUpdateInfo2";


            if (token != "")
            {
                token = String.Format("<TicketType Name=\"MSA\" Version=\"1.0\" Policy=\"MBI_SSL\"><User>{0}</User></TicketType>", token);
            }

            Windows.Data.Xml.Dom.XmlDocument soapEnvelopeXml = await CreateSoapEnvelope("ShellPages//Private//GoFly//WU//FE3//GetExtendedUpdateInfo2.xml", token, UpdateID, RevisionID, ring, contentt);
            var postop = _url.WithHeader("MS-CV", "0").WithHeader("SOAPAction", _action).WithHeader("User-agent", "Windows-Update-Agent/10.0.10011.16384 Client-Protocol/1.40").WithHeader("Method", "POST");

            var content = new StringContent(soapEnvelopeXml.GetXml(), System.Text.Encoding.UTF8, "application/soap+xml");

            var postopresult = await postop.SendAsync(HttpMethod.Post, content);

            Windows.Data.Xml.Dom.XmlDocument result = new Windows.Data.Xml.Dom.XmlDocument();

            result.LoadXml(await postopresult.Content.ReadAsStringAsync());
            return result;
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

        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token)
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
            result = String.Format(xmlcontent, token);

            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }


        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var cookie = await new Parser().GetCookieAsync();

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
            result = String.Format(xmlcontent, cookie.Expiration, cookie.EncryptedData, token, InsertString);

            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }

        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString, string InsertString2)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var cookie = await new Parser().GetCookieAsync();

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
            result = String.Format(xmlcontent, cookie.Expiration, cookie.EncryptedData, token, InsertString, InsertString2);
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }

        private async static Task<Windows.Data.Xml.Dom.XmlDocument> CreateSoapEnvelope(string xmlpath, string token, string InsertString, string InsertString2, string InsertString3)
        {
            Windows.Data.Xml.Dom.XmlDocument soapEnvelop = new Windows.Data.Xml.Dom.XmlDocument();
            String result;

            var cookie = await new Parser().GetCookieAsync();

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
            result = String.Format(xmlcontent, cookie.Expiration, cookie.EncryptedData, token, InsertString, InsertString2, InsertString3);
            soapEnvelop.LoadXml(result);
            return soapEnvelop;
        }
    }
}
