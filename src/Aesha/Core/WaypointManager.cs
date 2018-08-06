using System.Threading.Tasks;
using Aesha.Domain;

namespace Aesha.Core
{
    public class WaypointManager
    {
        private readonly Path _path;
        private Location _nextWaypoint;

        public WaypointManager(Path path)
        {
            _path = path;
        }

        public static void MoveToWaypoint(Location location, int stopAt = 20, bool continuousMode = false)
        {
            CommandManager.GetDefault().SetPlayerFacing(location);
            CommandManager.GetDefault().SendKeyDown(MappedKeys.Forward);

            var distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            while (distanceToWaypoint >= stopAt)
            {
                Task.Delay(100).Wait();
                distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            }

            if (!continuousMode)
                CommandManager.GetDefault().StopMovingForward();
        }

        public Location GetNextWaypoint()
        {
            var nextWaypoint = _nextWaypoint != null
                ? _path.GetNextWaypoint(_nextWaypoint)
                : _path.FindNearestWaypoint(ObjectManager.Me.Location);

            _nextWaypoint = nextWaypoint;
            return _nextWaypoint;
        }
    }
}
