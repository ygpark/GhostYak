namespace GhostYak.IO.DeviceIOControl.Objects.FileSystem
{
    public class FileSystemStats
    {
        public FILESYSTEM_STATISTICS Stats { get; set; }
        public IFSStats FSStats { get; set; }
    }
}