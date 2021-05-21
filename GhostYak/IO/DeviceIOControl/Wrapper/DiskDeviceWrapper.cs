using System;
using System.Runtime.InteropServices;
using GhostYak.IO.DeviceIOControl.Objects.Disk;
using GhostYak.IO.DeviceIOControl.Objects.Enums;
using GhostYak.IO.DeviceIOControl.Utilities;
using Microsoft.Win32.SafeHandles;

namespace GhostYak.IO.DeviceIOControl.Wrapper
{
    public class DiskDeviceWrapper : DeviceIoWrapperBase
    {
        public DiskDeviceWrapper(SafeFileHandle handle, bool ownsHandle = false)
            : base(handle, ownsHandle)
        {
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365169(v=vs.85).aspx"/></summary>
        public DISK_GEOMETRY DiskGetDriveGeometry()
        {
            return DeviceIoControlHelper.InvokeIoControl<DISK_GEOMETRY>(Handle, IOControlCode.DiskGetDriveGeometry);
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365171(v=vs.85).aspx"/></summary>
        public DISK_GEOMETRY_EX DiskGetDriveGeometryEx()
        {
            byte[] data = DeviceIoControlHelper.InvokeIoControlUnknownSize(Handle, IOControlCode.DiskGetDriveGeometryEx, 256);

            DISK_GEOMETRY_EX res;

            using (UnmanagedMemory mem = new UnmanagedMemory(data))
            {
                res.Geometry = (DISK_GEOMETRY)Marshal.PtrToStructure(mem, typeof(DISK_GEOMETRY));
                res.DiskSize = BitConverter.ToInt64(data, Marshal.SizeOf(typeof(DISK_GEOMETRY)));

                IntPtr tmpPtr = mem.Handle + Marshal.SizeOf(typeof(DISK_GEOMETRY)) + sizeof(long);
                res.PartitionInformation = (DISK_PARTITION_INFO)Marshal.PtrToStructure(tmpPtr, typeof(DISK_PARTITION_INFO));

                tmpPtr += res.PartitionInformation.SizeOfPartitionInfo;
                res.DiskInt13Info = (DISK_EX_INT13_INFO)Marshal.PtrToStructure(tmpPtr, typeof(DISK_EX_INT13_INFO));
            }

            return res;
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365179(v=vs.85).aspx"/></summary>
        public PARTITION_INFORMATION DiskGetPartitionInfo()
        {
            return DeviceIoControlHelper.InvokeIoControl<PARTITION_INFORMATION>(Handle, IOControlCode.DiskGetPartitionInfo);
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365180(v=vs.85).aspx"/></summary>
        public PARTITION_INFORMATION_EX DiskGetPartitionInfoEx()
        {
            return DeviceIoControlHelper.InvokeIoControl<PARTITION_INFORMATION_EX>(Handle, IOControlCode.DiskGetPartitionInfoEx);
        }

        //DiskSetPartitionInfo
        //DiskSetPartitionInfoEx

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365173(v=vs.85).aspx"/></summary>
        public DRIVE_LAYOUT_INFORMATION DiskGetDriveLayout()
        {
            DRIVE_LAYOUT_INFORMATION_INTERNAL data = DeviceIoControlHelper.InvokeIoControl<DRIVE_LAYOUT_INFORMATION_INTERNAL>(Handle, IOControlCode.DiskGetDriveLayout);

            DRIVE_LAYOUT_INFORMATION res = new DRIVE_LAYOUT_INFORMATION();

            res.PartitionCount = data.PartitionCount;
            res.Signature = data.Signature;
            res.PartitionEntry = new PARTITION_INFORMATION[res.PartitionCount];

            for (int i = 0; i < res.PartitionCount; i++)
                res.PartitionEntry[i] = data.PartitionEntry[i];

            return res;
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365174(v=vs.85).aspx"/></summary>
        public DRIVE_LAYOUT_INFORMATION_EX DiskGetDriveLayoutEx()
        {
            DRIVE_LAYOUT_INFORMATION_EX_INTERNAL data = DeviceIoControlHelper.InvokeIoControl<DRIVE_LAYOUT_INFORMATION_EX_INTERNAL>(Handle, IOControlCode.DiskGetDriveLayoutEx);

            DRIVE_LAYOUT_INFORMATION_EX res = new DRIVE_LAYOUT_INFORMATION_EX();

            res.PartitionStyle = data.PartitionStyle;
            res.PartitionCount = data.PartitionCount;
            res.DriveLayoutInformaiton = data.DriveLayoutInformaiton;
            res.PartitionEntry = new PARTITION_INFORMATION_EX[res.PartitionCount];

            for (int i = 0; i < res.PartitionCount; i++)
                res.PartitionEntry[i] = data.PartitionEntry[i];

            return res;
        }

        
        //DiskSetDriveLayout
        //DiskSetDriveLayoutEx
        //DiskVerify
        //DiskFormatTracks
        //DiskReassignBlocks
        //DiskPerformance
        //DiskIsWritable
        //DiskLogging
        //DiskFormatTracksEx
        //DiskHistogramStructure
        //DiskHistogramData
        //DiskHistogramReset
        //DiskRequestStructure
        //DiskRequestData
        //DiskControllerNumber

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/hardware/ff566202(v=vs.85).aspx"/></summary>
        public GETVERSIONINPARAMS DiskGetSmartVersion()
        {
            return DeviceIoControlHelper.InvokeIoControl<GETVERSIONINPARAMS>(Handle, IOControlCode.DiskSmartGetVersion);
        }


        //DiskSmartSendDriveCommand

        /// <summary>
        /// https://docs.microsoft.com/ko-kr/windows/desktop/api/winioctl/ni-winioctl-ioctl_storage_query_property
        /// </summary>
        /// <param name="driveName"></param>
        /// <param name="driveNumber"></param>
        /// <returns></returns>
        public SENDCMDOUTPARAMS DiskSmartRcvDriveData(string driveName, int driveNumber)
        {
            SENDCMDINPARAMS sci = new SENDCMDINPARAMS();
            SENDCMDOUTPARAMS sco = new SENDCMDOUTPARAMS();

            sci.DriveNumber = (byte)driveNumber;
            sci.BufferSize = Marshal.SizeOf(sco);
            // Compute the drive number.
            sci.irDriveRegs.DriveHead = (byte)(0xA0 | driveNumber << 4);
            sci.irDriveRegs.Command = IDEREGS_COMMAND.IDE_ATA_IDENTIFY;
            sci.irDriveRegs.SectorCount = 1;
            sci.irDriveRegs.SectorNumber = 1;

            return DeviceIoControlHelper.InvokeIoControl<SENDCMDOUTPARAMS, SENDCMDINPARAMS>(Handle, IOControlCode.DiskSmartRcvDriveData, sci);
        }
        //DiskUpdateDriveSize
        //DiskGrowPartition

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365165(v=vs.85).aspx"/></summary>
        public DISK_CACHE_INFORMATION DiskGetCacheInformation()
        {
            return DeviceIoControlHelper.InvokeIoControl<DISK_CACHE_INFORMATION>(Handle, IOControlCode.DiskGetCacheInformation);
        }

        //DiskSetCacheInformation
        //DiskDeleteDriveLayout
        //DiskFormatDrive
        //DiskSenseDevice
        //DiskCheckVerify
        //DiskMediaRemoval
        //DiskEjectMedia
        //DiskLoadMedia
        //DiskReserve
        //DiskRelease
        //DiskFindNewDevices
        //DiskCreateDisk

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365178(v=vs.85).aspx"/></summary>
        public long DiskGetLengthInfo()
        {
            return DeviceIoControlHelper.InvokeIoControl<GET_LENGTH_INFORMATION>(Handle, IOControlCode.DiskGetLengthInfo).Length;
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/hh706681(v=vs.85).aspx"/></summary>
        public GET_DISK_ATTRIBUTES DiskGetDiskAttributes()
        {
            return DeviceIoControlHelper.InvokeIoControl<GET_DISK_ATTRIBUTES>(Handle, IOControlCode.DiskGetDiskAttributes);
        }

        //DiskSetDiskAttributes
    }
}