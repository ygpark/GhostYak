using GhostYak.IO.DeviceIOControl.Objects.Enums;
using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STORAGE_DEVICE_NUMBER
    {
        /// <summary>
        /// DeviceIOControlLib.Objects.Enums.IOFileDevice
        /// </summary>
        public IOFileDevice DeviceType;
        public int DeviceNumber;
        public int PartitionNumber;
        public string DeviceTypeString { get => string.Format("{0}", (IOFileDevice)DeviceType); }
        public override string ToString()
        {
            return string.Format($"DeviceType: {DeviceTypeString}, DeviceNumber: {DeviceNumber}, PartitionNumber: {PartitionNumber}");
        }
    }
}
