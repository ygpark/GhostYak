using Microsoft.Win32.SafeHandles;
using GhostYak.IO.DeviceIOControl.Objects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GhostYak.IO.RawDiskDrive
{
    /// <summary>
    /// 삭제될 예정
    /// </summary>
    public class FileStreamEx : StreamEx
    {
        internal const int DefaultBufferSize = 4096;
        protected SafeFileHandle _handle;
        private long _pos;
        private int _readLen;
        private int _readPos;
        private byte[] _buffer;
        private int _bufferSize;

        public FileStreamEx(SafeFileHandle handle, int bufferSize = DefaultBufferSize)
        {
            if (handle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            _handle = handle;
            _pos = 0L;
            _bufferSize = bufferSize;
            _readPos = 0;
            _readLen = 0;
        }
        public FileStreamEx(string path, int bufferSize = DefaultBufferSize)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "null이 허용되지 않습니다.");
            }
            if (path.Length == 0)
            {
                throw new ArgumentException("path는 빈 문자열입니다.");
            }
            
            _handle = Win32Native.CreateFile(path, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            _pos = 0L;
            _bufferSize = bufferSize;
            _readPos = 0;
            _readLen = 0;
        }
        
        public override long Position
        {
            get
            {
                if (_handle.IsClosed)
                {
                    throw new Exception("FileNotOpen");
                }
                return _pos + (_readPos - _readLen);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "ArgumentOutOfRange_NeedNonNegNum");
                }
                _readPos = 0;
                _readLen = 0;
                Seek(value, SeekOrigin.Begin);
            }
        }
        

        public override int Read([In] [Out] byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", "ArgumentNull_Buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if (array.Length - offset < count)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
            }
            if (_handle.IsClosed)
            {
                throw new Exception("File Not Open");
            }
            bool flag = false;
            int num = _readLen - _readPos;
            if (num == 0)
            {
                if (count >= _bufferSize)
                {
                    num = ReadCore(array, offset, count);
                    _readPos = 0;
                    _readLen = 0;
                    return num;
                }
                if (_buffer == null)
                {
                    _buffer = new byte[_bufferSize];
                }
                num = ReadCore(_buffer, 0, _bufferSize);
                if (num == 0)
                {
                    return 0;
                }
                flag = (num < _bufferSize);
                _readPos = 0;
                _readLen = num;
            }
            if (num > count)
            {
                num = count;
            }
            Buffer.BlockCopy(_buffer, _readPos, array, offset, num);
            _readPos += num;
            if (num < count && !flag)
            {
                int num2 = ReadCore(array, offset + num, count - num);
                num += num2;
                _readPos = 0;
                _readLen = 0;
            }
            return num;
        }

        private int ReadCore(byte[] buffer, int offset, int count)
        {
            int hr = 0;
            int num = ReadFileNative(_handle, buffer, offset, count, out hr);
            if (num == -1)
            {
                switch (hr)
                {
                    case 109:
                        num = 0;
                        break;
                    case 87:
                        throw new ArgumentException("Arg_HandleNotSync");
                    default:
                        throw new Win32Exception(hr);
                }
            }
            _pos += num;
            return num;
        }

        private unsafe int ReadFileNative(SafeFileHandle handle, byte[] bytes, int offset, int count, out int hr)
        {
            if (bytes.Length - offset < count)
            {
                throw new IndexOutOfRangeException("IndexOutOfRange_IORaceCondition");
            }
            if (bytes.Length == 0)
            {
                hr = 0;
                return 0;
            }
            int numBytesRead = 0;
            int num;
            fixed (byte* ptr = bytes)
            {
                num = Win32Native.ReadFile(handle, ptr + offset, count, out numBytesRead, IntPtr.Zero);
            }
            if (num == 0)
            {
                hr = Marshal.GetLastWin32Error();
                if (hr == 109 || hr == 233)
                {
                    return -1;
                }
                if (hr == 6)
                {
                    _handle.Dispose();
                }
                return -1;
            }
            hr = 0;
            return numBytesRead;
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            int lDistanceToMoveLow = (int)offset;
            int lDistanceToMoveHigh = (int)(offset >> 32);
            _pos = Win32Native.SetFilePointer(_handle, lDistanceToMoveLow, ref lDistanceToMoveHigh, SeekOrigin.Begin);
            return _pos;
        }
    }
}
