using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STORAGE_DESCRIPTOR_HEADER
    {
        public uint Version;
        public uint Size;
    }
}
