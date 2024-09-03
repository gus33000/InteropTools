using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhotoshoppedUUPCLI.CompDB
{
    public static class MoveFilesBasedOnCompDB
    {
        /*public static void MoveFiles(string InputDir, string OutputDir, string InputCompDB)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CompDB));

                StreamReader reader = new StreamReader(InputCompDB);
                CompDB compdb = (CompDB)serializer.Deserialize(reader);
                reader.Close();

                var packagelist = compdb.Packages.Package;

                foreach (var package in packagelist)
                {
                    Console.WriteLine("Processing " + package.ID + " " + package.Version);

                    try
                    {
                        var filename = package.Payload.PayloadItem.Path.Split('\\').Last();
                        var path = string.Join("\\", package.Payload.PayloadItem.Path.Split('\\').Reverse().Skip(1).Reverse());

                        if (!System.IO.Directory.Exists(OutputDir + "\\" + path))
                            System.IO.Directory.CreateDirectory(OutputDir + "\\" + path);

                        System.IO.File.Move(InputDir + "\\" + filename, OutputDir + "\\" + package.Payload.PayloadItem.Path);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed! " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed! " + e.Message);
            }
        }*/
    }
}
