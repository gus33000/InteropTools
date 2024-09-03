using System.Collections.Generic;
using System.Xml.Serialization;

namespace PhotoshoppedUUPCLI.UUP.FE3.Responses
{
    public class XmlResponseClasses
    {
        [XmlRoot(ElementName = "UpdateIdentity")]
        public class UpdateIdentity
        {
            [XmlAttribute(AttributeName = "UpdateID")]
            public string UpdateID { get; set; }
            [XmlAttribute(AttributeName = "RevisionNumber")]
            public string RevisionNumber { get; set; }
        }

        [XmlRoot(ElementName = "Properties")]
        public class Properties
        {
            [XmlAttribute(AttributeName = "UpdateType")]
            public string UpdateType { get; set; }
            [XmlElement(ElementName = "SecuredFragment")]
            public string SecuredFragment { get; set; }
            [XmlAttribute(AttributeName = "ExplicitlyDeployable")]
            public string ExplicitlyDeployable { get; set; }
        }

        [XmlRoot(ElementName = "b.WindowsLanguage")]
        public class BWindowsLanguage
        {
            [XmlAttribute(AttributeName = "Language")]
            public string Language { get; set; }
        }

        [XmlRoot(ElementName = "IsInstalled")]
        public class IsInstalled
        {
            [XmlElement(ElementName = "b.WindowsLanguage")]
            public BWindowsLanguage BWindowsLanguage { get; set; }
            [XmlElement(ElementName = "Or")]
            public Or Or { get; set; }
            [XmlElement(ElementName = "b.RegSz")]
            public BRegSz BRegSz { get; set; }
            [XmlElement(ElementName = "ProductReleaseInstalled")]
            public ProductReleaseInstalled ProductReleaseInstalled { get; set; }
        }

        [XmlRoot(ElementName = "ApplicabilityRules")]
        public class ApplicabilityRules
        {
            [XmlElement(ElementName = "IsInstalled")]
            public IsInstalled IsInstalled { get; set; }
            [XmlElement(ElementName = "IsInstallable")]
            public IsInstallable IsInstallable { get; set; }
        }

        [XmlRoot(ElementName = "b.RegSz")]
        public class BRegSz
        {
            [XmlAttribute(AttributeName = "Key")]
            public string Key { get; set; }
            [XmlAttribute(AttributeName = "Subkey")]
            public string Subkey { get; set; }
            [XmlAttribute(AttributeName = "Value")]
            public string Value { get; set; }
            [XmlAttribute(AttributeName = "Comparison")]
            public string Comparison { get; set; }
            [XmlAttribute(AttributeName = "Data")]
            public string Data { get; set; }
            [XmlAttribute(AttributeName = "RegType32")]
            public string RegType32 { get; set; }
        }

        [XmlRoot(ElementName = "b.RegValueExists")]
        public class BRegValueExists
        {
            [XmlAttribute(AttributeName = "Key")]
            public string Key { get; set; }
            [XmlAttribute(AttributeName = "Subkey")]
            public string Subkey { get; set; }
            [XmlAttribute(AttributeName = "Value")]
            public string Value { get; set; }
            [XmlAttribute(AttributeName = "RegType32")]
            public string RegType32 { get; set; }
            [XmlAttribute(AttributeName = "Type")]
            public string Type { get; set; }
        }

        [XmlRoot(ElementName = "Not")]
        public class Not
        {
            [XmlElement(ElementName = "b.RegValueExists")]
            public BRegValueExists BRegValueExists { get; set; }
        }

        [XmlRoot(ElementName = "And")]
        public class And
        {
            [XmlElement(ElementName = "Not")]
            public Not Not { get; set; }
            [XmlElement(ElementName = "b.RegSz")]
            public BRegSz BRegSz { get; set; }
            [XmlElement(ElementName = "b.RegDword")]
            public List<BRegDword> BRegDword { get; set; }
            [XmlElement(ElementName = "Or")]
            public List<Or> Or { get; set; }
        }

        [XmlRoot(ElementName = "Or")]
        public class Or
        {
            [XmlElement(ElementName = "b.RegSz")]
            public BRegSz BRegSz { get; set; }
            [XmlElement(ElementName = "And")]
            public And And { get; set; }
            [XmlElement(ElementName = "Or")]
            public Or or { get; set; }
        }

        [XmlRoot(ElementName = "b.RegDword")]
        public class BRegDword
        {
            [XmlAttribute(AttributeName = "Key")]
            public string Key { get; set; }
            [XmlAttribute(AttributeName = "Subkey")]
            public string Subkey { get; set; }
            [XmlAttribute(AttributeName = "Value")]
            public string Value { get; set; }
            [XmlAttribute(AttributeName = "Data")]
            public string Data { get; set; }
            [XmlAttribute(AttributeName = "Comparison")]
            public string Comparison { get; set; }
        }

        [XmlRoot(ElementName = "b.SystemMetric")]
        public class BSystemMetric
        {
            [XmlAttribute(AttributeName = "Comparison")]
            public string Comparison { get; set; }
            [XmlAttribute(AttributeName = "Index")]
            public string Index { get; set; }
            [XmlAttribute(AttributeName = "Value")]
            public string Value { get; set; }
        }

        [XmlRoot(ElementName = "ProductReleaseInstalled")]
        public class ProductReleaseInstalled
        {
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "Version")]
            public string Version { get; set; }
        }

        [XmlRoot(ElementName = "IsInstallable")]
        public class IsInstallable
        {
            [XmlElement(ElementName = "True")]
            public string True { get; set; }
        }

        [XmlRoot(ElementName = "AtLeastOne")]
        public class AtLeastOne
        {
            [XmlElement(ElementName = "UpdateIdentity")]
            public UpdateIdentity UpdateIdentity { get; set; }
            [XmlAttribute(AttributeName = "IsCategory")]
            public string IsCategory { get; set; }
        }

        [XmlRoot(ElementName = "Prerequisites")]
        public class Prerequisites
        {
            [XmlElement(ElementName = "AtLeastOne")]
            public AtLeastOne AtLeastOne { get; set; }
        }

        [XmlRoot(ElementName = "Relationships")]
        public class Relationships
        {
            [XmlElement(ElementName = "Prerequisites")]
            public Prerequisites Prerequisites { get; set; }
        }

        [XmlRoot(ElementName = "LocalizedProperties")]
        public class LocalizedProperties
        {
            [XmlElement(ElementName = "Language")]
            public string Language { get; set; }
            [XmlElement(ElementName = "Title")]
            public string Title { get; set; }
            [XmlElement(ElementName = "Description")]
            public string Description { get; set; }
        }

        [XmlRoot(ElementName = "InstallationBehavior")]
        public class InstallationBehavior
        {
            [XmlAttribute(AttributeName = "RebootBehavior")]
            public string RebootBehavior { get; set; }
        }

        [XmlRoot(ElementName = "ExtendedProperties")]
        public class ExtendedProperties
        {
            [XmlElement(ElementName = "InstallationBehavior")]
            public InstallationBehavior InstallationBehavior { get; set; }
            [XmlAttribute(AttributeName = "ProductName")]
            public string ProductName { get; set; }
            [XmlAttribute(AttributeName = "ReleaseVersion")]
            public string ReleaseVersion { get; set; }
            [XmlAttribute(AttributeName = "Handler")]
            public string Handler { get; set; }
            [XmlAttribute(AttributeName = "MaxDownloadSize")]
            public string MaxDownloadSize { get; set; }
            [XmlAttribute(AttributeName = "MinDownloadSize")]
            public string MinDownloadSize { get; set; }
            [XmlAttribute(AttributeName = "DefaultPropertiesLanguage")]
            public string DefaultPropertiesLanguage { get; set; }
            [XmlAttribute(AttributeName = "CompatibleProtocolVersion")]
            public string CompatibleProtocolVersion { get; set; }
            [XmlAttribute(AttributeName = "ContentType")]
            public string ContentType { get; set; }
            [XmlAttribute(AttributeName = "AutoSelectOnWebsites")]
            public string AutoSelectOnWebsites { get; set; }
            [XmlAttribute(AttributeName = "BrowseOnly")]
            public string BrowseOnly { get; set; }
        }

        [XmlRoot(ElementName = "AdditionalDigest")]
        public class AdditionalDigest
        {
            [XmlAttribute(AttributeName = "Algorithm")]
            public string Algorithm { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "File")]
        public class File
        {
            [XmlElement(ElementName = "AdditionalDigest")]
            public AdditionalDigest AdditionalDigest { get; set; }
            [XmlAttribute(AttributeName = "Digest")]
            public string Digest { get; set; }
            [XmlAttribute(AttributeName = "DigestAlgorithm")]
            public string DigestAlgorithm { get; set; }
            [XmlAttribute(AttributeName = "FileName")]
            public string FileName { get; set; }
            [XmlAttribute(AttributeName = "Size")]
            public string Size { get; set; }
            [XmlAttribute(AttributeName = "Modified")]
            public string Modified { get; set; }
            [XmlAttribute(AttributeName = "PatchingType")]
            public string PatchingType { get; set; }
        }

        [XmlRoot(ElementName = "Files")]
        public class Files
        {
            [XmlElement(ElementName = "File")]
            public List<File> File { get; set; }
        }

        [XmlRoot(ElementName = "OSInstallData")]
        public class OSInstallData
        {
            [XmlAttribute(AttributeName = "InitialModule")]
            public string InitialModule { get; set; }
        }

        [XmlRoot(ElementName = "HandlerSpecificData")]
        public class HandlerSpecificData
        {
            [XmlElement(ElementName = "OSInstallData")]
            public OSInstallData OSInstallData { get; set; }
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
        }


        [XmlRoot(ElementName = "Xml")]
        public class Xml
        {
            [XmlElement(ElementName = "UpdateIdentity")]
            public List<UpdateIdentity> UpdateIdentity { get; set; }
            [XmlElement(ElementName = "Properties")]
            public List<Properties> Properties { get; set; }
            [XmlElement(ElementName = "ApplicabilityRules")]
            public List<ApplicabilityRules> ApplicabilityRules { get; set; }
            [XmlElement(ElementName = "Relationships")]
            public Relationships Relationships { get; set; }
            [XmlElement(ElementName = "LocalizedProperties")]
            public List<LocalizedProperties> LocalizedProperties { get; set; }
            [XmlElement(ElementName = "ExtendedProperties")]
            public List<ExtendedProperties> ExtendedProperties { get; set; }
            [XmlElement(ElementName = "Files")]
            public Files Files { get; set; }
            [XmlElement(ElementName = "HandlerSpecificData")]
            public HandlerSpecificData HandlerSpecificData { get; set; }
        }
    }
}
