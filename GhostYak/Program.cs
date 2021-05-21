using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GhostYak.Text.RegularExpressions;
using GhostYak.IO;

namespace GhostYak
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> list = PhysicalDiskInfo.GetNames();
            foreach(var item in list)
            {
                Console.WriteLine(item);
            }


        }
    }
}
