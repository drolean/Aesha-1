using System;
using System.Diagnostics;

namespace Vanilla.ObjectManager.Infrastucture
{
	public class ProcessConnection : IProcessConnection
	{
	    private readonly int _processId;
	    public IntPtr Process { get; private set; }
        public bool IsProcessOpen { get; private set; }
        public bool IsThreadOpen { get; private set; }
        public int ProcessId { get; private set; }
        public int ThreadId { get; private set; }
        public IntPtr Thread { get; private set; }
	    public bool SetDebugPrivileges { get; private set; }
        public IProcessMemoryReader Memory { get; set; }

        private const uint StandardRightsRequired = 0x000F0000;
	    private const uint Synchronize = 0x00100000;
	    private const uint ProcessAllAccess = StandardRightsRequired | Synchronize | 0xFFF;
	    private const uint ThreadAllAccess = StandardRightsRequired | Synchronize | 0x3FF;


        public ProcessConnection(int processId, IProcessMemoryReader memoryReader)
        {
            _processId = processId;
            Process = IntPtr.Zero;
            Thread = IntPtr.Zero;
            Memory = memoryReader;
        }

		~ProcessConnection()
		{
			this.Close();
		}

	    private bool Open()
		{
			if (_processId == 0)
				return false;

			if (_processId == ProcessId)
				return true;

			if (IsProcessOpen)
				this.CloseProcess();

			if (SetDebugPrivileges)
				System.Diagnostics.Process.EnterDebugMode();

			IsProcessOpen = (Process = Win32Imports.OpenProcess(ProcessAllAccess, false, _processId)) != IntPtr.Zero;

			if (IsProcessOpen)
			{
				ProcessId = _processId;
				FindWindowByProcessId(_processId);
			}

			return IsProcessOpen;
		}

	    private bool OpenThread(int dwThreadId)
		{
			if (dwThreadId == 0)
				return false;

			if (dwThreadId == ThreadId)
				return true;

			if (IsThreadOpen)
				this.CloseThread();

			IsThreadOpen = (Thread = Win32Imports.OpenThread(ThreadAllAccess, false, (uint)dwThreadId)) != IntPtr.Zero;

			if (IsThreadOpen)
				ThreadId = dwThreadId;

			return IsThreadOpen;
		}

	    private bool OpenThread()
		{
			if (IsProcessOpen)
				return this.OpenThread(GetMainThreadId(ProcessId));
			return false;
		}


		public bool OpenProcessAndThread()
		{
		    if (Open() && OpenThread())
		    {
                Memory.Open(Process);
		        return true;
		    }

		    Close();
			return false;
		}


	    private void Close()
		{

			this.CloseProcess();
			this.CloseThread();
		}

	    private void CloseProcess()
		{
			if (Process != IntPtr.Zero)
			    Win32Imports.CloseHandle(Process);

			Process = IntPtr.Zero;
		    ProcessId = 0;

		    IsProcessOpen = false;
		}


	    private void CloseThread()
		{
			if (Thread != IntPtr.Zero)
			    Win32Imports.CloseHandle(Thread);

			Thread = IntPtr.Zero;
			ThreadId = 0;

			IsThreadOpen = false;
		}

	    private static IntPtr FindWindowByProcessId(int dwProcessId)
        {
            Process proc = System.Diagnostics.Process.GetProcessById(dwProcessId);
            return proc.MainWindowHandle;
        }

	    private static int GetMainThreadId(int dwProcessId)
        {
            if (dwProcessId == 0)
                return 0;

            Process proc = System.Diagnostics.Process.GetProcessById(dwProcessId);
            return proc.Threads[0].Id;
        }
	}
}
