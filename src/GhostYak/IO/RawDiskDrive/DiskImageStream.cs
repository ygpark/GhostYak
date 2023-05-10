using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostYak.IO.RawDiskDrive
{
    class DiskImageStream : BaseStream
    {
        private EWF.Handle _handle;

        public DiskImageStream(string[] path)
        {

            _handle = new EWF.Handle();

            _handle.Open(path, 1);
            this.length = (long)_handle.GetMediaSize();

            this.canRead = true;
            this.canSeek = true;
            this.canWrite = false;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int br;
            if (offset == 0)
            {
                br = _handle.ReadBuffer(buffer, count);
            }
            else
            {
                byte[] somebuf = new byte[count];
                br = _handle.ReadBuffer(somebuf, count);
                Buffer.BlockCopy(somebuf, 0, buffer, offset, br);
            }

            this.position += br;
            return br;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return position = _handle.SeekOffset(offset, origin);
        }

        protected override void Dispose(bool disposing)
        {
            if (_handle != null)
            {
                EWF.Handle handle = _handle;
                _handle = null;
                handle.Dispose();
            }
        }

        public override void Close()
        {
            Dispose(true);
        }
    }
}
