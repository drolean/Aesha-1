using System.Threading.Tasks;
using Aesha.Core;
using Aesha.Domain;

namespace Aesha.Robots.Actions
{
    public class CastWand : IConditionalAction
    {

        private bool _isWanding = false;
        private readonly Spell _spell;
        public CastWand(Spell spell)
        {
            _spell = spell;
        }

        public bool Evaluate()
        {

            var target = ObjectManager.Me.Target;
            if (target == null)
            {
                _isWanding = false;
                return false;
            }

            return true;
        }

        public void Do()
        {
            if (_isWanding) return;

            _isWanding = true;
            
            CommandManager.GetDefault().SendKey(_spell.KeyAction);
            const int globalCooldown = 1500;

            if (_spell.CastTime > globalCooldown)
                Task.Delay(_spell.CastTime).Wait();
            else
                Task.Delay(globalCooldown).Wait();

        }
    }
}