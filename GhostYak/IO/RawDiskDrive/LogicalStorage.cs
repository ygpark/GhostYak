using GhostYak.IO.DeviceIOControl.Objects.Disk;
using GhostYak.IO.DeviceIOControl.Objects.Enums;
using GhostYak.IO.DeviceIOControl.Wrapper;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace GhostYak.IO.RawDiskDrive
{
    public class LogicalStorage :StorageBase
    {
        private string _volumeLabel;

        /// <summary>
        /// 볼륨 이름 ( C:\ )
        /// </summary>
        public string VolumeName { get; }
        public string DriveLetter { get => VolumeName.Replace("\\",""); }
        
        /// <summary>
        /// 볼륨 라벨
        /// </summary>
        public string VolumeLabel {
            get
            {
                if (this.DriveType == "Fixed" && this._volumeLabel == "")
                {
                    return "로컬 디스크";
                }
                else
                {
                    return _volumeLabel;
                }
            }
            set
            {
                _volumeLabel = value;
            }
        }
        /// <summary>
        /// 파티션 정보
        /// </summary>
        public string DriveFormat { get; }
        public string DriveType { get; private set; }
        public int PhysicalDrvieNumber { get; private set; } = -1;
        public DriveInfo DriveInfo { get; }

        public override string NameForDisplay
        {
            get
            {
                return $"{DriveLetter} ({VolumeLabel})";
            }
        }

        public string TotalFreeSpaceHumanReadable { get => this.GetHumanReadableSize(this.DriveInfo.TotalFreeSpace); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">\\.\C:</param>
        public LogicalStorage(string path) : base(path)
        {

            DriveInfo = GetDriveInfoByPath(path);
            this.VolumeName = DriveInfo.Name;
            this.DriveType = string.Format("{0}", DriveInfo.DriveType);
            if (DriveInfo.IsReady)
            {
                this.VolumeLabel = DriveInfo.VolumeLabel;
                this.DriveFormat = DriveInfo.DriveFormat;
            }

            if (DriveInfo.IsReady)
            {
                Size = DriveInfo.TotalSize;
                this.VolumeLabel = DriveInfo.VolumeLabel;
            }
            
            Init();
        }

        private void Init()
        {
            string sBytePerSector = GetPhysicalDiskInfoByLogicalDrive(DriveLetter, "BytesPerSector");
            BytesPerSector = (sBytePerSector == "") ? 0 : int.Parse(sBytePerSector);
        }

        /// <summary>
        /// 드라이버 레터(C:)로 물리 디스크(WMI WIN32_DISKDRIVE) 정보를 얻어온다.
        /// </summary>
        /// <param name="letter">C:</param>
        /// <param name="win32_diskDrive_property"></param>
        /// <returns></returns>
        public string GetPhysicalDiskInfoByLogicalDrive(string letter, string win32_diskDrive_property)
        {
            string value = "";
            using (var m1 = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + letter + "'} WHERE ResultClass=Win32_DiskPartition"))
            {
                foreach (var i1 in m1.Get())
                {
                    using (var m2 = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + i1["DeviceID"] + "'} WHERE ResultClass=Win32_DiskDrive"))
                    {
                        foreach (var i2 in m2.Get())
                        {
                            value = i2[win32_diskDrive_property].ToString();
                            break;
                        }
                    }
                    break;
                }
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DriveInfo GetDriveInfoByPath(string name)
        {
            DriveInfo[] items = DriveInfo.GetDrives();
            foreach (DriveInfo item in items)
            {
                if (name.Contains(item.Name.Replace("\\","")))
                    return item;
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format($"Name: {Path}, VolumeName: {VolumeName}, DriveType: {DriveType}, VolumeLabel: {VolumeLabel}, Size: {Size}, BytesPerSector: {BytesPerSector}");
        }

        /// <summary>
        /// 논리 드라이브({\\.\C:}, {\\.\D:}) 경로를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLogicalDriveNames()
        {
            List<string> logicalDriveNames = new List<string>();
            DriveInfo[] items = DriveInfo.GetDrives();
            foreach(DriveInfo item in items)
            {
                logicalDriveNames.Add(string.Format(@"\\.\{0}", item.Name.Replace("\\", "")));
            }
            return logicalDriveNames;
        }

        public static DriveInfo[] GetDriveInfo()
        {
            return DriveInfo.GetDrives();
        }

        public static LogicalStorage[] GetLogicalDrives()
        {
            List<string> names = LogicalStorage.GetLogicalDriveNames();
            LogicalStorage[] lds = new LogicalStorage[names.Count];
            for(int i = 0; i < names.Count; i++)
            {
                lds[i] = new LogicalStorage(names[i]);
            }
            return lds;
        }

        public static string GetDriveInfoToString()
        {
            DriveInfo[] myDrives;
            StringBuilder sb = new StringBuilder();
            try
            {
                myDrives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in myDrives)
                {
                    sb.Append("Drive:" + drive.Name);
                    sb.Append("Drive Type:" + drive.DriveType);

                    if (drive.IsReady == true)
                    {
                        sb.Append("Vol Label:" + drive.VolumeLabel);
                        sb.Append("File System: " + drive.DriveFormat);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }

            return sb.ToString();
        }
    }
}
