using System;
using System.Runtime.InteropServices;

namespace GhostYak.IO.DeviceIOControl.Utilities
{
    public class UnmanagedMemory : IDisposable
    {
        public IntPtr Handle { get; }
        private bool disposed = false;


        public UnmanagedMemory(int size)
        {
            Handle = Marshal.AllocHGlobal(size);
        }
        ~UnmanagedMemory()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            //관리되는 자원 해제
            if (disposing)
            {
            }

            //관리되지 않는 자원 해제
            //operator IntPtr에 의해 this는 this.Handle로 형변환된다.
            Marshal.FreeHGlobal(this);

            this.disposed = true;
        }

        public UnmanagedMemory(byte[] data)
        {
            Handle = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, Handle, data.Length);
        }

        public static implicit operator IntPtr(UnmanagedMemory mem)
        {
            return mem.Handle;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}