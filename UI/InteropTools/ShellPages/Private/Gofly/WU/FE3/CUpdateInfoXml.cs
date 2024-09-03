namespace GetAppxPackages.WU.FE3
{
    public class CUpdateInfoXml
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class Xml
        {

            private XmlUpdateIdentity updateIdentityField;

            private XmlProperties propertiesField;

            private XmlRelationships relationshipsField;

            private XmlApplicabilityRules applicabilityRulesField;

            /// <remarks/>
            public XmlUpdateIdentity UpdateIdentity
            {
                get
                {
                    return this.updateIdentityField;
                }
                set
                {
                    this.updateIdentityField = value;
                }
            }

            /// <remarks/>
            public XmlProperties Properties
            {
                get
                {
                    return this.propertiesField;
                }
                set
                {
                    this.propertiesField = value;
                }
            }

            /// <remarks/>
            public XmlRelationships Relationships
            {
                get
                {
                    return this.relationshipsField;
                }
                set
                {
                    this.relationshipsField = value;
                }
            }

            /// <remarks/>
            public XmlApplicabilityRules ApplicabilityRules
            {
                get
                {
                    return this.applicabilityRulesField;
                }
                set
                {
                    this.applicabilityRulesField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlUpdateIdentity
        {

            private string updateIDField;

            private byte revisionNumberField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string UpdateID
            {
                get
                {
                    return this.updateIDField;
                }
                set
                {
                    this.updateIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte RevisionNumber
            {
                get
                {
                    return this.revisionNumberField;
                }
                set
                {
                    this.revisionNumberField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlProperties
        {

            private string securedFragmentField;

            private string updateTypeField;

            private bool perUserField;

            private ushort packageRankField;

            /// <remarks/>
            public string SecuredFragment
            {
                get
                {
                    return this.securedFragmentField;
                }
                set
                {
                    this.securedFragmentField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string UpdateType
            {
                get
                {
                    return this.updateTypeField;
                }
                set
                {
                    this.updateTypeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public bool PerUser
            {
                get
                {
                    return this.perUserField;
                }
                set
                {
                    this.perUserField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ushort PackageRank
            {
                get
                {
                    return this.packageRankField;
                }
                set
                {
                    this.packageRankField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlRelationships
        {

            private XmlRelationshipsPrerequisites prerequisitesField;

            /// <remarks/>
            public XmlRelationshipsPrerequisites Prerequisites
            {
                get
                {
                    return this.prerequisitesField;
                }
                set
                {
                    this.prerequisitesField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlRelationshipsPrerequisites
        {

            private object[] itemsField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("AtLeastOne", typeof(XmlRelationshipsPrerequisitesAtLeastOne))]
            [System.Xml.Serialization.XmlElementAttribute("UpdateIdentity", typeof(XmlRelationshipsPrerequisitesUpdateIdentity))]
            public object[] Items
            {
                get
                {
                    return this.itemsField;
                }
                set
                {
                    this.itemsField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlRelationshipsPrerequisitesAtLeastOne
        {

            private XmlRelationshipsPrerequisitesAtLeastOneUpdateIdentity[] updateIdentityField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("UpdateIdentity")]
            public XmlRelationshipsPrerequisitesAtLeastOneUpdateIdentity[] UpdateIdentity
            {
                get
                {
                    return this.updateIdentityField;
                }
                set
                {
                    this.updateIdentityField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlRelationshipsPrerequisitesAtLeastOneUpdateIdentity
        {

            private string updateIDField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string UpdateID
            {
                get
                {
                    return this.updateIDField;
                }
                set
                {
                    this.updateIDField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlRelationshipsPrerequisitesUpdateIdentity
        {

            private string updateIDField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string UpdateID
            {
                get
                {
                    return this.updateIDField;
                }
                set
                {
                    this.updateIDField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlApplicabilityRules
        {

            private XmlApplicabilityRulesIsInstalled isInstalledField;

            private XmlApplicabilityRulesIsInstallable isInstallableField;

            private XmlApplicabilityRulesMetadata metadataField;

            /// <remarks/>
            public XmlApplicabilityRulesIsInstalled IsInstalled
            {
                get
                {
                    return this.isInstalledField;
                }
                set
                {
                    this.isInstalledField = value;
                }
            }

            /// <remarks/>
            public XmlApplicabilityRulesIsInstallable IsInstallable
            {
                get
                {
                    return this.isInstallableField;
                }
                set
                {
                    this.isInstallableField = value;
                }
            }

            /// <remarks/>
            public XmlApplicabilityRulesMetadata Metadata
            {
                get
                {
                    return this.metadataField;
                }
                set
                {
                    this.metadataField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlApplicabilityRulesIsInstalled
        {

            private object appxPackageInstalledField;

            /// <remarks/>
            public object AppxPackageInstalled
            {
                get
                {
                    return this.appxPackageInstalledField;
                }
                set
                {
                    this.appxPackageInstalledField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlApplicabilityRulesIsInstallable
        {

            private object appxPackageInstallableField;

            /// <remarks/>
            public object AppxPackageInstallable
            {
                get
                {
                    return this.appxPackageInstallableField;
                }
                set
                {
                    this.appxPackageInstallableField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlApplicabilityRulesMetadata
        {

            private XmlApplicabilityRulesMetadataAppxPackageMetadata appxPackageMetadataField;

            /// <remarks/>
            public XmlApplicabilityRulesMetadataAppxPackageMetadata AppxPackageMetadata
            {
                get
                {
                    return this.appxPackageMetadataField;
                }
                set
                {
                    this.appxPackageMetadataField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlApplicabilityRulesMetadataAppxPackageMetadata
        {

            private XmlApplicabilityRulesMetadataAppxPackageMetadataAppxMetadata appxMetadataField;

            /// <remarks/>
            public XmlApplicabilityRulesMetadataAppxPackageMetadataAppxMetadata AppxMetadata
            {
                get
                {
                    return this.appxMetadataField;
                }
                set
                {
                    this.appxMetadataField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlApplicabilityRulesMetadataAppxPackageMetadataAppxMetadata
        {

            private string applicabilityBlobField;

            private string packageTypeField;

            private bool isAppxBundleField;

            private string packageMonikerField;

            /// <remarks/>
            public string ApplicabilityBlob
            {
                get
                {
                    return this.applicabilityBlobField;
                }
                set
                {
                    this.applicabilityBlobField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string PackageType
            {
                get
                {
                    return this.packageTypeField;
                }
                set
                {
                    this.packageTypeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public bool IsAppxBundle
            {
                get
                {
                    return this.isAppxBundleField;
                }
                set
                {
                    this.isAppxBundleField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string PackageMoniker
            {
                get
                {
                    return this.packageMonikerField;
                }
                set
                {
                    this.packageMonikerField = value;
                }
            }
        }


    }
}
