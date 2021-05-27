using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GhostYak.IO.RawDiskDrive
{
    public class EWFStorage : StorageBase
    {
        private EWF.Handle _handle;
        private string[] _multiPath;
        public EWFStorage(string path) : base(path)
        {
            _handle = new EWF.Handle();
            _multiPath = GetEWFBro(path);
            _handle.Open(_multiPath, 1);

            BytesPerSector = (int)_handle.GetBytesPerSector();
            Size = (long)_handle.GetMediaSize();
            _handle.Dispose();
        }

        private string[] GetEWFBro(string path)
        {
            Regex regex;
            List<string> list = new List<string>();
            //string E01 = @"\.(E[0-9]{2})|([E-Z][A-Z]{2})";
            //string Ex01 = @"\.E(x[0-9]{2})|([x-z][A-Z]{2})";
            //string L01 = @"\.(L[0-9]{2})|([L-Z][A-Z]{2})";
            //string Lx01 = @"\.L(x[0-9]{2})|([x-z][A-Z]{2})";
            
            /* 수정사항 : 2020. 9. 22. 대소문자 구분안함 (Case ignore.)  */
            string E01 = "^\\.([Ee][0-9]{2})|([E-Ze-z][A-Za-z]{2})$";
            string Ex01 = "^\\.[Ee]([Xx][0-9]{2})|([X-Zx-z][A-Za-z]{2})$";
            string L01 = "^\\.([Ll][0-9]{2})|([L-Zl-z][A-Za-z]{2})$";
            string Lx01 = "^\\.[Ll]([Xx][0-9]{2})|([X-Zx-z][A-Za-z]{2})$";

            string dirName = System.IO.Path.GetDirectoryName(path);
            string fileName = System.IO.Path.GetFileName(path);
            string fileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(path);
            string ext = System.IO.Path.GetExtension(path);
            int len_ext = 0;
            char[] magic = new char[3];

            switch (ext.ToLower())
            {
                case ".e01":
                    regex = new Regex(E01);
                    len_ext = 4;
                    break;
                case ".l01":
                    regex = new Regex(L01);
                    len_ext = 4;
                    break;
                case ".ex01":
                    regex = new Regex(Ex01);
                    len_ext = 5;
                    break;
                case ".lx01":
                    regex = new Regex(Lx01);
                    len_ext = 5;
                    break;
                default:
                    throw new ArgumentException("EWF형식(E01, Ex01, L01, Lx01)의 확장자가 아닙니다.");
            }

            string[] files = Directory.GetFiles(dirName, $"{fileNameNoExt}.*");
            foreach (var file in files)
            {
                string ext1 = System.IO.Path.GetExtension(file);

                // 유효성 검사 : 확장자 길이
                if (ext1.Length != len_ext)
                    continue;

                // 유효성 검사 : EWF 확장자 네이밍1.. 정규식으로 필터(https://github.com/libyal/libewf/blob/master/documentation/Expert%20Witness%20Compression%20Format%20(EWF).asciidoc)
                bool success = regex.IsMatch(System.IO.Path.GetExtension(ext1));
                if (!success)
                {
                    continue;
                }

                // 유효성 검사 : EWF 확장자 네이밍2.. 정규식으로 필터링 안되는 것 (https://github.com/libyal/libewf/blob/master/documentation/Expert%20Witness%20Compression%20Format%20(EWF).asciidoc)
                if (ext == ".e00" || ext == ".ex00" || ext == ".l00" || ext == ".lx00")
                {
                    continue;
                }

                // 유효성 검사 : Magic 'EVF'
                using (StreamReader sr = new StreamReader(file))
                {
                    //reset buffer
                    magic[0] = char.MinValue;
                    magic[1] = char.MinValue;
                    magic[2] = char.MinValue;
                    //read
                    sr.Read(magic, 0, 3);
                    if( !(magic[0] == 'E' && magic[1] == 'V' && magic[2] == 'F') )
                    {
                        continue;
                    }
                }
                        
                    list.Add(file);
            }
            return list.ToArray();
        }

        public string FilePath { get => System.IO.Path.GetDirectoryName(Path); }

        public override Stream OpenRead()
        {
            return new DiskImageStream(_multiPath);
        }
    }
}