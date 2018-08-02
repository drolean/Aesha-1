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

        public override string Name {
            get { return "unknown :o("; }
        }

        public override Location Location {
            get
            {
                var x = _reader.ReadFloat(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_POS_X);
                var y = _reader.ReadFloat(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_POS_Y);

                return new Location(x, y);
            }
        }

        public float Facing {
            get
            {
                return _reader.ReadFloat(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_FACING);
            }
        }
    }
}
