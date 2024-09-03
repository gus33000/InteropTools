using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PhotoshoppedUUPCLI
{
    [StructLayout(LayoutKind.Sequential)]
    public class CabinetInfo //Cabinet API: "FDCABINETINFO"
    {
        public int cbCabinet;
        public short cFolders;
        public short cFiles;
        public short setID;
        public short iCabinet;
        public int fReserve;
        public int hasprev;
        public int hasnext;
    }

    public class CabExtract : IDisposable
    {
        //If any of these classes end up with a different size to its C equivilent, we end up with crash and burn.
        [StructLayout(LayoutKind.Sequential)]
        private class CabError //Cabinet API: "ERF"
        {
            public int erfOper;
            public int erfType;
            public int fError;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private class FdiNotification //Cabinet API: "FDINOTIFICATION"
        {
            public int cb;
            public string psz1;
            public string psz2;
            public string psz3;
            public IntPtr userData;
            public IntPtr hf;
            public short date;
            public short time;
            public short attribs;
            public short setID;
            public short iCabinet;
            public short iFolder;
            public int fdie;
        }

        private enum FdiNotificationType
        {
            CabinetInfo,
            PartialFile,
            CopyFile,
            CloseFileInfo,
            NextCabinet,
            Enumerate
        }

        private class DecompressFile
        {
            public IntPtr Handle { get; set; }
            public string Name { get; set; }
            public bool Found { get; set; }
            public int Length { get; set; }
            public byte[] Data { get; set; }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr FdiMemAllocDelegate(int numBytes);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FdiMemFreeDelegate(IntPtr mem);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr FdiFileOpenDelegate(string fileName, int oflag, int pmode);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int32 FdiFileReadDelegate(IntPtr hf,
                                                  [In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2,
                                                  ArraySubType = UnmanagedType.U1)] byte[] buffer, int cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int32 FdiFileWriteDelegate(IntPtr hf,
                                                   [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2,
                                                   ArraySubType = UnmanagedType.U1)] byte[] buffer, int cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int32 FdiFileCloseDelegate(IntPtr hf);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int32 FdiFileSeekDelegate(IntPtr hf, int dist, int seektype);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr FdiNotifyDelegate(
            FdiNotificationType fdint, [In] [MarshalAs(UnmanagedType.LPStruct)] FdiNotification fdin);

        [DllImport("cabinet.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FDICreate", CharSet = CharSet.Ansi)]
        private static extern IntPtr FdiCreate(
            FdiMemAllocDelegate fnMemAlloc,
            FdiMemFreeDelegate fnMemFree,
            FdiFileOpenDelegate fnFileOpen,
            FdiFileReadDelegate fnFileRead,
            FdiFileWriteDelegate fnFileWrite,
            FdiFileCloseDelegate fnFileClose,
            FdiFileSeekDelegate fnFileSeek,
            int cpuType,
            [MarshalAs(UnmanagedType.LPStruct)] CabError erf);

        [DllImport("cabinet.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FDIIsCabinet", CharSet = CharSet.Ansi)]
        private static extern bool FdiIsCabinet(
            IntPtr hfdi,
            IntPtr hf,
            [MarshalAs(UnmanagedType.LPStruct)] CabinetInfo cabInfo);

        [DllImport("cabinet.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FDIDestroy", CharSet = CharSet.Ansi)]
        private static extern bool FdiDestroy(IntPtr hfdi);

        [DllImport("cabinet.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FDICopy", CharSet = CharSet.Ansi)]
        private static extern bool FdiCopy(
            IntPtr hfdi,
            string cabinetName,
            string cabinetPath,
            int flags,
            FdiNotifyDelegate fnNotify,
            IntPtr fnDecrypt,
            IntPtr userData);

        private readonly FdiFileCloseDelegate _fileCloseDelegate;
        private readonly FdiFileOpenDelegate _fileOpenDelegate;
        private readonly FdiFileReadDelegate _fileReadDelegate;
        private readonly FdiFileSeekDelegate _fileSeekDelegate;
        private readonly FdiFileWriteDelegate _fileWriteDelegate;
        private readonly FdiMemAllocDelegate _femAllocDelegate;
        private readonly FdiMemFreeDelegate _memFreeDelegate;

        private readonly CabError _erf;
        private readonly List<DecompressFile> _decompressFiles;
        private readonly byte[] _inputData;
        private IntPtr _hfdi;
        private bool _disposed;
        private const int CpuTypeUnknown = -1;

        public CabExtract(byte[] inputData)
        {
            _fileReadDelegate = FileRead;
            _fileOpenDelegate = InputFileOpen;
            _femAllocDelegate = MemAlloc;
            _fileSeekDelegate = FileSeek;
            _memFreeDelegate = MemFree;
            _fileWriteDelegate = FileWrite;
            _fileCloseDelegate = InputFileClose;
            _inputData = inputData;
            _decompressFiles = new List<DecompressFile>();
            _erf = new CabError();
            _hfdi = IntPtr.Zero;
        }

        private static IntPtr FdiCreate(
            FdiMemAllocDelegate fnMemAlloc,
            FdiMemFreeDelegate fnMemFree,
            FdiFileOpenDelegate fnFileOpen,
            FdiFileReadDelegate fnFileRead,
            FdiFileWriteDelegate fnFileWrite,
            FdiFileCloseDelegate fnFileClose,
            FdiFileSeekDelegate fnFileSeek,
            CabError erf)
        {
            return FdiCreate(fnMemAlloc, fnMemFree, fnFileOpen, fnFileRead, fnFileWrite,
                             fnFileClose, fnFileSeek, CpuTypeUnknown, erf);
        }

        private static bool FdiCopy(
            IntPtr hfdi,
            FdiNotifyDelegate fnNotify)
        {
            return FdiCopy(hfdi, "<notused>", "<notused>", 0, fnNotify, IntPtr.Zero, IntPtr.Zero);
        }

        private IntPtr FdiContext
        {
            get
            {
                if (_hfdi == IntPtr.Zero)
                {
                    _hfdi = FdiCreate(_femAllocDelegate, _memFreeDelegate, _fileOpenDelegate, _fileReadDelegate, _fileWriteDelegate, _fileCloseDelegate, _fileSeekDelegate, _erf);
                    if (_hfdi == IntPtr.Zero)
                        throw new Exception("Failed to create FDI context.");
                }
                return _hfdi;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_hfdi != IntPtr.Zero)
                {
                    FdiDestroy(_hfdi);
                    _hfdi = IntPtr.Zero;
                }
                _disposed = true;
            }
        }

        private IntPtr NotifyCallback(FdiNotificationType fdint, FdiNotification fdin)
        {
            switch (fdint)
            {
                case FdiNotificationType.CopyFile:
                    return OutputFileOpen(fdin);
                case FdiNotificationType.CloseFileInfo:
                    return OutputFileClose(fdin);
                default:
                    return IntPtr.Zero;
            }
        }

        private IntPtr InputFileOpen(string fileName, int oflag, int pmode)
        {
            var stream = new MemoryStream(_inputData);
            GCHandle gch = GCHandle.Alloc(stream);
            return (IntPtr)gch;
        }

        private int InputFileClose(IntPtr hf)
        {
            var stream = StreamFromHandle(hf);
            stream.Dispose();
            ((GCHandle)(hf)).Free();
            return 0;
        }

        private IntPtr OutputFileOpen(FdiNotification fdin)
        {
            var extractFile = _decompressFiles.Where(ef => ef.Name == fdin.psz1).SingleOrDefault();

            if (extractFile != null)
            {
                var stream = new MemoryStream();
                GCHandle gch = GCHandle.Alloc(stream);
                extractFile.Handle = (IntPtr)gch;
                return extractFile.Handle;
            }

            //Don't extract
            return IntPtr.Zero;
        }

        private IntPtr OutputFileClose(FdiNotification fdin)
        {
            var extractFile = _decompressFiles.Where(ef => ef.Handle == fdin.hf).Single();
            var stream = StreamFromHandle(fdin.hf);

            extractFile.Found = true;
            extractFile.Length = (int)stream.Length;

            if (stream.Length > 0)
            {
                extractFile.Data = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(extractFile.Data, 0, (int)stream.Length);
            }

            stream.Dispose();
            return IntPtr.Zero;
        }

        private int FileRead(IntPtr hf, byte[] buffer, int cb)
        {
            var stream = StreamFromHandle(hf);
            return stream.Read(buffer, 0, cb);
        }

        private int FileWrite(IntPtr hf, byte[] buffer, int cb)
        {
            var stream = StreamFromHandle(hf);
            stream.Write(buffer, 0, cb);
            return cb;
        }

        private static Stream StreamFromHandle(IntPtr hf)
        {
            return (Stream)((GCHandle)hf).Target;
        }

        private IntPtr MemAlloc(int cb)
        {
            return Marshal.AllocHGlobal(cb);
        }

        private void MemFree(IntPtr mem)
        {
            Marshal.FreeHGlobal(mem);
        }

        private int FileSeek(IntPtr hf, int dist, int seektype)
        {
            var stream = StreamFromHandle(hf);
            return (int)stream.Seek(dist, (SeekOrigin)seektype);
        }

        public bool ExtractFile(string fileName, out byte[] outputData, out int outputLength)
        {
            if (_disposed)
                throw new ObjectDisposedException("CabExtract");

            var fileToDecompress = new DecompressFile();
            fileToDecompress.Found = false;
            fileToDecompress.Name = fileName;

            _decompressFiles.Add(fileToDecompress);

            FdiCopy(FdiContext, NotifyCallback);

            if (fileToDecompress.Found)
            {
                outputData = fileToDecompress.Data;
                outputLength = fileToDecompress.Length;
                _decompressFiles.Remove(fileToDecompress);
                return true;
            }

            outputData = null;
            outputLength = 0;
            return false;
        }

        public bool IsCabinetFile(out CabinetInfo cabinfo)
        {
            if (_disposed)
                throw new ObjectDisposedException("CabExtract");

            var stream = new MemoryStream(_inputData);
            GCHandle gch = GCHandle.Alloc(stream);

            try
            {
                var info = new CabinetInfo();
                var ret = FdiIsCabinet(FdiContext, (IntPtr)gch, info);
                cabinfo = info;
                return ret;
            }
            finally
            {
                stream.Dispose();
                gch.Free();
            }
        }

        public static bool IsCabinetFile(byte[] inputData, out CabinetInfo cabinfo)
        {
            using (var decomp = new CabExtract(inputData))
            {
                return decomp.IsCabinetFile(out cabinfo);
            }
        }

        //In an ideal world, this would take a stream, but Cabinet.dll seems to want to open the input several times.
        public static bool ExtractFile(byte[] inputData, string fileName, out byte[] outputData, out int length)
        {
            using (var decomp = new CabExtract(inputData))
            {
                return decomp.ExtractFile(fileName, out outputData, out length);
            }
        }

        //TODO: Add methods for enumerating/extracting multiple files
    }
}
