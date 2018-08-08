using System.Collections.Generic;
using System.Threading.Tasks;
using Aesha.Core;
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

        private readonly List<IWowObject> _lootList;
        // ReSharper disable InconsistentNaming
        
        private readonly Spell SummonImp = new Spell(0, 1, MappedKeys.ActionBar8,Timings.TenSeconds);
        private readonly Spell Drink = new Spell(0, 1, MappedKeys.ActionBar10, Timings.ThirtySeconds);

        private readonly Spell Wand = new Spell(0, 1, MappedKeys.ActionBar1, 3000);
        private readonly Spell ShadowBolt = new Spell(0,2, MappedKeys.ActionBar2, 2200);
        private readonly Spell DemonSkin = new Spell(696,2, MappedKeys.ActionBar3);
        private readonly Spell Corruption = new Spell(172,1, MappedKeys.ActionBar4, 2000);
        private readonly Spell CurseOfAgony = new Spell(980,2, MappedKeys.ActionBar5);
        private readonly Spell Immolate = new Spell(707,2, MappedKeys.ActionBar6, 2000);


        // ReSharper restore InconsistentNaming

        // ReSharper disable InconsistentNaming
        private readonly SummonPet SummonPet;
        private readonly CastDrink CastDrink;
        private readonly TargetManager _targetManager;
        private readonly LootManager _lootManager;


        private readonly PetAttack CastPetAttack;
        private readonly CastWand CastWand;
        private readonly CastOffensiveSpell CastShadowBolt;
        private readonly CastBuff CastDemonSkin;
        private readonly CastDebuff CastCorruption;
        private readonly CastDebuff CastCurseOfAgony;
        private readonly CastDebuff CastImmolate;
        

        private RobotState _state;
        private SkinningManager _skinningManager;
        private Location _currentWaypoint;
        // ReSharper restore InconsistentNaming

        public Warlock(CommandManager commandManager, WaypointManager waypointManager, ILogger logger)
        {
            _commandManager = commandManager;
            _waypointManager = waypointManager;
            _logger = logger;

            SummonPet = new SummonPet(SummonImp);
            CastDrink = new CastDrink(Drink);
            _targetManager = new TargetManager();
            _lootManager = new LootManager(_logger);
            _skinningManager = new SkinningManager(_logger);
            
            CastPetAttack = new PetAttack();
            CastShadowBolt = new CastOffensiveSpell(ShadowBolt);
            CastDemonSkin = new CastBuff(DemonSkin);
            CastCorruption = new CastDebuff(Corruption);
            CastCurseOfAgony = new CastDebuff(CurseOfAgony);
            CastImmolate = new CastDebuff(Immolate);
            CastWand = new CastWand(Wand);

            _state = RobotState.Passive;
            _lootList = new List<IWowObject>();

        }
        

        public void PassiveBehaviour()
        {
            //_commandManager.EvaluateAndPerform(SummonPet);
            //_commandManager.EvaluateAndPerform(CastDemonSkin);
            //_commandManager.EvaluateAndPerform(CastDrink);

            _currentWaypoint = _waypointManager.GetNextWaypoint();
            _waypointManager.MoveToWaypoint(_currentWaypoint, continuousMode: true);
        }


        public void AttackBehaviour()
        {
            var target = ObjectManager.Me.Target;

            _commandManager.SetPlayerFacing(ObjectManager.Me.Target?.Location);
            _commandManager.EvaluateAndPerform(CastPetAttack);
            _commandManager.EvaluateAndPerform(CastCorruption);
            _commandManager.EvaluateAndPerform(CastCurseOfAgony);
            _commandManager.EvaluateAndPerform(CastImmolate);
             //_commandManager.EvaluateAndPerform(CastShadowBolt);
            //_commandManager.EvaluateAndPerform(CastWand);

            while (ObjectManager.Me.Target?.Health?.Current > 0)
            {
                _commandManager.EvaluateAndPerform(CastShadowBolt);
            }

            _lootManager.Loot(target);
            _skinningManager.Skin(target);
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

            _targetManager.UpdateTarget();


            if (ObjectManager.Me.Target != null && ObjectManager.Me.Target?.Health.Current > 0)
            {
                _logger.Information($"Target: {ObjectManager.Me.Target}");

                if (_state == RobotState.Combat) return;
                _commandManager.StopMovingForward();
                _state = RobotState.Combat;
                _logger.Information($"Switching to COMBAT state");
            }
            else
            {
                if (_state == RobotState.Passive) return;
                
                if (ObjectManager.Me.Target != null) _commandManager.ClearTarget();
                _logger.Information($"No targets found");
                _state = RobotState.Passive;
                _logger.Information($"Switching to PASSIVE state");
            }

         
        }
    }
}
