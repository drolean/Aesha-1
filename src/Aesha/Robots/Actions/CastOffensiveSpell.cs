using System.Threading.Tasks;
using Aesha.Core;
using Aesha.Domain;

namespace Aesha.Robots.Actions
{
    public class CastOffensiveSpell : IConditionalAction
    {
        private readonly Spell _spell;
        public CastOffensiveSpell(Spell spell)
        {
            _spell = spell;
        }

        public bool Evaluate()
        {
            return ObjectManager.Me.Target != null;
        }

        public void Do()
        {
            CommandManager.GetDefault().SendKey(_spell.KeyAction);
            const int globalCooldown = 1500;

            if (_spell.CastTime > globalCooldown)
                Task.Delay(_spell.CastTime).Wait();
            else
                Task.Delay(globalCooldown).Wait();

        }
    }
}