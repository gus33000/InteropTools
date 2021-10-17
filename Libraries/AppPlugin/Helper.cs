using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AppPlugin
{
    internal static class Helper
    {
        internal static string Serilize<T>(T output)
        {
            string outputString;
            var serelizerOut = new DataContractSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                    serelizerOut.WriteObject(xmlWriter, output);
                outputString = stringWriter.ToString();
            }

            return outputString;
        }

        internal static T DeSerilize<T>(string inputString)
        {
            T input;
            var serelizerIn = new DataContractSerializer(typeof(T));
            using (var stringReader = new StringReader(inputString))
            using (var xmlReader = XmlReader.Create(stringReader))
                input = (T)serelizerIn.ReadObject(xmlReader);
            return input;
        }
    }
}
