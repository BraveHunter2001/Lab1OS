using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab1OS
{
    class FileManager
    {
        [DllImport("kernel32.dll")]
        protected static extern uint GetLastError();

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern IntPtr CreateFile(string fileName,
                                      uint desiredAccess, uint shareMode, winapiFlags.SECURITY_ATTRIBUTES securAttrs,
                                      uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        
    }
}
