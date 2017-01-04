using ObjectManager.Infrastructure;

namespace ObjectManager.Model
{
    public class WowGameObject : WowObject
    {
        private readonly ProcessMemoryReader _reader;
        private readonly uint _objectBaseAddress;

        public WowGameObject(ProcessMemoryReader reader, uint objectBaseAddress) 
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
                var x = _reader.Read<float>(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_POS_X);
                var y = _reader.Read<float>(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_POS_Y);
                var z = _reader.Read<float>(BaseAddress + (uint)Offsets.WowGameObject.GAMEOBJECT_POS_Z);

                return new Location(x, y, z);
            }
        }
    }
}
