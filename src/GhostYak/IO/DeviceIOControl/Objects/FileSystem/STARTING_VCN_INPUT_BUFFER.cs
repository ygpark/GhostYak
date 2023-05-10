using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.FileSystem
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STARTING_VCN_INPUT_BUFFER
    {
        public ulong StartingVcn;
    }
}