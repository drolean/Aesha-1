using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ObjectManager.Infrastructure;
using ObjectManager.Sample.Forms;

namespace ObjectManager.Sample
{
    class Program
    {
        static void Main(string[] args)
        {

            var process = Process.GetProcessesByName("WoW").FirstOrDefault();
            ObjectManager.Start(process);

            var me = ObjectManager.Me;
            var players = ObjectManager.Players;
            var units = ObjectManager.Units;

            var t = me.Target;

            var imps = units.Where(x => x.CreatureType == CreatureType.Demon && x.Level == 4);
            foreach (var imp in imps)
            {
                var x = imp.SummonedBy;
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Main());
        }
    }
}
