﻿using System.Linq;
using Aesha.Core;
using Aesha.Infrastructure;
using Aesha.Interfaces;

namespace Aesha.Domain
{
    public class WowPlayer : WowUnit
    {
        private readonly IWowProcess _wowProcess;
        private readonly IProcessMemoryReader _reader;

        public WowPlayer(IWowProcess wowProcess, IProcessMemoryReader reader, uint objectBaseAddress)
            : base(reader, objectBaseAddress)
        {
            _wowProcess = wowProcess;
            _reader = reader;
        }


        public bool IsActivePlayer
        {
            get
            {
                var currentManager = _reader.ReadUInt((uint)Offsets.WowObjectManager.BASE);
                var activePlayerGuid = _reader.ReadUInt64(currentManager + (uint)Offsets.WowObjectManager.PLAYER_GUID);
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
                var nameStoreAddress = (uint)_wowProcess.MainModuleBaseAddress + (uint)Offsets.WowPlayerNameCache.NAME_CACHE_BASE;
                var baseAddress = _reader.ReadUInt(nameStoreAddress);
                var currentGuid = _reader.ReadUInt64(baseAddress + (uint)Offsets.WowObjectManager.LOCAL_GUID);

                while (currentGuid != Guid)
                {
                    baseAddress = _reader.ReadUInt(baseAddress);
                    currentGuid = _reader.ReadUInt64(baseAddress + (uint)Offsets.WowObjectManager.LOCAL_GUID);
                }

                return _reader.ReadString(baseAddress + (uint)Offsets.WowPlayerNameCache.NAME_CACHE_STRING, 50);
            }
        }

        public override ObjectType Type => ObjectType.Player;

        public uint Xp => _reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowPlayer.PLAYER_XP);
        public uint XpRequired => _reader.ReadUInt(UnitFieldsAddress + (uint)Offsets.WowPlayer.PLAYER_NEXT_LEVEL_XP);

        public UnitClassTypes UnitClass
        {
            get
            {
                var ret = _reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_BYTES_0);
                return (UnitClassTypes)((ret >> 8) & 0xFF);
            }
        }

        public UnitRaceTypes UnitRace
        {
            get
            {
                var ret = _reader.ReadUInt(UnitFieldsAddress + (uint) Offsets.WowUnit.UNIT_FIELD_BYTES_0);
                return (UnitRaceTypes) (ret & 0xFF);
            }
        }
        
        public override string ToString()
        {
            return $"{Name} (Level {Level} {UnitRace} {UnitClass})";
        }
    }
}