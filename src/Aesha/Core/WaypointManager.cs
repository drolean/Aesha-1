using System.Linq;
using System.Threading;
using Aesha.Domain;
using Serilog;

namespace Aesha.Core
{
    public class WaypointManager
    {
        private readonly Path _path;
        private readonly ILogger _logger;
        private int _currentWaypointIndex;

        public WaypointManager(Path path, ILogger logger)
        {
            _path = path;
            _logger = logger;
        }

        public void MoveToWaypoint(Location location, int stopAt = 20, bool continuousMode = false, bool forseMemoryWriteFacing = false)
        {
            CommandManager.GetDefault().SetPlayerFacing(location, forceMemoryWrite: forseMemoryWriteFacing);

            var startingDistance = location.GetDistanceTo(ObjectManager.Me.Location);
            _logger.Information($"Starting location: {ObjectManager.Me.Location} Distance: {startingDistance}");

            if (startingDistance < 10)
            {
                _logger.Information($"Already at waypoint. Aborting move");
                return;
            }

            CommandManager.GetDefault().SendKeyDown(MappedKeys.Forward);
            
            var distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            while (distanceToWaypoint >= stopAt)
            {
                distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
                CommandManager.GetDefault().SetPlayerFacing(location);
            }

            _logger.Information($"Ending location: {ObjectManager.Me.Location} Distance: {distanceToWaypoint}");
            
            if (!continuousMode)
                CommandManager.GetDefault().StopMovingForward();
        }

        public Location GetNextWaypoint()
        {
            if (_currentWaypointIndex == 0)
            {
                _currentWaypointIndex = _path.FindNearestWaypointIndex(ObjectManager.Me.Location);
                _logger.Information($"Distance to nearest waypoint is: {_path.Entries[_currentWaypointIndex].GetDistanceTo(ObjectManager.Me.Location)}");
                return _path.Entries[_currentWaypointIndex];
            }

            _currentWaypointIndex = _path.GetNextWaypointIndex(_currentWaypointIndex + 2);
            return _path.Entries[_currentWaypointIndex];
        }
    }
}
