using System.Runtime.InteropServices;
using GhostYak.IO.DeviceIOControl.Objects.Enums;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PARTITION_INFORMATION_EX
    {
        [MarshalAs(UnmanagedType.U4)]
        public PartitionStyle PartitionStyle;
        public long StartingOffset;
        public long PartitionLength;
        public int PartitionNumber;
        public bool RewritePartition;
        public PARTITION_INFORMATION_UNION DriveLayoutInformaiton;
    }
}