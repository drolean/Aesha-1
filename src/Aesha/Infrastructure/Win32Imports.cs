using System;
using System.Runtime.InteropServices;

namespace Aesha.Infrastructure
{
    public static class Win32Imports
    {
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [DllImport("kernel32", EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId);

        [DllImport("kernel32", EntryPoint = "OpenThread")]
        public static extern IntPtr OpenThread(
            uint dwDesiredAccess,
            bool bInheritHandle, 
            uint dwThreadId);

        [DllImport("kernel32", EntryPoint = "CloseHandle")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(
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

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);


        [DllImport("kernel32")]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32")]
        public static extern uint ResumeThread(IntPtr hThread);


        [DllImport("kernel32")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32", EntryPoint = "VirtualAllocEx")]
        public static extern uint VirtualAllocEx(
            IntPtr hProcess,
            uint dwAddress,
            int nSize,
            uint dwAllocationType,
            uint dwProtect);

        [DllImport("kernel32", EntryPoint = "VirtualFreeEx")]
        public static extern bool VirtualFreeEx(
            IntPtr hProcess, 
            uint dwAddress, 
            int nSize, 
            uint dwFreeType);

        [DllImport("user32.dll")]
        public static extern int PostMessage(
            IntPtr hwnd, 
            uint msg,  
            int character, 
            uint lparam);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool SetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

    }
}