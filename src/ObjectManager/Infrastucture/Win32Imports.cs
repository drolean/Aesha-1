using System;
using System.Runtime.InteropServices;

namespace ObjectManager.Infrastucture
{
    public static class Win32Imports
    {
        [DllImport("kernel32", EntryPoint = "OpenProcess")]
        internal static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId);

        [DllImport("kernel32", EntryPoint = "OpenThread")]
        internal static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32", EntryPoint = "CloseHandle")]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
        internal static extern bool ReadProcessMemory(
            IntPtr hProcess,
            uint dwAddress,
            IntPtr lpBuffer,
            int nSize,
            out int lpBytesRead);
    }
}