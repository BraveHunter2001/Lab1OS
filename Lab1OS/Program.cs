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
        static void Main(string[] args)
        {
            driveinfo.PrintAllDrives();
            foreach (var d in driveinfo.GetAllDrives())
            {
               
                driveinfo.PrintFullInfo(d);
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
