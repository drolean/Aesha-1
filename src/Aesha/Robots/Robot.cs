using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aesha.Core;
using Aesha.Objects;
using Aesha.Objects.Model;
using FluentBehaviourTree;
using Serilog;

namespace Aesha.Robots
{
    public abstract class Robot
    {
        protected readonly CommandManager CommandManager;
        protected Path Path;
        protected readonly List<string> EnemyList;
        protected readonly ILogger Logger;
        protected IBehaviourTreeNode Tree { get; set; }

        public Robot(
            CommandManager commandManager,
            Path path, 
            List<string> enemyList, 
            ILogger logger)
        {
            CommandManager = commandManager;
            Path = path;
            EnemyList = enemyList;
            Logger = logger;

        }


        protected abstract void Behaviour();

        protected WowUnit GetNearestUntaggedMob()
        {
            var enemies = ObjectManager.Units.Where(u =>
                    EnemyList.Any(e => e.Contains(u.Name))
                    && u.Health.Percentage == 100
                    && u.Distance < 1200)
                .OrderBy(u => u.Distance).ToList();

            var nearest = enemies.FirstOrDefault();
            return nearest ?? null;
        }

        protected WowUnit GetEnemyAttackingMe()
        {
            var enemies = ObjectManager.Units.Where(u =>
                    EnemyList.Any(e => e.Contains(u.Name))
                    && u.Target == ObjectManager.Me)
                .OrderBy(u => u.Distance).ToList();

            var nearest = enemies.FirstOrDefault();
            return nearest ?? null;
        }

        protected void MoveToWaypoint(Location location, int stopAt = 30, bool continuousMode = true)
        {

            Logger.Information($"Setting player facing {location}");
            CommandManager.SetPlayerFacing(location);
            
            Logger.Information($"Sending W key");
            CommandManager.SendKeyDown('W');


            var distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            while (distanceToWaypoint >= stopAt)
            {
                Thread.Sleep(100);
                distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            }

            Logger.Information($"Moved to {location}. Distance to waypoint is {distanceToWaypoint}. Current location is {ObjectManager.Me.Location} Waiting 100ms");

            if (!continuousMode)
                CommandManager.SendKeyUp('W');
        }

        protected void WaitFor(Func<bool> condition)
        {
            while (!condition.Invoke())
                Thread.Sleep(500);
        }

        public void Pulse()
        {
            if (Tree == null) Behaviour();
            Tree?.Tick(new TimeData());

        }
    }
}