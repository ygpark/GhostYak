using System.Runtime.InteropServices;
using GhostYak.IO.DeviceIOControl.Objects.Enums;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DRIVE_LAYOUT_INFORMATION_EX_INTERNAL
    {
        [MarshalAs(UnmanagedType.U4)]
        public PartitionStyle PartitionStyle;
        public int PartitionCount;
        public DRIVE_LAYOUT_INFORMATION_UNION DriveLayoutInformaiton;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 128)]
        public PARTITION_INFORMATION_EX[] PartitionEntry;
    }
}