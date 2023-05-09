using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using Re2.Net;
using GhostYak.DateTime;
using GhostYak.RunTime.InteropService;
using GhostYak.Text.RegularExpressions;
using GhostYak.IO.RawDiskDrive;
using GhostYak.WMI;

namespace GhostYak
{
    class Program
    {
        static void Main(string[] args)
        {
            //UnixTime32.Test();
            //UnixTime64.Test();
            //BinaryRegex.Test();
            //BinaryRegexNet.Test();
            //HanwhaTime.Test();
            //DahuaTime.Test();
            //Win32_DiskDrive_Query.Test();
            //GhostYak.IO.RawDiskDrive.PhysicalStream.Test();

            TestWMIWin32_DiskDrive_AND_PhysicalStream_Length();
        }

        static void TestWMIWin32_DiskDrive_AND_PhysicalStream_Length()
        {
            var query = Win32_DiskDrive_Query.Instance;
            foreach (var disk in query.ToList())
            {
                using (var stream = new PhysicalStream(disk.DeviceID))
                {
                    Console.WriteLine($"{disk.ToShortString()}, {stream.Length}");
                }
            }
        }
    }
}
