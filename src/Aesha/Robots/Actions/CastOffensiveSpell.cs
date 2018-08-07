﻿using System.Threading.Tasks;
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
            Task.Delay(_spell.CastTime).Wait();
        }
    }
}