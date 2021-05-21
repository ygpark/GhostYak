using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GhostYak.IO.DeviceIOControl.Objects.Disk
{
    // IDSECTOR 크기는 512와 같거나 커야한다.
    // 더 정확히는 SENDCMDOUTPARAMS의 마지막 포인터 bBuffer가 가르키는 구조체는 512와 같거나 512보다 커야함.
    [StructLayout(LayoutKind.Sequential)]
    public struct IDSECTOR
    {
        public UInt16 GenConfig;
        public UInt16 NumberCylinders;
        public UInt16 wReserved;
        public UInt16 NumberHeads;
        public UInt16 BytesPerTrack;
        public UInt16 BytesPerSector;
        public UInt16 SectorsPerTrack;//14

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public UInt16[] VendorUnique;//20
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private char[] _SerialNumber;//40

        public UInt16 BufferType;
        public UInt16 BufferSize;
        public UInt16 ECCSize;//46

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private char[] _FirmwareRevision;//54

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        private char[] _ModelNumber;//94
        public UInt16 MoreVendorUnique;
        public UInt16 DoubleWordIO;
        public UInt16 Capabilities;//100
        public UInt16 Reserved1;
        public UInt16 PIOTiming;
        public UInt16 DMATiming;
        public UInt16 BS;
        public UInt16 NumCurrentCyls;
        public UInt16 NumCurrentHeads;
        public UInt16 NumCurrentSectorsPerTrack;//114
        public UInt32 CurrentSectorCapacity;//120 => 118이 아니고 왜 120이지..????????? +2
        public UInt16 MultSectorStuff;//124
        public UInt32 TotalAddressableSectors;//128
        public UInt16 SingleWordDMA;//132
        public UInt16 MultiWordDMA;//132 => 이건 또 왜 134가 아니고 132지?? 어쨌거나 여기서부터 크기 맞음

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 380)]
        public Byte[] bReserved;//512

        /// <summary>Device type</summary>
        public enum DeviceType
        {
            Unknown,
            NonMagnetic,
            Removable,
            Fixed,
        }

        public DeviceType Type
        {
            get
            {
                DeviceType result = DeviceType.Unknown;
                if ((this.GenConfig & 0x80) == 0x40)
                    result = DeviceType.Removable;
                else if ((this.GenConfig & 0x40) == 0x40)
                    result = DeviceType.Fixed;
                return result;
            }
        }

        public string ModelNumber { get => SwapChars(_ModelNumber).Trim(); }
        public string SerialNumber { get => SwapChars(_SerialNumber).Trim(); }
        public string FirmwareRevision { get => SwapChars(_FirmwareRevision).Trim(); }

        private string SwapChars(char[] buffer)
        {
            if (buffer == null)
                return "";

            StringBuilder result = new StringBuilder();
            for (Int32 loop = 0; loop < buffer.Length; loop += 2)
            {
                result.Append(buffer[loop + 1]);
                result.Append(buffer[loop]);
            }
            return result.ToString();
        }
    }
}
