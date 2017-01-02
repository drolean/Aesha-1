using System;
using Vanilla.ObjectManager.Infrastucture;

namespace Vanilla.ObjectManager.Model
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
            _unitFieldsAddress = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.WowObject.DataPTR);
        }

        public ulong Guid =>_reader.ReadUInt64(_objectBaseAddress + (uint) Offsets.WowObject.Guid);
        public uint Health => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnitFields.Health);
        public uint Mana => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnitFields.Power);

        public virtual string Name
        {
            get
            {
                var unitNameAddress1 = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.UnitName.UnitName1);
                var unitNameAddress2 = _reader.ReadUInt(unitNameAddress1 + (uint) Offsets.UnitName.UnitName2);
                return _reader.ReadString(unitNameAddress2, 50);
            }
        }

        public uint Level => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnitFields.Level);
        public uint BaseAddress => _objectBaseAddress;
        public virtual ObjectType Type => ObjectType.Unit;
        public float X => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.X);
        public float Y => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.Y);
        public float Z => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.Z);
        public float Rotation => _reader.ReadFloat(_objectBaseAddress + (uint) Offsets.WowObject.RotationOffset);

        public CreatureType CreatureType
        {
            get
            {
                try
                {
                    var creatureCache = _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.CreatureCache.CreatureCache);
                    return (CreatureType) _reader.ReadInt(creatureCache + (uint) Offsets.CreatureCache.CreatureType);
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
                    var creatureCache = _reader.ReadUInt(_objectBaseAddress + (uint)Offsets.CreatureCache.CreatureCache);
                    return _reader.ReadInt(creatureCache + (uint)Offsets.CreatureCache.Classification);
                }
                catch (Exception)
                {

                    return -1;
                }
                
            }
        }
        public ulong SummonedBy { get; set; }
        public ulong CreatedBy => _reader.ReadUInt64(_objectBaseAddress + (uint) Offsets.WowUnitFields.CreatedBy);
        public ulong Target => _reader.ReadUInt64(_objectBaseAddress + (uint) Offsets.WowUnitFields.Target);
        public uint CharmedBy => _reader.ReadUInt(_objectBaseAddress + (uint) Offsets.WowUnitFields.CharmedBy);

        public uint[] Auras { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Level}) - {Enum.GetName(typeof(CreatureType),CreatureType)}";
        }
    }
}