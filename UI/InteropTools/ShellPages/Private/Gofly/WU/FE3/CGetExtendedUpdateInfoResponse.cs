using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAppxPackages.WU.FE3
{
    public class CGetExtendedUpdateInfoResponse
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false)]
        public partial class Envelope
        {

            private EnvelopeHeader headerField;

            private EnvelopeBody bodyField;

            /// <remarks/>
            public EnvelopeHeader Header
            {
                get
                {
                    return this.headerField;
                }
                set
                {
                    this.headerField = value;
                }
            }

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
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public partial class EnvelopeHeader
        {

            private Action actionField;

            private string relatesToField;

            private Security securityField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
            public Action Action
            {
                get
                {
                    return this.actionField;
                }
                set
                {
                    this.actionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
            public string RelatesTo
            {
                get
                {
                    return this.relatesToField;
                }
                set
                {
                    this.relatesToField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" +
                "")]
            public Security Security
            {
                get
                {
                    return this.securityField;
                }
                set
                {
                    this.securityField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/08/addressing")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2005/08/addressing", IsNullable = false)]
        public partial class Action
        {

            private byte mustUnderstandField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public byte mustUnderstand
            {
                get
                {
                    return this.mustUnderstandField;
                }
                set
                {
                    this.mustUnderstandField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" +
            "")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" +
            "", IsNullable = false)]
        public partial class Security
        {

            private Timestamp timestampField;

            private byte mustUnderstandField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xs" +
                "d")]
            public Timestamp Timestamp
            {
                get
                {
                    return this.timestampField;
                }
                set
                {
                    this.timestampField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public byte mustUnderstand
            {
                get
                {
                    return this.mustUnderstandField;
                }
                set
                {
                    this.mustUnderstandField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xs" +
            "d")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xs" +
            "d", IsNullable = false)]
        public partial class Timestamp
        {

            private System.DateTime createdField;

            private System.DateTime expiresField;

            private string idField;

            /// <remarks/>
            public System.DateTime Created
            {
                get
                {
                    return this.createdField;
                }
                set
                {
                    this.createdField = value;
                }
            }

            /// <remarks/>
            public System.DateTime Expires
            {
                get
                {
                    return this.expiresField;
                }
                set
                {
                    this.expiresField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
            public string Id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public partial class EnvelopeBody
        {

            private GetExtendedUpdateInfoResponse getExtendedUpdateInfoResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public GetExtendedUpdateInfoResponse GetExtendedUpdateInfoResponse
            {
                get
                {
                    return this.getExtendedUpdateInfoResponseField;
                }
                set
                {
                    this.getExtendedUpdateInfoResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService", IsNullable = false)]
        public partial class GetExtendedUpdateInfoResponse
        {

            private GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResult getExtendedUpdateInfoResultField;

            /// <remarks/>
            public GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResult GetExtendedUpdateInfoResult
            {
                get
                {
                    return this.getExtendedUpdateInfoResultField;
                }
                set
                {
                    this.getExtendedUpdateInfoResultField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResult
        {

            private GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultUpdate[] updatesField;

            private GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileLocation[] fileLocationsField;

            private GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionData fileDecryptionDataField;

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("Update", IsNullable = false)]
            public GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultUpdate[] Updates
            {
                get
                {
                    return this.updatesField;
                }
                set
                {
                    this.updatesField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("FileLocation", IsNullable = false)]
            public GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileLocation[] FileLocations
            {
                get
                {
                    return this.fileLocationsField;
                }
                set
                {
                    this.fileLocationsField = value;
                }
            }

            /// <remarks/>
            public GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionData FileDecryptionData
            {
                get
                {
                    return this.fileDecryptionDataField;
                }
                set
                {
                    this.fileDecryptionDataField = value;
                }
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionData
        {

            private GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionDataFileDecryption fileDecryptionField;

            /// <remarks/>
            public GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionDataFileDecryption FileDecryption
            {
                get
                {
                    return this.fileDecryptionField;
                }
                set
                {
                    this.fileDecryptionField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionDataFileDecryption
        {

            private string fileDigestField;

            private string decryptionKeyField;

            private GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionDataFileDecryptionSecurityData securityDataField;

            /// <remarks/>
            public string FileDigest
            {
                get
                {
                    return this.fileDigestField;
                }
                set
                {
                    this.fileDigestField = value;
                }
            }

            /// <remarks/>
            public string DecryptionKey
            {
                get
                {
                    return this.decryptionKeyField;
                }
                set
                {
                    this.decryptionKeyField = value;
                }
            }

            /// <remarks/>
            public GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionDataFileDecryptionSecurityData SecurityData
            {
                get
                {
                    return this.securityDataField;
                }
                set
                {
                    this.securityDataField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileDecryptionDataFileDecryptionSecurityData
        {

            private string base64BinaryField;

            /// <remarks/>
            public string base64Binary
            {
                get
                {
                    return this.base64BinaryField;
                }
                set
                {
                    this.base64BinaryField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultUpdate
        {

            private ulong idField;

            private string xmlField;

            /// <remarks/>
            public ulong ID
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }

            /// <remarks/>
            public string Xml
            {
                get
                {
                    return this.xmlField;
                }
                set
                {
                    this.xmlField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class GetExtendedUpdateInfoResponseGetExtendedUpdateInfoResultFileLocation
        {

            private string fileDigestField;

            private string urlField;

            /// <remarks/>
            public string FileDigest
            {
                get
                {
                    return this.fileDigestField;
                }
                set
                {
                    this.fileDigestField = value;
                }
            }

            /// <remarks/>
            public string Url
            {
                get
                {
                    return this.urlField;
                }
                set
                {
                    this.urlField = value;
                }
            }
        }


    }
}
