using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using GhostYak.IO.DeviceIOControl.Objects.Disk;
using GhostYak.IO.DeviceIOControl.Objects.Enums;

namespace GhostYak.IO.RawDiskDrive
{
    public static class Win32Native
    {
        public const short FILE_ATTRIBUTE_NORMAL = 0x80;
        public const short INVALID_HANDLE_VALUE = -1;
        

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int QueryDosDevice([In] string lpDeviceName, [Out] StringBuilder lpTargetPath, [In] int ucchMax);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
           string lpFileName,
           [MarshalAs(UnmanagedType.U4)] FileAccessEx dwDesiredAccess,
           [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
           IntPtr lpSecurityAttributes,
           [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
           [MarshalAs(UnmanagedType.U4)] FileAttributesEx dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
           string lpFileName,
           [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
           [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
           IntPtr lpSecurityAttributes,
           [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
           [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        //[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        //internal static extern SafeFileHandle CreateFile(
        //    [In] LPCSTR fileName,
        //    [In] DesiredAccess desiredAccess,
        //    [In] ShareMode shareMode,
        //    [In] LPSECURITY_ATTRIBUTES securityAttributes,
        //    [In] CreationDisposition creationDisposition,
        //    [In] FlagsAndAttributes flagsAndAttributes,
        //    [In] HANDLE hTemplateFile);
        public const uint INVALID_SET_FILE_POINTER = 0xFFFFFFFF;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint SetFilePointer(
            [In] SafeFileHandle hFile,
            [In] int lDistanceToMove,
            [In, Out] ref int lpDistanceToMoveHigh,
            [In] SeekOrigin dwMoveMethod);

        [DllImport("kernel32.dll")]
        public static extern bool SetFilePointerEx(
            [In] SafeFileHandle hFile, 
            [In] long liDistanceToMove,
            [In, Out] ref long lpNewFilePointer,
            [In] SeekOrigin dwMoveMethod);

        [DllImport("kernel32.dll")]
        static extern bool SetFilePointerEx(
            IntPtr hFile,
            long liDistanceToMove,
            IntPtr lpNewFilePointer,
            uint dwMoveMethod);



        /// <summary>
        /// IoControlCode는 IoCtl class 참조
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="IoControlCode">IoCtl class</param>
        /// <param name="InBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="pBytesReturned"></param>
        /// <param name="Overlapped"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint IoControlCode,
            IntPtr InBuffer,
            uint nInBufferSize,
            IntPtr OutBuffer,
            uint nOutBufferSize,
            out uint pBytesReturned,
            IntPtr Overlapped);

        /// <summary>
        /// IoControlCode는 IoCtl class 참조
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="IoControlCode">IoCtl class</param>
        /// <param name="InBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="OutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="pBytesReturned"></param>
        /// <param name="Overlapped"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint IoControlCode,
            [In, Out]SENDCMDINPARAMS InBuffer,
            uint nInBufferSize,
            [In, Out]SENDCMDOUTPARAMS OutBuffer,
            uint nOutBufferSize,
            ref uint pBytesReturned,
            IntPtr Overlapped);

        //[DllImport("Kernel32.dll", SetLastError = true)]
        //public static extern bool DeviceIoControl(
        //    [In] SafeFileHandle hDevice,
        //    [In] DWORD IoControlCode,
        //    [In, Optional]  LPVOID inBuffer,
        //    [In] DWORD inBufferSize,
        //    [Out, Optional] LPVOID outBuffer,
        //    [In] DWORD outBufferSize,
        //    [Out] out DWORD bytesReturned,
        //    [In] LPOVERLAPPED overlapped);

        //[StructLayout(LayoutKind.Sequential)]
        //internal struct DiskGeometry
        //{
        //    public long Cylinders;
        //    public int MediaType;
        //    public int TracksPerCylinder;
        //    public int SectorsPerTrack;
        //    public int BytesPerSector;
        //}

        internal const uint FileAccessGenericRead = 0x80000000;
        internal const uint FileShareWrite = 0x2;
        internal const uint FileShareRead = 0x1;
        internal const uint CreationDispositionOpenExisting = 0x3;
        internal const uint IoCtlDiskGetDriveGeometry = 0x70000;

        

        [DllImport("kernel32", SetLastError = true)]
        internal extern static bool ReadFile(SafeFileHandle handle, byte[] bytes, int numBytesToRead, out int numBytesRead, IntPtr overlapped_MustBeZero);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal unsafe static extern int ReadFile(SafeFileHandle handle, byte* bytes, int numBytesToRead, out int numBytesRead, IntPtr mustBeZero);

    }
}
