using System;

namespace GhostYak.IO.DeviceIOControl.Objects.Enums
{
    /// <summary>
    /// File attributes are metadata values stored by the file system on disk and are used by the system and are available to developers via various file I/O APIs.
    /// </summary>
    [Flags]
    //[CLSCompliant(false)]
    public enum FileAttributesEx : uint
    {
        /// <summary>
        /// 파일에는 읽기 전용입니다.
        /// </summary>
        Readonly = 0x00000001,

        /// <summary>
        /// 파일 또는 디렉토리가 숨겨지고 따라서 일반 디렉터리 목록에 포함 되지 않습니다.
        /// </summary>
        Hidden = 0x00000002,

        /// <summary>
        /// 파일이 시스템 파일입니다. 즉, 파일이 운영 체제의 일부 이거나 운영 체제에서 독점적으로 사용 됩니다.
        /// </summary>
        System = 0x00000004,

        /// <summary>
        /// 파일 디렉터리입니다.
        /// </summary>
        Directory = 0x00000010,

        /// <summary>
        /// 파일에 백업 또는 제거에 대 한 후보입니다.
        /// </summary>
        Archive = 0x00000020,

        /// <summary>
        /// 나중에 사용하기 위해 예약되어 있습니다.
        /// </summary>
        Device = 0x00000040,

        /// <summary>
        /// 파일이 표준 파일을 특별 한 특성이 없습니다. 이 특성은 단독 사용 하는 경우에 사용할 수 있습니다.
        /// </summary>
        Normal = 0x00000080,

        /// <summary>
        ///     임시 파일이입니다. 임시 파일에는 응용 프로그램 실행 하지만 응용 프로그램이 완료 된 후에 필요 하지 않으므로 하는 동안 필요한 데이터를
        ///     포함 합니다. 파일 시스템에 빠르게 액세스 하지 않고 데이터를 플러시하는 메모리에 데이터를 다시 대용량 저장 장치에 모든 유지 하려고 합니다.
        ///     임시 파일을 삭제할 응용 프로그램에서 더 이상 필요 없는으로 합니다.
        /// </summary>
        Temporary           = 0x00000100,

        /// <summary>
        /// 파일이 스파스 파일입니다. 스파스 파일은 일반적으로 큰 파일 대부분 0 인 데이터 구성 됩니다.
        /// </summary>
        SparseFile          = 0x00000200,

        /// <summary>
        /// 파일에는 파일 또는 디렉터리와 연결 된 사용자 정의 데이터 블록을 재분석 지점이 포함 되어 있습니다.
        /// </summary>
        ReparsePoint        = 0x00000400,

        /// <summary>
        /// 압축 된 파일 또는 디렉토리. 파일의 경우 파일의 모든 데이터가 압축됩니다. 디렉토리의 경우, 새로 작성된 파일 및 서브 디렉토리의 경우 압축이 기본값입니다.
        /// </summary>
        Compressed          = 0x00000800,

        /// <summary>
        /// 파일의 데이터를 즉시 사용할 수 없습니다. 이 속성은 파일 데이터가 물리적으로 오프라인 스토리지로 옮겨졌음을 가리킵니다.
        /// 이 속성은 계층적인 저장소 관리 소프트웨어인 원격 저장소에서 사용됩니다.
        /// 응용 프로그램은이 속성을 임의로 변경하면 안됩니다.
        /// </summary>
        Offline             = 0x00001000,

        /// <summary>
        /// The file or directory is not to be indexed by the content indexing service.
        /// </summary>
        NotContentIndexed   = 0x00002000,

        /// <summary>
        /// A file or directory that is encrypted. For a file, all data streams in the file are encrypted. For a directory, encryption is the default for newly created files and subdirectories.
        /// </summary>
        Encrypted           = 0x00004000,

        //
        // 요약:
        //     파일 또는 디렉터리에는 데이터 무결성 지원이 포함 됩니다. 이 값이 파일에 적용 되 면 모든 데이터 스트림 파일에는 무결성 지원 합니다.
        //     이 값이 기본적으로 디렉터리, 모든 새 파일 및 해당 디렉터리 내에서 하위 디렉터리에 적용 될 때 무결성 지원이 포함 됩니다.
        IntegrityStream     = 0x00008000,

        /// <summary>
        /// This value is reserved for system use.
        /// </summary>
        Virtual             = 0x00010000,
        //
        // 요약:
        //     파일 또는 디렉터리 데이터 무결성 검사에서 제외 됩니다. 기본적으로이 값을 디렉터리에 적용 되는 모든 새 파일 및 하위 디렉터리 내에서 데이터
        //     무결성에서 제외 됩니다.
        NoScrubData         = 0x00020000,

        
    }
}