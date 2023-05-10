using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GET_LENGTH_INFORMATION
    {
        [MarshalAs(UnmanagedType.I8)]
        public long Length;
    }
}