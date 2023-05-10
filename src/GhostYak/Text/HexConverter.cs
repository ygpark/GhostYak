using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostYak.Text
{
    public class HexConverter
    {
        private HexConverter() { }

        public static string ToString(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", " ");
        }

        /// <summary>
        /// byte[]를 string으로 변환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="displayPerLine">한 줄에 출력할 바이트 문자 개수</param>
        /// <param name="sectorSize">sectorSize 사이즈마다 줄바꿈을 추가합니다. 이때, sectorSize는 displayPerLine의 배수여야 합니다</param>
        /// <returns></returns>
        public static string ToString(byte[] value, int displayPerLine, int sectorSize)
        {
            if (sectorSize % displayPerLine != 0)
                throw new Exception("sectorSize는 displayPerLine의 배수여야 합니다.");

            if (sectorSize < 0)
                throw new Exception("sectorSize는 0이거나 0보다 커야 합니다.");

            if (displayPerLine <= 0)
                throw new Exception("displayPerLine는 0보다 커야 합니다.");


            StringBuilder sb = new StringBuilder();
            int lineCount = (int)Math.Ceiling((double)value.Length / (double)displayPerLine);
            for (int i = 0; i < lineCount; i++)
            {
                if (i == lineCount - 1 && value.Length % displayPerLine != 0)// 마지막 줄이고, value 개수 끝이 조금 모자랄 때
                    displayPerLine = value.Length % displayPerLine;

                sb.Append(BitConverter.ToString(value, i * displayPerLine, displayPerLine).Replace("-", " "));

                if (i < lineCount-1)
                    sb.Append(System.Environment.NewLine);

                if(sectorSize != 0 && value.Length != sectorSize && (i+1) * displayPerLine % sectorSize == 0)
                {
                    sb.Append(System.Environment.NewLine);
                }
            }

            return sb.ToString();
        }

        public static string ToString(byte[] value, int displayPerLine)
        {
            return ToString(value, displayPerLine, 0);
        }
    }
}
