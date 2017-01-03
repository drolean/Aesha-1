using System;
using System.Collections.Generic;
using System.Reflection;
using ObjectManager.Infrastructure;

namespace ObjectManager.Model
{
    public class WowUnit : IWowObject
    {
        private readonly ProcessMemoryReader _reader;
        private readonly uint _objectBaseAddress;
        protected readonly uint _unitFieldsAddress;

        public WowUnit(ProcessMemoryReader reader, uint objectBaseAddress)
        {
            _reader = reader;
            _objectBaseAddress = objectBaseAddress;
            _unitFieldsAddress = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.WowObjectManager.DESCRIPTOR);
        }

        public ulong Guid =>_reader.ReadUInt64(_objectBaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_GUID);
        public uint Health => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_HEALTH);
        public uint Mana => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_POWER1);

        public virtual string Name
        {
            get
            {
                var unitNameAddress1 = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.WowUnit.UNIT_FIELD_NAME_1);
                var unitNameAddress2 = _reader.ReadUInt(unitNameAddress1 + (uint) Offsets.WowUnit.UNIT_FIELD_NAME_2);
                return _reader.ReadString(unitNameAddress2, 50);
            }
        }

        public uint Level => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_LEVEL);
        public uint BaseAddress => _objectBaseAddress;
        public virtual ObjectType Type => ObjectType.Unit;
        public float X => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_X);
        public float Y => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_Y);
        public float Z => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_Z);
        public float Rotation => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_ROTATION);

        public CreatureType CreatureType
        {
            get
            {
                try
                {
                    var creatureCache = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.WowCreatureCache.CREATURE_CACHE_BASE);
                    return (CreatureType) _reader.ReadInt(creatureCache + (uint) Offsets.WowCreatureCache.CREATURE_CACHE_TYPE);
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
                    var creatureCache = _reader.ReadUInt(_objectBaseAddress + (uint)Offsets.WowCreatureCache.CREATURE_CACHE_BASE);
                    return _reader.ReadInt(creatureCache + (uint)Offsets.WowCreatureCache.CREATURE_CACHE_CLASS);
                }
                catch (Exception)
                {

                    return -1;
                }
                
            }
        }
        public ulong SummonedBy => _reader.ReadUInt64(_unitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_SUMMONEDBY);
        public ulong CreatedBy => _reader.ReadUInt64(_unitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_CREATEDBY);
        public ulong Target => _reader.ReadUInt64(_unitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_TARGET);
        public uint CharmedBy => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_CHARMEDBY);

        public int[] Auras
        {
            get
            {
                var auras = new List<int>();
                uint auraPosition = 0;
                for (uint i = 0; i < 47; i++)
                {
                    auraPosition += 4;
                    var aura = _reader.ReadInt(_unitFieldsAddress + (uint)Offsets.WowUnit.UNIT_FIELD_AURA + auraPosition);
                    if (aura > 0)
                        auras.Add(aura);
                }

                return auras.ToArray();
            }

        }

        public override string ToString()
        {
            return $"{Name} ({Level}) - {Enum.GetName(typeof(CreatureType),CreatureType)}";
        }
    }
}