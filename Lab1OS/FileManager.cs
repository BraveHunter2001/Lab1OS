using System;
using System.Runtime.InteropServices;
using winapiFlags;


namespace Lab1OS
{
	class FileManager
	{
        #region DLLImport_for_WinAPI
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

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern uint GetFileAttributes(string fileName);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern bool SetFileAttributes(string fileName, uint fileAttributes);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern bool GetFileInformationByHandle(IntPtr fileHandle, BY_HANDLE_FILE_INFORMATION fileInfo);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern bool GetFileTime(IntPtr fileHandle, out FILETIME creationTime,
									   out FILETIME lastAccessTime, out FILETIME lastWriteTime);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		protected static extern bool SetFileTime(IntPtr fileHandle, in FILETIME creationTime,
							   in FILETIME lastAccessTime, in FILETIME lastWriteTime);


		#endregion


		protected readonly static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);


		protected string GetFilePath(string message = "Please, input full file path:")
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
		
		const uint INVALID_FILE_ATTRIBUTES = uint.MaxValue;
		public void PrintFileAttributes(string pathFile)
        {
			uint attrs = GetFileAttributes(pathFile);
			if (attrs != INVALID_FILE_ATTRIBUTES)
            {
				foreach (var att in Helper.ParseFlags<winapiFlags.FileAttributes>(attrs))
					Console.WriteLine("\t-" + att);
			
			}else
            {
                Console.WriteLine($"[ERROR] code {GetLastError()}");
            }

		}

		public void SetFileAttributes(string pathFile, winapiFlags.FileAttributes setAttribute)
		{
			if (SetFileAttributes( pathFile, (uint)setAttribute))
				Console.WriteLine("File attribute set successfully!");
			else
				Console.WriteLine($"[Error] setting attribute. Error code {GetLastError()}");
		}
		bool GetFileReadHandle(string pathPath, out IntPtr handle)
		{
			uint desAcc = (uint)DesiredAccess.GENERIC_READ;
			uint shareMode = (uint)ShareMode.FILE_SHARE_READ;

			handle = CreateFile(pathPath, desAcc, shareMode,
						null, (uint)CreationDisposition.OPEN_EXISTING, (uint)winapiFlags.FileAttributes.FILE_ATTRIBUTE_NORMAL,
						IntPtr.Zero);
			if (handle == INVALID_HANDLE_VALUE)
			{
				Console.WriteLine($"[Error] opening file. Error code: {GetLastError()}");
				return false;
			}
			return true;
		}

		bool GetFileWriteHandle(string pathPath, out IntPtr handle)
		{
			uint desAcc = (uint)DesiredAccess.GENERIC_WRITE | (uint)DesiredAccess.GENERIC_READ;
			uint shareMode = (uint)ShareMode.FILE_SHARE_READ | (uint)ShareMode.FILE_SHARE_WRITE | (uint)ShareMode.FILE_SHARE_DELETE;

			handle = CreateFile(pathPath, desAcc,shareMode,
						null, (uint)CreationDisposition.OPEN_EXISTING, (uint)winapiFlags.FileAttributes.FILE_ATTRIBUTE_NORMAL,
						IntPtr.Zero);
			if (handle == INVALID_HANDLE_VALUE)
			{
				Console.WriteLine($"[Error] opening file. Error code: {GetLastError()}");
				return false;
			}
			return true;
		}


		public void PrintFileAttributesByHandle(string pathPath)
		{
			IntPtr handle;
			if (!GetFileReadHandle(pathPath, out handle))
				return;

			BY_HANDLE_FILE_INFORMATION fileInfo = new BY_HANDLE_FILE_INFORMATION();

			if (GetFileInformationByHandle(handle, fileInfo))
			{
				Console.WriteLine("File attributes:");
				foreach (var arr in Helper.ParseFlags<winapiFlags.FileAttributes>(fileInfo.fileAttributes))
					Console.WriteLine("\t-" + arr);
				Console.WriteLine($"Volume serial: {fileInfo.volumeSerialNumber}");
				Console.WriteLine($"Filesize high id: {fileInfo.fileSizeHigh}");
				Console.WriteLine($"Filesize low id: {fileInfo.fileSizeLow}");
				Console.WriteLine($"File index high id: {fileInfo.fileIndexHigh}");
				Console.WriteLine($"File index low id: {fileInfo.fileIndexLow}");
				Console.WriteLine($"File links number: {fileInfo.numberOfLinks}");
			}
			else
				Console.WriteLine($"[Error]reading file attributes. Error code: {GetLastError()}");
			CloseHandle(handle);
		}

		public void PrintFileTimeAttributes(string pathPath)
		{
			IntPtr handle;
			if (!GetFileReadHandle(pathPath, out handle))
				return;
			FILETIME creationTime, accessTime, writeTime;
			if (GetFileTime(handle, out creationTime, out accessTime, out writeTime))
			{
				Console.WriteLine($"File creation time: {creationTime.FileTimeToString()}");
				Console.WriteLine($"Last file access time: {accessTime.FileTimeToString()}");
				Console.WriteLine($"Last file write time: {writeTime.FileTimeToString()}");
			}
			else
				Console.WriteLine($"[Error] reading file attributes. Error code: {GetLastError()}");
			CloseHandle(handle);
		}

		public void SetFileTimeAttributes(string pathPath)
		{
			IntPtr handle;
			if (!GetFileWriteHandle(pathPath, out handle))
				return;
			var t = Helper.GetSystemFileTime();
			if (SetFileTime(handle, in t, in t, in t))
				Console.WriteLine("File time attributes set successfully");
			else
				Console.WriteLine($"[Error] writing file time attributes. Error code: {GetLastError()}");
			CloseHandle(handle);
		}

	}
}