// ReSharper disable InconsistentNaming
namespace Aesha.Core
{

    public static class Offsets
    {
        public enum WowGame
        {
            MouseOverGuid = 0x00B4E2C8,
            TargetLastTargetGuid = 0x00B4E2E8,
            PlayerFacing = 0x86326C,
            MinimapZoneText = 0x74DA28,
            SubZoneText = 0x74E280,
            RealZoneText = 0x74B404,
            Zone = 0x25D85F08
        }

        public enum WowObjectManager
        {
            BASE = 0x00B41414,
            FIRST_OBJECT = 0xac,
            NEXT_OBJECT = 0x3c,
            LOCAL_GUID = 0xC,
            PLAYER_GUID = 0xC0,
            DESCRIPTOR = 0x8
        }

        // Descriptors: 0x0083A2C0
        public enum WowObject
        {
            OBJECT_FIELD_DATA = 0x8,
            OBJECT_FIELD_TYPE = 0x14,
            OBJECT_FIELD_GUID = 0x30,
            OBJECT_FIELD_Y = 0x9B8,
            OBJECT_FIELD_X = OBJECT_FIELD_Y + 0x4,
            OBJECT_FIELD_Z = OBJECT_FIELD_Y + 0x8,
            OBJECT_FIELD_ROTATION = 0x9C4,
        };

        // Descriptors: 0x0083A328
        public enum WowItem
        {
            ITEM_FIELD_OWNER = 0x18,
            ITEM_FIELD_CONTAINED = 0x20,
            ITEM_FIELD_CREATOR = 0x28,
            ITEM_FIELD_GIFTCREATOR = 0x30,
            ITEM_FIELD_STACK_COUNT = 0x38,
            ITEM_FIELD_DURATION = 0x3C,
            ITEM_FIELD_SPELL_CHARGES = 0x40,
            ITEM_FIELD_FLAGS = 0x54,
            ITEM_FIELD_ENCHANTMENT = 0x58,
            ITEM_FIELD_PROPERTY_SEED = 0xAC,
            ITEM_FIELD_RANDOM_PROPERTIES_ID = 0xB0,
            ITEM_FIELD_ITEM_TEXT_ID = 0xB4,
            ITEM_FIELD_DURABILITY = 0xB8,
            ITEM_FIELD_MAXDURABILITY = 0xBC,
            TOTAL_ITEM_FIELDS = 0xE
        };

        // Descriptors: 0x0083A440
        public enum WowContainer
        {
            CONTAINER_FIELD_NUM_SLOTS = 0x18,
            CONTAINER_ALIGN_PAD = 0x1C,
            CONTAINER_FIELD_SLOT_1 = 0x20,
            TOTAL_CONTAINER_FIELDS = 0x3
        };

        // Descriptors: 0x0083B880
        public enum WowGameObject
        {
            OBJECT_FIELD_CREATED_BY = 0x18,
            GAMEOBJECT_DISPLAYID = 0x20,
            GAMEOBJECT_FLAGS = 0x24,
            GAMEOBJECT_ROTATION = 0x28,
            GAMEOBJECT_STATE = 0x38,
            GAMEOBJECT_POS_X = 0x3C,
            GAMEOBJECT_POS_Y = 0x40,
            GAMEOBJECT_POS_Z = 0x44,
            GAMEOBJECT_FACING = 0x48,
            GAMEOBJECT_DYN_FLAGS = 0x4C,
            GAMEOBJECT_FACTION = 0x50,
            GAMEOBJECT_TYPE_ID = 0x54,
            GAMEOBJECT_LEVEL = 0x58,
            GAMEOBJECT_ARTKIT = 0x5C,
            GAMEOBJECT_ANIMPROGRESS = 0x60,
            GAMEOBJECT_PADDING = 0x64,
            GAMEOBJECT_BOBBING = 0xE8,
            TOTAL_GAMEOBJECT_FIELDS = 0x10
        };

        // Descriptors: 0x0083B9C0
        public enum WowDynamicObject
        {
            DYNAMICOBJECT_CASTER = 0x18,
            DYNAMICOBJECT_BYTES = 0x20,
            DYNAMICOBJECT_SPELLID = 0x24,
            DYNAMICOBJECT_RADIUS = 0x28,
            DYNAMICOBJECT_POS_X = 0x2C,
            DYNAMICOBJECT_POS_Y = 0x30,
            DYNAMICOBJECT_POS_Z = 0x34,
            DYNAMICOBJECT_FACING = 0x38,
            DYNAMICOBJECT_PAD = 0x3C,
            TOTAL_DYNAMICOBJECT_FIELDS = 0x9
        };

        // Descriptors: 0x0083BA78
        public enum WowCorpse
        {
            CORPSE_FIELD_OWNER = 0x18,
            CORPSE_FIELD_FACING = 0x20,
            CORPSE_FIELD_POS_X = 0x24,
            CORPSE_FIELD_POS_Y = 0x28,
            CORPSE_FIELD_POS_Z = 0x2C,
            CORPSE_FIELD_DISPLAY_ID = 0x30,
            CORPSE_FIELD_ITEM = 0x34,
            CORPSE_FIELD_BYTES_1 = 0x80,
            CORPSE_FIELD_BYTES_2 = 0x84,
            CORPSE_FIELD_GUILD = 0x88,
            CORPSE_FIELD_FLAGS = 0x8C,
            CORPSE_FIELD_DYNAMIC_FLAGS = 0x90,
            CORPSE_FIELD_PAD = 0x94,
            TOTAL_CORPSE_FIELDS = 0xD
        };

        // Descriptors: 0x0083A480
        public enum WowUnit
        {
            UNIT_FIELD_CHARM = 0x18,
            UNIT_FIELD_SUMMON = 0x20,
            UNIT_FIELD_CHARMEDBY = 0x28,
            UNIT_FIELD_SUMMONEDBY = 0x30,
            UNIT_FIELD_CREATEDBY = 0x38,
            UNIT_FIELD_TARGET = 0x40,
            UNIT_FIELD_PERSUADED = 0x48,
            UNIT_FIELD_CHANNEL_OBJECT = 0x50,
            UNIT_FIELD_HEALTH = 0x58,
            UNIT_FIELD_POWER1 = 0x5C,
            UNIT_FIELD_POWER2 = 0x60,
            UNIT_FIELD_POWER3 = 0x64,
            UNIT_FIELD_POWER4 = 0x68,
            UNIT_FIELD_POWER5 = 0x6C,
            UNIT_FIELD_MAXHEALTH = 0x70,
            UNIT_FIELD_MAXPOWER1 = 0x74,
            UNIT_FIELD_MAXPOWER2 = 0x78,
            UNIT_FIELD_MAXPOWER3 = 0x7C,
            UNIT_FIELD_MAXPOWER4 = 0x80,
            UNIT_FIELD_MAXPOWER5 = 0x84,
            UNIT_FIELD_LEVEL = 0x88,
            UNIT_FIELD_FACTIONTEMPLATE = 0x8C,
            UNIT_FIELD_BYTES_0 = 0x90,
            UNIT_VIRTUAL_ITEM_SLOT_DISPLAY = 0x94,
            UNIT_VIRTUAL_ITEM_INFO = 0xA0,
            UNIT_FIELD_FLAGS = 0xB8,
            UNIT_FIELD_AURA = 0xBC,
            UNIT_FIELD_AURAFLAGS = 0x17C,
            UNIT_FIELD_AURALEVELS = 0x194,
            UNIT_FIELD_AURAAPPLICATIONS = 0x1C4,
            UNIT_FIELD_AURASTATE = 0x1F4,
            UNIT_FIELD_BASEATTACKTIME = 0x1F8,
            UNIT_FIELD_RANGEDATTACKTIME = 0x200,
            UNIT_FIELD_BOUNDINGRADIUS = 0x204,
            UNIT_FIELD_COMBATREACH = 0x208,
            UNIT_FIELD_DISPLAYID = 0x20C,
            UNIT_FIELD_NATIVEDISPLAYID = 0x210,
            UNIT_FIELD_MOUNTDISPLAYID = 0x214,
            UNIT_FIELD_MINDAMAGE = 0x218,
            UNIT_FIELD_MAXDAMAGE = 0x21C,
            UNIT_FIELD_MINOFFHANDDAMAGE = 0x220,
            UNIT_FIELD_MAXOFFHANDDAMAGE = 0x224,
            UNIT_FIELD_BYTES_1 = 0x228,
            UNIT_FIELD_PETNUMBER = 0x22C,
            UNIT_FIELD_PET_NAME_TIMESTAMP = 0x230,
            UNIT_FIELD_PETEXPERIENCE = 0x234,
            UNIT_FIELD_PETNEXTLEVELEXP = 0x238,
            UNIT_DYNAMIC_FLAGS = 0x23C,
            UNIT_CHANNEL_SPELL = 0x240,
            UNIT_MOD_CAST_SPEED = 0x244,
            UNIT_CREATED_BY_SPELL = 0x248,
            UNIT_NPC_FLAGS = 0x24C,
            UNIT_NPC_EMOTESTATE = 0x250,
            UNIT_TRAINING_POINTS = 0x254,
            UNIT_FIELD_STAT0 = 0x258,
            UNIT_FIELD_STAT1 = 0x25C,
            UNIT_FIELD_STAT2 = 0x260,
            UNIT_FIELD_STAT3 = 0x264,
            UNIT_FIELD_STAT4 = 0x268,
            UNIT_FIELD_RESISTANCES = 0x26C,
            UNIT_FIELD_BASE_MANA = 0x288,
            UNIT_FIELD_BASE_HEALTH = 0x28C,
            UNIT_FIELD_BYTES_2 = 0x290,
            UNIT_FIELD_ATTACK_POWER = 0x294,
            UNIT_FIELD_ATTACK_POWER_MODS = 0x298,
            UNIT_FIELD_ATTACK_POWER_MULTIPLIER = 0x29C,
            UNIT_FIELD_RANGED_ATTACK_POWER = 0x2A0,
            UNIT_FIELD_RANGED_ATTACK_POWER_MODS = 0x2A4,
            UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER = 0x2A8,
            UNIT_FIELD_MINRANGEDDAMAGE = 0x2AC,
            UNIT_FIELD_MAXRANGEDDAMAGE = 0x2B0,
            UNIT_FIELD_POWER_COST_MODIFIER = 0x2B4,
            UNIT_FIELD_POWER_COST_MULTIPLIER = 0x2D0,
            UNIT_FIELD_PADDING = 0x2EC,
            UNIT_FIELD_NAME_1 = 0xB30,
            UNIT_FIELD_NAME_2 = 0x0,
            TOTAL_UNIT_FIELDS = 0x4A
        };

        // Descriptors: 0x0083AA48
        public enum WowPlayer
        {
            PLAYER_DUEL_ARBITER = 0x2F0,
            PLAYER_FLAGS = 0x2F8,
            PLAYER_GUILDID = 0x2FC,
            PLAYER_GUILDRANK = 0x300,
            PLAYER_BYTES = 0x304,
            PLAYER_BYTES_2 = 0x308,
            PLAYER_BYTES_3 = 0x30C,
            PLAYER_DUEL_TEAM = 0x310,
            PLAYER_GUILD_TIMESTAMP = 0x314,
            PLAYER_QUEST_LOG_1_1 = 0x318,
            PLAYER_QUEST_LOG_1_2 = 0x31C,
            PLAYER_QUEST_LOG_2_1 = 0x324,
            PLAYER_QUEST_LOG_2_2 = 0x328,
            PLAYER_QUEST_LOG_3_1 = 0x330,
            PLAYER_QUEST_LOG_3_2 = 0x334,
            PLAYER_QUEST_LOG_4_1 = 0x33C,
            PLAYER_QUEST_LOG_4_2 = 0x340,
            PLAYER_QUEST_LOG_5_1 = 0x348,
            PLAYER_QUEST_LOG_5_2 = 0x34C,
            PLAYER_QUEST_LOG_6_1 = 0x354,
            PLAYER_QUEST_LOG_6_2 = 0x358,
            PLAYER_QUEST_LOG_7_1 = 0x360,
            PLAYER_QUEST_LOG_7_2 = 0x364,
            PLAYER_QUEST_LOG_8_1 = 0x36C,
            PLAYER_QUEST_LOG_8_2 = 0x370,
            PLAYER_QUEST_LOG_9_1 = 0x378,
            PLAYER_QUEST_LOG_9_2 = 0x37C,
            PLAYER_QUEST_LOG_10_1 = 0x384,
            PLAYER_QUEST_LOG_10_2 = 0x388,
            PLAYER_QUEST_LOG_11_1 = 0x390,
            PLAYER_QUEST_LOG_11_2 = 0x394,
            PLAYER_QUEST_LOG_12_1 = 0x39C,
            PLAYER_QUEST_LOG_12_2 = 0x3A0,
            PLAYER_QUEST_LOG_13_1 = 0x3A8,
            PLAYER_QUEST_LOG_13_2 = 0x3AC,
            PLAYER_QUEST_LOG_14_1 = 0x3B4,
            PLAYER_QUEST_LOG_14_2 = 0x3B8,
            PLAYER_QUEST_LOG_15_1 = 0x3C0,
            PLAYER_QUEST_LOG_15_2 = 0x3C4,
            PLAYER_QUEST_LOG_16_1 = 0x3CC,
            PLAYER_QUEST_LOG_16_2 = 0x3D0,
            PLAYER_QUEST_LOG_17_1 = 0x3D8,
            PLAYER_QUEST_LOG_17_2 = 0x3DC,
            PLAYER_QUEST_LOG_18_1 = 0x3E4,
            PLAYER_QUEST_LOG_18_2 = 0x3E8,
            PLAYER_QUEST_LOG_19_1 = 0x3F0,
            PLAYER_QUEST_LOG_19_2 = 0x3F4,
            PLAYER_QUEST_LOG_20_1 = 0x3FC,
            PLAYER_QUEST_LOG_20_2 = 0x400,
            PLAYER_VISIBLE_ITEM_1_CREATOR = 0x408,
            PLAYER_VISIBLE_ITEM_1_0 = 0x410,
            PLAYER_VISIBLE_ITEM_1_PROPERTIES = 0x430,
            PLAYER_VISIBLE_ITEM_1_PAD = 0x434,
            PLAYER_VISIBLE_ITEM_2_CREATOR = 0x438,
            PLAYER_VISIBLE_ITEM_2_0 = 0x440,
            PLAYER_VISIBLE_ITEM_2_PROPERTIES = 0x460,
            PLAYER_VISIBLE_ITEM_2_PAD = 0x464,
            PLAYER_VISIBLE_ITEM_3_CREATOR = 0x468,
            PLAYER_VISIBLE_ITEM_3_0 = 0x470,
            PLAYER_VISIBLE_ITEM_3_PROPERTIES = 0x490,
            PLAYER_VISIBLE_ITEM_3_PAD = 0x494,
            PLAYER_VISIBLE_ITEM_4_CREATOR = 0x498,
            PLAYER_VISIBLE_ITEM_4_0 = 0x4A0,
            PLAYER_VISIBLE_ITEM_4_PROPERTIES = 0x4C0,
            PLAYER_VISIBLE_ITEM_4_PAD = 0x4C4,
            PLAYER_VISIBLE_ITEM_5_CREATOR = 0x4C8,
            PLAYER_VISIBLE_ITEM_5_0 = 0x4D0,
            PLAYER_VISIBLE_ITEM_5_PROPERTIES = 0x4F0,
            PLAYER_VISIBLE_ITEM_5_PAD = 0x4F4,
            PLAYER_VISIBLE_ITEM_6_CREATOR = 0x4F8,
            PLAYER_VISIBLE_ITEM_6_0 = 0x500,
            PLAYER_VISIBLE_ITEM_6_PROPERTIES = 0x520,
            PLAYER_VISIBLE_ITEM_6_PAD = 0x524,
            PLAYER_VISIBLE_ITEM_7_CREATOR = 0x528,
            PLAYER_VISIBLE_ITEM_7_0 = 0x530,
            PLAYER_VISIBLE_ITEM_7_PROPERTIES = 0x550,
            PLAYER_VISIBLE_ITEM_7_PAD = 0x554,
            PLAYER_VISIBLE_ITEM_8_CREATOR = 0x558,
            PLAYER_VISIBLE_ITEM_8_0 = 0x560,
            PLAYER_VISIBLE_ITEM_8_PROPERTIES = 0x580,
            PLAYER_VISIBLE_ITEM_8_PAD = 0x584,
            PLAYER_VISIBLE_ITEM_9_CREATOR = 0x588,
            PLAYER_VISIBLE_ITEM_9_0 = 0x590,
            PLAYER_VISIBLE_ITEM_9_PROPERTIES = 0x5B0,
            PLAYER_VISIBLE_ITEM_9_PAD = 0x5B4,
            PLAYER_VISIBLE_ITEM_10_CREATOR = 0x5B8,
            PLAYER_VISIBLE_ITEM_10_0 = 0x5C0,
            PLAYER_VISIBLE_ITEM_10_PROPERTIES = 0x5E0,
            PLAYER_VISIBLE_ITEM_10_PAD = 0x5E4,
            PLAYER_VISIBLE_ITEM_11_CREATOR = 0x5E8,
            PLAYER_VISIBLE_ITEM_11_0 = 0x5F0,
            PLAYER_VISIBLE_ITEM_11_PROPERTIES = 0x610,
            PLAYER_VISIBLE_ITEM_11_PAD = 0x614,
            PLAYER_VISIBLE_ITEM_12_CREATOR = 0x618,
            PLAYER_VISIBLE_ITEM_12_0 = 0x620,
            PLAYER_VISIBLE_ITEM_12_PROPERTIES = 0x640,
            PLAYER_VISIBLE_ITEM_12_PAD = 0x644,
            PLAYER_VISIBLE_ITEM_13_CREATOR = 0x648,
            PLAYER_VISIBLE_ITEM_13_0 = 0x650,
            PLAYER_VISIBLE_ITEM_13_PROPERTIES = 0x670,
            PLAYER_VISIBLE_ITEM_13_PAD = 0x674,
            PLAYER_VISIBLE_ITEM_14_CREATOR = 0x678,
            PLAYER_VISIBLE_ITEM_14_0 = 0x680,
            PLAYER_VISIBLE_ITEM_14_PROPERTIES = 0x6A0,
            PLAYER_VISIBLE_ITEM_14_PAD = 0x6A4,
            PLAYER_VISIBLE_ITEM_15_CREATOR = 0x6A8,
            PLAYER_VISIBLE_ITEM_15_0 = 0x6B0,
            PLAYER_VISIBLE_ITEM_15_PROPERTIES = 0x6D0,
            PLAYER_VISIBLE_ITEM_15_PAD = 0x6D4,
            PLAYER_VISIBLE_ITEM_16_CREATOR = 0x6D8,
            PLAYER_VISIBLE_ITEM_16_0 = 0x6E0,
            PLAYER_VISIBLE_ITEM_16_PROPERTIES = 0x700,
            PLAYER_VISIBLE_ITEM_16_PAD = 0x704,
            PLAYER_VISIBLE_ITEM_17_CREATOR = 0x708,
            PLAYER_VISIBLE_ITEM_17_0 = 0x710,
            PLAYER_VISIBLE_ITEM_17_PROPERTIES = 0x730,
            PLAYER_VISIBLE_ITEM_17_PAD = 0x734,
            PLAYER_VISIBLE_ITEM_18_CREATOR = 0x738,
            PLAYER_VISIBLE_ITEM_18_0 = 0x740,
            PLAYER_VISIBLE_ITEM_18_PROPERTIES = 0x760,
            PLAYER_VISIBLE_ITEM_18_PAD = 0x764,
            PLAYER_VISIBLE_ITEM_19_CREATOR = 0x768,
            PLAYER_VISIBLE_ITEM_19_0 = 0x770,
            PLAYER_VISIBLE_ITEM_19_PROPERTIES = 0x790,
            PLAYER_VISIBLE_ITEM_19_PAD = 0x794,
            PLAYER_FIELD_INV_SLOT_HEAD = 0x798,
            PLAYER_FIELD_PACK_SLOT_1 = 0x850,
            PLAYER_FIELD_BANK_SLOT_1 = 0x8D0,
            PLAYER_FIELD_BANKBAG_SLOT_1 = 0x990,
            PLAYER_FIELD_VENDORBUYBACK_SLOT_1 = 0x9C0,
            PLAYER_MOVEMENT_FLAGS = 0x9E8,
            PLAYER_FIELD_KEYRING_SLOT_1 = 0xA20,
            PLAYER_FARSIGHT = 0xB20,
            PLAYER_FIELD_COMBO_TARGET = 0xB28,
            PLAYER_XP = 0xB30,
            PLAYER_NEXT_LEVEL_XP = 0xB34,
            PLAYER_SKILL_INFO_1_1 = 0xB38,
            PLAYER_CHARACTER_POINTS1 = 0x1138,
            PLAYER_CHARACTER_POINTS2 = 0x113C,
            PLAYER_TRACK_CREATURES = 0x1140,
            PLAYER_TRACK_RESOURCES = 0x1144,
            PLAYER_BLOCK_PERCENTAGE = 0x1148,
            PLAYER_DODGE_PERCENTAGE = 0x114C,
            PLAYER_PARRY_PERCENTAGE = 0x1150,
            PLAYER_CRIT_PERCENTAGE = 0x1154,
            PLAYER_RANGED_CRIT_PERCENTAGE = 0x1158,
            PLAYER_EXPLORED_ZONES_1 = 0x115C,
            PLAYER_REST_STATE_EXPERIENCE = 0x125C,
            PLAYER_FIELD_COINAGE = 0x1260,
            PLAYER_FIELD_POSSTAT0 = 0x1264,
            PLAYER_FIELD_POSSTAT1 = 0x1268,
            PLAYER_FIELD_POSSTAT2 = 0x126C,
            PLAYER_FIELD_POSSTAT3 = 0x1270,
            PLAYER_FIELD_POSSTAT4 = 0x1274,
            PLAYER_FIELD_NEGSTAT0 = 0x1278,
            PLAYER_FIELD_NEGSTAT1 = 0x127C,
            PLAYER_FIELD_NEGSTAT2 = 0x1280,
            PLAYER_FIELD_NEGSTAT3 = 0x1284,
            PLAYER_FIELD_NEGSTAT4 = 0x1288,
            PLAYER_FIELD_RESISTANCEBUFFMODSPOSITIVE = 0x128C,
            PLAYER_FIELD_RESISTANCEBUFFMODSNEGATIVE = 0x12A8,
            PLAYER_FIELD_MOD_DAMAGE_DONE_POS = 0x12C4,
            PLAYER_FIELD_MOD_DAMAGE_DONE_NEG = 0x12E0,
            PLAYER_FIELD_MOD_DAMAGE_DONE_PCT = 0x12FC,
            PLAYER_FIELD_BYTES = 0x1318,
            PLAYER_AMMO_ID = 0x131C,
            PLAYER_SELF_RES_SPELL = 0x1320,
            PLAYER_FIELD_PVP_MEDALS = 0x1324,
            PLAYER_FIELD_BUYBACK_PRICE_1 = 0x1328,
            PLAYER_FIELD_BUYBACK_TIMESTAMP_1 = 0x1358,
            PLAYER_FIELD_SESSION_KILLS = 0x1388,
            PLAYER_FIELD_YESTERDAY_KILLS = 0x138C,
            PLAYER_FIELD_LAST_WEEK_KILLS = 0x1390,
            PLAYER_FIELD_THIS_WEEK_KILLS = 0x1394,
            PLAYER_FIELD_THIS_WEEK_CONTRIBUTION = 0x1398,
            PLAYER_FIELD_LIFETIME_HONORBALE_KILLS = 0x139C,
            PLAYER_FIELD_LIFETIME_DISHONORBALE_KILLS = 0x13A0,
            PLAYER_FIELD_YESTERDAY_CONTRIBUTION = 0x13A4,
            PLAYER_FIELD_LAST_WEEK_CONTRIBUTION = 0x13A8,
            PLAYER_FIELD_LAST_WEEK_RANK = 0x13AC,
            PLAYER_FIELD_BYTES2 = 0x13B0,
            PLAYER_FIELD_WATCHED_FACTION_INDEX = 0x13B4,
            PLAYER_FIELD_COMBAT_RATING_1 = 0x13B8,
            TOTAL_PLAYER_FIELDS = 0xB6
        };

        public enum WowPlayerNameCache
        {
            NAME_CACHE_BASE = 0x80E230,
            NAME_CACHE_STRING = 0x14
        }

        public enum WowCreatureCache : uint
        {
            CREATURE_CACHE_BASE = 0xB30,
            CREATURE_CACHE_TYPE = 0x18,
            CREATURE_CACHE_CLASS = 0x20
        }

        internal enum ClickToMoveAction : uint
        {
            FaceTarget = 0x1,
            Stop = 0x3,
            WalkTo = 0x4,
            InteractNpc = 0x5,
            Loot = 0x6,
            InteractObject = 0x7
        }

        internal enum Misc : uint
        {
            GameVersion = 0x00837C04,
            MapId = 0x84C498,
            LoginState = 0xB41478
        }

        internal enum MovementFlags : uint
        {
            None = 0x00000000,
            Forward = 0x00000001,
            Back = 0x00000002,
            TurnLeft = 0x00000010,
            TurnRight = 0x00000020,
            Stunned = 0x00001000,
            Swimming = 0x00200000,
        }

        internal enum LuaFunctions : uint
        {
            LastHardwareAction = 0x00CF0BC8,
            AutoLoot = 0x4C1FA0,
            ClickToMove = 0x00611130,
            GetText = 0x703BF0,
            DoString = 0x00704CD0,
            GetEndscene = 0x5A17B6,
            IsLooting = 0x006126B0,
            GetLootSlots = 0x004C2260,
            OnRightClickObject = 0x005F8660,
            OnRightClickUnit = 0x60BEA0,
            SetFacing = 0x007C6F30,
            SendMovementPacket = 0x00600A30,
            PerformDefaultAction = 0x00481F60,
            CGInputControl__GetActive = 0x005143E0,
            CGInputControl__SetControlBit = 0x00515090,

        }
    }
}
