using System;
using System.Collections.Generic;
using System.Linq;
using Aesha.Core;
using Aesha.Infrastructure;
using Aesha.Interfaces;

namespace Aesha.Domain
{
    public class WowUnit : WowObject
    {
        private readonly IProcessMemoryReader _reader;
        private readonly uint _objectBaseAddress;

        public WowUnit(IProcessMemoryReader reader, uint objectBaseAddress)
            : base(reader,objectBaseAddress)
        {
            _reader = reader;
            _objectBaseAddress = objectBaseAddress;
        }


        public Health Health
        {
            get
            {
                var current = _reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_HEALTH);
                var max = _reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_MAXHEALTH);

                return new Health((int) current, (int) max);
            }
        }

        public Power Mana
        {
            get
            {
                var current = _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_POWER1);
                var max = _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_MAXPOWER1);
                
                return new Power((int)current, (int)max);
            }
        }

        public Power Energy
        {
            get
            {
                var current = _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_POWER3);
                var max = _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_MAXPOWER3);

                return new Power((int)current, (int)max);
            }
        }

        public Power Rage
        {
            get
            {
                var current = _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_POWER4);
                var max = _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_MAXPOWER4);

                return new Power((int)current, (int)max);
            }
        }


        public override string Name
        {
            get
            {
                var unitNameAddress1 = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.WowUnit.UNIT_FIELD_NAME_1);
                var unitNameAddress2 = _reader.ReadUInt(unitNameAddress1 + (uint) Offsets.WowUnit.UNIT_FIELD_NAME_2);
                return _reader.ReadString(unitNameAddress2, 50);
            }
        }

        public int Level => (int)_reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_LEVEL);
        public override ObjectType Type => ObjectType.Unit;
        
        public CreatureType CreatureType
        {
            get
            {
                try
                {
                    var creatureCache = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.WowCreatureCache.CREATURE_CACHE_BASE);
                    return (CreatureType) _reader.ReadUInt(creatureCache + (uint) Offsets.WowCreatureCache.CREATURE_CACHE_TYPE);
                }
                catch (Exception)
                {

                    return CreatureType.NotSpecified;
                }

            }
        }

        public UnitClassificationTypes Classification
        {
            get
            {
                try
                {
                    var creatureCache = _reader.ReadUInt(_objectBaseAddress + (uint)Offsets.WowCreatureCache.CREATURE_CACHE_BASE);
                    return (UnitClassificationTypes)_reader.ReadInt(creatureCache + (uint)Offsets.WowCreatureCache.CREATURE_CACHE_CLASS);
                }
                catch (Exception)
                {

                    return UnitClassificationTypes.NotSpecified;
                }
                
            }
        }

        public WowUnit Target
        {
            get
            {
                var targetGuid = _reader.ReadUInt64(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_TARGET);
                return (WowUnit)ObjectManager.Objects.SingleOrDefault(u => u.Guid == targetGuid);
            }
        }

        public WowUnit SummonedBy
        {
            get
            {
                var summonedBy = _reader.ReadUInt64(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_SUMMONEDBY);
                return (WowUnit)ObjectManager.Objects.SingleOrDefault(u => u.Guid == summonedBy);
            }
        }

        public WowUnit CreatedBy
        {
            get
            {
                var createdBy = _reader.ReadUInt64(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_CREATEDBY);
                return (WowUnit)ObjectManager.Objects.SingleOrDefault(u => u.Guid == createdBy);
            }
        }

        public WowUnit CharmedBy
        {
            get
            {
                var charmedBy = _reader.ReadUInt64(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_CHARMEDBY);
                return (WowUnit)ObjectManager.Objects.SingleOrDefault(u => u.Guid == charmedBy);
            }
        }

        public float Distance
        {
            get { return Location.GetDistanceTo(ObjectManager.Me.Location); }
        }

        public bool HasAura(Spell spell)
        {
            return Auras.Any(a => a.Equals(spell));
        }

        public List<Spell> Auras
        {
            get
            {
                var auras = new List<Spell>();
                uint auraPosition = 0;
                for (uint i = 0; i < 47; i++)
                {
                   
                    var aura = _reader.ReadInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_AURA + auraPosition);
                    if (aura > 0)
                        auras.Add(new Spell(aura,"",0,new MappedKeyAction(char.MinValue)));

                    auraPosition += 4;
                }

                return auras;
            }

        }

        public UnitAttributes Attributes
        {
            get
            {
                //not working yet
                var npc = _reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_NPC_FLAGS) != 0;
                var repairNpc = (_reader.ReadInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_NPC_FLAGS) & 0x00001000) != 0;


                var lootable = (_reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 0x0D) != 0;
                var skinnable = (_reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 0x4000000) != 0;
                var tapped = (_reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 4) != 0;
                var tappedByMe = (_reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowUnit.UNIT_DYNAMIC_FLAGS) & 8) != 0;

                return new UnitAttributes(npc, lootable, skinnable, tapped, tappedByMe);

            }
        }

        public override string ToString()
        {
            return $"{Name} ({Level}) - {Enum.GetName(typeof(CreatureType),CreatureType)}";
        }
    }
}