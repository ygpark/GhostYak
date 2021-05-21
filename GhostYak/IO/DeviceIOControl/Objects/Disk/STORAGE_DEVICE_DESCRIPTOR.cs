using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STORAGE_DEVICE_DESCRIPTOR
    {
        public uint Version;
        public uint Size;
        public byte DeviceType;
        public byte DeviceTypeModifier;
        [MarshalAs(UnmanagedType.U1)]
        public bool RemovableMedia;
        [MarshalAs(UnmanagedType.U1)]
        public bool CommandQueueing;
        public uint VendorIdOffset;
        public uint ProductIdOffset;
        public uint ProductRevisionOffset;
        public uint SerialNumberOffset;
        public STORAGE_BUS_TYPE BusType;
        public uint RawPropertiesLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x16)]
        public byte[] RawDeviceProperties;
        public string BusTypeToString
        {
            get
            {
                string rtn = string.Empty;
                switch (BusType)
                {
                    case STORAGE_BUS_TYPE.BusTypeUnknown:
                        rtn = "Unknown";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeScsi:
                        rtn = "SCSI";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeAtapi:
                        rtn = "ATAPI";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeAta:
                        rtn = "ATA";
                        break;

                    case STORAGE_BUS_TYPE.BusType1394:
                        rtn = "1394";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeSsa:
                        rtn = "SSA";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeFibre:
                        rtn = "FIBRE";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeUsb:
                        rtn = "USB";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeRAID:
                        rtn = "RAID";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeiScsi:
                        rtn = "ISCSI";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeSas:
                        rtn = "SAS";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeSata:
                        rtn = "SATA";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeSd:
                        rtn = "SD";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeMmc:
                        rtn = "MMC";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeVirtual:
                        rtn = "Virtual";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeFileBackedVirtual:
                        rtn = "FileBackedVirtual";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeSpaces:
                        rtn = "Spaces";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeNvme:
                        rtn = "Nvme";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeSCM:
                        rtn = "SCM";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeUfs:
                        rtn = "Ufs";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeMax:
                        rtn = "Max";
                        break;
                    case STORAGE_BUS_TYPE.BusTypeMaxReserved:
                        rtn = "MaxReserved";
                        break;
                    default:
                        throw new System.Exception("Unknown Bus Type");
                }
                return rtn;
            }
            
        }
    }
}
