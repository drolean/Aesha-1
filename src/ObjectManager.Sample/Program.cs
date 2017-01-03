using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ObjectManager.Sample.Forms;

namespace ObjectManager.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
