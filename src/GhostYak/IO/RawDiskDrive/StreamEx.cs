using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GhostYak.IO.RawDiskDrive
{
    /// <summary>
    /// 바이트 시퀀스에 대한 일반 뷰를 제공합니다.
    /// 이 클래스는 추상 클래스입니다.
    /// </summary>
    public abstract class StreamEx : IDisposable
    {
        public abstract long Position { get; set; }

        public abstract int Read(byte[] array, int offset, int count);

        public abstract long Seek(long offset, System.IO.SeekOrigin origin);


        public virtual void Close()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected StreamEx()
        {
        }

    }
}
