using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GhostYak.DateTime
{
    public class UnixTime
    {
        private int _seconds;
        public int Seconds
        {
            get { return _seconds; }
            set { _seconds = value; }
        }


        public UnixTime(int seconds)
        {
            _seconds = seconds;
        }

        public UnixTime(byte[] seconds, bool isLittleEndian)
        {
            if (seconds.Length != 4)
                throw new ArgumentException("byte[] seconds의 Length는 반드시 4입니다.");

            if (!isLittleEndian)
                Array.Reverse(seconds);

            _seconds = BitConverter.ToInt32(seconds, 0);
        }

        public System.DateTime ToDateTime()
        {
            var dt = new System.DateTime(1970, 1, 1);
            return dt.AddSeconds(_seconds);
        }

        public override string ToString()
        {
            return _seconds.ToString();
        }

        public string ToDateTimeString()
        {
            return ToDateTime().ToString();
        }

        public static void Test()
        {
            var dt1 = new System.DateTime(2021, 1, 1, 0, 0, 0);
            var unixtime = new UnixTime(1609459200);
            var dt2 = unixtime.ToDateTime();

            Debug.Assert(dt1 == dt2);
            Debug.Assert(unixtime.Seconds == 1609459200);

            var arrLittleEndian = new byte[] { 0x00, 0x66, 0xEE, 0x5F };
            var unixtimeLittleEndian = new UnixTime(arrLittleEndian, true);
            Debug.Assert(dt1 == unixtimeLittleEndian.ToDateTime());

            var arrBigEndian = new byte[] { 0x5F, 0xEE, 0x66, 0x00 };
            var unixtimeBigEndian = new UnixTime(arrBigEndian, false);
            Debug.Assert(dt1 == unixtimeBigEndian.ToDateTime());
        }
    }
}
