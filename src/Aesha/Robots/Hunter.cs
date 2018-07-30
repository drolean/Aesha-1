using System.Collections.Generic;
using System.Threading;
using Aesha.Core;
using Aesha.Objects;
using Aesha.Objects.Model;
using FluentBehaviourTree;
using Serilog;

namespace Aesha.Robots
{
    public class Hunter : Robot
    {

        public Hunter(CommandManager commandManager, Path path, List<string> enemyList, ILogger logger)
            : base(commandManager, path, enemyList, logger)
        {

        }
        
        private WowUnit _currentTarget;
        private Location _nearestWaypoint;
        private readonly Spell _serpentSting = new Spell(13550,"Serpent Sting",3);

        protected override void Behaviour()
        {
            var builder = new BehaviourTreeBuilder();
            Tree = builder
                .Sequence("root")
                    .Selector("passive")
                        .Do("find nearest waypoint", t =>
                        {
                            _nearestWaypoint = _nearestWaypoint != null 
                                ? Path.GetNextWaypoint(_nearestWaypoint) 
                                : Path.FindNearestWaypoint(ObjectManager.Me.Location);

                            CommandManager.SetPlayerFacing(_nearestWaypoint);
                            Logger.Information("find nearest waypoint");
                            return BehaviourTreeStatus.Failure;
                        })
                        .Do("check for enemies", t =>
                        {
                            Logger.Information("check for enemies");
                            var target = FindTarget();
                            if (target != null)
                            {
                                Logger.Information($"enemy {target} found. Exiting 'passive'");
                                return BehaviourTreeStatus.Success;
                            }

                            return BehaviourTreeStatus.Failure;
                        })
                        .Do("move to next waypoint", t =>
                        {
                            Logger.Information("move to next waypoint");
                            MoveToWaypoint(_nearestWaypoint);
                            if (FindTarget() != null)
                                return BehaviourTreeStatus.Success;

                            return BehaviourTreeStatus.Running;
                        })
                    .End()
                    .Sequence("combat")
                        .Do("set target", t =>
                        {
                            _currentTarget = FindTarget();
                            return _currentTarget == null ? BehaviourTreeStatus.Failure : BehaviourTreeStatus.Success;
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
                            Logger.Information($"Waiting for {_currentTarget} to die");
                            while (_currentTarget.Health.Current != 0)
                            {
                                while (_currentTarget.Distance < 400)
                                {
                                    CommandManager.SendKeyDown('S');
                                }
                                CommandManager.SendKeyUp('S');



                                if (!_currentTarget.HasAura(_serpentSting)) CommandManager.SendKey('4');
                            }

                            return BehaviourTreeStatus.Success;
                        })
                    .End()
                .End().Build();
        }

        private WowUnit FindTarget()
        {
            var target = GetEnemyAttackingMe();
            if (target == null) return GetNearestUntaggedMob();
            return target;
        }

    }
}
