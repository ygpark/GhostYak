using GhostYak.IO.DeviceIOControl.Objects.Enums;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    public struct DRIVE_LAYOUT_INFORMATION_EX
    {
        public PartitionStyle PartitionStyle;
        public int PartitionCount;
        public DRIVE_LAYOUT_INFORMATION_UNION DriveLayoutInformaiton;
        public PARTITION_INFORMATION_EX[] PartitionEntry;
    }
}