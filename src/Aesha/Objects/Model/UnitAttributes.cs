﻿namespace Aesha.Objects.Model
{
    public class UnitAttributes
    {
        public UnitAttributes(bool npc, bool lootable, bool skinnable, bool tapped, bool tappedByMe)
        {
            NPC = npc;
            Lootable = lootable;
            Skinnable = skinnable;
            Tapped = tapped;
            TappedByMe = tappedByMe;
        }

        public bool NPC { get; }
        public bool Lootable { get; }
        public bool Skinnable { get; }
        public bool Tapped { get; }
        public bool TappedByMe { get; }
        
    }
}
