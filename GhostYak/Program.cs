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

namespace GhostYak
{
    class Program
    {
        static int _debug = 0;

        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.AutoFlush = true;

            int _help = 0;
            int _list = 0;
            long _pos = 0;
            string _fileName = string.Empty;
            string _expression = string.Empty;
            int _lineWidth = 100;
            
            long fileSize = 0;
            Stream stream;
            
            

            OptionSet o = new OptionSet();
            o.Add("h|help", "도움말", v => _help++)
                .Add("l|list", "모든 물리 디스크 목록 출력", v => _list++)
                .Add("v|version", "시작 위치 변경", v => ShowVersion())
                .Add("i|ifile", "입력 파일 이름 또는 물리 디스크 이름", v => _fileName = v.Trim())
                .Add("e|regex", "정규표현식. -e=\"regular expression\"", v => _expression = v)
                .Add("w|width", "한줄에 표시할 바이트 문자열 개수 (기본값: 100)", v => _lineWidth = int.Parse(v))
                .Add("p|position", "시작 위치 변경", v => _pos = int.Parse(v))
                .Add("d|debug", "디버그 모드", v => _debug++)
            ;

            o.Parse(args);

            if (0 < _help) {
                ShowHelp(o);
                return;
            }

            if (0 < _list) {
                foreach (var item in PhysicalDiskInfo.GetNames())
                {
                    try
                    {
                        PhysicalStorage ps = new PhysicalStorage(item);
                        Console.WriteLine($"{item}\t({ps.SizeHumanReadable}\t{ps.ModelNumber})");
                    }
                    catch(Exception) 
                    {
                    }
                }

                return;
            }

            if(_fileName != string.Empty) {
                
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

                while ( stream.Read(buffer, 0, BUFFER_SIZE) != 0)
                {
                    Google.MatchCollection matches = BinaryRegex.Matches(buffer, _expression);
                    foreach (Google.Match match in matches)
                    {
                        newHitPos = stream.Position - buffer.Length + match.Index;
                        if (newHitPos <= lastHitPos)
                            continue;

                        //
                        // 정규식은 hit했는데, BitConverter.ToString()에서 LINE_WIDTH가 배열 범위를 이탈하는 경우 hit 지점부터 다시 Read한다.
                        // BufferOverflow 오류 방지 목적
                        //
                        if (buffer.Length < match.Index + LINE_WIDTH)
                        {
                            lastHitPos = stream.Position - buffer.Length + match.Index;
                            stream.Position = lastHitPos;
                            break;
                        }
                        
                        string value = BitConverter.ToString(buffer, match.Index, LINE_WIDTH).Replace("-", " ");
                        Console.WriteLine(value);

                        lastHitPos = stream.Position - buffer.Length + match.Index;
                        stream.Position = stream.Position - BUFFER_PADDING;
                    }
                }

                


                stream.Close();
            }
        }


        private static void ShowHelp(OptionSet optionSet)
        {
            string name = Version.AssemblyTitle;
            Console.WriteLine($"{name} [Options]");
            Console.WriteLine("Options:");
            Console.WriteLine(optionSet.GetOptionDescriptions());
            Console.WriteLine("");
            Console.WriteLine("-regex (정규표현식):");
            Console.WriteLine("  이 프로그램의 정규표현식은 C# 언어 및 Re2 정규표현식 라이브러리의 문법을 따릅니다.");
            Console.WriteLine("  예시) -regex=\"\\x00\\x00\\x00\\x01\\x67\"");
            Console.WriteLine("");
            Console.WriteLine("Example 01. 디스크 목록 출력 : ");
            Console.WriteLine($"{name} -l");

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
    }
}
