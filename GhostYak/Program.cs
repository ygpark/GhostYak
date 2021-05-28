using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Original = System.Text.RegularExpressions;
using Google = Re2.Net;
using GhostYak.IO;
using GhostYak.Text;
using GhostYak.Text.RegularExpressions;
using GhostYak.IO.RawDiskDrive;
using GhostYak.IO.CommandLine.Options;
using System.Diagnostics;
using System.Security.Principal;

namespace GhostYak
{
    class Program
    {
        static int _debug = 0;
        static int _help = 0;
        static int _version = 0;
        static int _list = 0;
        static long _pos = 0;
        static string _fileName = string.Empty;
        static string _address_bool = "true";
        static string _expression = "";
        static string _separator = " ";/*바이트 문자열 분리 기호*/
        static int _lineWidth = 16;

        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.AutoFlush = true;

            if (!IsAdministrator())
            {
                Console.WriteLine("관리자 권한이 필요합니다. 터미널을 관리자권한으로 실행하세요.");
                return;
            }


            long fileSize = 0;
            Stream stream;

            OptionSet o = new OptionSet();
            o.Add("h|help", "도움말", v => _help++)
                .Add("l|list", "모든 물리 디스크 목록 출력", v => _list++)
                .Add("v|version", "시작 위치 변경", v => _version++)
                .Add("d|debug", "디버그 모드", v => _debug++)
                //.Add("i|ifile", "입력 파일 이름 또는 물리 디스크 이름", v => _fileName = v.Trim())
                .Add("e=|regex=", "정규표현식 (예시:  -e=\"\\x00\\x00\\x00\\x01\\x67\")", v => _expression = v)
                .Add("n=|length=", "한줄에 표시할 바이트 문자열 개수 (기본값: 16)", v => _lineWidth = int.Parse(v))
                .Add("p=|position=", "시작 위치 (단위: byte)", v => _pos = int.Parse(v))
                .Add("a=|address=", "주소 표시 설정 ( -a=true 또는 -a=false )", v => _address_bool = v)
                .Add("s=|separator=", "바이트 문자열 분리 기호", v => _separator = v)
                ;
            
            o.Parse(args);

            //
            // 단일 옵션 기능
            //
            if (0 < _help) {
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
                List<string> physicalDiskNames = PhysicalDiskInfo.GetNames();
                foreach (var item in physicalDiskNames)
                {
                    try
                    {
                        PhysicalStorage ps = new PhysicalStorage(item);
                        Console.WriteLine($"{item}\t({ps.SizeHumanReadable}\t{ps.ModelNumber})");
                    }
                    catch(Exception e) 
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

                return;
            }

            //
            // 파일명 필수 기능
            //
            if(args.Length == 0)
            {
                ShowHelp(o);
                return;
            }

            _fileName = args[0];

            if (_fileName != string.Empty) {
                
                if(Original.Regex.IsMatch(_fileName, @"\\\\.\\PHYSICALDRIVE[0-9]+")) {
                    PhysicalStorage ps = new PhysicalStorage(_fileName);
                    stream = ps.OpenRead();
                    fileSize = ps.Size;
                } else {
                    string ext = Path.GetExtension(_fileName);
                    if(ext.ToLower() == ".e01") {
                        EWFStorage storage = new EWFStorage(_fileName);
                        fileSize = storage.Size;
                        stream = storage.OpenRead();
                    } else {
                        DDImageStorage storage = new DDImageStorage(_fileName);
                        fileSize = storage.Size;
                        stream = storage.OpenRead();
                    }
                }
                
                if (fileSize <= _lineWidth) {
                    Console.WriteLine("width 옵션은 파일 크기보다 작아야 합니다.");
                    return;
                }

                /// BUFFER_PADDING : 다음 버퍼를 읽어올 때 BUFFER_PADDING 만큼 돌아가서 읽어온다.
                ///                  정규식이 실제로 커버하는 문자열의 길이. CCTV용 정규식은 보통 150~200정도 나온다.
                const long BUFFER_PADDING = 200;

                /// BUFFER_SIZE : 파일 사이즈
                const int BUFFER_SIZE = 1024 * 1024;
                byte[] buffer = new byte[BUFFER_SIZE];

                /// LINE_WIDTH : 한줄에 출력할 바이트 문자열 개수
                int LINE_WIDTH = _lineWidth;

                long lastHitPos = 0;
                long newHitPos = 0;

                stream.Position = _pos;


                if(_expression == "")
                {
                    long pos = stream.Position;
                    int BYTES_PER_LINE = 16;
                    int numberOfRead;
                    while ( (numberOfRead = stream.Read(buffer, 0, BYTES_PER_LINE)) != 0)
                    {

                        string value = BitConverter.ToString(buffer, 0, numberOfRead).Replace("-", " ");

                        if ( pos <= 0xFFFFFFFF)
                            Console.WriteLine($"{pos:X8}h : {value}");
                        else
                            Console.WriteLine($"{pos:X16}h : {value}");

                        pos += BYTES_PER_LINE;
                    }
                }
                else
                {
                    long startOffsetToRead = stream.Position;
                    while (stream.Read(buffer, 0, BUFFER_SIZE) != 0)
                    {
                        
                        Google.MatchCollection matches = BinaryRegex.Matches(buffer, _expression);
                        foreach (Google.Match match in matches)
                        {
                            newHitPos = startOffsetToRead + match.Index;
                            if (newHitPos <= lastHitPos)
                                continue;

                            //
                            // 정규식은 hit했는데, BitConverter.ToString()에서 LINE_WIDTH가 배열 범위를 이탈하는 경우 hit 지점부터 다시 Read한다.
                            // BufferOverflow 오류 방지 목적
                            //
                            if (buffer.Length < match.Index + LINE_WIDTH)
                            {
                                lastHitPos = startOffsetToRead + match.Index;
                                stream.Position = lastHitPos;
                                break;
                            }

                            string value = BitConverter.ToString(buffer, match.Index, LINE_WIDTH).Replace("-", _separator);
                            long displayOffset = startOffsetToRead + match.Index;
                            if (_address_bool != "false")
                            {
                                if(displayOffset <= 0xFFFFFFFF)
                                {
                                    Console.WriteLine($"{displayOffset:X8}h : {value}");
                                } else {
                                    Console.WriteLine($"{displayOffset:X16}h : {value}");
                                }
                                
                                
                            }
                            else
                            {
                                Console.WriteLine($"{value}");
                            }

                            lastHitPos = startOffsetToRead + match.Index;
                        }
                        //상태 저장
                        stream.Position = stream.Position - BUFFER_PADDING;
                        startOffsetToRead = stream.Position;
                    }
                }

                

                


                stream.Close();
            }
        }


        private static void ShowHelp(OptionSet optionSet)
        {
            string name = Version.AssemblyTitle;
            Console.WriteLine("");
            Console.WriteLine($"{name} <파일경로> [Options]");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine(optionSet.GetOptionDescriptions());
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("정규표현식:");
            Console.WriteLine("");
            Console.WriteLine("  이 프로그램의 정규표현식은 C# 언어 및 Re2 정규표현식 라이브러리의 문법을 따릅니다.");
            Console.WriteLine("  예시) -regex=\"\\x00\\x00\\x00\\x01\\x67\"");

            Console.WriteLine("");
            Console.WriteLine("Example 01 파일 내용을 HEX값으로 출력 : ");
            Console.WriteLine("");
            Console.WriteLine($"\t{name} \"C:\\path_to_file.txt\" -n=16");
            Console.WriteLine($"\t{name} \"C:\\path_to_file.txt\" -n=32");

            Console.WriteLine("");
            Console.WriteLine("Example 02 파일 내용을 정규표현식으로 검색 : ");
            Console.WriteLine("");
            Console.WriteLine($"\t{name} \"C:\\path_to_file.txt\" -e=\"\\x00\\x00\\x00\\x01\\x67\" -n=100");
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
            Console.WriteLine($"\t{name} \"\\\\.\\PHYSICALDRIVE0\" -e=\"\\x00\\x00\\x00\\x01\\x67\" -n=100");
            Console.WriteLine("");

        }

        private static void ShowVersion()
        {
            Console.WriteLine($"{Version.AssemblyTitle} v{Version.AssemblyVersionBig2}");
        }

        private static void Trace(string txt)
        {
            if(0 < _debug)
            {
                Console.WriteLine($"  [Trace] {txt}");
                Console.Out.Flush();
            }
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

    }
}
