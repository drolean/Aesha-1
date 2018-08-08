﻿using Aesha.Core;
using Aesha.Domain;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Aesha.Robots.Actions;
using Serilog;

namespace Aesha.Robots
{
    public class Warlock : IRobot
    {
        private readonly CommandManager _commandManager;
        private readonly WaypointManager _waypointManager;
        private readonly ILogger _logger;
        
        // ReSharper disable InconsistentNaming
        private readonly Spell DemonSkin = new Spell(687, "Demon Skin", 1, MappedKeys.ActionBar3);
        private readonly Spell SummonImp = new Spell(0, "Summon Imp", 1, MappedKeys.ActionBar8,Timings.TenSeconds);
        private readonly Spell Drink = new Spell(0, "Drink", 1, MappedKeys.ActionBar10, Timings.ThirtySeconds);

        private readonly Spell Corruption = new Spell(172, "Corruption", 1, MappedKeys.ActionBar4, 2000);
        private readonly Spell ShadowBolt = new Spell(0, "Shadow Bolt", 2, MappedKeys.ActionBar2, 2200);
        // ReSharper restore InconsistentNaming

        // ReSharper disable InconsistentNaming
        private readonly SummonPet SummonPet;
        private readonly CastBuff CastDemonSkin;
        private readonly CastDrink CastDrink;
        private readonly AcquireTarget AcquireTarget;
        private readonly LootUnits LootUnits;

        private readonly CastDebuff CastCorruption;
        private readonly CastOffensiveSpell CastShadowBolt;

        private RobotState _state;
        // ReSharper restore InconsistentNaming

        public Warlock(CommandManager commandManager, WaypointManager waypointManager, ILogger logger)
        {
            _commandManager = commandManager;
            _waypointManager = waypointManager;
            _logger = logger;

            SummonPet = new SummonPet(SummonImp);
            CastDemonSkin = new CastBuff(DemonSkin);
            CastDrink = new CastDrink(Drink);
            AcquireTarget = new AcquireTarget();
            LootUnits = new LootUnits(_logger);

            CastCorruption = new CastDebuff(Corruption);
            CastShadowBolt = new CastOffensiveSpell(ShadowBolt);

            _state = RobotState.Passive;

        }
     
        
        public void PassiveBehaviour()
        {
            _commandManager.EvaluateAndPerform(LootUnits);
            _commandManager.EvaluateAndPerform(SummonPet);
            _commandManager.EvaluateAndPerform(CastDemonSkin);
            _commandManager.EvaluateAndPerform(CastDrink);

            var waypoint = _waypointManager.GetNextWaypoint();
            _waypointManager.MoveToWaypoint(waypoint, continuousMode: true);
        }


        public void AttackBehaviour()
        {
            _commandManager.SetPlayerFacing(ObjectManager.Me.Target.Location);
            _commandManager.EvaluateAndPerform(CastCorruption);
            _commandManager.EvaluateAndPerform(CastShadowBolt);
        }
        
        public void Tick()
        {
            switch (_state)
            {
                case RobotState.Combat:
                    AttackBehaviour();
                    break;
                case RobotState.Passive:
                    PassiveBehaviour();
                    break;
            }

            _commandManager.EvaluateAndPerform(AcquireTarget);
            if (ObjectManager.Me.Target != null && ObjectManager.Me.Target?.Health.Current > 0)
            {
                _commandManager.StopMovingForward();
                LootUnits.AddUnit(ObjectManager.Me.Target);

                if (_state == RobotState.Combat) return;
        
                _state = RobotState.Combat;
                _logger.Information($"Switching to COMBAT state");
            }
            else
            {
                if (_state == RobotState.Passive) return;
                
                _commandManager.ClearTarget();
                _logger.Information($"No targets found");
                _state = RobotState.Passive;
                _logger.Information($"Switching to PASSIVE state");
            }

         
        }
    }
}
