using System;

namespace Aesha.Interfaces
{
    public interface IProcessMemoryReader
    {
        byte ReadByte(uint dwAddress);
        byte[] ReadBytes(uint dwAddress, int nSize);
        float ReadFloat(uint dwAddress);
        int ReadInt(uint dwAddress);
        int ReadRawMemory(uint dwAddress, IntPtr lpBuffer, int nSize);
        string ReadString(uint dwAddress, int length);
        uint ReadUInt(uint dwAddress);
        ulong ReadUInt64(uint dwAddress);
        bool WriteBytes(uint dwAddress, byte[] lpBytes, int nSize);
        bool WriteFloat(uint dwAddress, float value);
        void WriteUInt64(uint dwAddress, ulong value);
        void WriteInt(uint dwAddress, int value);
    }
}