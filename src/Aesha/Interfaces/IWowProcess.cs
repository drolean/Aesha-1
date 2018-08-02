using System;
using System.Diagnostics;

namespace Aesha.Interfaces
{
    public interface IWowProcess
    {
        ProcessThreadCollection Threads {get;}
        IntPtr MainWindowHandle { get; }
        int Id { get; }
        IntPtr MainModuleBaseAddress { get; }
    }
}