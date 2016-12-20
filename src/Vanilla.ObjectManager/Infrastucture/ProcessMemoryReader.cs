using System;
using System.Runtime.InteropServices;

namespace Vanilla.ObjectManager.Infrastucture
{
    public sealed class ProcessMemoryReader : IProcessMemoryReader
    {
        private IntPtr _processPtr;
        
        public void Open(IntPtr processPtr)
        {
            _processPtr = processPtr;
        }

        private byte ReadByte(IntPtr hProcess, uint dwAddress)
        {
            byte[] buf = ReadBytes(hProcess, dwAddress, sizeof(byte));
            if (buf == null)
                throw new Exception("ReadByte failed.");

            return buf[0];
        }

        private static byte[] ReadBytes(IntPtr hProcess, uint dwAddress, int nSize)
        {
            var lpBuffer = IntPtr.Zero;
            byte[] results;

            try
            {
                lpBuffer = Marshal.AllocHGlobal(nSize);

                var iBytesRead = ReadRawMemory(hProcess, dwAddress, lpBuffer, nSize);
                if (iBytesRead != nSize)
                    throw new Exception("ReadProcessMemory error in ReadBytes");

                results = new byte[iBytesRead];
                Marshal.Copy(lpBuffer, results, 0, iBytesRead);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (lpBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(lpBuffer);
            }

            return results;
        }

        private static int ReadRawMemory(IntPtr hProcess, uint dwAddress, IntPtr lpBuffer, int nSize)
        {
            try
            {
                var lpBytesRead = 0;
                if (!Win32Imports.ReadProcessMemory(hProcess, dwAddress, lpBuffer, nSize, out lpBytesRead))
                    throw new Exception("ReadProcessMemory failed");

                return (int)lpBytesRead;
            }
            catch
            {
                return 0;
            }
        }

        private static uint ReadUInt(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            byte[] buffer = ReadBytes(hProcess, dwAddress, sizeof(uint));
            if (buffer == null)
                throw new Exception("ReadUInt failed.");

            if (bReverse)
                Array.Reverse(buffer);

            return BitConverter.ToUInt32(buffer, 0);
        }

        public byte ReadByte(uint dwAddress)
        {
            return ReadByte(_processPtr, dwAddress);
        }

        public uint ReadUInt(uint dwAddress, bool bReverse = false)
        {
            return ReadUInt(_processPtr, dwAddress, bReverse);
        }
    }
}