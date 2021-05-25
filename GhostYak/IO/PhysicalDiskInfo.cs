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
                
                var mmObjectList = from item in searcher.Get().Cast<ManagementObject>() 
                                    orderby item["DeviceID"] 
                                    select item;

                foreach (ManagementObject mmObject in mmObjectList)
                {
                    drivelist.Add(mmObject["DeviceID"].ToString());
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            return drivelist;
        }
    }
}
