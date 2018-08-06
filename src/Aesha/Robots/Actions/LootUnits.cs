using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Core;
using Aesha.Domain;

namespace Aesha.Robots.Actions
{
    public class LootUnits : IConditionalAction
    {

        public readonly List<IWowObject> LootList = new List<IWowObject>();
        public readonly List<IWowObject> UnitsLooted = new List<IWowObject>();

        public bool Evaluate()
        {
            return LootList.Count > 0;
        }

        private ulong GetPositionMouseOverUnit(Point point)
        {
            Cursor.Position = point;
            Task.Delay(10).Wait();
            return CommandManager.GetDefault().MouseOverUnit;
        }

        private IWowObject GetValidUnit(ulong mouseOverUnitGuid)
        {
            var foundUnit = LootList.SingleOrDefault(u => u.Guid == mouseOverUnitGuid);

            if (foundUnit == null) return null;
            if (foundUnit == ObjectManager.Me.Pet) return null;
            if (UnitsLooted.Contains(foundUnit)) return null;

            return foundUnit;
        }

        public void Do()
        {
            foreach (var unit in LootList)
            {
                WaypointManager.MoveToWaypoint(unit.Location, 1);
                
                for (var x = 700; x <= 1150; x += 30)
                {
                    for (var y = 450; y <= 850; y += 20)
                    {
                        var mouseOverUnitGuid = GetPositionMouseOverUnit(new Point(x, y));
                        if (mouseOverUnitGuid > 0)
                        {
                            var foundUnit = GetValidUnit(mouseOverUnitGuid);
                            if (foundUnit == null) continue;
                            
                            KeyboardCommandDispatcher.GetKeyboard().SendShiftClick(new Point(x, y));
                            Task.Delay(500).Wait();
                            UnitsLooted.Add(foundUnit);

                            var outstandingWork = false;
                            foreach (var u in LootList)
                            {
                                if (!UnitsLooted.Contains(u))
                                    outstandingWork = true;
                            }

                            if (!outstandingWork)
                            {
                                LootList.Clear();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
