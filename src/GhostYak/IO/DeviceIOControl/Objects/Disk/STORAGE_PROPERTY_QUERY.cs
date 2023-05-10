using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STORAGE_PROPERTY_QUERY
    {
        public STORAGE_PROPERTY_ID PropertyId;
        public STORAGE_QUERY_TYPE QueryType;
        public byte[] AdditionalParameters;
    }
}
