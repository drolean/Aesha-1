using Aesha.Core;
using Aesha.Domain;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Aesha.Robots.Actions;
using Serilog;

namespace Aesha.Robots
{
    public class Fisherman : IRobot
    {
        private readonly CommandManager _commandManager;

        private readonly Spell CastRod = new Spell(0, 1, MappedKeys.ActionBar1,2000);
        private readonly LocateBobber _bobberLocator = new LocateBobber();

        private ILogger _logger;

       

        public Fisherman(CommandManager commandManager, ILogger logger)
        {
            _commandManager = commandManager;
            _logger = logger;
        }

        public void PassiveBehaviour()
        {
            throw new System.NotImplementedException();
        }

        public void AttackBehaviour()
        {
            throw new System.NotImplementedException();
        }

        public void Tick()
        {
            var fish = new CastBuff(CastRod);
            _commandManager.EvaluateAndPerform(fish); 
            _commandManager.EvaluateAndPerform(_bobberLocator);

        }
    }
}