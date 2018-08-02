using System;
using System.Threading;
using Aesha.Core;
using Aesha.Domain;
using FluentBehaviourTree;
using Serilog;

namespace Aesha.Robots
{
    public class GenericBehaviour
    {
        private readonly CommandManager _commandManager;
        private readonly WaypointManager _waypointManager;
        private readonly ILogger _logger;
        private Location _nextWaypoint;

        public GenericBehaviour(
            CommandManager commandManager,
            WaypointManager waypointManager,
            ILogger logger)
        {
            _commandManager = commandManager;
            _waypointManager = waypointManager;
            _logger = logger;
        }       

        public BehaviourTreeStatus GetNextWaypoint()
        {
            _nextWaypoint = _waypointManager.GetNextWaypoint();
            _commandManager.SetPlayerFacing(_nextWaypoint);
            _logger.Information("find nearest waypoint");
            return BehaviourTreeStatus.Success;
        }
        
        public BehaviourTreeStatus MoveToNextWaypoint()
        {
            _logger.Information("move to next waypoint");
            _waypointManager.MoveToWaypoint(_nextWaypoint);
            return BehaviourTreeStatus.Running;
        }

        public BehaviourTreeStatus SetTarget(WowUnit unit)
        {
            _commandManager.SetTarget(unit);
            return BehaviourTreeStatus.Success;
        }


        public BehaviourTreeStatus MoveCloserToTarget(WowUnit unit)
        {
            if (unit.Distance > 400)
            {
                _logger.Information($"Distance is over 400. Moving closer to target");
                _waypointManager.MoveToWaypoint(unit.Location, 400, false);
            }

            return BehaviourTreeStatus.Success;
        }
        
        public void WaitFor(Func<bool> condition, int interval = 500)
        {
            while (!condition.Invoke())
                Thread.Sleep(interval);
        }
    }
}