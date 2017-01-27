using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Aesha.Core;
using NUnit.Framework;

namespace Aesha.UnitTests
{
    public class KeyboardTests
    {

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern bool SetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        
        [Test]
        public void AttachThreadsAndSetState()
        {
            var process = Process.GetProcessesByName("Notepad").First();

            var kcd = new KeyboardCommandDispatcher(process);
            //kcd.SendKeys("!!!");
            kcd.SendKeys("Hello World!");
            kcd.SendKeys("!\"£$%^&*()_+");

        }
    }
}
