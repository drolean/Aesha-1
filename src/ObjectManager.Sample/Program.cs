using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
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
            //ObjectManager.SetPlayerFacing();
            //var facing = ObjectManager.Me.Rotation;
            //return;



            // var me = ObjectManager.Me;
            // //var players = ObjectManager.Players;
            // var units = ObjectManager.Units;
            // //var objects = ObjectManager.Objects;
            // //var npcs = ObjectManager.Npcs;

            //// ObjectManager.Test();

            // var firstUnit = units.FirstOrDefault(u => u.Name == "Kobold Vermin");
            // ObjectManager.SetTarget(firstUnit.Guid);

            // var target = me.Target;

            //var imps = units.Where(x => x.CreatureType == CreatureType.Demon && x.Level == 4);
            //foreach (var imp in imps)
            //{
            //    var x = imp.SummonedBy;
            //}

            //var reader = new ProcessMemoryReader(process);
            //var kcd = new KeyboardCommandDispatcher();

            //var x = reader.ReadInt(0x00884CC8);
            //var y = reader.ReadInt(0x00884CCC);

            //reader.WriteUInt64(0x00884CC8, 10);
            //reader.WriteUInt64(0x00884CCC, 10);


            //  var requiredX = 10;

            //  var xMovementsRequired = Math.Abs(x - 10);
            ////  var yMovementsRequired = Math.Abs(absY - 1000);


            //  for (var i = 0; i < xMovementsRequired;i++)
            //  {
            //      x = x < requiredX ? x + i : x - i;
            //      kcd.MoveMouse(process, new Point((int) (x + i), y));
            //  }


            //   kcd.SendShiftClick(process,new Point(1062,564));


            //while (true)
            //{
            //    var mot = ObjectManager.GetMouseOverTarget();
            //    if (mot != null)
            //        ObjectManager.SetTarget(mot.Guid);
            //    Thread.Sleep(1000);
            //}




            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
