using Microsoft.Win32.SafeHandles;
using GhostYak.IO.DeviceIOControl.Objects.Disk;
using GhostYak.IO.DeviceIOControl.Wrapper;
using GhostYak.WMI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GhostYak.IO.RawDiskDrive
{
    class PhysicalStream : BaseStream
    {
        private SafeFileHandle _handle = null;
        private int _bytesPerSector;

        // Win32 API 함수 선언
        const uint IOCTL_DISK_GET_DRIVE_GEOMETRY_EX = 0x700A0;
        const int INVALID_HANDLE_VALUE = -1;


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, ref DISK_GEOMETRY_EX lpOutBuffer, int nOutBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

        // DISK_GEOMETRY_EX 구조체
        [StructLayout(LayoutKind.Sequential)]
        struct DISK_GEOMETRY_EX
        {
            public DISK_GEOMETRY Geometry;
            public long DiskSize;
            public byte Data;
        }

        // DISK_GEOMETRY 구조체
        [StructLayout(LayoutKind.Sequential)]
        struct DISK_GEOMETRY
        {
            public long Cylinders;
            public uint MediaType;
            public uint TracksPerCylinder;
            public uint SectorsPerTrack;
            public uint BytesPerSector;
        }


        public PhysicalStream(string path)
        {
            //--------------------------------------------------------------------------------
            // DATE: 2023-05-04
            // AUTHOR: 박영기
            // REMARKS: 특정 HDD의 Size 정보를 읽어오지 못하는 오류가 확인되었다.
            //          현재 사용중인 DiskDeviceWrapper class 를 대체할만한 API가 WMI Win32_DiskDrive 밖에는 마땅히 없다.
            //          WMI Win32_DiskDrive API의 Size는 실제 HDD의 Sector보다 약 400K정도 작은 값을 리턴한다.
            //          용량의 오차는 저장매체별로 상이하지만 무시할만한 수준이다.
            //--------------------------------------------------------------------------------

            _handle = Win32Native.CreateFile(path, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);

            var diskDrive = Win32_DiskDrive_Query.Instance;
            diskDrive.Refresh();
            var diskDriveInfo = diskDrive.ToList().Find(o => o.DeviceID == path);

            //this.length = (long)diskDriveInfo.Size;
            this.length = GetFileSize();
            this._bytesPerSector = (int)diskDriveInfo.BytesPerSector;
            this.canRead = true;
            this.canSeek = true;
            this.canWrite = false;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int hr = 0;
            int br = 0;

            if (this.length < this.position + count)
            {
                count = (int)(this.length - this.position);
            }
            // 섹터 배수인지 검사
            //   -> 블록 디바이스 특성
            if (this.position % _bytesPerSector == 0 && count % _bytesPerSector == 0)
            {


                br = ReadFileNative(_handle, buffer, offset, count, out hr);//throw new IndexOutOfRangeException()
                if (br == -1)
                {
                    switch (hr)
                    {
                        case 109:
                            br = 0;
                            break;
                        case 87:
                            //섹터배수가 아닌 경우 예외 발생함1
                            throw new ArgumentException("INVALID_PARAMETER");
                        default:
                            //섹터배수가 아닌 경우 예외 발생함2
                            throw new Win32Exception(hr);
                    }
                }

                this.position += br;
            }
            else
            {
                long lBytesPerSector = (long)_bytesPerSector;
                long lPos = this.position;
                long lStartSectorIdx = lPos / lBytesPerSector;
                long sourceIdx = lPos % lBytesPerSector;
                long endSectorIdx = (lPos + (long)count - 1L) / lBytesPerSector;
                int sectorCount = (int)(endSectorIdx - lStartSectorIdx + 1); // 0 - 0 + 1 = 1
                long newPos = lStartSectorIdx * lBytesPerSector;
                int newCount = (int)(sectorCount * lBytesPerSector);
                byte[] tmpBuffer = new byte[newCount];

                Seek(newPos, SeekOrigin.Begin);
                br = ReadFileNative(_handle, tmpBuffer, 0, newCount, out hr);
                if (br == -1)
                {
                    switch (hr)
                    {
                        case 109:
                            br = 0;
                            break;
                        case 87:
                            //섹터배수가 아닌 경우 예외 발생함1
                            throw new ArgumentException("INVALID_PARAMETER");
                        default:
                            //섹터배수가 아닌 경우 예외 발생함2...
                            //<2021-10-09>HDD가 갑자기 Offline이 되어도 발생함.
                            throw new Win32Exception(hr);
                    }
                }
                Array.Copy(tmpBuffer, sourceIdx, buffer, 0, count);

                this.position += br;
            }

            return br;
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

            long newpos = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newpos = offset;
                    break;
                case SeekOrigin.Current:
                    newpos = offset + position;
                    break;
                case SeekOrigin.End:
                    newpos = offset + length;
                    break;
            }
            int lDistanceToMoveLow = (int)newpos;
            int lDistanceToMoveHigh = (int)(newpos >> 32);

            uint rst = Win32Native.SetFilePointer(_handle, lDistanceToMoveLow, ref lDistanceToMoveHigh, SeekOrigin.Begin);
            if (rst == Win32Native.INVALID_SET_FILE_POINTER)
            {
                throw new ArgumentException("cant seek to before the start of the stream.");
            }
            position = newpos;

            return position;
        }

        protected override void Dispose(bool disposing)
        {
            if (_handle != null)
            {
                SafeFileHandle handle = _handle;
                _handle = null;
                handle.Dispose();
            }
        }

        public override void Close()
        {
            _handle.Close();
        }

        public long GetFileSize()
        {
            long diskSize = long.MaxValue;

            // DISK_GEOMETRY_EX 구조체를 초기화합니다.
            DISK_GEOMETRY_EX diskGeometry = new DISK_GEOMETRY_EX();
            int bytesReturned;

            // IOCTL_DISK_GET_DRIVE_GEOMETRY_EX 명령을 사용하여 디스크의 기하학 정보를 가져옵니다.
            if (!DeviceIoControl(_handle, IOCTL_DISK_GET_DRIVE_GEOMETRY_EX, IntPtr.Zero, 0, ref diskGeometry, Marshal.SizeOf(diskGeometry), out bytesReturned, IntPtr.Zero))
            {
                Console.WriteLine("Failed to get disk geometry.");
                return long.MaxValue;
            }

            // 하드디스크의 크기를 출력합니다.
            diskSize = diskGeometry.DiskSize;
            //Console.WriteLine("Disk size: " + diskSize);

            return diskSize;
        }

        public static void Test()
        {
            using (var stream = new PhysicalStream(@"\\.\PHYSICALDRIVE0"))
            {
                byte[] buffer = new byte[512];
                stream.Position = 22800262757888;
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                var str = GhostYak.Text.HexConverter.ToString(buffer);
                Console.WriteLine(str);
                Console.WriteLine(stream.Length);
            }
        }
    }
}
