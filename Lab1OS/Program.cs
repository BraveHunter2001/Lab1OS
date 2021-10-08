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

			fileManager.CreateFile();
			Console.ReadKey();
		}
	}
}
