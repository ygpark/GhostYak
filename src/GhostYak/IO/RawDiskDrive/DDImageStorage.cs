using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GhostYak.IO.RawDiskDrive
{
    /// <summary>
    /// 경고
    /// </summary>
    public class DDImageStorage : StorageBase
    {
        public DDImageStorage(string path) : base(path)
        {
            Size = new FileInfo(path).Length;
            BytesPerSector = 0;
        }

        public string FilePath { get => System.IO.Path.GetDirectoryName(Path); }

        public override Stream OpenRead()
        {
            return new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
