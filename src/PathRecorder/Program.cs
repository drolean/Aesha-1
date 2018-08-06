using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Core;

namespace PathRecorder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);;

            var proc = Process.GetProcessesByName("wow").FirstOrDefault();
            var wowProc = new WowProcess(proc);
            Application.Run(new Main(wowProc));
        }
    }
}
