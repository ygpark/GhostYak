using System;
using System.Collections.Generic;
using System.Text;

namespace GhostYak.IO.DeviceIOControl.Objects.Enums
{
    //
    // 요약:
    //     Defines constants for read, write, or read/write access to a file.
    [Flags]
    public enum FileAccessEx
    {
        /// <summary>
        /// IOCTL에 사용하기 위해 추가하였음. System.IO.FileAccess 기반
        /// </summary>
        Any = 0,
        /// <summary>
        ///     Read access to the file. Data can be read from the file. Combine with Write for
        ///     read/write access.
        /// </summary>
        Read = 1,
        //
        // 요약:
        //     Write access to the file. Data can be written to the file. Combine with Read
        //     for read/write access.
        Write = 2,
        //
        // 요약:
        //     Read and write access to the file. Data can be written to and read from the file.
        ReadWrite = 3
    }
}
