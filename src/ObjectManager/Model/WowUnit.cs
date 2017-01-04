using System;
using System.Collections.Generic;
using System.Linq;
using ObjectManager.Infrastructure;

namespace ObjectManager.Model
{
    public class WowUnit : WowObject
    {
        private readonly ProcessMemoryReader _reader;
        private readonly uint _objectBaseAddress;

        public WowUnit(ProcessMemoryReader reader, uint objectBaseAddress)
            : base(reader,objectBaseAddress)
        {
            _reader = reader;
            _objectBaseAddress = objectBaseAddress;
        }


        public Health Health
        {
            get
            {
                var current = _reader.Read<uint>(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_HEALTH);
                var max = _reader.Read<uint>(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_MAXHEALTH);

                return new Health((int) current, (int) max);
            }
        }

        public Power Mana
        {
            get
            {
                var current = _reader.Read<uint>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_POWER1);
                var max = _reader.Read<uint>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_MAXPOWER1);

                return new Power((int)current, (int)max);
            }
        }
        

        public override string Name
        {
            get
            {
                var unitNameAddress1 = _reader.Read<uint>(_objectBaseAddress + (uint) Offsets.WowUnit.UNIT_FIELD_NAME_1);
                var unitNameAddress2 = _reader.Read<uint>(unitNameAddress1 + (uint) Offsets.WowUnit.UNIT_FIELD_NAME_2);
                return _reader.ReadString(unitNameAddress2, 50);
            }
        }

        public int Level => (int)_reader.Read<uint>(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_LEVEL);
        public override ObjectType Type => ObjectType.Unit;
        
        public CreatureType CreatureType
        {
            get
            {
                try
                {
                    var creatureCache = _reader.Read<uint>(_objectBaseAddress + (uint) Offsets.WowCreatureCache.CREATURE_CACHE_BASE);
                    return (CreatureType) _reader.Read<uint>(creatureCache + (uint) Offsets.WowCreatureCache.CREATURE_CACHE_TYPE);
                }
                catch (Exception)
                {

                    return CreatureType.NotSpecified;
                }

            }
        }

        public int Classification
        {
            get
            {
                try
                {
                    var creatureCache = _reader.Read<uint>(_objectBaseAddress + (uint)Offsets.WowCreatureCache.CREATURE_CACHE_BASE);
                    return _reader.Read<int>(creatureCache + (uint)Offsets.WowCreatureCache.CREATURE_CACHE_CLASS);
                }
                catch (Exception)
                {

                    return -1;
                }
                
            }
        }

        public WowUnit Target
        {
            get
            {
                var targetGuid = _reader.Read<ulong>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_TARGET);
                return ObjectManager.Units.SingleOrDefault(u => u.Guid == targetGuid);
            }
        }

        public WowUnit SummonedBy
        {
            get
            {
                var summonedBy = _reader.Read<ulong>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_SUMMONEDBY);
                return ObjectManager.Units.SingleOrDefault(u => u.Guid == summonedBy);
            }
        }

        public WowUnit CreatedBy
        {
            get
            {
                var createdBy = _reader.Read<ulong>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_CREATEDBY);
                return ObjectManager.Units.SingleOrDefault(u => u.Guid == createdBy);
            }
        }

        public WowUnit CharmedBy
        {
            get
            {
                var charmedBy = _reader.Read<ulong>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_CHARMEDBY);
                return ObjectManager.Units.SingleOrDefault(u => u.Guid == charmedBy);
            }
        }
        
        public List<Spell> Auras
        {
            get
            {
                var auras = new List<Spell>();
                uint auraPosition = 0;
                for (uint i = 0; i < 47; i++)
                {
                    auraPosition += 4;
                    var aura = _reader.Read<int>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_AURA + auraPosition);
                    if (aura > 0)
                        auras.Add(new Spell(aura));
                }

                return auras;
            }

        }

        public UnitAttributes Attributes
        {
            get
            {
                var npc = _reader.Read<bool>(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_NPC_FLAGS);
                var lootable = (_reader.Read<uint>(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 0x0D) != 0;
                var skinnable = (_reader.Read<uint>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 0x4000000) != 0;
                var tapped = (_reader.Read<uint>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 4) != 0;
                var tappedByMe = (_reader.Read<uint>(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 8) != 0;

                return new UnitAttributes(npc, lootable, skinnable, tapped, tappedByMe);

            }
        }

        public override string ToString()
        {
            return $"{Name} ({Level}) - {Enum.GetName(typeof(CreatureType),CreatureType)}";
        }
    }
}