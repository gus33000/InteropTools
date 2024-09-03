using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAppxPackages.WU.FE2
{
    public class CGetCookieResult
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
        public partial class Envelope
        {

            private EnvelopeBody bodyField;

            /// <remarks/>
            public EnvelopeBody Body
            {
                get
                {
                    return this.bodyField;
                }
                set
                {
                    this.bodyField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeBody
        {

            private GetCookieResponse getCookieResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public GetCookieResponse GetCookieResponse
            {
                get
                {
                    return this.getCookieResponseField;
                }
                set
                {
                    this.getCookieResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService", IsNullable = false)]
        public partial class GetCookieResponse
        {

            private GetCookieResponseGetCookieResult getCookieResultField;

            /// <remarks/>
            public GetCookieResponseGetCookieResult GetCookieResult
            {
                get
                {
                    return this.getCookieResultField;
                }
                set
                {
                    this.getCookieResultField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class GetCookieResponseGetCookieResult
        {

            private System.DateTime expirationField;

            private string encryptedDataField;

            /// <remarks/>
            public System.DateTime Expiration
            {
                get
                {
                    return this.expirationField;
                }
                set
                {
                    this.expirationField = value;
                }
            }

            /// <remarks/>
            public string EncryptedData
            {
                get
                {
                    return this.encryptedDataField;
                }
                set
                {
                    this.encryptedDataField = value;
                }
            }
        }


    }
}
