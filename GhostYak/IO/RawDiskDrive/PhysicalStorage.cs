using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Management;
using System.IO;
using System.Diagnostics;
using GhostYak.IO.DeviceIOControl.Wrapper;
using GhostYak.IO.DeviceIOControl.Objects.Disk;
using Microsoft.Win32.SafeHandles;


namespace GhostYak.IO.RawDiskDrive
{
    public class PhysicalStorage : StorageBase
    {
        private string _interface;

        private DISK_GEOMETRY_EX _diskGeometryEx;
        private STORAGE_DEVICE_DESCRIPTOR _storage_device_descriptor;

        private int _Number;
        /// <summary>
        /// 드라이브 넘버
        /// </summary>
        public int Number { get => _Number; }

        private List<LogicalStorage> _Volumes = new List<LogicalStorage>();
        public List<LogicalStorage> Volumes { get => _Volumes; }

        public int SectorsPerTrack { get => _diskGeometryEx.Geometry.SectorsPerTrack; }
        public long Cylinder { get => _diskGeometryEx.Geometry.Cylinders; }
        public MEDIA_TYPE MediaType { get => _diskGeometryEx.Geometry.MediaType; }

        private string _SerialNumber;
        public string SerialNumber { get => _SerialNumber; }
        private string _ModelNumber;
        public string ModelNumber { get => _ModelNumber; }
        public string Interface { get => _interface; }
        public string BusType { get => _storage_device_descriptor.BusTypeToString; }

        public override string NameForDisplay { get => $"디스크 {Number}"; }



        /// <summary>
        /// 물리 드라이브와 연결된 파일명(\\.\PhysicalDrive#)으로 PhysicalDrive를 초기화한다. 
        /// </summary>
        /// <see cref="List<string> PhysicalDrive.GetPhysicalDriveNames()"/>
        /// <param name="path"></param>
        public PhysicalStorage(string path) : base(path)
        {
            _Number = Convert.ToInt32(path.Replace(@"\\.\PHYSICALDRIVE", ""));
            Init();
        }

        /// <summary>
        /// 물리 드라이브 넘버로 PhysicalDrive를 초기화한다.
        /// </summary>
        /// <param name="Number">물리 드라이브 넘버. \\.\PhysicalDrive{#Number} 에서 숫자(Number)</param>
        public PhysicalStorage(int Number) : base(string.Format(@"\\.\PHYSICALDRIVE{0}", Number))
        {
            this._Number = Number;
            Init();
        }

        private void Init()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>().OrderBy(obj => obj["DeviceID"]))
            {
                if(queryObj["DeviceID"].ToString() == Path)
                {
                    _interface = queryObj["InterfaceType"].ToString().Trim();
                    _ModelNumber = queryObj["Model"].ToString().Trim().Replace(" SCSI Disk Device", "").Replace(" USB Device", "");
                    _SerialNumber = queryObj["SerialNumber"].ToString().Trim();
                    break;
                }
            }

            SafeFileHandle handle = Win32Native.CreateFile(Path, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            using (DiskDeviceWrapper diskIo = new DiskDeviceWrapper(handle, true))
            {
                using (StorageDeviceWrapper storageio = new StorageDeviceWrapper(handle, false))
                {
                    try
                    {
                        _diskGeometryEx = diskIo.DiskGetDriveGeometryEx();
                        Size = _diskGeometryEx.DiskSize;
                        BytesPerSector = _diskGeometryEx.Geometry.BytesPerSector;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }

                    try
                    {
                        _storage_device_descriptor = storageio.StorageQueryProperty();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }

            handle.Close();
        }



        /// <summary> 
        /// Returns a list of physical drive IDs 
        /// </summary> 
        /// <returns> 
        /// List<string>: Device IDs of all connected physical hard drives 
        ///  </returns> 
        public static List<string> GetPhysicalDriveNames()
        {
            List<string> drivelist = new List<string>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>().OrderBy(obj => obj["DeviceID"]))
                {
                    drivelist.Add(queryObj["DeviceID"].ToString());
                }
            }
            catch (ManagementException)
            {
                return null;
            }
            return drivelist;
        }

        public static PhysicalStorage[] GetPhysicalDrives()
        {
            List<string> names = PhysicalStorage.GetPhysicalDriveNames();
            PhysicalStorage[] pds = new PhysicalStorage[names.Count];
            for(int i =0; i < names.Count; i++)
            {
                pds[i] = new PhysicalStorage(names[i]);
            }
            return pds;
        }

        

        //TODO
        public override string ToString()
        {
            string sizeByte = string.Format("{0:#,##0}byte", this.Size);

            string rtn = string.Format($"{this.NameForDisplay}, Model: {this.ModelNumber}, " +
                $"Size: {this.SizeHumanReadable}({sizeByte}), " +
                $"SerialNumber: {this.SerialNumber}, " +
                $"MediaType:{this.MediaType}, " +
                $"Interface: {this.Interface}, " +
                $"BusType: {this.BusType}, " +
                $"BytesPerSector:{this.BytesPerSector}, " +
                $"Cylinder:{this.Cylinder}");

            return rtn;
        }

        public override Stream OpenRead()
        {
            return new PhysicalStream(Path);
        }
    }
}
