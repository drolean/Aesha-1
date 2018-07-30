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
                .Sequence("passive")
                    .Do("find nearest waypoint", t =>
                    {
                        _nearestWaypoint = Path.FindNearestWaypoint(ObjectManager.Me.Location);
                        CommandManager.SetPlayerFacing(_nearestWaypoint);
                        return BehaviourTreeStatus.Success;
                    })
                    .Do("check if im being attacked", t =>
                    {
                        var target = GetEnemyAttackingMe();
                        if (target != null) return BehaviourTreeStatus.Failure;
                        return BehaviourTreeStatus.Success;
                    })
                    .Do("move to next waypoint", t =>
                    {
                        MoveToWaypoint(_nearestWaypoint);
                        return BehaviourTreeStatus.Success;
                    })
                .Sequence("combat")
                    .Do("set target", t =>
                    {
                        var target = GetEnemyAttackingMe();
                        if (target != null)
                        {
                            _currentTarget = target;
                            return BehaviourTreeStatus.Success;
                        }

                        _currentTarget = GetNearestUntaggedMob();
                        Logger.Information($"Nearest enemy is {_currentTarget}");

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
                        while (_currentTarget.Health.Current != 0)
                        {
                            Logger.Information(
                                $"Waiting for {_currentTarget} to die. Current health is {_currentTarget.Health.Percentage}");
                            CommandManager.SendKey('3');
                            Thread.Sleep(1500);

                            if (!_currentTarget.HasAura(_serpentSting)) CommandManager.SendKey('4');

                            Thread.Sleep(4500);
                        }

                        return BehaviourTreeStatus.Success;
                    })
                .End().Build();
        }

    }
}
