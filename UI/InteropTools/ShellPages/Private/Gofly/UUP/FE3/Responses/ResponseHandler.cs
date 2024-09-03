using GetAppxPackages.WU.FE3;
using StoreProjectApp.WU.FE3;
using System.IO;
using System.Xml.Serialization;

namespace PhotoshoppedUUPCLI.UUP.FE3.Responses
{
    public class ResponseHandler
    {
        public static GetCookieResponseClasses.GetCookieResponse GetCookieResponse(string response)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GetCookieResponseClasses.Envelope));

            using (StringReader textReader = new StringReader(response))
            {
                return ((GetCookieResponseClasses.Envelope)serializer.Deserialize(textReader)).Body.GetCookieResponse;
            }
        }

        public static SyncUpdatesResponseClasses.SyncUpdatesResponse GetSyncUpdatesResponse(string response)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SyncUpdatesResponseClasses.Envelope));

            using (StringReader textReader = new StringReader(response))
            {
                return ((SyncUpdatesResponseClasses.Envelope)serializer.Deserialize(textReader)).Body.SyncUpdatesResponse;
            }
        }

        public static GetExtendedUpdateInfo2ResponseClasses.GetExtendedUpdateInfo2Response GetExtendedUpdateInfo2Response(string response)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GetExtendedUpdateInfo2ResponseClasses.Envelope));

            using (StringReader textReader = new StringReader(response))
            {
                return ((GetExtendedUpdateInfo2ResponseClasses.Envelope)serializer.Deserialize(textReader)).Body.GetExtendedUpdateInfo2Response;
            }
        }
    }
}
