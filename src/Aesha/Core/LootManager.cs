using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Domain;
using Serilog;

namespace Aesha.Core
{
    public class LootManager
    {
        private readonly ILogger _logger;
        private readonly WaypointManager _waypointManager;
        private readonly List<WowUnit> _looted;

        public LootManager(ILogger logger)
        {
            _logger = logger;
            _waypointManager = new WaypointManager(new Path(), logger);
            _looted = new List<WowUnit>();
        }

        private WowUnit GetPositionMouseOverUnit(Point point)
        {
            var offsetPoint = KeyboardCommandDispatcher.GetKeyboard().GetOffsetPoint(point);
            Cursor.Position = offsetPoint;
            Task.Delay(10).Wait();
            var mouseOverUnit = CommandManager.GetDefault().MouseOverUnit;

            return ObjectManager.Units.FirstOrDefault(u => u.Guid == mouseOverUnit);
        }

        public void Loot(WowUnit unit)
        {
            if (unit == null) return;
     
            _logger.Information($"Moving to unit for looting: {unit}. Current: {ObjectManager.Me.Location} Distance: {ObjectManager.Me.Location.GetDistanceTo(unit.Location)}");
            _waypointManager.MoveToWaypoint(unit.Location, 5, forseMemoryWriteFacing: true);
            
            for (var x = 1150; x >= 900; x -= 30)
            {
                for (var y = 850; y >= 450; y -= 20)
                {
                    var evalPoint = new Point(x, y);

                    var foundUnit = GetPositionMouseOverUnit(evalPoint);
                    if (foundUnit == null) continue;

                    InternalLoot(evalPoint);
                    _looted.Add(foundUnit);
                    return;
                }
            }
            
            
            if (ObjectManager.Me.Target != null && ObjectManager.Me.Target.Health.Current == 0)
                CommandManager.GetDefault().ClearTarget();
        }

       

        private void InternalLoot(Point point)
        {
            _logger.Information($"Attempting to loot unit");
            KeyboardCommandDispatcher.GetKeyboard().SendShiftClick(point);
            Task.Delay(1000).Wait();
        }
    }
}
