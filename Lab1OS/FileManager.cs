using System;
using System.Runtime.InteropServices;
using winapiFlags;


namespace Lab1OS
{
	class FileManager
	{
		[DllImport("kernel32.dll")]
		protected static extern uint GetLastError();

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
		protected static extern bool CloseHandle(IntPtr hObject);


		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi,  CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern IntPtr CreateFile(string fileName,
									  uint desiredAccess, uint shareMode, SECURITY_ATTRIBUTES securityAttributes,
									  uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

		

	

		protected readonly static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
		
		private string GetFilePath()
		{
			Console.WriteLine("Please, input full file path:");
			return Console.ReadLine();
		}
		public void CreateFile()
		{
			string fileName = "";

			fileName = GetFilePath();

			if (fileName == "")
				return;

			uint desAcc = (uint)DesiredAccess.GENERIC_WRITE | (uint)DesiredAccess.GENERIC_READ;
			uint shareMode = (uint)ShareMode.FILE_SHARE_READ | (uint)ShareMode.FILE_SHARE_WRITE | (uint)ShareMode.FILE_SHARE_DELETE;
			 

			var handle = CreateFile(fileName, desAcc,shareMode, null, (uint)CreationDisposition.CREATE_ALWAYS,
				(uint)FileAttributes.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);


			if (handle != INVALID_HANDLE_VALUE)
			{
				Console.WriteLine($"this file {fileName} created successfully.");
				CloseHandle(handle);
			}
			else
			{
				Console.WriteLine($"[ERROR] Invalid creating file. Error code:{GetLastError()}");
			}
		}

	
	
	}
}