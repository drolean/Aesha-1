namespace ObjectManager.Infrastucture
{
    public enum ObjectType : byte
    {
        None = 0,
        Item = 1,
        Container = 2,
        Unit = 3,
        Player = 4,
        GameObject = 5,
        DynamicObject = 6,
        Corpse = 7
    }

    public enum CreatureType : int
    {
        Unknown = 0,
        Beast,
        Dragon,
        Demon,
        Elemental,
        Giant,
        Undead,
        Humanoid,
        Critter,
        Mechanical,
        NotSpecified,
        Totem,
        NonCombatPet,
        GasCloud
    }

    public enum ClassFlags : uint
    {
        None = 0,
        Warrior = 1,
        Paladin = 2,
        Hunter = 3,
        Rogue = 4,
        Priest = 5,
        DeathKnight = 6,
        Shaman = 7,
        Mage = 8,
        Warlock = 9,
        Druid = 11,
    }

    public enum RaceFlags : uint
    {
        Human = 1,
        Orc,
        Dwarf,
        NightElf,
        Undead,
        Tauren,
        Gnome,
        Troll,
        Goblin,
        BloodElf,
        Draenei,
        FelOrc,
        Naga,
        Broken,
        Skeleton = 15,
    }

}
