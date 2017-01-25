using System;
using System.Runtime.InteropServices;

namespace ObjectManager.Infrastructure
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

        [DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess, 
            uint dwAddress, 
            IntPtr lpBuffer, 
            int nSize, 
            out IntPtr iBytesWritten);

        [DllImport("kernel32")]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32")]
        public static extern uint ResumeThread(IntPtr hThread);

        [DllImport("USER32.DLL")]
        public static extern int PostMessage(IntPtr hwnd, uint msg, int character, uint lparam);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}