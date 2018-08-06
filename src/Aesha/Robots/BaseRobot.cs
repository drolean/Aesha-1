using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aesha.Core;
using Aesha.Domain;
using Aesha.Infrastructure;
using Serilog;

namespace Aesha.Robots
{
    public abstract class BaseRobot
    {
        protected readonly CommandManager CommandManager;
        protected readonly WaypointManager WaypointManager;
        private readonly ILogger _logger;
        private Location _nextWaypoint;

        protected readonly List<WowUnit> Blacklist = new List<WowUnit>();

        public BaseRobot(
            CommandManager commandManager,
            WaypointManager waypointManager,
            ILogger logger)
        {
            CommandManager = commandManager;
            WaypointManager = waypointManager;
            _logger = logger;
        }       

        public void GetNextWaypoint()
        {
            _nextWaypoint = WaypointManager.GetNextWaypoint();
            _logger.Information($"Found nearest waypoint: {_nextWaypoint}. Current Location: {ObjectManager.Me.Location}. Distance: {ObjectManager.Me.Location.GetDistanceTo(_nextWaypoint)}");
            CommandManager.SetPlayerFacing(_nextWaypoint);
            _logger.Information($"Facing waypoint");
        }
        
        public void MoveToNextWaypoint()
        {
            _logger.Information($"Moving to nearest waypoint: {_nextWaypoint}. Current Location: {ObjectManager.Me.Location}. Distance: {ObjectManager.Me.Location.GetDistanceTo(_nextWaypoint)}");
            WaypointManager.MoveToWaypoint(_nextWaypoint,20, true);
            _logger.Information($"Arrived at nearest waypoint: {_nextWaypoint}. Current Location: {ObjectManager.Me.Location}. Distance: {ObjectManager.Me.Location.GetDistanceTo(_nextWaypoint)}");
        }

        public void SetTarget(WowUnit unit)
        {
            CommandManager.SetTarget(unit);
        }
        
        public void WaitFor(Func<bool> condition, int interval = 500)
        {
            while (!condition.Invoke())
                Task.Delay(interval).Wait();
        }

        public void LootTargets()
        {
            var lootableMobs = ObjectManager.Units.Where(u => 
                !Blacklist.Contains(u)
                && u.Attributes.Lootable 
                && u.CreatureType != CreatureType.Critter
                && u.Attributes.TappedByMe
                && u.Health.Current == 0).ToList();

            if (lootableMobs.Count > 0)
                CommandManager.SendKey(MappedKeys.Forward);
            else return;
            
            _logger.Information($"Looting mobs in the area. Found: {lootableMobs.Count}");
            
            foreach (var mob in lootableMobs)
            {
                _logger.Information($"Looting mob: {mob}");
                _logger.Information($"Moving to loot location {mob.Location}");
                WaypointManager.MoveToWaypoint(mob.Location, 1, false);
                _logger.Information($"Moved to loot location {mob.Location}");

                CommandManager.Loot(lootableMobs);
                Blacklist.Add(mob);
            }
        }
    }
}