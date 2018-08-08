using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Domain;
using Serilog;

namespace Aesha.Core
{
    public class SkinningManager
    {
        private readonly ILogger _logger;
        private readonly WaypointManager _waypointManager;
        private readonly List<WowUnit> _skinned;

        public SkinningManager(ILogger logger)
        {
            _logger = logger;
            _waypointManager = new WaypointManager(new Path(), logger);
            _skinned = new List<WowUnit>();
        }

        private WowUnit GetPositionMouseOverUnit(Point point)
        {
            var offsetPoint = KeyboardCommandDispatcher.GetKeyboard().GetOffsetPoint(point);
            Cursor.Position = offsetPoint;
            Task.Delay(10).Wait();
            var mouseOverUnit = CommandManager.GetDefault().MouseOverUnit;

            return ObjectManager.Units.FirstOrDefault(u => u.Guid == mouseOverUnit);
        }

        public void Skin(WowUnit unit)
        {
            //var unit = GetNearestSkinnableUnit();
            if (unit == null) return;

            _logger.Information($"Moving to unit for skinning: {unit.Location}. Current: {ObjectManager.Me.Location} Distance: {ObjectManager.Me.Location.GetDistanceTo(unit.Location)}");
            _waypointManager.MoveToWaypoint(unit.Location, 5, forseMemoryWriteFacing: true);

            for (var x = 1150; x >= 900; x -= 30)
            {
                for (var y = 850; y >= 450; y -= 20)
                {
                    var evalPoint = new Point(x, y);

                    var foundUnit = GetPositionMouseOverUnit(evalPoint);
                    if (foundUnit == null) continue;

                    InternalSkin(evalPoint);
                    _skinned.Add(foundUnit);
                    return;
                }
            }

            if (ObjectManager.Me.Target != null && ObjectManager.Me.Target.Health.Current == 0)
                CommandManager.GetDefault().ClearTarget();
        }


        private WowUnit GetNearestSkinnableUnit()
        {
            return ObjectManager.Units
                .Where(u => u.Attributes.Skinnable
                    && _skinned.All(l => l.Guid != u.Guid)
                    && u.Location.GetDistanceTo(ObjectManager.Me.Location) < 800)
                .OrderBy(u => u.Location.GetDistanceTo(ObjectManager.Me.Location))
                .FirstOrDefault();

        }

        private void InternalSkin(Point point)
        {
            _logger.Information($"Attempting to skin unit");
            KeyboardCommandDispatcher.GetKeyboard().SendShiftClick(point);
            Task.Delay(2400).Wait();
        }
    }
}
