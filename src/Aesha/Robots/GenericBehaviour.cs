using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aesha.Core;
using Aesha.Domain;
using Aesha.Infrastructure;
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

        private List<WowUnit> _blacklist = new List<WowUnit>();

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
            _logger.Information($"Found nearest waypoint: {_nextWaypoint}. Current Location: {ObjectManager.Me.Location}. Distance: {ObjectManager.Me.Location.GetDistanceTo(_nextWaypoint)}");
            _commandManager.SetPlayerFacing(_nextWaypoint);
            _logger.Information($"Facing waypoint");
            return BehaviourTreeStatus.Success;
        }
        
        public BehaviourTreeStatus MoveToNextWaypoint()
        {
            _logger.Information($"Moving to nearest waypoint: {_nextWaypoint}. Current Location: {ObjectManager.Me.Location}. Distance: {ObjectManager.Me.Location.GetDistanceTo(_nextWaypoint)}");
            _waypointManager.MoveToWaypoint(_nextWaypoint);
            _logger.Information($"Arrived at nearest waypoint: {_nextWaypoint}. Current Location: {ObjectManager.Me.Location}. Distance: {ObjectManager.Me.Location.GetDistanceTo(_nextWaypoint)}");
            return BehaviourTreeStatus.Running;
        }

        public BehaviourTreeStatus SetTarget(WowUnit unit)
        {
            _commandManager.SetTarget(unit);
            return BehaviourTreeStatus.Success;
        }


        public BehaviourTreeStatus Drink()
        {
            while (ObjectManager.Me.Mana.Current <= 25)
            {
                _commandManager.SendKey(MappedKey.ActionBar10);
                Thread.Sleep(10000);
            }

            return BehaviourTreeStatus.Success;
        }
        
        public void WaitFor(Func<bool> condition, int interval = 500)
        {
            while (!condition.Invoke())
                Thread.Sleep(interval);
        }

        public BehaviourTreeStatus LootTargets()
        {
            var lootableMobs = ObjectManager.Units.Where(u => 
                !_blacklist.Contains(u)
                && u.Attributes.Lootable 
                && u.CreatureType != CreatureType.Critter
                && u.Attributes.TappedByMe
                && u.Health.Current == 0).ToList();
            
            if (lootableMobs.Count > 0)
                _commandManager.SendKey(MappedKey.Forward);
            else return BehaviourTreeStatus.Success;
            
            _logger.Information($"Looting mobs in the area. Found: {lootableMobs.Count}");
            
            foreach (var mob in lootableMobs)
            {
                _logger.Information($"Looting mob: {mob}");
                _logger.Information($"Moving to loot location {mob.Location}");
                _waypointManager.MoveToWaypoint(mob.Location, 1, false);
                _logger.Information($"Moved to loot location {mob.Location}");

                _commandManager.Loot(lootableMobs);
                _blacklist.Add(mob);
            }

            return BehaviourTreeStatus.Success;
        }
    }
}