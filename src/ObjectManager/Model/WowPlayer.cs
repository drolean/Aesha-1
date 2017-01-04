using System.Diagnostics;
using System.Linq;
using ObjectManager.Infrastructure;

namespace ObjectManager.Model
{
    public class WowPlayer : WowUnit
    {
        private readonly Process _wowProcess;
        private readonly ProcessMemoryReader _reader;
        private readonly uint _objectBaseAddress;

        public WowPlayer(Process wowProcess, ProcessMemoryReader reader, uint objectBaseAddress)
            : base(reader, objectBaseAddress)
        {
            _wowProcess = wowProcess;
            _reader = reader;
            _objectBaseAddress = objectBaseAddress;
        }


        public bool IsActivePlayer
        {
            get
            {
                var currentManager = _reader.Read<uint>((uint)Offsets.WowObjectManager.BASE);
                var activePlayerGuid = _reader.Read<ulong>(currentManager + (uint)Offsets.WowObjectManager.PLAYER_GUID);
                return Guid == activePlayerGuid;
            }
        }

        public WowUnit Pet
        {
            get { return ObjectManager.Units.SingleOrDefault(u => u.SummonedBy?.Guid == Guid); }
        }

        public override string Name {
            get
            {
                var nameStoreAddress = (uint)_wowProcess.MainModule.BaseAddress + (uint)Offsets.WowPlayerNameCache.NAME_CACHE_BASE;
                var baseAddress = _reader.Read<uint>(nameStoreAddress);
                var currentGuid = _reader.Read<ulong>(baseAddress + (uint)Offsets.WowObjectManager.LOCAL_GUID);

                while (currentGuid != Guid)
                {
                    baseAddress = _reader.Read<uint>(baseAddress);
                    currentGuid = _reader.Read<ulong>(baseAddress + (uint)Offsets.WowObjectManager.LOCAL_GUID);
                }

                return _reader.ReadString(baseAddress + (uint)Offsets.WowPlayerNameCache.NAME_CACHE_STRING, 50);
            }
        }

        public override ObjectType Type => ObjectType.Player;

        public uint Xp => _reader.Read<uint>(UnitFieldsAddress + (uint) Offsets.WowPlayer.PLAYER_XP);
        public uint XpRequired => _reader.Read<uint>(UnitFieldsAddress + (uint)Offsets.WowPlayer.PLAYER_NEXT_LEVEL_XP);

        public ClassFlags Class
        {
            get
            {
                var ret = _reader.Read<uint>(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_BYTES_0);
                return (ClassFlags)((ret >> 8) & 0xFF);
            }
        }

        public RaceFlags Race
        {
            get
            {
                var ret = _reader.Read<uint>(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_BYTES_0);
                return (RaceFlags) (ret & 0xFF);
            }
        }

        public override string ToString()
        {
            return $"{Name} (Level {Level} {Race} {Class})";
        }
    }
}