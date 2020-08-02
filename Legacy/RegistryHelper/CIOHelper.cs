using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace RegistryHelper
{
    public sealed class CIOHelper
    {
        public static uint CopyFile(string sourceFilePath, string targetFilePath)
        {
#if ARM
            return FieldMedicUtils.Utils.CopyFile(sourceFilePath, targetFilePath);
#else
            return 0;
#endif
        }

        public static uint CreateFile(string filePath, [ReadOnlyArray] byte[] data, bool overwrite)
        {
#if ARM
            return FieldMedicUtils.Utils.CreateFile(filePath, data, overwrite);
#else
            return 0;
#endif
        }

        public static uint CreateFolder(string folderName)
        {
#if ARM
            return FieldMedicUtils.Utils.CreateFolder(folderName);
#else
            return 0;
#endif
        }

        public static uint DeleteAllSubFolders(string folderPath)
        {
#if ARM
            return FieldMedicUtils.Utils.DeleteAllSubFolders(folderPath);
#else
            return 0;
#endif
        }

        public static uint DeleteFolder(string folderPath)
        {
#if ARM
            return FieldMedicUtils.Utils.DeleteFolder(folderPath);
#else
            return 0;
#endif
        }

        public static bool FileExists(string filePath)
        {
#if ARM
            return FieldMedicUtils.Utils.FileExists(filePath);
#else
            return 0;
#endif
        }

        public static string[] FindFolders(string baseFolderPath)
        {
#if ARM
            return FieldMedicUtils.Utils.FindFolders(baseFolderPath);
#else
            return 0;
#endif
        }

        public static string[] FindItems(string baseFolderPath)
        {
#if ARM
            return FieldMedicUtils.Utils.FindItems(baseFolderPath);
#else
            return 0;
#endif
        }

        public static string[] FindItemsUnderPath(string baseFolderPath)
        {
#if ARM
            return FieldMedicUtils.Utils.FindItemsUnderPath(baseFolderPath);
#else
            return 0;
#endif
        }

        public static bool FolderExists(string folderName)
        {
#if ARM
            return FieldMedicUtils.Utils.FolderExists(folderName);
#else
            return 0;
#endif
        }

        public static uint GetFileSize(string filePath, out ulong fileSize)
        {
#if ARM
            return FieldMedicUtils.Utils.GetFileSize(filePath, out fileSize);
#else
            return 0;
#endif
        }

        public static uint GetFolderSize(string folderPath, out ulong folderSize)
        {
#if ARM
            return FieldMedicUtils.Utils.GetFolderSize(folderPath, out folderSize);
#else
            return 0;
#endif
        }

        public static uint MoveFile(string sourceFilePath, string targetFilePath)
        {
#if ARM
            return FieldMedicUtils.Utils.MoveFile(sourceFilePath, targetFilePath);
#else
            return 0;
#endif
        }
    }
}
