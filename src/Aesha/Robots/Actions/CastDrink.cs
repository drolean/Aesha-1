using System;
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
            var percentageMana = (ObjectManager.Me.Mana.Current/(double) ObjectManager.Me.Mana.Max)*100;
            return percentageMana < 30;
        }

        public void Do()
        {
            CommandManager.GetDefault().SendKey(_spell.KeyAction);

            var castTimeDelay = Task.Delay(_spell.CastTime);
            while (!castTimeDelay.IsCompleted)
            {
                var percentageMana = (ObjectManager.Me.Mana.Current / (double)ObjectManager.Me.Mana.Max) * 100;
                if (percentageMana > 99) break;
            }
            
            CommandManager.GetDefault().SendKey(MappedKeys.Forward);

        }
    }
}