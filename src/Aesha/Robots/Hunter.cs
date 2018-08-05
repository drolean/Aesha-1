using System.Collections.Generic;
using Aesha.Core;
using Aesha.Domain;
using Aesha.Interfaces;
using FluentBehaviourTree;
using Serilog;

namespace Aesha.Robots
{
    public class Hunter : IRobot
    {
        private readonly CommandManager _commandManager;
        private readonly Spell _serpentSting = new Spell(13550, "Serpent Sting", 3);
        private readonly GenericBehaviour _generic;
        private ILogger _logger;
        private WowUnit _currentTarget;


        public Hunter(CommandManager commandManager, WaypointManager waypointManager, List<string> enemyList, ILogger logger)

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
            _commandManager.SendKey(MappedKey.ActionBar2);
            return BehaviourTreeStatus.Success;
        }

        private BehaviourTreeStatus KillTarget()
        {
            while (ObjectManager.Me.Target.Health.Current != 0)
            {
                while (ObjectManager.Me.Target.Distance < 400)
                {
                    _commandManager.SendKeyDown(MappedKey.Backward);
                }

                _commandManager.SendKeyUp(MappedKey.Backward);
                if (!ObjectManager.Me.Target.HasAura(_serpentSting)) _commandManager.SendKey(MappedKey.ActionBar4);
            }

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

        public IBehaviourTreeNode AttackBehaviour {
            get
            {
                return new BehaviourTreeBuilder()
                    .Sequence("combat")
                        .Do("set target", t => _generic.SetTarget(_currentTarget))
                        .Do("send pet attack", t => SetPetAttack())
                        .Do("move closer to target", t => _generic.Drink())
                        .Do("wait for target to target pet", t => WaitForTargetToTargetPet())
                        .Do("auto attack", t => AutoAttack())
                        .Do("kill target", t => KillTarget())
                    .End()
                    .Build();
            }
        }

        void IRobot.PassiveBehaviour()
        {
            throw new System.NotImplementedException();
        }

        void IRobot.AttackBehaviour()
        {
            throw new System.NotImplementedException();
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
