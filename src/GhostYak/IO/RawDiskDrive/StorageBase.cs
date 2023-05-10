using GhostYak.IO.DeviceIOControl.Objects.Enums;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GhostYak.IO.RawDiskDrive
{
    /// <summary>
    /// TODO : IDispose 구현해야함.
    /// </summary>
    public class StorageBase : IEquatable<StorageBase>
    {
        #region 멤버 변수
        /// <summary>
        /// 드라이브 파일명
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// 디스플레이용 이름 ex) '디스크 0', 'C:\ (로컬 디스크)', '파일명'
        /// </summary>
        public virtual string NameForDisplay { get => System.IO.Path.GetFileName(Path); }
        /// <summary>
        /// Byte단위의 전체 드라이브 크기
        /// </summary>
        public long Size { get; protected set; }
        /// <summary>
        /// 섹터당 바이트 수. 일반적으로 512byte 이다.
        /// <para>DDImageStorage 클래스는 BytesPerSector가 1byte 이다. </para>
        /// </summary>
        /// <remarks>/// <para>StorageBase 클래스는 일반 파일도 상속받도록 설계되어있기 때문에 </para></remarks>
        public int BytesPerSector { get; protected set; }
        /// <summary>
        /// Byte단위의 전체 드라이브 크기를 섹터로 나눈 값
        /// </summary>
        public long TotalSector
        {
            get
            {
                if (BytesPerSector == 0)
                    return 0;
                else
                    return Size / BytesPerSector;
            }
        }

        #endregion



        protected StorageBase(string path)
        {
            Path = path;
        }

        public virtual Stream OpenRead()
        {
            throw new NotImplementedException("StorageBase class의 OpenRead() 메소드를 구현해야합니다.");
        }

        public string SizeHumanReadable { get => GetHumanReadableSize(this.Size); }

        protected string GetHumanReadableSize(double byteSize)
        {
            string rtn;
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int idx = 0;
            double dHumanReadableSize = byteSize;
            for (int i = 0; i < units.Length; i++) {
                if (dHumanReadableSize >= (double)1000) {
                    dHumanReadableSize /= (double)1024;
                    idx++;
                }
            }

            dHumanReadableSize = Math.Floor(100 * dHumanReadableSize) / 100;

            //GB 이상만 소수점 이하 표시
            if (dHumanReadableSize < 100) {
                rtn = string.Format("{0}{1}", GetCommaSeperatedNumber(dHumanReadableSize.ToString()), units[idx]);
            } else {
                rtn = string.Format("{0}{1}", GetCommaSeperatedNumber(((long)dHumanReadableSize).ToString()), units[idx]);
            }
            return rtn;
        }

        static string GetCommaSeperatedNumber(string number)
        {
            bool isDouble = number.Contains(".");
            StringBuilder sb = new StringBuilder();
            string[] doubleArray = null;
            string n;

            if (isDouble) {
                doubleArray = number.Split('.');
                n = doubleArray[0];
            } else {
                n = number;
            }

            int reverseIndex = n.Length - 1;
            //23,456,789
            for (int i = 0; i < n.Length; i++) {
                sb.Append(n[i]);
                if (n.Length > 3 && reverseIndex != 0 && reverseIndex % 3 == 0) {
                    sb.Append(",");
                }
                reverseIndex--;
            }

            if (isDouble) {
                sb.Append("." + doubleArray[1]);
            }
            return sb.ToString();
        }

        public override bool Equals(object p)
        {
            return this.Equals(p as StorageBase);
        }

        public bool Equals(StorageBase p)
        {
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // 레퍼런트 타입의 참조가 같은지 판단한다. 포인터가 같은지 비교.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            return this.Path == p.Path;
        }

        public static bool operator ==(StorageBase lhs, StorageBase rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null
                    return true;
                }
                // Only left size is null
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(StorageBase lhs, StorageBase rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Path.Length * (int)Size;
        }

        public bool IsBlockDevice { get => (BytesPerSector != 0); }
    }
}
