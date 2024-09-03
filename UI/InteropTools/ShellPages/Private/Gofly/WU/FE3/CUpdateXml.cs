namespace GetAppxPackages.WU.FE3
{
    public class CUpdateXml
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class Xml
        {
            private XmlExtendedProperties extendedPropertiesField;

            private XmlFile[] filesField;

            private XmlHandlerSpecificData handlerSpecificDataField;

            private XmlLocalizedProperties localizedPropertiesField;

            /// <remarks/>
            public XmlExtendedProperties ExtendedProperties
            {
                get
                {
                    return this.extendedPropertiesField;
                }
                set
                {
                    this.extendedPropertiesField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("File", IsNullable = false)]
            public XmlFile[] Files
            {
                get
                {
                    return this.filesField;
                }
                set
                {
                    this.filesField = value;
                }
            }

            /// <remarks/>
            public XmlHandlerSpecificData HandlerSpecificData
            {
                get
                {
                    return this.handlerSpecificDataField;
                }
                set
                {
                    this.handlerSpecificDataField = value;
                }
            }

            /// <remarks/>
            public XmlLocalizedProperties LocalizedProperties
            {
                get
                {
                    return this.localizedPropertiesField;
                }
                set
                {
                    this.localizedPropertiesField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlExtendedProperties
        {

            private XmlExtendedPropertiesInstallationBehavior installationBehaviorField;

            private string handlerField;

            private string defaultPropertiesLanguageField;

            private ulong maxDownloadSizeField;

            private byte minDownloadSizeField;

            private System.DateTime creationDateField;

            private string contentTypeField;

            private ushort buildNumberField;

            private bool buildNumberFieldSpecified;

            private string branchNameField;

            private string ringField;

            private string packageContentIdField;

            private bool isAppxFrameworkField;

            private bool isAppxFrameworkFieldSpecified;

            private decimal compatibleProtocolVersionField;

            private bool compatibleProtocolVersionFieldSpecified;

            private bool fromStoreServiceField;

            private bool fromStoreServiceFieldSpecified;

            private string packageIdentityNameField;

            private string legacyMobileProductIdField;

            private string licensingPayloadIdField;

            private string licensingSaltField;

            /// <remarks/>
            public XmlExtendedPropertiesInstallationBehavior InstallationBehavior
            {
                get
                {
                    return this.installationBehaviorField;
                }
                set
                {
                    this.installationBehaviorField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Handler
            {
                get
                {
                    return this.handlerField;
                }
                set
                {
                    this.handlerField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string DefaultPropertiesLanguage
            {
                get
                {
                    return this.defaultPropertiesLanguageField;
                }
                set
                {
                    this.defaultPropertiesLanguageField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ulong MaxDownloadSize
            {
                get
                {
                    return this.maxDownloadSizeField;
                }
                set
                {
                    this.maxDownloadSizeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte MinDownloadSize
            {
                get
                {
                    return this.minDownloadSizeField;
                }
                set
                {
                    this.minDownloadSizeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public System.DateTime CreationDate
            {
                get
                {
                    return this.creationDateField;
                }
                set
                {
                    this.creationDateField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string ContentType
            {
                get
                {
                    return this.contentTypeField;
                }
                set
                {
                    this.contentTypeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ushort BuildNumber
            {
                get
                {
                    return this.buildNumberField;
                }
                set
                {
                    this.buildNumberField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool BuildNumberSpecified
            {
                get
                {
                    return this.buildNumberFieldSpecified;
                }
                set
                {
                    this.buildNumberFieldSpecified = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string BranchName
            {
                get
                {
                    return this.branchNameField;
                }
                set
                {
                    this.branchNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Ring
            {
                get
                {
                    return this.ringField;
                }
                set
                {
                    this.ringField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string PackageContentId
            {
                get
                {
                    return this.packageContentIdField;
                }
                set
                {
                    this.packageContentIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public bool IsAppxFramework
            {
                get
                {
                    return this.isAppxFrameworkField;
                }
                set
                {
                    this.isAppxFrameworkField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool IsAppxFrameworkSpecified
            {
                get
                {
                    return this.isAppxFrameworkFieldSpecified;
                }
                set
                {
                    this.isAppxFrameworkFieldSpecified = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public decimal CompatibleProtocolVersion
            {
                get
                {
                    return this.compatibleProtocolVersionField;
                }
                set
                {
                    this.compatibleProtocolVersionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool CompatibleProtocolVersionSpecified
            {
                get
                {
                    return this.compatibleProtocolVersionFieldSpecified;
                }
                set
                {
                    this.compatibleProtocolVersionFieldSpecified = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public bool FromStoreService
            {
                get
                {
                    return this.fromStoreServiceField;
                }
                set
                {
                    this.fromStoreServiceField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool FromStoreServiceSpecified
            {
                get
                {
                    return this.fromStoreServiceFieldSpecified;
                }
                set
                {
                    this.fromStoreServiceFieldSpecified = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string PackageIdentityName
            {
                get
                {
                    return this.packageIdentityNameField;
                }
                set
                {
                    this.packageIdentityNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string LegacyMobileProductId
            {
                get
                {
                    return this.legacyMobileProductIdField;
                }
                set
                {
                    this.legacyMobileProductIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string LicensingPayloadId
            {
                get
                {
                    return this.licensingPayloadIdField;
                }
                set
                {
                    this.licensingPayloadIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string LicensingSalt
            {
                get
                {
                    return this.licensingSaltField;
                }
                set
                {
                    this.licensingSaltField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlExtendedPropertiesInstallationBehavior
        {

            private string rebootBehaviorField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string RebootBehavior
            {
                get
                {
                    return this.rebootBehaviorField;
                }
                set
                {
                    this.rebootBehaviorField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlFile
        {

            private XmlFileAdditionalDigest additionalDigestField;

            private string digestField;

            private string digestAlgorithmField;

            private string fileNameField;

            private ulong sizeField;

            private System.DateTime modifiedField;

            private string patchingTypeField;

            private bool isEncryptedField;

            private bool isEncryptedFieldSpecified;

            /// <remarks/>
            public XmlFileAdditionalDigest AdditionalDigest
            {
                get
                {
                    return this.additionalDigestField;
                }
                set
                {
                    this.additionalDigestField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Digest
            {
                get
                {
                    return this.digestField;
                }
                set
                {
                    this.digestField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string DigestAlgorithm
            {
                get
                {
                    return this.digestAlgorithmField;
                }
                set
                {
                    this.digestAlgorithmField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string FileName
            {
                get
                {
                    return this.fileNameField;
                }
                set
                {
                    this.fileNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ulong Size
            {
                get
                {
                    return this.sizeField;
                }
                set
                {
                    this.sizeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public System.DateTime Modified
            {
                get
                {
                    return this.modifiedField;
                }
                set
                {
                    this.modifiedField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string PatchingType
            {
                get
                {
                    return this.patchingTypeField;
                }
                set
                {
                    this.patchingTypeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public bool IsEncrypted
            {
                get
                {
                    return this.isEncryptedField;
                }
                set
                {
                    this.isEncryptedField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool IsEncryptedSpecified
            {
                get
                {
                    return this.isEncryptedFieldSpecified;
                }
                set
                {
                    this.isEncryptedFieldSpecified = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlFileAdditionalDigest
        {

            private string algorithmField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Algorithm
            {
                get
                {
                    return this.algorithmField;
                }
                set
                {
                    this.algorithmField = value;
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
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlHandlerSpecificData
        {

            private XmlHandlerSpecificDataAppxPackageInstallData appxPackageInstallDataField;

            private XmlHandlerSpecificDataInstallCommand installCommandField;

            private string typeField;

            /// <remarks/>
            public XmlHandlerSpecificDataAppxPackageInstallData AppxPackageInstallData
            {
                get
                {
                    return this.appxPackageInstallDataField;
                }
                set
                {
                    this.appxPackageInstallDataField = value;
                }
            }

            /// <remarks/>
            public XmlHandlerSpecificDataInstallCommand InstallCommand
            {
                get
                {
                    return this.installCommandField;
                }
                set
                {
                    this.installCommandField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlHandlerSpecificDataAppxPackageInstallData
        {

            private string packageFileNameField;

            private bool mainPackageField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string PackageFileName
            {
                get
                {
                    return this.packageFileNameField;
                }
                set
                {
                    this.packageFileNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public bool MainPackage
            {
                get
                {
                    return this.mainPackageField;
                }
                set
                {
                    this.mainPackageField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlHandlerSpecificDataInstallCommand
        {

            private string programField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Program
            {
                get
                {
                    return this.programField;
                }
                set
                {
                    this.programField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class XmlLocalizedProperties
        {

            private string languageField;

            private string titleField;

            private string descriptionField;

            /// <remarks/>
            public string Language
            {
                get
                {
                    return this.languageField;
                }
                set
                {
                    this.languageField = value;
                }
            }

            /// <remarks/>
            public string Title
            {
                get
                {
                    return this.titleField;
                }
                set
                {
                    this.titleField = value;
                }
            }

            /// <remarks/>
            public string Description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }
        }


    }
}
