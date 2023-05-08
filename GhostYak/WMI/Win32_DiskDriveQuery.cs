using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GhostYak.WMI
{
    /// <summary>
    /// Win32_DiskDrive 정보를 담는 Singleton 클래스
    /// </summary>
    /// 
    /// <example>
    ///     // Case #1 초기화
    ///     var query = Win32_DiskDrive_Query.Instance;
    ///     
    ///     // Case #2 필요한 경우 정보 갱신하기
    ///     query.Refresh();
    ///     
    ///     // Case #3 모든 디스크의 모든 정보 출력
    ///     for (int i = 0; i < query.Count; i++)
    ///     {
    ///       Console.WriteLine(query[i].ToString());
    ///     }
    ///     
    ///     // Case #4 Index 속성이 0인 디스크의 정보 가져오기
    ///     var win32_diskdrive = query.GetByDiskIndex(0);
    ///     
    ///     // Case #5 DeviceID 속성이 "\\.\PHYSICALDRIVE2"인 디스크의 정보 가져오기
    ///     var win32_diskdrive = query.GetByDeviceID("\\.\PHYSICALDRIVE2");
    ///     
    ///     // Case #6 Name 속성이 "\\.\PHYSICALDRIVE2"인 디스크의 정보 가져오기
    ///     var win32_diskdrive = query.GetByName("\\.\PHYSICALDRIVE2");
    ///     
    ///     // Case #7 Generic List의 Find() 메서드를 직접 활용
    ///     var win32_diskdrive = query.ToList().Find(o => o.DeviceID == "\\.\PHYSICALDRIVE2");
    ///
    /// </example>
    /// 
    /// <remarks>
    /// Win32_DiskDrive 클래스의 일부 속성은 제외되었다.
    /// </remarks>
    public class Win32_DiskDrive_Query
    {
        ///
        /// private 멤버변수
        ///
        private List<Win32_DiskDrive> _win32_diskdrives;
        public int Count { get { return _win32_diskdrives.Count; } }
        public Win32_DiskDrive GetByDiskIndex(int index)
        {
            return _win32_diskdrives.Find(x => x.Index == index);
        }
        public Win32_DiskDrive GetByDeviceID(string deviceID)
        {
            return _win32_diskdrives.Find(x => x.DeviceID == deviceID);
        }
        public Win32_DiskDrive GetByName(string name)
        {
            return _win32_diskdrives.Find(x => x.Name == name);
        }

        // singletone pattern
        private static Win32_DiskDrive_Query _instance;
        public static Win32_DiskDrive_Query Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Win32_DiskDrive_Query();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        private Win32_DiskDrive_Query()
        {
            // 멤버변수 초기화
            _win32_diskdrives = new List<Win32_DiskDrive>();

            // WMI를 이용하여 Win32_DiskDrive 정보를 가져온다.
            initialize();
        }

        /// 
        /// public 메서드
        /// 
        public void Refresh()
        {
            initialize();
        }

        public List<Win32_DiskDrive> ToList()
        {
            return new List<Win32_DiskDrive>(_win32_diskdrives);
        }

        /// 
        /// private 메서드
        /// 
        private void initialize()
        {
            _win32_diskdrives.Clear();

            var query = new SelectQuery("Win32_DiskDrive");
            var searcher = new ManagementObjectSearcher(query);
            var physicalDriveList = searcher.Get().Cast<ManagementObject>().ToList();

            foreach (var item in physicalDriveList)
            {
                Win32_DiskDrive newItem = new Win32_DiskDrive();

                newItem.BytesPerSector = (UInt32)(item["BytesPerSector"] ?? (UInt32)0);
                newItem.Caption = ((string)item["Caption"] ?? "").Trim();
                newItem.CompressionMethod = ((string)item["CompressionMethod"] ?? "").Trim();
                newItem.ConfigManagerErrorCode = (UInt32)(item["ConfigManagerErrorCode"] ?? (UInt32)0);
                newItem.ConfigManagerUserConfig = (bool)(item["ConfigManagerUserConfig"] ?? false);
                newItem.CreationClassName = ((string)item["CreationClassName"] ?? "").Trim();
                newItem.DefaultBlockSize = (UInt64)(item["DefaultBlockSize"] ?? (UInt64)0);
                newItem.Description = ((string)item["Description"] ?? "").Trim();
                newItem.DeviceID = ((string)item["DeviceID"] ?? "").Trim();
                newItem.ErrorCleared = (bool)(item["ErrorCleared"] ?? false);
                newItem.ErrorDescription = ((string)item["ErrorDescription"] ?? "").Trim();
                newItem.ErrorMethodology = ((string)item["ErrorMethodology"] ?? "").Trim();
                newItem.FirmwareRevision = ((string)item["FirmwareRevision"] ?? "").Trim();
                newItem.Index = (UInt32)(item["Index"] ?? (UInt32)0);
                newItem.InterfaceType = ((string)item["InterfaceType"] ?? "").Trim();
                newItem.LastErrorCode = (UInt32)(item["LastErrorCode"] ?? (UInt32)0);
                newItem.Manufacturer = ((string)item["Manufacturer"] ?? "").Trim();
                newItem.MaxBlockSize = (UInt64)(item["MaxBlockSize"] ?? (UInt64)0);
                newItem.MaxMediaSize = (UInt64)(item["MaxMediaSize"] ?? (UInt64)0);
                newItem.MediaLoaded = (bool)(item["MediaLoaded"] ?? false);
                newItem.MediaType = ((string)item["MediaType"] ?? "").Trim();
                newItem.MinBlockSize = (UInt64)(item["MinBlockSize"] ?? (UInt64)0);
                newItem.Model = ((string)item["Model"] ?? "").Trim();
                newItem.Name = ((string)item["Name"] ?? "").Trim();
                newItem.NeedsCleaning = (bool)(item["NeedsCleaning"] ?? false);
                newItem.NumberOfMediaSupported = (UInt32)(item["NumberOfMediaSupported"] ?? (UInt32)0);
                newItem.Partitions = (UInt32)(item["Partitions"] ?? (UInt32)0);
                newItem.PNPDeviceID = ((string)item["PNPDeviceID"] ?? "").Trim();
                newItem.PowerManagementSupported = (bool)(item["PowerManagementSupported"] ?? false);
                newItem.SCSIBus = (UInt32)(item["SCSIBus"] ?? (UInt32)0);
                newItem.SCSILogicalUnit = (UInt16)(item["SCSILogicalUnit"] ?? (UInt16)0);
                newItem.SCSIPort = (UInt16)(item["SCSIPort"] ?? (UInt16)0);
                newItem.SCSITargetId = (UInt16)(item["SCSITargetId"] ?? (UInt16)0);
                newItem.SectorsPerTrack = (UInt32)(item["SectorsPerTrack"] ?? (UInt32)0);
                newItem.SerialNumber = ((string)item["SerialNumber"] ?? "").Trim();
                newItem.Signature = (UInt32)(item["Signature"] ?? (UInt32)0);
                newItem.Size = (UInt64)(item["Size"] ?? (UInt64)0);
                newItem.Status = ((string)item["Status"] ?? "").Trim();
                newItem.StatusInfo = (UInt16)(item["StatusInfo"] ?? (UInt16)0);
                newItem.SystemCreationClassName = ((string)item["SystemCreationClassName"] ?? "").Trim();
                newItem.SystemName = ((string)item["SystemName"] ?? "").Trim();
                newItem.TotalCylinders = (UInt64)(item["TotalCylinders"] ?? (UInt64)0);
                newItem.TotalHeads = (UInt32)(item["TotalHeads"] ?? (UInt32)0);
                newItem.TotalSectors = (UInt64)(item["TotalSectors"] ?? (UInt64)0);
                newItem.TotalTracks = (UInt64)(item["TotalTracks"] ?? (UInt64)0);
                newItem.TracksPerCylinder = (UInt32)(item["TracksPerCylinder"] ?? (UInt32)0);
                //win32_DiskDrive.Availability = (ushort)physicalDrive["Availability"];
                //newItem.InstallDate = (DateTime)item["InstallDate"] ?? DateTime.MinValue.ToLocalTime();
                //newItem.InstallDate = (string)item["InstallDate"] ?? "";
                //newItem.PowerManagementCapabilities = (UInt16)(item["PowerManagementCapabilities[]"] ?? 0);

                _win32_diskdrives.Add(newItem);
            }

            _win32_diskdrives = _win32_diskdrives.OrderBy(o => o.Name).ToList();
        }

        ///
        /// Operator 재정의
        ///
        public Win32_DiskDrive this[int index]
        {
            get { return _win32_diskdrives[index]; }
        }

        ///
        /// Override
        ///
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in _win32_diskdrives)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }


        public static void Test()
        {
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("+ Win32_DiskDrive_Query.Test1()");
            Console.WriteLine("---------------------------------------");
            var query = Win32_DiskDrive_Query.Instance;
            query.Refresh();
            for (int i = 0; i < query.Count; i++)
            {
                Console.WriteLine(query[i].ToShortString());
            }
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("+ Win32_DiskDrive_Query.Test2()");
            Console.WriteLine("---------------------------------------");
            var win32_diskdrive = query.GetByDiskIndex(0);
            Console.WriteLine(win32_diskdrive.ToShortString());
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("+ Win32_DiskDrive_Query.Test3()");
            Console.WriteLine("---------------------------------------");
            win32_diskdrive = query.GetByDeviceID(@"\\.\PHYSICALDRIVE0");
            Console.WriteLine(win32_diskdrive.ToShortString());
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("+ Win32_DiskDrive_Query.Test4()");
            Console.WriteLine("---------------------------------------");
            win32_diskdrive = query.ToList().Find(o => o.DeviceID == @"\\.\PHYSICALDRIVE0");
            Console.WriteLine(win32_diskdrive.ToShortString());
        }
    }
}
