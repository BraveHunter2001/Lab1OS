﻿using System;
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
		static FlexMenu driveFlexMenu = new FlexMenu("Get hard drive details", new IMenuItem[] { });


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
		static IMenu menu = new Menu("Main menu", new IMenuItem[]
			{
				new Menu("Hard drive management", new IMenuItem[]
					{
						new MenuItem("Get all hard drives list", driveManager.PrintAllDrives),
						driveFlexMenu
					}).AddOnFirstSelectAction(CreateDriveNamedMenus), 
				new Menu("Directory management", new IMenuItem[]
					{
						new MenuItem("Create directory", directoryManager.CreateDirectory),
						new MenuItem("Remove directory", directoryManager.RemoveDirectory)
					}),
				new Menu("File management", new IMenuItem[]
					{
						new MenuItem("Copy file", () => fileManager.CopyFile(true, YNmessageBox)),
						//new MenuItem("Copy file overlapped", overlappedFileCopier.Copy),
						new MenuItem("Move file", () => fileManager.MoveFile(true, YNmessageBox)),
						new MenuItem("Create file", () => fileManager.CreateFile()),
						fileAttributeManagementMenu
					})
			});
		static string SelectFile(bool checkFileExistance = false)
		{
			var result = "";
			do
			{
				Console.WriteLine("Input file full path");
				result = Console.ReadLine();
				if (checkFileExistance && !File.Exists(result))
					Console.WriteLine("Incorrect path or file");
			} while (checkFileExistance && !File.Exists(result));
			return result;
		}
		static IMenuItem[] CreateSetAttributesMenu()
		{
			winapiFlags.FileAttributes[] values = (winapiFlags.FileAttributes[])Enum.GetValues(typeof(winapiFlags.FileAttributes));
			var res = new IMenuItem[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				int j = i; //so the delegate wont corrupt
				res[i] = new MenuItem(values[i].ToString(), () => fileManager.SetFileAttributes(
					fileAttributeManagementMenu.Data, values[j]));
			}
			return res;
		}

		static void CreateDriveNamedMenus()
		{
			var drives = driveManager.GetAllDrives();
			IMenuItem[] menuItems = new IMenuItem[drives.Count];
			for (int i = 0; i < drives.Count; i++)
			{
				int j = i; //so the delegate wont corrupt
				menuItems[i] = new MenuItem("Drive " + drives[i], () => driveManager.PrintFullInfo(drives[j]));
			}
			driveFlexMenu.AddItems(menuItems);
		}

		static void Main(string[] args)
		{
			menu.Select();
		}
	}
}
