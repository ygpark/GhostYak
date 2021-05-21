using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GhostYak.IO
{
    public class PhysicalDiskInfo
    {
        private PhysicalDiskInfo() 
        { 

        }
        public static List<string> GetNames()
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
    }
}
