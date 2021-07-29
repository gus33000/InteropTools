using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace AppPlugin
{
    internal static class Helper
    {
        internal static string Serilize<T>(T output)
        {
            string outputString;
            DataContractSerializer serelizerOut = new(typeof(T));
            using (StringWriter stringWriter = new())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                {
                    serelizerOut.WriteObject(xmlWriter, output);
                }

                outputString = stringWriter.ToString();
            }

            return outputString;
        }

        internal static T DeSerilize<T>(string inputString)
        {
            T input;
            DataContractSerializer serelizerIn = new(typeof(T));
            using (StringReader stringReader = new(inputString))
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                input = (T)serelizerIn.ReadObject(xmlReader);
            }

            return input;
        }
    }
}
