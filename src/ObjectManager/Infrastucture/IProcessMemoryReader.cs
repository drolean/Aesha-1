using System;

namespace Vanilla.ObjectManager.Infrastucture
{
    public interface IProcessMemoryReader
    {
        void Open(IntPtr processPtr);
        byte ReadByte(uint dwAddress);
        uint ReadUInt(uint dwAddress, bool bReverse = false);
    }
}