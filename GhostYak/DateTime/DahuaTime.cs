using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GhostYak.DateTime
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DahuaTime
    {
        private uint _bitfieldTime;

        #region CTor
        public DahuaTime()
        {
            // Marshal.PtrToStructure 때문에 반드시 필요하다.
        }

        public DahuaTime(uint timeUnion) : this(timeUnion, false)
        {
        }

        public DahuaTime(uint timeUnion, bool isBigEndian)
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
        public DahuaTime(byte[] bitfieldTime) : this(bitfieldTime, false)
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="bitfieldTime"></param>
        /// <param name="isLittleEndian"></param>
        public DahuaTime(byte[] bitfieldTime, bool isBigEndian)
        {
            byte[] clone = bitfieldTime.ToArray();
            int len = BitConverter.GetBytes(_bitfieldTime).Length;

            if (bitfieldTime.Length != len)
                throw new ArgumentOutOfRangeException();

            if (isBigEndian)
                Array.Reverse(clone);

            _bitfieldTime = BitConverter.ToUInt32(clone, 0);
        }

        public static DahuaTime FromDateTime(System.DateTime dateTime)
        {
            //int unixTime = (int)((DateTimeOffset)dateTime).ToUnixTimeSeconds();

            uint year = ((uint)dateTime.Year - (uint)2000) << 26;
            uint month = (uint)dateTime.Month << 22;
            uint day = (uint)dateTime.Day << 17;
            uint hour = (uint)dateTime.Hour << 12;
            uint minute = (uint)dateTime.Minute << 6;
            uint second = (uint)dateTime.Second;

            return new DahuaTime(year | month | day | hour | minute | second);
        }
        #endregion

        #region override
        /// <summary>
        /// 현재 개체를 나타내는 문자열을 반환합니다.
        /// </summary>
        /// <returns>현재 개체를 나타내는 문자열</returns>
        public override string ToString()
        {
            return ToDateTime().ToString();
        }

        /// <summary>
        /// 지정한 개체와 현재 개체가 같은지 여부를 확인합니다.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>지정한 개체와 현재 개체가 같으면 true, 다르면 false입니다.</returns>
        public override bool Equals(object obj)
        {
            return ToDateTime() == ((DahuaTime)(obj)).ToDateTime();
        }

        /// <summary>
        /// 지정한 개체와 현재 개체가 같은지 여부를 확인합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator== (DahuaTime a, DahuaTime b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 지정한 개체와 현재 개체가 다른지 여부를 확인합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(DahuaTime a, DahuaTime b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// 기본 해시 함수로 작동합니다.
        /// </summary>
        /// <returns>현재 개체의 해시코드입니다.</returns>
        public override int GetHashCode()
        {
            return (int)_bitfieldTime;
        }

        #endregion

        #region To___Method()
        /// <summary>
        /// 현재 개체를 System.DateTime 개체로 변환합니다.
        /// </summary>
        /// <returns>System.DateTime 개체</returns>
        public System.DateTime ToDateTime()
        {
            try
            {
                //21098765432109876543210987654321 //4byte bit order
                //11111100000000000000000000000000 //MASK for year
                //00000011110000000000000000000000 //MASK for month
                //00000000001111100000000000000000 //MASK for day
                //00000000000000011111000000000000 //MASK for hour
                //00000000000000000000111111000000 //MASK for minute
                //00000000000000000000000000111111 //MASK for second
                int year = (int)((_bitfieldTime >> 26) + 2000);
                int month = (int)((_bitfieldTime >> 22) & 0xF);
                int day = (int)((_bitfieldTime >> 17) & 0x1F);
                int hour = (int)((_bitfieldTime >> 12) & 0x1F);
                int minute = (int)((_bitfieldTime >> 6) & 0x3F);
                int second = (int)(_bitfieldTime & 0x3F);

                return new System.DateTime(year, month, day, hour, minute, second);
            }
            catch(ArgumentOutOfRangeException e)
            {
                return System.DateTime.MaxValue;
            }
        }

        /// <summary>
        /// 현재 개체를 uint값으로 변환합니다.
        /// </summary>
        /// <returns>uint</returns>
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
        /// 현재 개체를 byte[]로 변환합니다.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] ToArray()
        {
            return BitConverter.GetBytes(_bitfieldTime);
        }

        /// <summary>
        /// 현재 개체를 byte[]의 역순으로 변환합니다.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArrayReverse()
        {
            byte[] arr = BitConverter.GetBytes(_bitfieldTime);
            Array.Reverse(arr);
            return arr;
        }
        #endregion

        #region Test

        /// <summary>
        /// 바이트 배열을 T타입의 구조체로 캐스팅합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns>T타입 </returns>
        private static T ByteArrayToStructure<T>(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;
        }



        /// <summary>
        /// 유닛 테스트
        /// </summary>
        public static void Test()
        {
            var dotnetDateTime = new System.DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var myTime1 = DahuaTime.FromDateTime(dotnetDateTime);
                
            uint srcUInt = myTime1.ToUInt();
            byte[] srcByteArray = myTime1.ToArray();//-> Marshal
            byte[] srcByteArrayR = myTime1.ToArrayReverse();
            uint srcUIntR = BitConverter.ToUInt32(srcByteArrayR, 0);
            uint data4 = myTime1.ToUnixTimeSeconds();

            var myTime2 = new DahuaTime(srcUInt);
            var myTime3 = new DahuaTime(srcUInt, false);
            var myTime4 = new DahuaTime(srcUIntR, true);
            var myTime5 = new DahuaTime(srcByteArray);
            var myTime6 = new DahuaTime(srcByteArray, false);
            var myTime7 = new DahuaTime(srcByteArrayR, true);
            var myTime8 = ByteArrayToStructure <DahuaTime> (srcByteArray);

            Debug.Assert(dotnetDateTime == myTime2.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime3.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime4.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime5.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime6.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime7.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime8.ToDateTime());

            Debug.Assert(myTime1 == myTime2);
            Debug.Assert(myTime1 == myTime3);
            Debug.Assert(myTime1 == myTime4);
            Debug.Assert(myTime1 == myTime5);
            Debug.Assert(myTime1 == myTime6);
            Debug.Assert(myTime1 == myTime7);
            Debug.Assert(myTime1 == myTime8);
            Debug.Assert(myTime2 != new DahuaTime());
        }

        #endregion
    }
}
