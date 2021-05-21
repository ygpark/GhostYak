using System.ComponentModel;

namespace GhostYak.IO.DeviceIOControl.Utilities
{
    internal static class Utils
    {
        public static string GetWin32ErrorMessage(int errorCode)
        {
            return new Win32Exception(errorCode).Message;
        }
    }
}
