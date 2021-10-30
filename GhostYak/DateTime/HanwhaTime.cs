/*
할일
1. 연산자== 오버라이딩
    
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GhostYak.DateTime
{
    public class HanwhaTime
    {
        private uint _bitfieldTime;

        public HanwhaTime(uint timeUnion) : this(timeUnion, false)
        {
        }

        public HanwhaTime(uint timeUnion, bool isBigEndian)
        {
            if(isBigEndian)
            {
                byte[] data = BitConverter.GetBytes(timeUnion);
                Array.Reverse(data);
                _bitfieldTime = BitConverter.ToUInt32(data, 0);
            }
            else 
            {
                _bitfieldTime = timeUnion;
            }
        }

        /// <summary>
        /// 리틀앤디언 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="bitfieldTime"></param>
        public HanwhaTime(byte[] bitfieldTime) : this(bitfieldTime, false)
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="bitfieldTime"></param>
        /// <param name="isLittleEndian"></param>
        public HanwhaTime(byte[] bitfieldTime, bool isBigEndian)
        {
            byte[] clone = bitfieldTime.ToArray();
            int len = BitConverter.GetBytes(_bitfieldTime).Length;

            if (bitfieldTime.Length != len)
                throw new ArgumentOutOfRangeException();

            if (isBigEndian)
                Array.Reverse(clone);

            _bitfieldTime = BitConverter.ToUInt32(clone, 0);
        }

        public System.DateTime ToDateTime()
        {
            long time1 = (_bitfieldTime & 0xFF000000) >> 2;
            long time2 = (_bitfieldTime & 0x007FFF00) >> 1;//15bit
            long time3 = (_bitfieldTime & 0x0000007F);//7bit

            long unixTime = time1 + time2 + time3 + 0x40000000;
            System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddSeconds(unixTime);
            return epoch;
        }

        public static HanwhaTime FromDateTime(System.DateTime dateTime)
        {
            uint unixTime = (uint)((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            unixTime -= 0x40000000;

            uint time1 = (unixTime & (0xFF000000 >> 2)) << 2;
            uint time2 = (unixTime & (0x007FFF00 >> 1)) << 1;//15bit
            uint time3 = (unixTime & 0x0000007F);//7bit

            return new HanwhaTime(time1 + time2 + time3);
        }

        public override string ToString()
        {
            return ToDateTime().ToString();
        }

        public string ToDateTimeString()
        {
            return ToDateTime().ToString();
        }

        public uint ToUInt()
        {
            return _bitfieldTime;
        }

        /// <summary>
        /// 1970-01-01T00:00:00Z 이후 경과된 시간(초)을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public uint ToUnixTimeSeconds()
        {
            System.DateTime dt = this.ToDateTime();
            return (uint)((DateTimeOffset)dt).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 1970-01-01T00:00:00Z 이후 경과된 시간(초)을 배열로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return BitConverter.GetBytes(_bitfieldTime);
        }

        /// <summary>
        /// 1970-01-01T00:00:00Z 이후 경과된 시간(초)을 배열로 만들고 순서를 뒤집어 반환합니다.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArrayReverse()
        {
            byte[] arr = BitConverter.GetBytes(_bitfieldTime);
            Array.Reverse(arr);
            return arr;
        }

        public static void Test()
        {
            var dotnetDateTime = new System.DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var myTime1 = HanwhaTime.FromDateTime(dotnetDateTime);
                
            uint data1 = myTime1.ToUInt();
            byte[] data2 = myTime1.ToArray();
            byte[] data3 = myTime1.ToArrayReverse();
            uint data1R = BitConverter.ToUInt32(data3, 0);
            uint data4 = myTime1.ToUnixTimeSeconds();

            var myTime2 = new HanwhaTime(data1);
            var myTime3 = new HanwhaTime(data1, false);
            var myTime4 = new HanwhaTime(data1R, true);
            var myTime5 = new HanwhaTime(data2);
            var myTime6 = new HanwhaTime(data2, false);
            var myTime7 = new HanwhaTime(data3, true);

            Debug.Assert(dotnetDateTime == myTime2.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime3.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime4.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime5.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime6.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime7.ToDateTime());
        }
    }
}
