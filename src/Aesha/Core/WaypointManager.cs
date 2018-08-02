using System.Threading;
using Aesha.Domain;

namespace Aesha.Core
{
    public class WaypointManager
    {
        private readonly CommandManager _commandManager;
        private readonly Path _path;
        private Location _nextWaypoint;

        public WaypointManager(CommandManager commandManager, Path path)
        {
            _commandManager = commandManager;
            _path = path;
        }

        public void MoveToWaypoint(Location location, int stopAt = 30, bool continuousMode = true)
        {
            _commandManager.SetPlayerFacing(location);
            _commandManager.SendKeyDown(MappedKey.Forward);

            var distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            while (distanceToWaypoint >= stopAt)
            {
                Thread.Sleep(100);
                distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            }

            if (!continuousMode)
                _commandManager.SendKeyUp(MappedKey.Forward);
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
