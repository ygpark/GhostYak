using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GhostYak.DateTime
{
    public class UnixTime32
    {
        private uint _seconds;

        public UnixTime32(uint seconds)
        {
            _seconds = seconds;
        }

        /// <summary>
        /// 리틀앤디언 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="seconds"></param>
        public UnixTime32(byte[] seconds) : this(seconds, false)
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="seconds"></param>
        /// <param name="isLittleEndian"></param>
        public UnixTime32(byte[] seconds, bool isBigEndian)
        {
            byte[] secondsClone = seconds.ToArray();
            int len = BitConverter.GetBytes(_seconds).Length;
            if (seconds.Length != len)
                throw new ArgumentOutOfRangeException();

            if (isBigEndian)
                Array.Reverse(secondsClone);

            _seconds = BitConverter.ToUInt32(secondsClone, 0);
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

        /// <summary>
        /// 1970-01-01T00:00:00Z 이후 경과된 시간(초)을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public uint ToUnixTimeSeconds()
        {
            return _seconds;
        }

        /// <summary>
        /// 1970-01-01T00:00:00Z 이후 경과된 시간(초)을 배열로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return BitConverter.GetBytes(_seconds);
        }

        /// <summary>
        /// 1970-01-01T00:00:00Z 이후 경과된 시간(초)을 배열로 만들고 순서를 뒤집어 반환합니다.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArrayReverse()
        {
            byte[] arr = BitConverter.GetBytes(_seconds);
            Array.Reverse(arr);
            return arr;
        }

        public static void Test()
        {
            var dotnetDateTime = new System.DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            uint dotnetUnixTime = (uint)((DateTimeOffset)dotnetDateTime).ToUnixTimeSeconds();

            var myUnixTime1 = new UnixTime32(dotnetUnixTime);
            var myDateTime1 = myUnixTime1.ToDateTime();

            byte[] arr2 = BitConverter.GetBytes(dotnetUnixTime);
            var myUnixTime2 = new UnixTime32(arr2);
            var myDateTime2 = myUnixTime2.ToDateTime();

            byte[] arr3 = BitConverter.GetBytes(dotnetUnixTime);
            var myUnixTime3 = new UnixTime32(arr3, false);
            var myDateTime3 = myUnixTime3.ToDateTime();

            byte[] arr4Reverse = BitConverter.GetBytes(dotnetUnixTime);
            Array.Reverse(arr4Reverse);
            var myUnixTime4 = new UnixTime32(arr4Reverse, true);
            var myDateTime4 = myUnixTime4.ToDateTime();

            Debug.Assert(dotnetDateTime == myDateTime1);
            Debug.Assert(dotnetUnixTime == myUnixTime1.ToUnixTimeSeconds());

            Debug.Assert(dotnetDateTime == myDateTime2);
            Debug.Assert(dotnetUnixTime == myUnixTime2.ToUnixTimeSeconds());

            Debug.Assert(dotnetDateTime == myDateTime3);
            Debug.Assert(dotnetUnixTime == myUnixTime3.ToUnixTimeSeconds());
            Debug.Assert(arr3.SequenceEqual(myUnixTime3.ToArray()));

            Debug.Assert(dotnetDateTime == myDateTime4);
            Debug.Assert(dotnetUnixTime == myUnixTime4.ToUnixTimeSeconds());
            Debug.Assert(arr4Reverse.SequenceEqual(myUnixTime4.ToArrayReverse()));
        }
    }
}
