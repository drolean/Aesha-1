﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Core;
using Aesha.Domain;
using Serilog;
using Serilog.Events;

namespace Aesha.Robots.Actions
{
    public class LootUnits : IConditionalAction
    {
        private readonly ILogger _logger;

        public readonly List<IWowObject> LootList = new List<IWowObject>();
        public readonly List<IWowObject> UnitsLooted = new List<IWowObject>();

        public LootUnits(ILogger logger)
        {
            _logger = logger;
        }

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
            var foundUnit = LootList.FirstOrDefault(u => u.Guid == mouseOverUnitGuid);

            if (foundUnit == null) return null;
            if (foundUnit == ObjectManager.Me.Pet) return null;
            if (UnitsLooted.Contains(foundUnit)) return null;

            return foundUnit;
        }

        public void Do()
        {
            var waypointManager = new WaypointManager(new Path(), _logger);

            foreach (var unit in LootList)
            {
                _logger.Information($"Moving to unit for looting: {unit.Location}. Current: {ObjectManager.Me.Location} Distance: {ObjectManager.Me.Location.GetDistanceTo(unit.Location)}");
                waypointManager.MoveToWaypoint(unit.Location, 5);
                
                KeyboardCommandDispatcher.GetKeyboard().SendShiftClick(new Point(900,450));
                for (var x = 900; x <= 1150; x += 30)
                {
                    for (var y = 450; y <= 850; y += 20)
                    {
                        var mouseOverUnitGuid = GetPositionMouseOverUnit(new Point(x, y));
                        if (mouseOverUnitGuid > 0)
                        {
                            var foundUnit = GetValidUnit(mouseOverUnitGuid);
                            if (foundUnit == null) continue;
                            
                            _logger.Information($"Attempting to loot unit: {unit}");
                            KeyboardCommandDispatcher.GetKeyboard().SendShiftClick(new Point(x, y));
                            Task.Delay(1000).Wait();

                            var wowUnit = foundUnit as WowUnit;
                            if (wowUnit != null)
                            {
                                if (wowUnit.Attributes.Skinnable)
                                {
                                    _logger.Information($"Attempting to skin unit: {unit}");
                                    KeyboardCommandDispatcher.GetKeyboard().SendShiftClick(new Point(x, y));
                                    Task.Delay(2000).Wait();
                                }
                            }

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
