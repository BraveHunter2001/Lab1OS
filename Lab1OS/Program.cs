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
		static IYNmessageBox YNmessageBox = new YNMessageBox();
		static AdaptiveMenu adaptiveDriveMenu = new AdaptiveMenu("Get hard drive details", new IMenuItem[] { });


		static MenuWithDataRequest<String> fileAttributeManagementMenu =
				new MenuWithDataRequest<String>("File attribute management",
					new IMenuItem[]{
						new MenuItem("Get file attributes",
							()=>fileManager.PrintFileAttributes(fileAttributeManagementMenu.Data)),
						new MenuItem("Get file attributes by handle",
							()=>fileManager.PrintFileAttributesByHandle(fileAttributeManagementMenu.Data)),
						new MenuItem("Get file time attributes",
							()=>fileManager.PrintFileTimeAttributes(fileAttributeManagementMenu.Data)),
						new MenuItem("Set file time attributes",
							()=>fileManager.SetFileTimeAttributes(fileAttributeManagementMenu.Data)),
						new MenuWithDataRequest<String>("Set file attributes",
							CreateSetAttributesMenu(), ()=>fileAttributeManagementMenu.Data)
					},
					() => SelectFile());

		static IMenu menu = new Menu("Main", new IMenuItem[] 
		{
			new Menu ("Drive info", new IMenuItem[]
			{ 
				new MenuItem("Get all drivers on PC", driveManager.PrintAllDrives),
				adaptiveDriveMenu
			}),

			new Menu ("Directory Manager", new IMenuItem[]
			{
				new MenuItem("Create directory", directoryManager.CreateDirectory),
				new MenuItem("Remove directory", directoryManager.RemoveDirectory)

			}),

			new Menu ("File Manager", new IMenuItem[]
			{
				new MenuItem("Copy file", () => fileManager.CopyFile(true, YNmessageBox)),
				new MenuItem("Move file", () => fileManager.MoveFile(true, YNmessageBox)),
				new MenuItem("Create file", fileManager.CreateFile),
				fileAttributeManagementMenu
			})
		});

		static void CreateDriveMenu()
		{
			var drs = driveManager.GetAllDrives();
			IMenuItem[] items = new IMenuItem[drs.Count];

			for (int i = 0; i < drs.Count; i++)
			{
				var dr = drs[i];
				items[i] = new MenuItem("Drive: " + dr, () => driveManager.PrintFullInfo(dr));
			}

			adaptiveDriveMenu.AddItems(items);

		}

		static string SelectFile()
		{
			var result = "";
			do
			{
				Console.WriteLine("Input file full path");
				result = Console.ReadLine();
				if (!File.Exists(result))
					Console.WriteLine("Incorrect path or file");
			} while ( !File.Exists(result));
			return result;
		}
		static IMenuItem[] CreateSetAttributesMenu()
		{
			winapiFlags.FileAttributes[] values = (winapiFlags.FileAttributes[])Enum.GetValues(typeof(winapiFlags.FileAttributes));
			var res = new IMenuItem[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				int j = i;
				res[i] = new MenuItem(values[i].ToString(), () => fileManager.SetFileAttributes(
					fileAttributeManagementMenu.Data, values[j]));
			}
			return res;
		}

		static void Main(string[] args)
		{

			CreateDriveMenu();

			menu.Select();
			
		}
	}
}
