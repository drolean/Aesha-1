using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vanilla.ObjectManager.Infrastucture;
using Vanilla.ObjectManager.Model;

namespace Vanilla.ObjectManager
{
    public class ObjectManager
    {

        private Process _process;
        private  ProcessMemoryReader _reader;

        private const uint StandardRightsRequired = 0x000F0000;
        private const uint Synchronize = 0x00100000;
        private const uint ProcessAllAccess = StandardRightsRequired | Synchronize | 0xFFF;

        private List<IWowObject> _objects;
       
        public ObjectManager(Process process)
        {
            OpenProcess(process);
        }

        private void OpenProcess(Process process)
        {
            _process = process;

            var processPtr = Win32Imports.OpenProcess(ProcessAllAccess, false, process.Id);
            _reader = new ProcessMemoryReader();
            _reader.Open(processPtr);
        }

        public WowPlayer Me
        {
            get { return Players.SingleOrDefault(p => p.IsActivePlayer); }
        }

        public IEnumerable<WowPlayer> Players
        {
            get { return _objects.Where(o => o.Type == ObjectType.Player).Select(p => (WowPlayer) p).ToList(); }
        }

        public IEnumerable<WowUnit> Units
        {
            get { return _objects.Where(o => o.Type == ObjectType.Unit).Select(u => (WowUnit) u).ToList(); }
        }

        public void Pulse()
        {
            _objects = new List<IWowObject>();

            var objectManager = _reader.ReadUInt((uint)Offsets.WowObjectManager.Base);
            var currentObject = _reader.ReadUInt(objectManager + (uint)Offsets.WowObjectManager.FirstObject);
           
            while (currentObject != 0 && (currentObject & 1) == 0)
            {
                var objectType = _reader.ReadByte(currentObject + (uint)Offsets.WowObjectManager.ObjectType);
                switch (objectType)
                {
                    case (byte)ObjectType.Unit:
                    {
                        var unit = new WowUnit(_reader, currentObject);
                        _objects.Add(unit);
                        break;
                    }
                    case (byte)ObjectType.Player:
                    {
                        var player = new WowPlayer(_process, _reader, currentObject);
                        _objects.Add(player);
                        break;
                    }
                }

                var nextObject = _reader.ReadUInt(currentObject + (uint)Offsets.WowObjectManager.NextObject);

                if (nextObject == currentObject)
                    break;
                
                currentObject = nextObject;
            }

        }

        //var unitFieldsAddress = _reader.ReadUInt(objectAddress + (uint)Offsets.WowObject.DataPTR);
        //var dump = _reader.ReadBytes(unitFieldsAddress, 20000);
        //uint buffId = 687;
        //var buffIndex = dump.FindPattern(buffId);

    }
}
