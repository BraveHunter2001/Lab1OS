using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace winapiFlags
{
	public enum DriveType
	{
		UNKNOWN_TYPE,
		INVALID_PATH,
		DRIVE_REMOVABLE,
		DRIVE_FIXED,
		DRIVE_REMOTE,
		DRIVE_CDROM,
		DRIVE_RAMDISK
	}

	public enum FileSystemFlags: uint
	{
		FILE_CASE_PRESERVED_NAMES = 0x00000002,
		FILE_CASE_SENSITIVE_SEARCH = 0x00000001,
		FILE_FILE_COMPRESSION = 0x00000010,
		FILE_NAMED_STREAMS = 0x00040000,
		FILE_PERSISTENT_ACLS = 0x00000008,
		FILE_READ_ONLY_VOLUME = 0x00080000,
		FILE_SEQUENTIAL_WRITE_ONCE = 0x00100000,
		FILE_SUPPORTS_ENCRYPTION = 0x00020000,
		FILE_SUPPORTS_EXTENDED_ATTRIBUTES = 0x00800000,
		FILE_SUPPORTS_HARD_LINKS = 0x00400000,
		FILE_SUPPORTS_OBJECT_IDS = 0x00010000,
		FILE_SUPPORTS_OPEN_BY_FILE_ID = 0x01000000,
		FILE_SUPPORTS_REPARSE_POINTS = 0x00000080,
		FILE_SUPPORTS_SPARSE_FILES = 0x00000040,
		FILE_SUPPORTS_TRANSACTIONS = 0x00200000,
		FILE_SUPPORTS_USN_JOURNAL = 0x02000000,
		FILE_UNICODE_ON_DISK = 0x00000004,
		FILE_VOLUME_IS_COMPRESSED = 0x00008000,
		FILE_VOLUME_QUOTAS = 0x00000020,
		FILE_SUPPORTS_BLOCK_REFCOUNTING = 0x08000000
	}

	[StructLayout(LayoutKind.Sequential)]
	public class SECURITY_ATTRIBUTES
	{
		public uint length;
		public IntPtr securityDescriptor;
		public bool inheritHandle;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class SYSTEMTIME
	{
		public ushort year;
		public ushort month;
		public ushort dayOfWeek;
		public ushort day;
		public ushort hour;
		public ushort minutes;
		public ushort seconds;
		public ushort milliseconds;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class BY_HANDLE_FILE_INFORMATION
	{
		public uint fileAttributes;
		public FILETIME creationTime;
		public FILETIME lastAccessTime;
		public FILETIME lastWriteTime;
		public uint volumeSerialNumber;
		public uint fileSizeHigh;
		public uint fileSizeLow;
		public uint numberOfLinks;
		public uint fileIndexHigh;
		public uint fileIndexLow;
	}

	[Flags]
	public enum MoveFlags : uint
	{
		MOVEFILE_REPLACE_EXISTING = 1,
		MOVEFILE_COPY_ALLOWED = 2,
		MOVEFILE_DELAY_UNTIL_REBOOT = 4,
		MOVEFILE_WRITE_THROUGH = 8,
		MOVEFILE_CREATE_HARDLINK = 16,
		MOVEFILE_FAIL_IF_NOT_TRACKABLE = 32
	}

	[Flags]
	public enum ShareMode : uint
	{
		None = 0,
		FILE_SHARE_READ = 1,
		FILE_SHARE_WRITE = 2,
		FILE_SHARE_DELETE = 4
	}

	public enum CreationDisposition : uint
	{
		CREATE_NEW = 1,
		CREATE_ALWAYS = 2,
		OPEN_EXISTING = 3,
		OPEN_ALWAYS = 4,
		TRUNCATE_EXISTING = 5
	}

	public enum FileAttributes : uint
	{
		FILE_ATTRIBUTE_READONLY = 0x1,
		FILE_ATTRIBUTE_HIDDEN = 0x2,
		FILE_ATTRIBUTE_SYSTEM = 0x4,
		FILE_ATTRIBUTE_ARCHIVE = 0x20,
		FILE_ATTRIBUTE_NORMAL = 0x80,
		FILE_ATTRIBUTE_TEMPORARY = 0x100,
		FILE_ATTRIBUTE_OFFLINE = 0x1000,
		FILE_ATTRIBUTE_ENCRYPTED = 0x4000
	}
	public enum FileFlags : uint
	{
		FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
		FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
		FILE_FLAG_NO_BUFFERING = 0x20000000,
		FILE_FLAG_OPEN_NO_RECALL = 0x00100000,
		FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,
		FILE_FLAG_OVERLAPPED = 0x40000000,
		FILE_FLAG_POSIX_SEMANTICS = 0x01000000,
		FILE_FLAG_RANDOM_ACCESS = 0x10000000,
		FILE_FLAG_SESSION_AWARE = 0x00800000,
		FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
		FILE_FLAG_WRITE_THROUGH = 0x80000000
	}

	public enum DesiredAccess : uint
	{
		GENERIC_READ = 0x80000000,
		GENERIC_WRITE = 0x40000000,
		GENERIC_EXECUTE = 0x20000000,
		GENERIC_ALL = 0x10000000
	}

	public enum EMoveMethod : uint
	{
		Begin = 0,
		Current = 1,
		End = 2
	}
}
