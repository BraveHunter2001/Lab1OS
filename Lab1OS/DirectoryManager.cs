using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using winapiFlags;

namespace Lab1OS
{
    class DirectoryManager
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        static extern bool CreateDirectory(string pathName, SECURITY_ATTRIBUTES securAttr);


        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        static extern bool RemoveDirectory(string pathName);

        public void CreateDirectory()
        {
            Console.WriteLine("Input directory full path name for creating");
            string path = Console.ReadLine();

            if (CreateDirectory(path, null))
            {
                Console.WriteLine($"Directory {path} was created");
            } else
            {
                Console.WriteLine($"Error creating directory {path}");
            }
        }

        public void RemoveDirectory()
        {
            Console.WriteLine("Input directory full path name for deleting");
            string path = Console.ReadLine();
            if (RemoveDirectory(path))
                Console.WriteLine($"Directory {path} was deleted");
            else
                Console.WriteLine($"Error deleting directory {path}");
        }


    }
}

