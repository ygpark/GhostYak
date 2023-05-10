using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.FileSystem
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RETRIEVAL_POINTERS_EXTENT
    {
        public ulong NextVcn;
        public ulong Lcn;
    }
}