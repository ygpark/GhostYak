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
            byte[] bytes = new byte[4] { 0x5D, 0xA1, 0xF8, 0x75 };
            UnixTime unixTime = new UnixTime(bytes, true);
            Console.WriteLine(unixTime.ToDateTime().ToString() );
        }
    }
}
