using System;
using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential, Size = 49)]
    public class SENDCMDINPARAMS
    {
        public int BufferSize;       // 4
        public IDEREGS irDriveRegs;      // 8
        public byte DriveNumber;      // 1

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] bReserved = new byte[3];     // 3

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int64[] dwReserved = new Int64[4];     // 4 * 8 = 32

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] bBuffer = new byte[1];     // 1
    }
}
