using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1OS
{
    class Program
    {
        static DriveInfo driveinfo = new DriveInfo();
        static DirectoryManager directoryManager = new DirectoryManager();
        static void Main(string[] args)
        {
            directoryManager.RemoveDirectory();
            Console.ReadKey();
        }
    }
}
