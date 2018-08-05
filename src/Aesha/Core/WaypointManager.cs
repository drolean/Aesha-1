using System.Threading;
using Aesha.Domain;
using Serilog;

namespace Aesha.Core
{
    public class WaypointManager
    {
        private readonly CommandManager _commandManager;
        private readonly Path _path;
        private readonly ILogger _logger;
        private Location _nextWaypoint;

        public WaypointManager(CommandManager commandManager, Path path, ILogger logger)
        {
            _commandManager = commandManager;
            _path = path;
            _logger = logger;
        }

        public void MoveToWaypoint(Location location, int stopAt = 20, bool continuousMode = true)
        {
            _commandManager.SetPlayerFacing(location);
            _commandManager.SendKeyDown(MappedKey.Forward);

            var distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            if (distanceToWaypoint > 1000)
            {   
                //too far. stop moving
                _logger.Information("Distance too far. Aborting movement");
                _commandManager.SendKeyUp(MappedKey.Forward);
                return;
            }

            int iterations = 0;
            while (distanceToWaypoint >= stopAt)
            {
                Thread.Sleep(100);
                distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
                iterations++;

                if (iterations > 100) //10 seconds
                {
                    _logger.Information("Too many movement iterations. Might be stuck");
                    _commandManager.SendKeyUp(MappedKey.Forward);
                    return;
                }

            }

            if (!continuousMode)
            {
                _commandManager.SendKeyUp(MappedKey.Forward);
                _logger.Information("Arrived at waypoint, running in continuous mode");
            }
            
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
