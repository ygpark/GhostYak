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
                byte[] byte64 = Convert.FromBase64String(data);
                return Encoding.UTF8.GetString(byte64);
            }
            catch (Exception e)
            {
                throw new Exception("Error in Base64Decode: " + e.Message);
            }
        }

        public static string EnCode(string data)
        {
            try
            {
                byte[] byteString = Encoding.UTF8.GetBytes(data);
                return Convert.ToBase64String(byteString);
            }
            catch (Exception e)
            {
                throw new Exception("Error in Base64Encode: " + e.Message);
            }
        }
    }
}
