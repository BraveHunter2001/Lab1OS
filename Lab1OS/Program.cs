using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1OS
{
	class Program
	{
		static DriveInfo driveManager = new DriveInfo();
		static DirectoryManager directoryManager = new DirectoryManager();
		static FileManager fileManager = new FileManager();
		

		
		static void Main(string[] args)
		{

			fileManager.PrintFileTimeAttributes("C:\\Users\\Илья\\Videos\\2021-10-08 13-50-23.mp4");
			Console.ReadKey();
		}
	}
}
