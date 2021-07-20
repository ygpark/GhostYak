using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostYak.Text
{
    public class Base64
    {
        public static string Decode(string data)
        {
            try
            {
                byte[] byte64 = Convert.FromBase64String(data.Trim());
                return Encoding.UTF8.GetString(byte64);
            }
            catch (Exception e)
            {
                throw new Exception("Error in Base64Decode: " + e.Message);
            }
        }
    }
}
