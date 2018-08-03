using Aesha.Core;
using Aesha.Infrastructure;
using Aesha.Interfaces;

namespace Aesha.Domain
{
    public class WowObject : IWowObject
    {
        private readonly IProcessMemoryReader _reader;
        protected readonly uint UnitFieldsAddress;

        protected WowObject(IProcessMemoryReader reader, uint objectBaseAddress)
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

                return new Location(x, y);
            }
        }

        public float Rotation => _reader.ReadFloat(BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_ROTATION);
        
    }

    public interface IWowObject
    {
        ulong Guid { get; }
        uint BaseAddress { get; }
        ObjectType Type { get; }
        string Name { get; }
        Location Location { get; }
        float Rotation { get; }

    }
}
