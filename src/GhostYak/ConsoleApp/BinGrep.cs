//
// bingrep v1.2
//
using GhostYak.IO;
using GhostYak.CommandLine.Options;
using GhostYak.IO.RawDiskDrive;
using GhostYak.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Dotnet = System.Text.RegularExpressions;

namespace GhostYak.ConsoleApp
{
    class BinGrep
    {
        static int _help = 0;
        static int _version = 0;
        static int _list = 0;
        static long _pos = 0;
        static string _fileName = string.Empty;
        static bool _isShowOffset = true;
        static string _expression = "";
        static string _separator = " ";/*바이트 문자열 분리 기호*/
        static int _lineWidth = 16;
        static int _limit = 0;
        static int _sector = 512;

        static void Run(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.AutoFlush = true;

            long fileSize = 0;
            Stream stream;

            OptionSet o = new OptionSet();
            o.Add("h|help",        "도움말",                   v => _help++)
             .Add("l|list",        "모든 물리 디스크 목록 출력",  v => _list++)
             .Add("v|version",     "시작 위치 변경",            v => _version++)
             .Add("hideoffset",    "Offset 출력 안함",         v => _isShowOffset = false)
             .Add("e=|regex=",     "정규표현식 (예시:  -e=\"\\x00\\x00\\x00\\x01\\x67\")", v => _expression = v)
             .Add("w=|width=",     "한줄에 표시할 바이트 문자열 개수 (미구현)", v => _lineWidth = int.Parse(v))
             .Add("n=|line=",      "출력할 라인 수 (기본값: 0(무제한))",      v => _limit = int.Parse(v))
             .Add("s=|position=",  "시작 위치 (단위: byte)",               v => _pos = int.Parse(v))
             .Add("t=|separator=", "바이트 문자열 분리 기호",                v => _separator = v)
                ;

            o.Parse(args);

            //
            // 단일 옵션 기능
            //
            if (0 < _help)
            {
                ShowHelp(o);
                return;
            }

            if (0 < _version)
            {
                ShowVersion();
                return;
            }

            if (0 < _list)
            {
                if (!IsAdministrator())
                {
                    Console.WriteLine("관리자 권한이 필요합니다. 터미널을 관리자권한으로 실행하세요.");
                    return;
                }

                List<string> physicalDiskNames = PhysicalDiskInfo.GetNames();
                foreach (var item in physicalDiskNames)
                {
                    try
                    {
                        PhysicalStorage ps = new PhysicalStorage(item);
                        Console.WriteLine($"{item}\t({ps.SizeHumanReadable}\t{ps.ModelNumber})");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

                return;
            }

            if (_lineWidth <= 0 || 8192 < _lineWidth)
            {
                Console.WriteLine("-width 옵션값이 범위를 벗어났습니다. 1부터 1024 사이의 값을 입력해 주세요.");
                return;
            }

            if (args.Length == 0)
            {
                ShowHelp(o);
                return;
            }

            _fileName = args[0];
            bool isPhysicalDisk = Dotnet.Regex.IsMatch(_fileName, @"\\\\.\\PHYSICALDRIVE[0-9]+");
            bool isFile = !isPhysicalDisk;
            bool hasRegexCondition = _expression == "" ? false : true;

            if (_fileName != string.Empty)
            {
                if (isPhysicalDisk)
                {
                    if (!IsAdministrator())
                    {
                        Console.WriteLine("관리자 권한이 필요합니다. 터미널을 관리자권한으로 실행하세요.");
                        return;
                    }

                    PhysicalStorage ps = new PhysicalStorage(_fileName);
                    _sector = ps.BytesPerSector;
                    stream = ps.OpenRead();
                    fileSize = ps.Size;

                }
                else
                {

                    string ext = Path.GetExtension(_fileName);
                    if (ext.ToLower() == ".e01")
                    {
                        EWFStorage storage = new EWFStorage(_fileName);
                        fileSize = storage.Size;
                        stream = storage.OpenRead();
                    }
                    else
                    {
                        DDImageStorage storage = new DDImageStorage(_fileName);
                        fileSize = storage.Size;
                        stream = storage.OpenRead();
                    }
                }

                if (fileSize <= _lineWidth)
                {
                    Console.WriteLine("width 옵션은 파일 크기보다 작아야 합니다.");
                    return;
                }

                stream.Position = _pos;

                if (hasRegexCondition)
                {
                    PrintStreamByRegex(stream, _lineWidth, _limit, _separator, _expression);
                }
                else if (!hasRegexCondition && isFile)
                {
                    PrintFileStream(stream, _lineWidth, _limit, _separator);
                }
                else if (!hasRegexCondition && isPhysicalDisk)
                {
                    PrintPhysicalStream(stream, _lineWidth, _limit, _separator, _sector);
                }

                stream.Close();

            }
            else
            {

                Console.WriteLine("파일명을 입력해 주세요.");
                return;
            }
        }



        private static void PrintPhysicalStream(Stream stream, int width, int limit, string separator, int sector)
        {
            // 버퍼가 512의 배수인 이유: 블록 디바이스는 한 번에 읽을 수 있는 최소 사이즈가 Sector 크기이다.
            //                        그런데 한 줄에 width만큼 잘라서 출력해야 하는 경우,
            //                        buffer.Length/width 연산에서 나머지가 생기면 버퍼를 잘라서 붙여야 하는데 로직이 복잡해 진다.
            //                        따라서 애초에 나머지가 생기지 않도록 width*sector 만큼 버퍼를 잡는다.
            byte[] buffer = new byte[width * sector];
            int bufferSize = buffer.Length;
            long pos = stream.Position;
            int numberOfRead;
            int line = 0;
            int nHexOffsetLength = string.Format("{0:X}", stream.Length).Length;
            string sHexOffsetLength = nHexOffsetLength.ToString();

            while ((numberOfRead = stream.Read(buffer, 0, bufferSize)) != 0)
            {
                int count = numberOfRead / 16;
                for (int i = 0; i < count; i++)
                {
                    line++;
                    string value = BitConverter.ToString(buffer, i * width, width).Replace("-", separator);
                    if (_isShowOffset == true)
                        Console.WriteLine(string.Format("{0:X" + sHexOffsetLength + "}h : {1}", pos, value));
                    else
                        Console.WriteLine(value);

                    // 종료 조건
                    if (limit != 0 && limit <= line)
                        return;

                    //상태 저장
                    pos += width;
                }
            }
        }



        private static void PrintStreamByRegex(Stream stream, int width, int limit, string separator, string expression)
        {
            byte[] buffer = new byte[100 * 1024];
            byte[] bufferValue = new byte[width];
            int BUFFER_SIZE = buffer.Length;
            int numberOfRead;
            int nHexOffsetLength = string.Format("{0:X}", stream.Length).Length;
            int line = 0;
            const long BUFFER_PADDING = 300;//<주의> 반드시 buffer[] 사이즈보다 작을 것
            string sHexOffsetLength = nHexOffsetLength.ToString();
            long tmpPos;
            string value;
            long offset;
            long latestOffset = -1;

            expression = ChangeDotToByteExpression(expression);

            while ((numberOfRead = stream.Read(buffer, 0, BUFFER_SIZE)) != 0)
            {
                //찌꺼기 제거
                if (numberOfRead != BUFFER_SIZE)
                {
                    Array.Clear(buffer, numberOfRead, BUFFER_SIZE - numberOfRead);
                }

                Re2.Net.MatchCollection matches = BinaryRegex.Matches(buffer, expression);
                foreach (Re2.Net.Match match in matches)
                {
                    if (buffer.Length >= match.Index + width)
                    {
                        value = BitConverter.ToString(buffer, match.Index, width).Replace("-", separator);
                        offset = stream.Position - numberOfRead + match.Index;
                    }
                    else
                    {
                        //
                        // 출력해야할 바이트가 buffer 배열 끝에 있는 경우 직접 읽어온다.
                        //
                        tmpPos = stream.Position;//push
                        stream.Position = stream.Position - buffer.Length + match.Index;
                        stream.Read(bufferValue, 0, width);
                        stream.Position = tmpPos;//pop

                        value = BitConverter.ToString(bufferValue, 0, width).Replace("-", separator);
                        offset = stream.Position - numberOfRead + match.Index;
                    }


                    //
                    // BUFFER_PADDING에 의한 중복 Hit 방지
                    //
                    if (0 < latestOffset && latestOffset == offset)
                    {
                        continue;
                    }
                    if (offset != latestOffset)
                    {
                        latestOffset = offset;
                    }


                    line++;

                    if (_isShowOffset == true)
                        Console.WriteLine(string.Format("{0:X" + sHexOffsetLength + "}h : {1}", offset, value));
                    else
                        Console.WriteLine($"{value}");
                }

                if (numberOfRead == buffer.Length)
                {
                    stream.Position = stream.Position - BUFFER_PADDING;
                }

                // 종료 조건
                if (limit != 0 && limit <= line)
                {
                    break;
                }
            }
        }

        private static void PrintFileStream(Stream stream, int width, int limit, string separator)
        {
            byte[] buffer = new byte[4 * 1024];
            long pos = stream.Position;
            int numberOfRead;
            int line = 0;
            int nHexOffsetLength = string.Format("{0:X}", stream.Length).Length;
            string sHexOffsetLength = nHexOffsetLength.ToString();

            while ((numberOfRead = stream.Read(buffer, 0, width)) != 0)
            {
                line++;
                string value = BitConverter.ToString(buffer, 0, numberOfRead).Replace("-", separator);
                if (_isShowOffset == true)
                    Console.WriteLine(string.Format("{0:X" + sHexOffsetLength + "}h : {1}", pos, value));
                else
                    Console.WriteLine(value);
                pos += width;

                if (limit != 0 && limit <= line)
                    break;
            }
        }



        private static void ShowHelp(OptionSet optionSet)
        {
            string name = GhostYak.Version.AssemblyTitle + ".exe";
            Console.WriteLine("");
            Console.WriteLine($"{name} <파일경로> [옵션]");
            Console.WriteLine("");
            Console.WriteLine("옵션:");
            Console.WriteLine(optionSet.GetOptionDescriptions());
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("정규표현식:");
            Console.WriteLine("");
            Console.WriteLine("  이 프로그램의 정규표현식은 C# 언어 및 Re2 정규표현식 라이브러리의 문법을 따릅니다.");
            Console.WriteLine("  예시) -regex='\\x00\\x00\\x00\\x01\\x67'");

            Console.WriteLine("");
            Console.WriteLine("Example 01 파일 내용을 HEX값으로 출력 : ");
            Console.WriteLine("");
            Console.WriteLine($"\t{name} 'C:\\path_to_file.txt'");
            Console.WriteLine($"\t{name} 'C:\\path_to_file.txt' -n=10 -w=32");

            Console.WriteLine("");
            Console.WriteLine("Example 02 파일 내용을 정규표현식으로 검색 : ");
            Console.WriteLine("");
            Console.WriteLine($"\t{name} 'C:\\path_to_file.txt' -e=\".{{60}}\\x00\\x00\\x00\\x01\\x67\" -n=10 -w=65");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Example 03 디스크 목록 출력 : ");
            Console.WriteLine("");
            Console.WriteLine($"\t{name} -l");
            Console.WriteLine("");
            Console.WriteLine("\t> \\\\.\\PHYSICALDRIVE0     (238GB  Samsung SSD 850 PRO 256GB)");
            Console.WriteLine("\t> \\\\.\\PHYSICALDRIVE1     (1.81TB TOSHIBA DT01ACA200)");

            Console.WriteLine("");
            Console.WriteLine("Example 04 디스크 내용을 정규표현식으로 검색 : ");
            Console.WriteLine("");
            Console.WriteLine($"\t{name} \"\\\\.\\PHYSICALDRIVE0\" -e=\".{{60}}\\x00\\x00\\x00\\x01\\x67\" -w=65");
            Console.WriteLine("");
        }

        private static void ShowVersion()
        {
            string name = GhostYak.Version.AssemblyTitle;
            string version = GhostYak.Version.AssemblyVersionBig2;
            string buildDate = GhostYak.Version.AssemblyBuildDate;

            Console.WriteLine($"{name} v{version} (BUILD {buildDate})");
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }

        private static string ChangeDotToByteExpression(string expression)
        {
            bool isWorking = true;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '[')
                {
                    isWorking = false;
                }
                if (expression[i] == ']')
                {
                    isWorking = true;
                }

                if (isWorking)
                {
                    if (0 == i && expression[i] == '.')
                    {
                        sb.Append("[\\x00-\\xFF]");
                    }
                    else if (1 < i && expression[i - 1] != '\\' && expression[i] == '.')
                    {
                        sb.Append("[\\x00-\\xFF]");
                    }
                    else
                    {
                        sb.Append(expression[i]);
                    }
                }
                else
                {
                    sb.Append(expression[i]);
                }
            }
            return sb.ToString();
        }

    }
}
