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
    public class UnixTime64
    {
        private long _seconds;


        #region CTor

        public UnixTime64()
        {
            // Marshal.PtrToStructure 때문에 반드시 필요하다.
        }


        public UnixTime64(long seconds) : this(seconds, false)
        {
        }

        public UnixTime64(long seconds, bool isBigEndian)
        {
            if (isBigEndian)
            {
                byte[] bSeconds = BitConverter.GetBytes(seconds);
                Array.Reverse(bSeconds);
                _seconds = BitConverter.ToInt64(bSeconds, 0);
            }
            else
            {
                _seconds = seconds;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="seconds"></param>
        public UnixTime64(byte[] seconds) : this(seconds, false)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="seconds"></param>
        /// <param name="isBigEndian"></param>
        public UnixTime64(byte[] seconds, bool isBigEndian)
        {
            byte[] secondsClone = seconds.ToArray();
            int len = BitConverter.GetBytes(_seconds).Length;
            if (seconds.Length != len)
                throw new ArgumentOutOfRangeException();

            if (isBigEndian)
                Array.Reverse(secondsClone);

            _seconds = BitConverter.ToInt64(secondsClone, 0);
        }
        #endregion

        #region override

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
            return _seconds == ((UnixTime64)(obj))._seconds;
        }

        public static bool operator ==(UnixTime64 a, UnixTime64 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(UnixTime64 a, UnixTime64 b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// 기본 해시 함수로 작동합니다.
        /// </summary>
        /// <returns>현재 개체의 해시코드입니다.</returns>
        public override int GetHashCode()
        {
            return (int)_seconds;
        }
        #endregion

        #region To___Method()

        public System.DateTime ToDateTime()
        {
            var dt = new System.DateTime(1970, 1, 1);
            return dt.AddSeconds(_seconds);
        }

        public long ToLong()
        {
            return _seconds;
        }

        /// <summary>
        /// 1970-01-01T00:00:00Z 이후 경과된 시간(초)을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public long ToUnixTimeSeconds()
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
            long srcLong = ((DateTimeOffset)dotnetDateTime).ToUnixTimeSeconds();
            byte[] srcByteArray = BitConverter.GetBytes(srcLong);
            byte[] srcByteArrayR = BitConverter.GetBytes(srcLong);
            Array.Reverse(srcByteArrayR);
            long srcLongR = BitConverter.ToInt64(srcByteArrayR, 0);

            var myTime1 = new UnixTime64(srcLong);
            var myTime2 = new UnixTime64(srcLong, false);
            var myTime3 = new UnixTime64(srcLongR, true);
            var myTime4 = new UnixTime64(srcByteArray);
            var myTime5 = new UnixTime64(srcByteArray, false);
            var myTime6 = new UnixTime64(srcByteArrayR, true);

            Debug.Assert(dotnetDateTime == myTime1.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime2.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime3.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime4.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime5.ToDateTime());
            Debug.Assert(dotnetDateTime == myTime6.ToDateTime());

            Debug.Assert(myTime1 == myTime2);
            Debug.Assert(myTime1 == myTime3);
            Debug.Assert(myTime1 == myTime4);
            Debug.Assert(myTime1 == myTime5);
            Debug.Assert(myTime1 == myTime6);
            Debug.Assert(myTime1 != new UnixTime64());
        }

        #endregion
    }
}
