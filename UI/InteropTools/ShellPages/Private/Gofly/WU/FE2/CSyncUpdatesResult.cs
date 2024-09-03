using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAppxPackages.WU.FE2
{
    public class CSyncUpdatesResult
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

            private SyncUpdatesResponse syncUpdatesResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
            public SyncUpdatesResponse SyncUpdatesResponse
            {
                get
                {
                    return this.syncUpdatesResponseField;
                }
                set
                {
                    this.syncUpdatesResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService", IsNullable = false)]
        public partial class SyncUpdatesResponse
        {

            private SyncUpdatesResponseSyncUpdatesResult syncUpdatesResultField;

            /// <remarks/>
            public SyncUpdatesResponseSyncUpdatesResult SyncUpdatesResult
            {
                get
                {
                    return this.syncUpdatesResultField;
                }
                set
                {
                    this.syncUpdatesResultField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class SyncUpdatesResponseSyncUpdatesResult
        {

            private SyncUpdatesResponseSyncUpdatesResultUpdateInfo[] newUpdatesField;

            private SyncUpdatesResponseSyncUpdatesResultUpdateInfo1[] changedUpdatesField;

            private bool truncatedField;

            private SyncUpdatesResponseSyncUpdatesResultNewCookie newCookieField;

            private bool driverSyncNotNeededField;

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("UpdateInfo", IsNullable = false)]
            public SyncUpdatesResponseSyncUpdatesResultUpdateInfo[] NewUpdates
            {
                get
                {
                    return this.newUpdatesField;
                }
                set
                {
                    this.newUpdatesField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("UpdateInfo", IsNullable = false)]
            public SyncUpdatesResponseSyncUpdatesResultUpdateInfo1[] ChangedUpdates
            {
                get
                {
                    return this.changedUpdatesField;
                }
                set
                {
                    this.changedUpdatesField = value;
                }
            }

            /// <remarks/>
            public bool Truncated
            {
                get
                {
                    return this.truncatedField;
                }
                set
                {
                    this.truncatedField = value;
                }
            }

            /// <remarks/>
            public SyncUpdatesResponseSyncUpdatesResultNewCookie NewCookie
            {
                get
                {
                    return this.newCookieField;
                }
                set
                {
                    this.newCookieField = value;
                }
            }

            /// <remarks/>
            public bool DriverSyncNotNeeded
            {
                get
                {
                    return this.driverSyncNotNeededField;
                }
                set
                {
                    this.driverSyncNotNeededField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class SyncUpdatesResponseSyncUpdatesResultUpdateInfo
        {

            private uint idField;

            private SyncUpdatesResponseSyncUpdatesResultUpdateInfoDeployment deploymentField;

            private bool isLeafField;

            private string xmlField;

            /// <remarks/>
            public uint ID
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
            public SyncUpdatesResponseSyncUpdatesResultUpdateInfoDeployment Deployment
            {
                get
                {
                    return this.deploymentField;
                }
                set
                {
                    this.deploymentField = value;
                }
            }

            /// <remarks/>
            public bool IsLeaf
            {
                get
                {
                    return this.isLeafField;
                }
                set
                {
                    this.isLeafField = value;
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
        public partial class SyncUpdatesResponseSyncUpdatesResultUpdateInfoDeployment
        {

            private uint idField;

            private string actionField;

            private bool isAssignedField;

            private System.DateTime lastChangeTimeField;

            private byte autoSelectField;

            private byte autoDownloadField;

            private byte supersedenceBehaviorField;

            private byte priorityField;

            private byte handlerSpecificActionField;

            /// <remarks/>
            public uint ID
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
            public string Action
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
            public bool IsAssigned
            {
                get
                {
                    return this.isAssignedField;
                }
                set
                {
                    this.isAssignedField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime LastChangeTime
            {
                get
                {
                    return this.lastChangeTimeField;
                }
                set
                {
                    this.lastChangeTimeField = value;
                }
            }

            /// <remarks/>
            public byte AutoSelect
            {
                get
                {
                    return this.autoSelectField;
                }
                set
                {
                    this.autoSelectField = value;
                }
            }

            /// <remarks/>
            public byte AutoDownload
            {
                get
                {
                    return this.autoDownloadField;
                }
                set
                {
                    this.autoDownloadField = value;
                }
            }

            /// <remarks/>
            public byte SupersedenceBehavior
            {
                get
                {
                    return this.supersedenceBehaviorField;
                }
                set
                {
                    this.supersedenceBehaviorField = value;
                }
            }

            /// <remarks/>
            public byte Priority
            {
                get
                {
                    return this.priorityField;
                }
                set
                {
                    this.priorityField = value;
                }
            }

            /// <remarks/>
            public byte HandlerSpecificAction
            {
                get
                {
                    return this.handlerSpecificActionField;
                }
                set
                {
                    this.handlerSpecificActionField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class SyncUpdatesResponseSyncUpdatesResultUpdateInfo1
        {

            private uint idField;

            private SyncUpdatesResponseSyncUpdatesResultUpdateInfoDeployment1 deploymentField;

            private bool isLeafField;

            /// <remarks/>
            public uint ID
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
            public SyncUpdatesResponseSyncUpdatesResultUpdateInfoDeployment1 Deployment
            {
                get
                {
                    return this.deploymentField;
                }
                set
                {
                    this.deploymentField = value;
                }
            }

            /// <remarks/>
            public bool IsLeaf
            {
                get
                {
                    return this.isLeafField;
                }
                set
                {
                    this.isLeafField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class SyncUpdatesResponseSyncUpdatesResultUpdateInfoDeployment1
        {

            private uint idField;

            private string actionField;

            private bool isAssignedField;

            private System.DateTime lastChangeTimeField;

            private byte autoSelectField;

            private byte autoDownloadField;

            private byte supersedenceBehaviorField;

            private byte priorityField;

            private byte handlerSpecificActionField;

            /// <remarks/>
            public uint ID
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
            public string Action
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
            public bool IsAssigned
            {
                get
                {
                    return this.isAssignedField;
                }
                set
                {
                    this.isAssignedField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime LastChangeTime
            {
                get
                {
                    return this.lastChangeTimeField;
                }
                set
                {
                    this.lastChangeTimeField = value;
                }
            }

            /// <remarks/>
            public byte AutoSelect
            {
                get
                {
                    return this.autoSelectField;
                }
                set
                {
                    this.autoSelectField = value;
                }
            }

            /// <remarks/>
            public byte AutoDownload
            {
                get
                {
                    return this.autoDownloadField;
                }
                set
                {
                    this.autoDownloadField = value;
                }
            }

            /// <remarks/>
            public byte SupersedenceBehavior
            {
                get
                {
                    return this.supersedenceBehaviorField;
                }
                set
                {
                    this.supersedenceBehaviorField = value;
                }
            }

            /// <remarks/>
            public byte Priority
            {
                get
                {
                    return this.priorityField;
                }
                set
                {
                    this.priorityField = value;
                }
            }

            /// <remarks/>
            public byte HandlerSpecificAction
            {
                get
                {
                    return this.handlerSpecificActionField;
                }
                set
                {
                    this.handlerSpecificActionField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService")]
        public partial class SyncUpdatesResponseSyncUpdatesResultNewCookie
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
