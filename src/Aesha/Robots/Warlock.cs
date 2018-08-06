using System.Threading.Tasks;
using Aesha.Core;
using Aesha.Domain;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Aesha.Robots.Actions;
using Serilog;

namespace Aesha.Robots
{
    public class Warlock : BaseRobot, IRobot
    {
        private readonly CommandManager _commandManager;
        private ILogger _logger;
        
        // ReSharper disable InconsistentNaming
        private readonly Spell DemonSkin = new Spell(687, "Demon Skin", 1, MappedKeys.ActionBar4);
        private readonly Spell SummonImp = new Spell(0, "Summon Imp", 1, MappedKeys.ActionBar8,Timings.TenSeconds);
        private readonly Spell Drink = new Spell(0, "Drink", 1, MappedKeys.ActionBar10, Timings.ThirtySeconds);
        // ReSharper restore InconsistentNaming

        // ReSharper disable InconsistentNaming
        private readonly SummonPet SummonPet;
        private readonly CastBuff CastDemonSkin;
        private readonly CastDrink CastDrink;
        private readonly AcquireTarget AcquireTarget;
        private readonly LootUnits LootUnits;
        // ReSharper restore InconsistentNaming

        public Warlock(CommandManager commandManager, WaypointManager waypointManager, ILogger logger)
        : base(commandManager,waypointManager,logger)
        {
            _commandManager = commandManager;
            _logger = logger;

            SummonPet = new SummonPet(SummonImp);
            CastDemonSkin = new CastBuff(DemonSkin);
            CastDrink = new CastDrink(Drink);
            AcquireTarget = new AcquireTarget();
            LootUnits = new LootUnits();

        }
        
        private void AutoAttack()
        {
            _commandManager.StopMovingForward();
            _commandManager.SendKey(MappedKeys.ActionBar1);
        }

        private void KillTarget()
        {
            while (ObjectManager.Me.Target != null && ObjectManager.Me.Target.Health.Current != 0)
            {
                if (ObjectManager.Me.Mana.Current >= 25)
                {
                    _commandManager.SendKey(MappedKeys.ActionBar2);
                    Task.Delay(2000).Wait();
                }
            }
        }

        
        public void PassiveBehaviour()
        {
            _commandManager.EvaluateAndPerform(LootUnits);
            _commandManager.EvaluateAndPerform(SummonPet);
            _commandManager.EvaluateAndPerform(CastDemonSkin);
            _commandManager.EvaluateAndPerform(CastDrink);
            
            base.GetNextWaypoint();
            base.MoveToNextWaypoint();
        }


        public void AttackBehaviour()
        {
            AutoAttack();
            KillTarget();
        }
        
        public void Tick()
        {
            var state = RobotState.Passive;

            _commandManager.EvaluateAndPerform(AcquireTarget);
            if (ObjectManager.Me.Target != null)
                state = RobotState.Combat;

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
    }
}
