using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.FileSystem
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RETRIEVAL_POINTERS_BUFFER
    {
        public ulong ExtentCount;
        public ulong StartingVcn;
        public RETRIEVAL_POINTERS_EXTENT[] Extents;
    }
}