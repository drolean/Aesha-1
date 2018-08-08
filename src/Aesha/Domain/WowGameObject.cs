using Aesha.Core;
using Aesha.Infrastructure;
using Aesha.Interfaces;

namespace Aesha.Domain
{
    public class WowGameObject : WowObject
    {
        private readonly IProcessMemoryReader _reader;
        private readonly uint _objectBaseAddress;

        public WowGameObject(IProcessMemoryReader reader, uint objectBaseAddress) 
            : base(reader, objectBaseAddress)
        {
            _reader = reader;
            _objectBaseAddress = objectBaseAddress;
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

        public override Location Location {
            get
            {
                var x = _reader.ReadFloat(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_POS_X);
                var y = _reader.ReadFloat(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_POS_Y);

                return new Location(x, y);
            }
        }

        public uint CreatedBy => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.OBJECT_FIELD_CREATED_BY);
        public uint DisplayId => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_DISPLAYID);
        public uint Flags => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_FLAGS);
        public uint State => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_STATE);
        public uint Faction => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_FACTION);
        public uint TypeId => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_TYPE_ID);
        public uint Level => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_LEVEL);
        public uint Artkit => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_ARTKIT);
        public ushort Bobbing => _reader.ReadUShort(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_BOBBING);
        public uint AnimationProgress => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_ANIMPROGRESS);
    }
}
