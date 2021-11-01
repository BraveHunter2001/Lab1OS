using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using winapiFlags;
using CsvHelper;

namespace Lab1OS
{
    class Overlapped : FileManager
    {
        #region DLLImport_for_WinAPI
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SetFilePointer(IntPtr file,
            int distanceToMove, IntPtr distanceToMoveHigh, EMoveMethod moveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetFileSize(IntPtr file,
            ref uint fileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SleepEx(
              uint dwMilliseconds,
              bool bAlertable);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetEndOfFile(IntPtr hFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ReadFileEx(
            IntPtr hFile,
            [Out] byte[] buffer,
            [In] uint numberOfBytesToRead,
            [In, Out] ref NativeOverlapped overlapped,
            [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback completionRoutine);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFileEx(IntPtr hFile,
            [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
            uint numberOfBytesToWrite,
            [In] ref NativeOverlapped overlapped,
            [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback completionRoutine);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint timeGetTime();

        #endregion


        int callBackCount = 0;
        private unsafe void FileIOCompletionRoutine(uint errorCode, uint numberOfBytesTransfered, NativeOverlapped* overlapped)

        {
            callBackCount++;
        }
        private readonly IOCompletionCallback callback;

        public unsafe Overlapped()
        {
            callback += FileIOCompletionRoutine;
        }

        private unsafe void WaitOperation(int operationsStarted)
        {
            while (callBackCount < operationsStarted)
                SleepEx(uint.MaxValue, true);
        }

        private unsafe void MoveNextMemBlock(NativeOverlapped[] overlappeds, uint blockSize, int operationsCount)
        {
            for (int i = 0; i < operationsCount; i++)
                overlappeds[i].OffsetLow += (int)blockSize * operationsCount;

        }


        [STAThread]
        private unsafe void ReadOverlapped(uint fileSize, uint blockSize, int operationsCount,
            NativeOverlapped[] overlappeds, byte[][] buffer, IntPtr fileHandle)
        {
            int operationsStarted = 0;
            for (int i = 0; i < operationsCount; i++)
            {
                if (fileSize > 0)
                {
                    operationsStarted++;
                    ReadFileEx(fileHandle, buffer[i], blockSize, ref overlappeds[i], callback);
                    fileSize -= blockSize;
                }
            }

            WaitOperation(operationsStarted);

            MoveNextMemBlock(overlappeds, blockSize, operationsCount);

            callBackCount = 0;

        }

        [STAThread]
        private unsafe void WriteOverlapped(uint fileSize, uint blockSize, int operationsCount,
            NativeOverlapped[] overlappeds, byte[][] buffer, IntPtr fileHandle)
        {
            
            int operationsStarted = 0;
            for (int i = 0; i < operationsCount; i++)
            {
                if (fileSize > 0)
                {
                    operationsStarted++;
                    WriteFileEx(fileHandle, buffer[i], blockSize, ref overlappeds[i], callback);
                    fileSize -= blockSize;
                }
            }

            WaitOperation(operationsStarted);

            MoveNextMemBlock(overlappeds, blockSize, operationsCount);

            callBackCount = 0;
        }


        private unsafe void CopyOverlapped(IntPtr sourceHandle, IntPtr targetHandle, uint blockSize, int operationsCount)
        {
            int srcSize = 0, curSize = 0; uint high = 0;
            srcSize = curSize = (int)GetFileSize(sourceHandle, ref high);

            byte[][] buffer = new byte[operationsCount][];
            var pins = new GCHandle[operationsCount];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new byte[(int)blockSize];
                

                pins[i] = GCHandle.Alloc(buffer[i], GCHandleType.Pinned);
            }
            try
            {
                NativeOverlapped[] over_1 = new NativeOverlapped[operationsCount];
                NativeOverlapped[] over_2 = new NativeOverlapped[operationsCount];

                for (int i = 0; i < operationsCount; i++)
                {
                    over_1[i] = new NativeOverlapped();
                    over_2[i] = new NativeOverlapped();

                    over_1[i].OffsetLow = over_2[i].OffsetLow = i * (int)blockSize;
                    over_1[i].OffsetHigh = over_2[i].OffsetHigh = i * (int)high;
                    
                }
                
                do
                {
                    ReadOverlapped((uint)srcSize, blockSize, operationsCount, over_1, buffer, sourceHandle);

                    WriteOverlapped((uint)srcSize, blockSize, operationsCount, over_2, buffer, targetHandle);

                    curSize -= (int)(blockSize * operationsCount);
                } while (curSize > 0);
                
                SetFilePointer(targetHandle, srcSize, IntPtr.Zero, EMoveMethod.Begin);
                SetEndOfFile(targetHandle);
            }
            finally 
            {
                for (int i = 0; i < buffer.Length; i++)
                    pins[i].Free();
            }
        }

       

        public unsafe uint Copy(string fromFile, string toFile,  uint blockSize, int operations)
        {
            

            uint timeCopy = 0;

            if (fromFile == "" || toFile == "")
            {
                Console.WriteLine("You invalid input path");
                return timeCopy;
            }

            uint flagAnttr = (uint)FileFlags.FILE_FLAG_NO_BUFFERING | (uint)FileFlags.FILE_FLAG_OVERLAPPED;

            IntPtr sourceHandle = CreateFile(fromFile, (uint)DesiredAccess.GENERIC_READ,
                (uint)ShareMode.None, null, (uint)CreationDisposition.OPEN_EXISTING, flagAnttr, IntPtr.Zero);

            uint er = GetLastError();
            if (sourceHandle == INVALID_HANDLE_VALUE || er != 0)
            {
                Console.WriteLine($"Error creating target file. Error code: {er}");
                CloseHandle(sourceHandle);

                return timeCopy;
            }



            IntPtr targetHandle = CreateFile(toFile, (uint)DesiredAccess.GENERIC_WRITE,
                (uint)ShareMode.None, null, (uint)CreationDisposition.CREATE_ALWAYS,
                flagAnttr, IntPtr.Zero);

            er = GetLastError();
            if (targetHandle == INVALID_HANDLE_VALUE || er != 0)
            {
                Console.WriteLine($"Error creating target file. Error code: {er}");
                CloseHandle(targetHandle);

                return timeCopy;
            }


            blockSize = blockSize * 4096;
            

            try
            {
                uint start = timeGetTime();
                CopyOverlapped(sourceHandle, targetHandle, blockSize, operations);
                uint end = timeGetTime();
                timeCopy = end - start;
                Console.WriteLine($"File copied successfully. Copy time: {timeCopy} milliseconds");
            }
            finally //dispose descriptors
            {
                CloseHandle(sourceHandle);
                CloseHandle(targetHandle);
            }

            return timeCopy;
        }
    }
}
