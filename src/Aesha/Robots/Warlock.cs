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

        private void AutoAttack()
        {
            _commandManager.SendKeyUp(MappedKey.Forward);
            _commandManager.SendKey(MappedKey.ActionBar1);
        }

        private void KillTarget()
        {
            while (ObjectManager.Me.Target != null && ObjectManager.Me.Target.Health.Current != 0)
            {
                if (ObjectManager.Me.Mana.Current >= 25)
                {
                    _commandManager.SendKey(MappedKey.ActionBar2);
                    Thread.Sleep(1700);
                }
            }
        }


        private void Buff()
        {
            if(!ObjectManager.Me.Auras.Contains(_demonSkinRank1))
                _commandManager.SendKey(MappedKey.ActionBar3);
        }



        public void PassiveBehaviour()
        {
            SummonPet();
            _generic.LootTargets(); 
            _generic.GetNextWaypoint();
            _generic.MoveToNextWaypoint();
        }

        private void SummonPet()
        {
            if (ObjectManager.Me.Pet == null)
            {
                _commandManager.SendKeyUp(MappedKey.Forward);
                _commandManager.SendKey(MappedKey.ActionBar9);
                Thread.Sleep(11000);
            }
        }

        public void AttackBehaviour()
        {
            Buff();
            _generic.Drink();
            _generic.SetTarget(_currentTarget);
            AutoAttack();
            KillTarget();
        }
        
        public void Tick(RobotState state)
        {
            switch (state)
            {
                case RobotState.Combat:
                    AttackBehaviour();
                    break;
                case RobotState.Passive:
                    PassiveBehaviour();
                    break;
            }
        }

        public void SetTarget(WowUnit target)
        {
            _currentTarget = target;
        }
    }
}
