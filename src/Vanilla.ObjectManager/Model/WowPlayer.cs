using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Vanilla.ObjectManager.Infrastucture;

namespace Vanilla.ObjectManager.Model
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
                var currentManager = _reader.ReadUInt((uint)Offsets.WowObjectManager.Base);
                var activePlayerGuid = _reader.ReadUInt64(currentManager + (uint)Offsets.WowObjectManager.LocalGUID);
                return Guid == activePlayerGuid;
            }
        }

        public override string Name {
            get
            {
                var nameStoreAddress = (uint)_wowProcess.MainModule.BaseAddress + (uint)Offsets.UnitName.PlayerNameCachePointer;
                var baseAddress = _reader.ReadUInt(nameStoreAddress);
                var currentGuid = _reader.ReadUInt64(baseAddress + (uint)Offsets.UnitName.PlayerNameGUIDOffset);

                while (currentGuid != Guid)
                {
                    baseAddress = _reader.ReadUInt(baseAddress);
                    currentGuid = _reader.ReadUInt64(baseAddress + (uint)Offsets.UnitName.PlayerNameGUIDOffset);
                }

                return _reader.ReadString(baseAddress + (uint)Offsets.UnitName.PlayerNameStringOffset, 50);
            }
        }

        public override ObjectType Type => ObjectType.Player;

        public uint Xp => _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WoWPlayerFields.PLAYER_XP);
        public uint XpRequired => _reader.ReadUInt(_unitFieldsAddress + (uint)Offsets.WoWPlayerFields.PLAYER_NEXT_LEVEL_XP);

        public ClassFlags Class
        {
            get
            {
                var ret = _reader.ReadUInt(_unitFieldsAddress + (uint) Offsets.WowUnitFields.ClassAndRace);
                return (ClassFlags)((ret >> 8) & 0xFF);
            }
        }

        public RaceFlags Race {
            get
            {
                var ret = _reader.ReadUInt(_unitFieldsAddress + (uint)Offsets.WowUnitFields.ClassAndRace);
                return (RaceFlags)(ret & 0xFF);
            }
        }

        public override string ToString()
        {
            return $"{Name} (Level {Level} {Race} {Class})";
        }
    }

    public interface IWowObject
    {
        ulong Guid { get; }
        uint BaseAddress { get; }
        ObjectType Type { get;}

    }
}