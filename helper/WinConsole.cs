using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace usbip_tunnel.helper
{
    public static class WinConsole
    {
        static public void Allocate(bool alwaysCreateNewConsole = true)
        {
            bool consoleAttached = true;
            if (alwaysCreateNewConsole || (NativeMethods.AttachConsole(NativeMethods.ATTACH_PARRENT) == 0 && Marshal.GetLastWin32Error() != NativeMethods.ERROR_ACCESS_DENIED))
            {
                consoleAttached = NativeMethods.AllocConsole() != 0;
            }

            if (consoleAttached)
            {
                InitializeOutStream();
                InitializeInStream();
            }
        }

        static public void Deallocate()
        {
            NativeMethods.FreeConsole();
        }

        private static void InitializeOutStream()
        {
            var fs = CreateFileStream("CONOUT$", NativeMethods.GENERIC_WRITE, NativeMethods.FILE_SHARE_WRITE, FileAccess.Write);
            if (fs != null)
            {
                var writer = new StreamWriter(fs, Encoding.GetEncoding(866)) { AutoFlush = true };
                Console.SetOut(writer);
                Console.SetError(writer);
            }
        }

        private static void InitializeInStream()
        {
            var fs = CreateFileStream("CONIN$", NativeMethods.GENERIC_READ, NativeMethods.FILE_SHARE_READ, FileAccess.Read);
            if (fs != null)
            {
                Console.SetIn(new StreamReader(fs, Encoding.GetEncoding(866)));
            }
        }

        private static FileStream CreateFileStream(string name, uint win32DesiredAccess, uint win32ShareMode, FileAccess dotNetFileAccess)
        {
            var file = new SafeFileHandle(NativeMethods.CreateFileW(name, win32DesiredAccess, win32ShareMode, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero), true);
            if (!file.IsInvalid) return new FileStream(file, dotNetFileAccess);
            return null;
        }
    }
}
