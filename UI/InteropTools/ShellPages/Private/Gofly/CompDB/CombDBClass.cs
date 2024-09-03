using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace PhotoshoppedUUPCLI.CompDB
{
    [XmlRoot(ElementName = "Package", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class Package
    {
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
        [XmlAttribute(AttributeName = "PackageType")]
        public string PackageType { get; set; }
        [XmlAttribute(AttributeName = "FIP")]
        public string FIP { get; set; }
        [XmlElement(ElementName = "Payload", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public Payload Payload { get; set; }
        [XmlAttribute(AttributeName = "Partition")]
        public string Partition { get; set; }
        [XmlAttribute(AttributeName = "PublicKeyToken")]
        public string PublicKeyToken { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "SatelliteType")]
        public string SatelliteType { get; set; }
        [XmlAttribute(AttributeName = "SatelliteValue")]
        public string SatelliteValue { get; set; }
        [XmlAttribute(AttributeName = "UserInstallable")]
        public string UserInstallable { get; set; }
    }

    [XmlRoot(ElementName = "Packages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class Packages
    {
        [XmlElement(ElementName = "Package", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public List<Package> Package { get; set; }
    }

    [XmlRoot(ElementName = "Feature", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class Feature
    {
        [XmlElement(ElementName = "Packages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public Packages Packages { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "FeatureID")]
        public string FeatureID { get; set; }
        [XmlAttribute(AttributeName = "Group")]
        public string Group { get; set; }
        [XmlAttribute(AttributeName = "FMID")]
        public string FMID { get; set; }
    }

    [XmlRoot(ElementName = "Features", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class Features
    {
        [XmlElement(ElementName = "Feature", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public List<Feature> Feature { get; set; }
    }

    [XmlRoot(ElementName = "Condition", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class Condition
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Operator")]
        public string Operator { get; set; }
        [XmlAttribute(AttributeName = "RegistryKey")]
        public string RegistryKey { get; set; }
    }

    [XmlRoot(ElementName = "ConditionalFeature", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class ConditionalFeature
    {
        [XmlElement(ElementName = "Condition", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public Condition Condition { get; set; }
        [XmlAttribute(AttributeName = "FeatureID")]
        public string FeatureID { get; set; }
        [XmlAttribute(AttributeName = "UpdateAction")]
        public string UpdateAction { get; set; }
    }

    [XmlRoot(ElementName = "MSConditionalFeatures", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class MSConditionalFeatures
    {
        [XmlElement(ElementName = "ConditionalFeature", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public List<ConditionalFeature> ConditionalFeature { get; set; }
    }

    [XmlRoot(ElementName = "PayloadItem", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class PayloadItem
    {
        [XmlAttribute(AttributeName = "PayloadHash")]
        public string PayloadHash { get; set; }
        [XmlAttribute(AttributeName = "PayloadSize")]
        public string PayloadSize { get; set; }
        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
        [XmlAttribute(AttributeName = "PayloadType")]
        public string PayloadType { get; set; }
    }

    [XmlRoot(ElementName = "Payload", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class Payload
    {
        [XmlElement(ElementName = "PayloadItem", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public PayloadItem PayloadItem { get; set; }
    }

    [XmlRoot(ElementName = "CompDB", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class CompDB
    {
        [XmlElement(ElementName = "Features", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public Features Features { get; set; }
        [XmlElement(ElementName = "MSConditionalFeatures", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public MSConditionalFeatures MSConditionalFeatures { get; set; }
        [XmlElement(ElementName = "Packages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public Packages Packages { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "CreatedDate")]
        public string CreatedDate { get; set; }
        [XmlAttribute(AttributeName = "Revision")]
        public string Revision { get; set; }
        [XmlAttribute(AttributeName = "SchemaVersion")]
        public string SchemaVersion { get; set; }
        [XmlAttribute(AttributeName = "Product")]
        public string Product { get; set; }
        [XmlAttribute(AttributeName = "BuildID")]
        public string BuildID { get; set; }
        [XmlAttribute(AttributeName = "BuildInfo")]
        public string BuildInfo { get; set; }
        [XmlAttribute(AttributeName = "OSVersion")]
        public string OSVersion { get; set; }
        [XmlAttribute(AttributeName = "BuildArch")]
        public string BuildArch { get; set; }
        [XmlAttribute(AttributeName = "ReleaseType")]
        public string ReleaseType { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
