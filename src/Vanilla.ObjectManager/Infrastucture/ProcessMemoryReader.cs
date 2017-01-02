using System;
using System.Runtime.InteropServices;

namespace Vanilla.ObjectManager.Infrastucture
{
    public sealed class ProcessMemoryReader
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

        public static float ReadFloat(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            byte[] numArray = ReadBytes(hProcess, dwAddress, 4);
            if (numArray == null)
                throw new Exception("ReadFloat failed.");
            if (bReverse)
                Array.Reverse((Array)numArray);
            return BitConverter.ToSingle(numArray, 0);
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

        private static int ReadInt(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            var buffer = ReadBytes(hProcess, dwAddress, sizeof(int));
            if (buffer == null)
                throw new Exception("ReadUInt failed.");

            if (bReverse)
                Array.Reverse(buffer);

            return BitConverter.ToInt32(buffer, 0);
        }

        private static uint ReadUInt(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            var buffer = ReadBytes(hProcess, dwAddress, sizeof(uint));
            if (buffer == null)
                throw new Exception("ReadUInt failed.");

            if (bReverse)
                Array.Reverse(buffer);

            return BitConverter.ToUInt32(buffer, 0);
        }

        private static ulong ReadUInt64(IntPtr hProcess, uint dwAddress, bool bReverse)
        {
            var numArray = ReadBytes(hProcess, dwAddress, 8);
            if (numArray == null)
                throw new Exception("ReadUInt64 failed.");
            if (bReverse)
                Array.Reverse(numArray);
            return BitConverter.ToUInt64(numArray, 0);
        }

        private static string ReadString(IntPtr hProcess, uint dwAddress, int length)
        {
            var num = IntPtr.Zero;
            string stringAnsi;
            try
            {
                var nSize = length;
                num = Marshal.AllocHGlobal(nSize + 1);
                Marshal.WriteByte(num, length, (byte)0);
                if (ReadRawMemory(hProcess, dwAddress, num, nSize) != nSize)
                    throw new Exception();
                stringAnsi = Marshal.PtrToStringAnsi(num);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                if (num != IntPtr.Zero)
                    Marshal.FreeHGlobal(num);
            }
            return stringAnsi;
        }

        public byte ReadByte(uint dwAddress)
        {
            return ReadByte(_processPtr, dwAddress);
        }

        public byte[] ReadBytes(uint dwAddress, int length)
        {
            return ReadBytes(_processPtr,dwAddress,length);
        }

        public int ReadInt(uint dwAddress)
        {
            return ReadInt(_processPtr, dwAddress, false);
        }

        public uint ReadUInt(uint dwAddress)
        {
            return ReadUInt(_processPtr, dwAddress, false);
        }

        public float ReadFloat(uint dwAddress)
        {
            return ReadFloat(_processPtr, dwAddress, false);
        }

        public ulong ReadUInt64(uint dwAddress)
        {
            return ReadUInt64(_processPtr, dwAddress, false);
        }

        public string ReadString(uint dwAddress, int length)
        {
            return ReadString(_processPtr,dwAddress,length);
        }


    }
}