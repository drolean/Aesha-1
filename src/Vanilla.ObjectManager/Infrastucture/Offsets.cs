namespace Vanilla.ObjectManager.Infrastucture
{
    public static class Offsets
    {
        internal enum WowObjectManager : uint
        {
            Base = 0x00B41414,
            FirstObject = 0xac,
            NextObject = 0x3c,
            ObjectType = 0x14,
            LocalGUID = 0xC0
        }

        internal enum WowUnitFields : uint
        {
            Charm = 0x18,
            Summon = 0x20,
            CharmedBy = 0x28,
            SummonedBy = 0x30,
            CreatedBy = 0x38,
            Target = 0x40,
            ChannelObject = 0x50,
            Health = 0x58,
            Power = 0x5c,
            MaxHealth = 0x70,
            MaxPower = 0x74,
            Level = 0x88,
            ClassAndRace = 0x90
        }

        internal enum WoWPlayerFields : uint
        {
            Class = 0x827E81,
            IsIngame = 0xB4B424,
            IsGhost = 0x435A48,
            TargetGuid = 0x74E2D8,
            IsCasting = 0xCECA88,

            Xp = 0x28A0,//??
            NextLevelXp = 0x28A4 //??
                ,
            PLAYER_XP = 0xB30,
            PLAYER_NEXT_LEVEL_XP = 0xB34,
        }

        internal enum WowObject : uint
        {
            Guid = 0x30,
            DataPTR = 0x8,
            Type = 0x14,
            Y = 0x9b8,
            X = Y + 0x4,
            Z = Y + 0x8,
            RotationOffset = X + 0x10,  // This seems to be wrong
            GameObjectY = 0x2C4, // *DataPTR (0x288) + 0x3c
            GameObjectX = GameObjectY + 0x4,
            GameObjectZ = GameObjectY + 0x8,
            Speed = 0xA34
        }

        

        internal enum UnitName : uint
        {
            ObjectName1 = 0x214, //pointing to itemtype of objectdescription
            ItemType = 0x2DC, // *DataPTR (0x288) + 0x54
            ObjectName2 = 0x8,
            UnitName1 = 0xB30,
            UnitName2 = 0x0,

            PlayerNameCachePointer = 0x80E230,
            PlayerNameGUIDOffset = 0xc,
            PlayerNameStringOffset = 0x14
        }

        internal enum CreatureCache : uint
        {
            CreatureCache = 0xB30,
            CreatureType = 0x18,
            Classification = 0x20
        }
    }
}
