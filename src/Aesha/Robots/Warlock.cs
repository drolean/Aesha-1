using System;
using System.Collections.Generic;
using System.Threading;
using Aesha.Core;
using Aesha.Domain;
using Aesha.Interfaces;
using FluentBehaviourTree;
using Serilog;

namespace Aesha.Robots
{
    public class Warlock : IRobot
    {
        private readonly CommandManager _commandManager;
        private readonly GenericBehaviour _generic;
        private ILogger _logger;
        private WowUnit _currentTarget;

        private Spell _demonSkinRank1 = new Spell(687,"Demon Skin",1);


        public Warlock(CommandManager commandManager, WaypointManager waypointManager, List<string> enemyList, ILogger logger)

        {
            _commandManager = commandManager;
            _logger = logger;
            _generic = new GenericBehaviour(commandManager, waypointManager, logger);
        }

        private BehaviourTreeStatus SetPetAttack()
        {
            _commandManager.SendKey(MappedKey.ActionBar1);
            return BehaviourTreeStatus.Success;
        }

        private BehaviourTreeStatus WaitForTargetToTargetPet()
        {
            _generic.WaitFor(() => ObjectManager.Me.Target.Target == ObjectManager.Me.Pet);
            return BehaviourTreeStatus.Success;
        }

        private BehaviourTreeStatus AutoAttack()
        {
            _commandManager.SendKeyUp(MappedKey.Forward);
            _commandManager.SendKey(MappedKey.ActionBar1);
            return BehaviourTreeStatus.Success;
        }

        private BehaviourTreeStatus KillTarget()
        {
            while (ObjectManager.Me.Target != null && ObjectManager.Me.Target.Health.Current != 0)
            {
                _commandManager.SetPlayerFacing(_currentTarget.Location);

                if (ObjectManager.Me.Mana.Current >= 25)
                {
                    _commandManager.SendKey(MappedKey.ActionBar2);
                    Thread.Sleep(1700);
                }
            }

            return BehaviourTreeStatus.Success;
        }


        private BehaviourTreeStatus Buff()
        {
            if(!ObjectManager.Me.Auras.Contains(_demonSkinRank1))
                _commandManager.SendKey(MappedKey.ActionBar3);

            return BehaviourTreeStatus.Success;
        }



        public IBehaviourTreeNode PassiveBehaviour
        {
            get
            {
                return new BehaviourTreeBuilder()
                    .Sequence("passive")
                        .Do("find nearest waypoint", t => _generic.GetNextWaypoint())
                        .Do("move to next waypoint", t => _generic.MoveToNextWaypoint())
                    .End()
                    .Build();
            }
        }

        public IBehaviourTreeNode AttackBehaviour
        {
            get
            {
                return new BehaviourTreeBuilder()
                    .Sequence("combat")
                        .Do("buff", t => Buff())
                        .Do("set target", t => _generic.SetTarget(_currentTarget))
                       // .Do("send pet attack", t => SetPetAttack())
                        .Do("Wait for mana", t => _generic.WaitForMana(_currentTarget))
                       // .Do("wait for target to target pet", t => WaitForTargetToTargetPet())
                        .Do("auto attack", t => AutoAttack())
                        .Do("kill target", t => KillTarget())
                    .End()
                    .Build();
            }
        }

        public void Tick(RobotState state)
        {
            switch (state)
            {
                case RobotState.Combat:
                    AttackBehaviour.Tick(new TimeData());
                    break;
                case RobotState.Passive:
                    PassiveBehaviour.Tick(new TimeData());
                    break;
            }
        }

        public void SetTarget(WowUnit target)
        {
            _currentTarget = target;
        }
    }
}
