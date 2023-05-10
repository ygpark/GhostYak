using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    //https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/content/ntdddisk/ns-ntdddisk-_sendcmdinparams

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct IDEREGS
    {
        public byte Features;
        public byte SectorCount;
        public byte SectorNumber;
        public byte CylinderLow;
        public byte CylinderHigh;
        public byte DriveHead;
        public IDEREGS_COMMAND Command;
        public byte Reserved;
    }

    //  Valid values for the bCommandReg member of IDEREGS.
    public enum IDEREGS_COMMAND : byte
    {
        IDE_ATAPI_IDENTIFY = 0xA1,  //  Returns ID sector for ATAPI.
        IDE_ATA_IDENTIFY = 0xEC     //  Returns ID sector for ATA.
    }
}
