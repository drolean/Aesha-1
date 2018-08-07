using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aesha.Domain;
using Aesha.Robots.Actions;
using Serilog;

namespace Aesha.Core
{
    public class WaypointManager
    {
        private readonly Path _path;
        private readonly ILogger _logger;
        private Location _nextWaypoint;

        public WaypointManager(Path path, ILogger logger)
        {
            _path = path;
            _logger = logger;
        }

        public void MoveToWaypoint(Location location, int stopAt = 20, bool continuousMode = false)
        {
            CommandManager.GetDefault().SetPlayerFacing(location);

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

                _logger.Information($"Current location: {ObjectManager.Me.Location} Distance: {distanceToWaypoint}");

                Thread.Sleep(50);
            }

            _logger.Information($"Ending location: {ObjectManager.Me.Location} Distance: {distanceToWaypoint}");
            
            if (!continuousMode)
                CommandManager.GetDefault().StopMovingForward();
        }

        public Location GetNextWaypoint()
        {
            if (_nextWaypoint == null)
            {
                _nextWaypoint = _path.Entries.First().Value;
                _logger.Information($"Distance to start of path: {_nextWaypoint.GetDistanceTo(ObjectManager.Me.Location)}");
                return _nextWaypoint;
            }

            _nextWaypoint = _path.GetNextWaypoint(_nextWaypoint);
            return _nextWaypoint;
        }
    }
}
