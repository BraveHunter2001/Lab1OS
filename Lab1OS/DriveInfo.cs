using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using winapiFlags;

namespace Lab1OS
{
    class DriveInfo
    {
        

        [DllImport("kernel32.dll")]
        static extern uint GetLogicalDrives();

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        static extern uint GetDriveType(string lpRootPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        static extern uint GetVolumeInformation(string rootPathName,
                                               StringBuilder nameBuffer, uint nameBufferLength, out uint volumeSerialNumber,
                                               out uint maximumComponentLength, out uint fileSystemFlags,
                                               StringBuilder fileSystemNameBuffer, uint fileSystemNameBufferLength);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        static extern uint GetDiskFreeSpace(string rootPathName,
                                                out uint sectorsPerCluster, out uint bytesPerSector,
                                                out uint numberOfFreeClusters, out uint totalNumberOfClusters);

        const uint nameBufferLength = 256;

        public  List<char> GetAllDrives()
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var drives = GetLogicalDrives();
            List<char> driveletters = new List<char>();

            for (int i = 0; i < alphabet.Length; i++)
            {
                if( (drives & 1) == 1)
                {
                    driveletters.Add(alphabet[i]);
                }

                drives = drives >> 1; 
            }
            return driveletters;
        }

        public void PrintAllDrives()
        {
            List<char> drives = GetAllDrives();

            Console.WriteLine($"Found {drives.Count} drives :");

            foreach(var dr in drives)
            {
                Console.Write(dr+ " ");
            }
            Console.WriteLine();
        }

       
      

        public DriveType GetDriveType(char driveLetter)
        {
            return (DriveType)GetDriveType(driveLetter + ":\\");
        }



    public void PrintVolumeInfo(char driveLetter)
        {
            StringBuilder nameBuffer = new StringBuilder((int)nameBufferLength);
            StringBuilder fileSystemNameBuffer = new StringBuilder((int)nameBufferLength);
            uint volumeSerialNumber = 0, maximumComponentLength = 0, fileSystemFlags = 0;

            GetVolumeInformation(driveLetter + ":\\", nameBuffer, nameBufferLength,
                out volumeSerialNumber, out maximumComponentLength, out fileSystemFlags,
                fileSystemNameBuffer, nameBufferLength).ToString();

            Console.WriteLine($"==VolumeInformation==");

            Console.WriteLine("Volume name: " + nameBuffer.ToString());
            Console.WriteLine("Serial Number: " + volumeSerialNumber);
            Console.WriteLine("Maximum component length: " + maximumComponentLength);
            Console.WriteLine("File system name: " + fileSystemNameBuffer.ToString());
            Console.WriteLine("File system flags:");
            foreach(var flag in Helper.ParseFlags<FileSystemFlags>(fileSystemFlags))
                Console.WriteLine($"\t - {flag}");
        }

        public void PrintDiskFreeSpaceInfo(char driveLetter)
        {
            uint sectorsPerCluster = 0, bytesPerSector = 0,
                numberOfFreeClusters = 0, totalNumberOfClusters = 0;
            GetDiskFreeSpace(driveLetter + ":\\", out sectorsPerCluster, out bytesPerSector,
                out numberOfFreeClusters, out totalNumberOfClusters);
            Console.WriteLine("==Disk Free Space==");
            Console.WriteLine("Sectors per cluster: " + sectorsPerCluster);
            Console.WriteLine("Bytes per sector: " + bytesPerSector);
            Console.WriteLine("Number of free clusters: " + numberOfFreeClusters);
            Console.WriteLine("Total number of clusters: " + totalNumberOfClusters);
        }


        public void PrintFullInfo(char driveLetter)
        {
            Console.WriteLine($"===INFO ABOUT DRIVE {driveLetter}===");
            Console.WriteLine($"Drive type: {GetDriveType(driveLetter)}");
            PrintVolumeInfo(driveLetter);
            PrintDiskFreeSpaceInfo(driveLetter);
        }
    }
}
