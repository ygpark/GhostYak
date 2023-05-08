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
            UnixTime32.Test();
            UnixTime64.Test();
            BinaryRegex.Test();
            BinaryRegexNet.Test();
            HanwhaTime.Test();
            DahuaTime.Test();

            Win32_DiskDrive_Query.Test();
            //TestPhysicalStream();
        }

        static void TestPhysicalStream()
        {
            PhysicalStorage ps = new PhysicalStorage(0);
            using (Stream stream = ps.OpenRead())
            {
                byte[] buffer = new byte[512];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read != buffer.Length)
                {
                    Console.WriteLine($"Can't read {buffer.Length} bytes");
                    return;
                }
                var hexdump = GhostYak.Text.HexConverter.ToString(buffer, 16);
                Console.WriteLine(hexdump);
            }
        }
    }
}
