using System.Collections.Generic;
using System.Xml.Serialization;

namespace PhotoshoppedUUPCLI.UUP.FE3.Requests
{
    public class SyncUpdatesRequestClasses
    {
        [XmlRoot(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
        public class Action
        {
            [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public string MustUnderstand { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "To", Namespace = "http://www.w3.org/2005/08/addressing")]
        public class To
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
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "TicketType")]
        public class TicketType
        {
            [XmlElement(ElementName = "Device")]
            public string Device { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "Version")]
            public string Version { get; set; }
            [XmlAttribute(AttributeName = "Policy")]
            public string Policy { get; set; }
        }

        [XmlRoot(ElementName = "WindowsUpdateTicketsToken", Namespace = "http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization")]
        public class WindowsUpdateTicketsToken
        {
            [XmlElement(ElementName = "TicketType")]
            public TicketType TicketType { get; set; }
            [XmlAttribute(AttributeName = "id", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
            public string Id { get; set; }
            [XmlAttribute(AttributeName = "wsu", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Wsu { get; set; }
            [XmlAttribute(AttributeName = "wuws", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Wuws { get; set; }
        }

        [XmlRoot(ElementName = "Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
        public class Security
        {
            [XmlElement(ElementName = "Timestamp", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
            public Timestamp Timestamp { get; set; }
            [XmlElement(ElementName = "WindowsUpdateTicketsToken", Namespace = "http://schemas.microsoft.com/msus/2014/10/WindowsUpdateAuthorization")]
            public WindowsUpdateTicketsToken WindowsUpdateTicketsToken { get; set; }
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
            [XmlElement(ElementName = "MessageID", Namespace = "http://www.w3.org/2005/08/addressing")]
            public string MessageID { get; set; }
            [XmlElement(ElementName = "To", Namespace = "http://www.w3.org/2005/08/addressing")]
            public To To { get; set; }
            [XmlElement(ElementName = "Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
            public Security Security { get; set; }
        }

        [XmlRoot(ElementName = "cookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class Cookie
        {
            [XmlElement(ElementName = "Expiration", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Expiration { get; set; }
            [XmlElement(ElementName = "EncryptedData", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string EncryptedData { get; set; }
        }

        [XmlRoot(ElementName = "XmlUpdateFragmentTypes", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class XmlUpdateFragmentTypes
        {
            [XmlElement(ElementName = "XmlUpdateFragmentType", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public List<string> XmlUpdateFragmentType { get; set; }
        }

        [XmlRoot(ElementName = "Locales", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class Locales
        {
            [XmlElement(ElementName = "string", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public List<string> String { get; set; }
        }

        [XmlRoot(ElementName = "ExtendedUpdateInfoParameters", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class ExtendedUpdateInfoParameters
        {
            [XmlElement(ElementName = "XmlUpdateFragmentTypes", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public XmlUpdateFragmentTypes XmlUpdateFragmentTypes { get; set; }
            [XmlElement(ElementName = "Locales", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public Locales Locales { get; set; }
        }

        [XmlRoot(ElementName = "ClientPreferredLanguages", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class ClientPreferredLanguages
        {
            [XmlElement(ElementName = "string", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string String { get; set; }
        }

        [XmlRoot(ElementName = "ProductsParameters", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class ProductsParameters
        {
            [XmlElement(ElementName = "SyncCurrentVersionOnly", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string SyncCurrentVersionOnly { get; set; }
            [XmlElement(ElementName = "DeviceAttributes", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string DeviceAttributes { get; set; }
            [XmlElement(ElementName = "CallerAttributes", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string CallerAttributes { get; set; }
            [XmlElement(ElementName = "Products", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Products { get; set; }
        }

        [XmlRoot(ElementName = "parameters", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class Parameters
        {
            [XmlElement(ElementName = "ExpressQuery", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string ExpressQuery { get; set; }
            [XmlElement(ElementName = "InstalledNonLeafUpdateIDs", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string InstalledNonLeafUpdateIDs { get; set; }
            [XmlElement(ElementName = "OtherCachedUpdateIDs", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string OtherCachedUpdateIDs { get; set; }
            [XmlElement(ElementName = "SkipSoftwareSync", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string SkipSoftwareSync { get; set; }
            [XmlElement(ElementName = "NeedTwoGroupOutOfScopeUpdates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string NeedTwoGroupOutOfScopeUpdates { get; set; }
            [XmlElement(ElementName = "AlsoPerformRegularSync", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string AlsoPerformRegularSync { get; set; }
            [XmlElement(ElementName = "ComputerSpec", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string ComputerSpec { get; set; }
            [XmlElement(ElementName = "ExtendedUpdateInfoParameters", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public ExtendedUpdateInfoParameters ExtendedUpdateInfoParameters { get; set; }
            [XmlElement(ElementName = "ClientPreferredLanguages", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public ClientPreferredLanguages ClientPreferredLanguages { get; set; }
            [XmlElement(ElementName = "ProductsParameters", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public ProductsParameters ProductsParameters { get; set; }
        }

        [XmlRoot(ElementName = "SyncUpdates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class SyncUpdates
        {
            [XmlElement(ElementName = "cookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public Cookie Cookie { get; set; }
            [XmlElement(ElementName = "parameters", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public Parameters Parameters { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class Body
        {
            [XmlElement(ElementName = "SyncUpdates", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public SyncUpdates SyncUpdates { get; set; }
        }

        [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class Envelope
        {
            [XmlElement(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public Header Header { get; set; }
            [XmlElement(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public Body Body { get; set; }
            [XmlAttribute(AttributeName = "a", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string A { get; set; }
            [XmlAttribute(AttributeName = "s", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string S { get; set; }
        }
    }
}
