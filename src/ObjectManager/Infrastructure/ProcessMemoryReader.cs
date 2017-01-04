using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ObjectManager.Infrastructure
{
    public sealed class ProcessMemoryReader
    {
        private readonly IntPtr _processPtr;
        private const uint StandardRightsRequired = 0x000F0000;
        private const uint Synchronize = 0x00100000;
        private const uint ProcessAllAccess = StandardRightsRequired | Synchronize | 0xFFF;

        public ProcessMemoryReader(Process process)
        {
            _processPtr = Win32Imports.OpenProcess(ProcessAllAccess, false, process.Id);
        }
        
        public T Read<T>(uint address) where T : struct
        {
            var buffer = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf(typeof(T));
                buffer = Marshal.AllocHGlobal(size);

                ReadRawMemory(address, buffer, size);
                return (T) Marshal.PtrToStructure(buffer, typeof(T));
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            }

        }
        
        public string ReadString(uint address, int length)
        {
            var buffer = IntPtr.Zero;
            string stringAnsi;
            try
            {
                var allocationSize = length;
                buffer = Marshal.AllocHGlobal(allocationSize + 1);
                Marshal.WriteByte(buffer, length, (byte)0);
                if (ReadRawMemory(address, buffer, allocationSize) != allocationSize)
                    throw new Exception();
                stringAnsi = Marshal.PtrToStringAnsi(buffer);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            }
            return stringAnsi;
        }


        private int ReadRawMemory(uint address, IntPtr buffer, int size)
        {
            try
            {
                var lpBytesRead = 0;
                if (!Win32Imports.ReadProcessMemory(_processPtr, address, buffer, size, out lpBytesRead))
                    throw new Exception("ReadProcessMemory failed");

                return (int)lpBytesRead;
            }
            catch
            {
                return 0;
            }
        }
    }
}