using System.Threading.Tasks;
using Aesha.Core;
using Aesha.Domain;

namespace Aesha.Robots.Actions
{
    public class CastDrink : IConditionalAction
    {
        private readonly Spell _spell;

        public CastDrink(Spell spell)
        {
            _spell = spell;
        }

        public bool Evaluate()
        {
            return ObjectManager.Me.Mana.Percentage < 30;
        }

        public void Do()
        {
            CommandManager.GetDefault().SendKey(_spell.KeyAction);
            Task.Delay(_spell.CastTime).Wait();

        }
    }
}