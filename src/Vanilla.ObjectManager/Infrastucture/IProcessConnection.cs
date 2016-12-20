using System;

namespace Vanilla.ObjectManager.Infrastucture
{
    public interface IProcessConnection
    {
        IntPtr Process { get; }
        bool IsProcessOpen { get; }
        bool IsThreadOpen { get; }
        int ProcessId { get; }
        int ThreadId { get; }
        IntPtr Thread { get; }
        bool SetDebugPrivileges { get; }
        IProcessMemoryReader Memory { get; }
        bool OpenProcessAndThread();
    }
}