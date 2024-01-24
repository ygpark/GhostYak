using System;
using System.Linq;

namespace GhostYak.DateTime
{
    public class UnixTime
    {
        private int _seconds;

        public UnixTime()
        {
        }

        public UnixTime(byte[] seconds, bool isBigEndian)
        {
            if (seconds.Length != sizeof(int/*type of _seconds*/))
                throw new ArgumentOutOfRangeException();

            byte[] secondsClone = (byte[])seconds.Clone();
            if (isBigEndian)
                Array.Reverse(secondsClone);

            _seconds = BitConverter.ToInt32(secondsClone, 0);
        }

        public System.DateTime ToDateTime()
        {
            return new System.DateTime(1970, 1, 1).AddSeconds(_seconds);
        }
    }
}
