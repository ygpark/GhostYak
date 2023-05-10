using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SENDCMDOUTPARAMS
    {
        public uint BufferSize;
        public DRIVERSTATUS Status;
        public IDSECTOR IDS;
    }
}
