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

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern bool CopyFile(string fromPathName, string toPathName, bool failIfExists);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern bool MoveFile(string fromPathName, string toPathName);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern bool MoveFileEx(string fromPathName, string toPathName, uint flags);

		protected readonly static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
		
		private string GetFilePath(string message = "Please, input full file path:")
		{
			Console.WriteLine(message);
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


		public void CopyFile(bool isOwerwrite, IYNmessageBox mes )
		{
			string fromFile = GetFilePath("Please, input from path file!");
			string toFile = GetFilePath("Please, input to path file!");

			if (fromFile == "" || toFile == "")
			{
				Console.WriteLine("You invalid input path");
				return;
			}

			if(CopyFile(fromFile,toFile,isOwerwrite))
            {
                Console.WriteLine("Copied file successfully!");
            }
            else 
			{
				uint err = GetLastError();
				if (!isOwerwrite)
					Console.WriteLine($"File copy failed. Error code: {err}");


				else if (err == 80 || err == 183 && mes.Show("File already exists. Overwrite?"))
				{
					if (CopyFile(fromFile, toFile, false))
						Console.WriteLine("File overwritten successfully");
					else
					{
						err = GetLastError();
						Console.WriteLine($"File copy failed. Error code: {err}");
					}
				}
				else
					Console.WriteLine($"File copy failed. Error code: {err}");
			}

		}
	

		public void MoveFile(bool isOwerwrite, IYNmessageBox mes)
        {
			string fromFile = GetFilePath("Please, input from path file!");
			string toFile = GetFilePath("Please, input to path file!");

			if (fromFile == "" || toFile == "")
			{
				Console.WriteLine("You invalid input path");
				return;
			}

			if (isOwerwrite)
			{
				if (MoveFile(fromFile, toFile))
					Console.WriteLine("File moved successfully");
				else
				{
					uint lastErr = GetLastError();
					if (lastErr == 80 || lastErr == 183 && mes.Show("File already exists. Overwrite?"))
						if (MoveFileEx(fromFile, toFile, (uint)MoveFlags.MOVEFILE_REPLACE_EXISTING
							| (uint)MoveFlags.MOVEFILE_COPY_ALLOWED))
							Console.WriteLine("File replaced successfully");
						else
						{
							lastErr = GetLastError();
							Console.WriteLine($"File replacement failed. Error code: {lastErr}");
						}
					else
						Console.WriteLine($"File move failed. Error code: {lastErr}");
				}
			}
			else if (MoveFile(fromFile, toFile))
				Console.WriteLine("File moved successfully");
			else
			{
				uint lastErr = GetLastError();
				Console.WriteLine($"File move failed. Error code: {lastErr}");
			}
	
		}
	
	}
}