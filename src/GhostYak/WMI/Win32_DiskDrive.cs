using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GhostYak.WMI
{
    public class Win32_DiskDrive
    {
        public UInt16 Availability;

        public UInt32 BytesPerSector;

        public string Caption;
        public string CompressionMethod;
        public UInt32 ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;

        public UInt64 DefaultBlockSize;
        public string Description;
        public string DeviceID;

        public bool ErrorCleared;
        public string ErrorDescription;
        public string ErrorMethodology;

        public string FirmwareRevision;

        public UInt32 Index;
        public string InterfaceType;

        public UInt32 LastErrorCode;

        public string Manufacturer;
        public UInt64 MaxBlockSize;
        public UInt64 MaxMediaSize;
        public bool MediaLoaded;
        public string MediaType;
        public UInt64 MinBlockSize;
        public string Model;

        public string Name;
        public bool NeedsCleaning;
        public UInt32 NumberOfMediaSupported;

        public UInt32 Partitions;
        public string PNPDeviceID;
        public bool PowerManagementSupported;

        public UInt32 SCSIBus;
        public UInt16 SCSILogicalUnit;
        public UInt16 SCSIPort;
        public UInt16 SCSITargetId;
        public UInt32 SectorsPerTrack;
        public string SerialNumber;
        public UInt32 Signature;
        public UInt64 Size;
        public string Status;
        public UInt16 StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;

        public UInt64 TotalCylinders;
        public UInt32 TotalHeads;
        public UInt64 TotalSectors;
        public UInt64 TotalTracks;
        public UInt32 TracksPerCylinder;
        //public UInt16 Capabilities[];
        //public string CapabilityDescriptions[];
        //public string InstallDate;
        //public UInt16 PowerManagementCapabilities[];

        public string ToShortString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Description}{Index}, {Caption}, {SerialNumber}, ({GetHumanReadableDiskSize(Size)}, {InterfaceType})");
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Win32_DiskDrive {");

            sb.AppendLine($"Name={Name}");

            sb.AppendLine($"BytesPerSector={BytesPerSector.ToString()}");

            sb.AppendLine($"Caption={Caption}");
            sb.AppendLine($"CompressionMethod={CompressionMethod}");
            sb.AppendLine($"ConfigManagerErrorCode={ConfigManagerErrorCode.ToString()}");
            sb.AppendLine($"ConfigManagerUserConfig={ConfigManagerUserConfig.ToString()}");
            sb.AppendLine($"CreationClassName={CreationClassName}");

            sb.AppendLine($"DefaultBlockSize={DefaultBlockSize.ToString()}");
            sb.AppendLine($"Description={Description}");
            sb.AppendLine($"DeviceID={DeviceID}");

            sb.AppendLine($"ErrorCleared={ErrorCleared.ToString()}");
            sb.AppendLine($"ErrorDescription={ErrorDescription}");
            sb.AppendLine($"ErrorMethodology={ErrorMethodology}");

            sb.AppendLine($"FirmwareRevision={FirmwareRevision}");

            sb.AppendLine($"Index={Index.ToString()}");
            sb.AppendLine($"InterfaceType={InterfaceType}");

            sb.AppendLine($"LastErrorCode={LastErrorCode.ToString()}");

            sb.AppendLine($"Manufacturer={Manufacturer}");
            sb.AppendLine($"MaxBlockSize={MaxBlockSize.ToString()}");
            sb.AppendLine($"MaxMediaSize={MaxMediaSize.ToString()}");
            sb.AppendLine($"MediaLoaded={MediaLoaded.ToString()}");
            sb.AppendLine($"MediaType={MediaType}");
            sb.AppendLine($"MinBlockSize={MinBlockSize.ToString()}");
            sb.AppendLine($"Model={Model}");

            sb.AppendLine($"NeedsCleaning={NeedsCleaning.ToString()}");
            sb.AppendLine($"NumberOfMediaSupported={NumberOfMediaSupported.ToString()}");

            sb.AppendLine($"Partitions={Partitions.ToString()}");
            sb.AppendLine($"PNPDeviceID={PNPDeviceID}");
            sb.AppendLine($"PowerManagementSupported={PowerManagementSupported.ToString()}");

            sb.AppendLine($"SCSIBus={SCSIBus.ToString()}");
            sb.AppendLine($"SCSILogicalUnit={SCSILogicalUnit.ToString()}");
            sb.AppendLine($"SCSIPort={SCSIPort.ToString()}");
            sb.AppendLine($"SCSITargetId={SCSITargetId.ToString()}");
            sb.AppendLine($"SectorsPerTrack={SectorsPerTrack.ToString()}");
            sb.AppendLine($"SerialNumber={SerialNumber}");
            sb.AppendLine($"Signature={Signature.ToString()}");
            sb.AppendLine($"Size={Size.ToString()}");
            sb.AppendLine($"Status={Status}");
            sb.AppendLine($"StatusInfo={StatusInfo.ToString()}");
            sb.AppendLine($"SystemCreationClassName={SystemCreationClassName}");
            sb.AppendLine($"SystemName={SystemName}");

            sb.AppendLine($"TotalCylinders={TotalCylinders.ToString()}");
            sb.AppendLine($"TotalHeads={TotalHeads.ToString()}");
            sb.AppendLine($"TotalSectors={TotalSectors.ToString()}");
            sb.AppendLine($"TotalTracks={TotalTracks.ToString()}");
            sb.AppendLine($"TracksPerCylinder={TracksPerCylinder.ToString()}");
            sb.AppendLine("}");
            //sb.AppendLine($"InstallDate={InstallDate.ToString()}");
            //sb.AppendLine({PowerManagementCapabilities[]}");
            return sb.ToString();
        }

        static string GetHumanReadableDiskSize(ulong byteSize)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int idx = 0;
            double dHumanReadableSize = byteSize;

            while (dHumanReadableSize >= 1024 && idx < units.Length - 1)
            {
                dHumanReadableSize /= 1024;
                idx++;
            }

            string sizeStr = dHumanReadableSize.ToString(idx > 0 ? "#.##" : "#");
            return string.Format("{0}{1}", sizeStr, units[idx]);
        }
    }
}
