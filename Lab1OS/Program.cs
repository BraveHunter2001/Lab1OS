using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace Lab1OS
{
	class Program
	{
		static DriveInfo driveManager = new DriveInfo();
		static DirectoryManager directoryManager = new DirectoryManager();
		static FileManager fileManager = new FileManager();
		static IYNmessageBox YNmessageBox = new YNMessageBox();
		static AdaptiveMenu adaptiveDriveMenu = new AdaptiveMenu("Get hard drive details", new IMenuItem[] { });
		static Overlapped overlapped = new Overlapped();

		class Data
		{
			private uint blockSize;
			private int operations;
			private uint t;

			public uint BlockSize { get => blockSize; set => blockSize = value; }
			public int Operation { get => operations; set => operations = value; }
			public uint Time { get => t; private set => t = value; }


			public Data(uint blockSize, int operations, uint t)
			{
				this.blockSize = blockSize;
				this.operations = operations;
				this.t = t;
			}
		}

		static void Main(string[] args)
		{
			uint t = 0;
			string sourse = "d:\\test\\osnt.mp4";
			List<Data> dataTimeCopy = new List<Data>();

			for(int operations = 1; operations <= 16; operations *=2)
			{
				for (uint blockSize = 1; blockSize <= 256; blockSize *= 2)
				{
					string target = $"d:\\test\\osnt_{operations}_{blockSize}.mp4";
					t = overlapped.Copy(sourse, target, blockSize, operations);
					dataTimeCopy.Add(new Data(blockSize, operations, t));
					Console.WriteLine($"File name<{target}> Time = {t} Done");
					File.Delete(target);
				}
			}
			

			using (var streamWriter = new StreamWriter(@"c:\test\data.csv"))
			using (var csv = new CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture)) 
			{
				csv.WriteRecords(dataTimeCopy);
			}
			
			

			Console.ReadLine();
		}
	}
}
