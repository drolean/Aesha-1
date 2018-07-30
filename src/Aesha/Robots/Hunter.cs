using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aesha.Commands;
using Aesha.Core;
using Aesha.Infrastructure;
using Aesha.Objects;
using Aesha.Objects.Model;
using FluentBehaviourTree;
using Serilog;

namespace Aesha.Robots
{
    public class Hunter : Robot
    {
        
        public Hunter(CommandManager commandManager, Path path, List<string> enemyList, CommandDispatcher dispatcher, ILogger logger)
            : base(commandManager, path, enemyList, dispatcher, logger)
        {
            EnemyList.Add("Felslayer");
            EnemyList.Add("Lesser Felguard");
            EnemyList.Add("Ghostpaw Runner");
        }

        private WowUnit _currentTarget;

        public void LoadBehaviour()
        {
            var builder = new BehaviourTreeBuilder();
            Tree = builder.Sequence("aquire target")
                .Do("set target", t =>
                {
                    _currentTarget = GetNearestUntaggedMob();
                    Logger.Information($"Nearest enemy is {_currentTarget}");
                    return BehaviourTreeStatus.Success;
                })
                .Do("target target", t =>
                {
                    CommandManager.SetPlayerFacing(_currentTarget.Location);
                    CommandManager.SetTarget(_currentTarget.Guid);
                    CommandManager.SendKey('G');
                    return BehaviourTreeStatus.Success;
                })
                .Do("send pet attack", t =>
                {
                    CommandManager.SendKey('1');
                    return BehaviourTreeStatus.Success;
                })
                .Do("move closer to target", t =>
                {
                    if (_currentTarget.Distance > 400)
                    {
                        Logger.Information($"Distance is over 400. Moving closer to target");
                        MoveToWaypoint(_currentTarget.Location, 400, false);
                    }

                    return BehaviourTreeStatus.Success;
                })
                .Do("wait for target to target pet", t =>
                {
                    WaitFor(() => _currentTarget.Target == ObjectManager.Me.Pet);
                    return BehaviourTreeStatus.Success;
                })
                .Do("auto attack", t =>
                {
                    CommandManager.SendKey('2');
                    return BehaviourTreeStatus.Success;
                })
                .Do("kill target", t =>
                {
                    while (_currentTarget.Health.Current != 0)
                    {
                        Logger.Information(
                            $"Waiting for {_currentTarget} to die. Current health is {_currentTarget.Health.Percentage}");
                        CommandManager.SendKey('3');
                        Thread.Sleep(1500);

                        if (!_currentTarget.HasAura(13550)) CommandManager.SendKey('4');

                        Thread.Sleep(4500);
                    }

                    return BehaviourTreeStatus.Success;
                }).End().Build();
        }

        public IBehaviourTreeNode Tree { get; set; }

        public void Handle(StartRobot c)
        {
            var nearestWaypoint = Path.FindNearestWaypoint(ObjectManager.Me.Location);
            CommandManager.SetPlayerFacing(nearestWaypoint);
        }

    }

    public abstract class Robot
    {
        protected readonly CommandManager CommandManager;
        protected Path Path;
        protected readonly List<string> EnemyList;
        protected CommandDispatcher Dispatcher;
        protected readonly ILogger Logger;

        public Robot(
            CommandManager commandManager,
            Path path, 
            List<string> enemyList, 
            CommandDispatcher dispatcher, 
            ILogger logger)
        {
            CommandManager = commandManager;
            Path = path;
            EnemyList = enemyList;
            Dispatcher = dispatcher;
            Logger = logger;
        }


        protected WowUnit GetNearestUntaggedMob()
        {
            var enemies = ObjectManager.Units.Where(u =>
                    EnemyList.Any(e => e.Contains(u.Name))
                    && u.Health.Percentage == 100
                    && u.Distance < 800)
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
    }
}
