using System;
using System.Diagnostics;
using Aesha.Interfaces;

namespace Aesha.Core
{
    public class WowProcess : IWowProcess
    {
        private readonly Process _process;

        public WowProcess(Process process)
        {
            _process = process;
        }

        public ProcessThreadCollection Threads => _process.Threads;
        public IntPtr MainWindowHandle => _process.MainWindowHandle;
        public int Id => _process.Id;
        public IntPtr MainModuleBaseAddress => _process.MainModule.BaseAddress;
    }
}
