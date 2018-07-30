using Aesha.Infrastructure;

namespace Aesha.Objects.Model
{
    public class WowObject : IWowObject
    {
        private readonly ProcessMemoryReader _reader;
        protected readonly uint UnitFieldsAddress;

        public WowObject(ProcessMemoryReader reader, uint objectBaseAddress)
        {
            _reader = reader;
            BaseAddress = objectBaseAddress;
            UnitFieldsAddress = _reader.ReadUInt(BaseAddress + (uint)Offsets.WowObjectManager.DESCRIPTOR);
        }

        public ulong Guid => _reader.ReadUInt64(BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_GUID);
        public uint BaseAddress { get; }
        public virtual ObjectType Type => ObjectType.None;
        public virtual string Name => string.Empty;

        public virtual Location Location
        {
            get
            {
                var x = _reader.ReadFloat(BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_X);
                var y = _reader.ReadFloat(BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_Y);
                //var z = _reader.ReadFloat(BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_Z);

                return new Location(x, y);
            }
        }

        public float Rotation => _reader.ReadFloat(BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_ROTATION);
        
    }
}
