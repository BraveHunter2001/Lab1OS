﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using winapiFlags;

namespace Lab1OS
{
    class OverlappedFileCopier : FileManager
    {
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
             IOCompletionCallback completionRoutine);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFileEx(IntPtr hFile,
            [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
            uint numberOfBytesToWrite,
            [In] ref NativeOverlapped overlapped,
            [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback completionRoutine);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint timeGetTime();

        int callbackCounter = 0;
        private unsafe void OperationEndRoutine(uint errorCode,
            uint numberOfBytesTransfered, NativeOverlapped* overlapped)
        {
            callbackCounter++;
        }

        private readonly IOCompletionCallback callback;

        public unsafe OverlappedFileCopier()
        {
            //so the delegate won't be collected by GC
            callback += OperationEndRoutine;
        }

        [STAThread]
        private unsafe void ReadFileOverlapped(uint fileSize, uint blockSize, int operationsCount,
            NativeOverlapped[] overlappeds, byte[][] buffer, IntPtr fileHandle)
        {
            //read via multiple streams
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
            //wait for all operations to end
            while (callbackCounter < operationsStarted)
                SleepEx(uint.MaxValue, true);
            //move to next memory block
            for (int i = 0; i < operationsCount; i++)
                overlappeds[i].OffsetLow += (int)blockSize * operationsCount;
            callbackCounter = 0;
        }

        [STAThread]
        private unsafe void WriteFileOverlapped(uint fileSize, uint blockSize, int operationsCount,
            NativeOverlapped[] overlappeds, byte[][] buffer, IntPtr fileHandle)
        {
            //write via multiple streams
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
            //wait for all operations to end
            while (callbackCounter < operationsStarted)
                SleepEx(uint.MaxValue, true);
            //move to next memory block
            for (int i = 0; i < operationsCount; i++)
                overlappeds[i].OffsetLow += (int)blockSize * operationsCount;
            callbackCounter = 0;
        }

        private unsafe void CopyFileOverlapped(IntPtr sourceHandle, IntPtr targetHandle, uint blockSize, int operationsCount)
        {
            int srcSize = 0, curSize = 0; uint high = 0;
            srcSize = curSize = (int)GetFileSize(sourceHandle, ref high);

            byte[][] buffer = new byte[operationsCount][];
            var pins = new GCHandle[operationsCount];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new byte[(int)blockSize];
                //make sure that GC won't move the buffers in memory
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
                //main block
                do
                {
                    ReadFileOverlapped((uint)srcSize, blockSize, operationsCount, over_1, buffer, sourceHandle);
                    WriteFileOverlapped((uint)srcSize, blockSize, operationsCount, over_2, buffer, targetHandle);
                    curSize -= (int)(blockSize * operationsCount);
                } while (curSize > 0);
                //finish operation
                SetFilePointer(targetHandle, srcSize, IntPtr.Zero, EMoveMethod.Begin);
                SetEndOfFile(targetHandle);
            }
            finally //free pinned memory
            {
                for (int i = 0; i < buffer.Length; i++)
                    pins[i].Free();
            }
        }

        public unsafe void Copy()
        {
            string sourcePath, targetPath;
            if (!GetSourceTargetPaths(out sourcePath, out targetPath, true))
                return;
            IntPtr sourceHandle = CreateFile(sourcePath, (uint)DesiredAccess.GENERIC_READ,
                (uint)ShareMode.None, null, (uint)CreationDisposition.OPEN_EXISTING,
                (uint)FileFlags.FILE_FLAG_NO_BUFFERING | (uint)FileFlags.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
            if (sourceHandle == INVALID_HANDLE_VALUE)
            {
                Console.WriteLine($"Error reading file. Error code: {GetLastError()}");
                return;
            }
            IntPtr targetHandle = CreateFile(targetPath, (uint)DesiredAccess.GENERIC_WRITE,
                (uint)ShareMode.None, null, (uint)CreationDisposition.CREATE_ALWAYS,
                (uint)FileFlags.FILE_FLAG_NO_BUFFERING | (uint)FileFlags.FILE_FLAG_OVERLAPPED, IntPtr.Zero);

            uint er = GetLastError();
            if (targetHandle == INVALID_HANDLE_VALUE || er != 0)
            {
                Console.WriteLine($"Error creating target file. Error code: {er}");
                CloseHandle(sourceHandle);
                return;
            }
            uint blockSize = Helper.GetUintFromConsole("block size", false) * 4096;
            int operations = (int)Helper.GetUintFromConsole("operations count", false);
            try
            {
                uint start = timeGetTime();
                CopyFileOverlapped(sourceHandle, targetHandle, blockSize, operations);
                uint end = timeGetTime();
                Console.WriteLine($"File copied successfully. Copy time: {end - start} milliseconds");
            }
            finally //dispose descriptors
            {
                CloseHandle(sourceHandle);
                CloseHandle(targetHandle);
            }

        }
    }
}
