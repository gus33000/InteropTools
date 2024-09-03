using System.Collections.Generic;
using System.Xml.Serialization;

namespace PhotoshoppedUUPCLI.UUP.FE3.Responses
{
    public class SyncUpdatesResponseClasses
    {
        [XmlRoot(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
        public class Action
        {
            [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public string MustUnderstand { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Timestamp", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public class Timestamp
        {
            [XmlElement(ElementName = "Created", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
            public string Created { get; set; }
            [XmlElement(ElementName = "Expires", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
            public string Expires { get; set; }
            [XmlAttribute(AttributeName = "Id", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
            public string Id { get; set; }
        }

        [XmlRoot(ElementName = "Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
        public class Security
        {
            [XmlElement(ElementName = "Timestamp", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
            public Timestamp Timestamp { get; set; }
            [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public string MustUnderstand { get; set; }
            [XmlAttribute(AttributeName = "o", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string O { get; set; }
        }

        [XmlRoot(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class Header
        {
            [XmlElement(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
            public Action Action { get; set; }
            [XmlElement(ElementName = "RelatesTo", Namespace = "http://www.w3.org/2005/08/addressing")]
            public string RelatesTo { get; set; }
            [XmlElement(ElementName = "Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
            public Security Security { get; set; }
        }

        [XmlRoot(ElementName = "Deployment", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class Deployment
        {
            [XmlElement(ElementName = "ID", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string ID { get; set; }
            [XmlElement(ElementName = "Action", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Action2 { get; set; }
            [XmlElement(ElementName = "IsAssigned", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string IsAssigned { get; set; }
            [XmlElement(ElementName = "LastChangeTime", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string LastChangeTime { get; set; }
            [XmlElement(ElementName = "AutoSelect", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string AutoSelect { get; set; }
            [XmlElement(ElementName = "AutoDownload", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string AutoDownload { get; set; }
            [XmlElement(ElementName = "SupersedenceBehavior", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string SupersedenceBehavior { get; set; }
            [XmlElement(ElementName = "Priority", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Priority { get; set; }
            [XmlElement(ElementName = "HandlerSpecificAction", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string HandlerSpecificAction { get; set; }
            [XmlElement(ElementName = "FlightId", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string FlightId { get; set; }
            [XmlElement(ElementName = "FlightMetadata", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string FlightMetadata { get; set; }
        }

        [XmlRoot(ElementName = "Verification", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class Verification
        {
            [XmlAttribute(AttributeName = "Timestamp")]
            public string Timestamp { get; set; }
            [XmlAttribute(AttributeName = "LeafCertificateId")]
            public string LeafCertificateId { get; set; }
            [XmlAttribute(AttributeName = "Signature")]
            public string Signature { get; set; }
            [XmlAttribute(AttributeName = "Algorithm")]
            public string Algorithm { get; set; }
        }

        [XmlRoot(ElementName = "UpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class UpdateInfo
        {
            [XmlElement(ElementName = "ID", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string ID { get; set; }
            [XmlElement(ElementName = "Deployment", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public Deployment Deployment { get; set; }
            [XmlElement(ElementName = "IsLeaf", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string IsLeaf { get; set; }
            [XmlElement(ElementName = "IsShared", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string IsShared { get; set; }
            [XmlElement(ElementName = "Xml", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Xml { get; set; }
            [XmlElement(ElementName = "Verification", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public Verification Verification { get; set; }
        }

        [XmlRoot(ElementName = "NewUpdates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class NewUpdates
        {
            [XmlElement(ElementName = "UpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public List<UpdateInfo> UpdateInfo { get; set; }
        }

        [XmlRoot(ElementName = "NewCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class NewCookie
        {
            [XmlElement(ElementName = "Expiration", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Expiration { get; set; }
            [XmlElement(ElementName = "EncryptedData", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string EncryptedData { get; set; }
        }

        [XmlRoot(ElementName = "Update", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class Update
        {
            [XmlElement(ElementName = "ID", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string ID { get; set; }
            [XmlElement(ElementName = "Xml", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Xml { get; set; }
            [XmlElement(ElementName = "Verification", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public Verification Verification { get; set; }
        }

        [XmlRoot(ElementName = "Updates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class Updates
        {
            [XmlElement(ElementName = "Update", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public List<Update> Update { get; set; }
        }

        [XmlRoot(ElementName = "ExtendedUpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class ExtendedUpdateInfo
        {
            [XmlElement(ElementName = "Updates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public Updates Updates { get; set; }
        }

        [XmlRoot(ElementName = "SyncUpdatesResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class SyncUpdatesResult
        {
            [XmlElement(ElementName = "NewUpdates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public NewUpdates NewUpdates { get; set; }
            [XmlElement(ElementName = "Truncated", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Truncated { get; set; }
            [XmlElement(ElementName = "NewCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public NewCookie NewCookie { get; set; }
            [XmlElement(ElementName = "DriverSyncNotNeeded", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string DriverSyncNotNeeded { get; set; }
            [XmlElement(ElementName = "ExtendedUpdateInfo", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public ExtendedUpdateInfo ExtendedUpdateInfo { get; set; }
        }

        [XmlRoot(ElementName = "SyncUpdatesResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class SyncUpdatesResponse
        {
            [XmlElement(ElementName = "SyncUpdatesResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public SyncUpdatesResult SyncUpdatesResult { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class Body
        {
            [XmlElement(ElementName = "SyncUpdatesResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public SyncUpdatesResponse SyncUpdatesResponse { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsd { get; set; }
        }

        [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class Envelope
        {
            [XmlElement(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public Header Header { get; set; }
            [XmlElement(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public Body Body { get; set; }
            [XmlAttribute(AttributeName = "s", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string S { get; set; }
            [XmlAttribute(AttributeName = "a", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string A { get; set; }
            [XmlAttribute(AttributeName = "u", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string U { get; set; }
        }
    }
}
