using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace StoreProjectApp.WU.FE3
{
    public class GetCookieResponseClasses
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

        [XmlRoot(ElementName = "GetCookieResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class GetCookieResult
        {
            [XmlElement(ElementName = "Expiration", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string Expiration { get; set; }
            [XmlElement(ElementName = "EncryptedData", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public string EncryptedData { get; set; }
        }

        [XmlRoot(ElementName = "GetCookieResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public class GetCookieResponse
        {
            [XmlElement(ElementName = "GetCookieResult", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public GetCookieResult GetCookieResult { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public class Body
        {
            [XmlElement(ElementName = "GetCookieResponse", Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public GetCookieResponse GetCookieResponse { get; set; }
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
