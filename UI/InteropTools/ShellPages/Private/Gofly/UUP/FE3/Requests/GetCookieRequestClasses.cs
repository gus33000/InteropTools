using System.Xml.Serialization;

namespace PhotoshoppedUUPCLI.UUP.FE3.Requests
{
    public class GetCookieRequestClasses
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
            [XmlElement(ElementName = "User")]
            public string User { get; set; }
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
            [XmlAttribute(AttributeName = "wuws", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Wuws { get; set; }
            [XmlAttribute(AttributeName = "wsu", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Wsu { get; set; }
            [XmlAttribute(AttributeName = "id", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
            public string Id { get; set; }
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

        [XmlRoot(ElementName = "oldCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class OldCookie
        {
            [XmlElement(ElementName = "Expiration", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Expiration { get; set; }
        }

        [XmlRoot(ElementName = "GetCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class GetCookie
        {
            [XmlElement(ElementName = "oldCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public OldCookie OldCookie { get; set; }
            [XmlElement(ElementName = "lastChange", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string LastChange { get; set; }
            [XmlElement(ElementName = "currentTime", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string CurrentTime { get; set; }
            [XmlElement(ElementName = "protocolVersion", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string ProtocolVersion { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class Body
        {
            [XmlElement(ElementName = "GetCookie", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public GetCookie GetCookie { get; set; }
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
