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
        }
    }
}
