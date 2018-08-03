using System;
using System.Runtime.InteropServices;
using Aesha.Interfaces;

namespace Aesha.Infrastructure
{
    public sealed class ProcessMemoryReader : IProcessMemoryReader
    {
        private readonly IntPtr _processPtr;

        private const uint StandardRightsRequired = 0x000F0000;
        private const uint Synchronize = 0x00100000;
        private const uint ProcessAllAccess = StandardRightsRequired | Synchronize | 0xFFF;

        public ProcessMemoryReader(IWowProcess process)
        {
            _processPtr = Win32Imports.OpenProcess(ProcessAllAccess, false, process.Id);

        }

        public byte ReadByte( uint dwAddress)
        {
            byte[] buf = ReadBytes(dwAddress, sizeof(byte));
            if (buf == null)
                throw new Exception("ReadByte failed.");

            return buf[0];
        }



        public float ReadFloat( uint dwAddress)
        {
            byte[] numArray = ReadBytes(dwAddress, 4);
            if (numArray == null)
                throw new Exception("ReadFloat failed.");
           
            return BitConverter.ToSingle(numArray, 0);
        }


        public byte[] ReadBytes( uint dwAddress, int nSize)
        {
            var lpBuffer = IntPtr.Zero;
            byte[] results;

            try
            {
                lpBuffer = Marshal.AllocHGlobal(nSize);

                var iBytesRead = ReadRawMemory(dwAddress, lpBuffer, nSize);
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

        public bool WriteFloat(uint dwAddress, float value)
        {
            var bytes = BitConverter.GetBytes(value);
            return WriteBytes(dwAddress, bytes, 4);
        }

        public bool WriteBytes(uint dwAddress, byte[] lpBytes, int nSize)
        {
            IntPtr num1 = IntPtr.Zero;
            try
            {
                num1 = Marshal.AllocHGlobal(Marshal.SizeOf((object)lpBytes[0]) * nSize);
                Marshal.Copy(lpBytes, 0, num1, nSize);
                int num2 = WriteRawMemory(dwAddress, num1, nSize);
                if (nSize != num2)
                    throw new Exception("WriteBytes failed!  Number of bytes actually written differed from request.");
            }
            catch
            {
                return false;
            }
            finally
            {
                if (num1 != IntPtr.Zero)
                    Marshal.FreeHGlobal(num1);
            }
            return true;
        }

        public int ReadRawMemory( uint dwAddress, IntPtr lpBuffer, int nSize)
        {
            try
            {
                var lpBytesRead = 0;
                if (!Win32Imports.ReadProcessMemory(_processPtr, dwAddress, lpBuffer, nSize, out lpBytesRead))
                    throw new Exception("ReadProcessMemory failed");

                return lpBytesRead;
            }
            catch
            {
                return 0;
            }
        }

        private int WriteRawMemory( uint dwAddress, IntPtr lpBuffer, int nSize)
        {
            IntPtr iBytesWritten = IntPtr.Zero;
            if (!Win32Imports.WriteProcessMemory(_processPtr, dwAddress, lpBuffer, nSize, out iBytesWritten))
                return 0;
            return (int)iBytesWritten;
        }

        public int ReadInt( uint dwAddress)
        {
            var buffer = ReadBytes(dwAddress, sizeof(int));
            if (buffer == null)
                throw new Exception("ReadUInt failed.");

            return BitConverter.ToInt32(buffer, 0);
        }

        public uint ReadUInt( uint dwAddress)
        {
            var buffer = ReadBytes(dwAddress, sizeof(uint));
            return buffer == null ? 0 : BitConverter.ToUInt32(buffer, 0);
        }

        public ulong ReadUInt64( uint dwAddress)
        {
            var numArray = ReadBytes(dwAddress, 8);
            if (numArray == null)
                throw new Exception("ReadUInt64 failed.");

            return BitConverter.ToUInt64(numArray, 0);
        }

        public void WriteUInt64(uint dwAddress, ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(dwAddress, bytes, 8);
        }

        public void WriteInt(uint dwAddress, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(dwAddress, bytes, 4);
        }

        public string ReadString( uint dwAddress, int length)
        {
            var num = IntPtr.Zero;
            string stringAnsi;
            try
            {
                var nSize = length;
                num = Marshal.AllocHGlobal(nSize + 1);
                Marshal.WriteByte(num, length, (byte)0);
                if (ReadRawMemory(dwAddress, num, nSize) != nSize)
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
        

    }
}