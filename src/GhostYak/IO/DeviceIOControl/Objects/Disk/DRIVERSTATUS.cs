using System;
using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential, Size = 12)]
    public struct DRIVERSTATUS
    {
        public byte bDriveError;
        public byte bIDEError;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] Reserved2;
    }
}
