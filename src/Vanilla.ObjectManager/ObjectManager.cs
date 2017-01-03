using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private ConcurrentDictionary<ulong, IWowObject> _objects;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly Task _pulseTask;
        

        public ObjectManager(Process process)
        {
            OpenProcess(process);
            _cancellationSource = new CancellationTokenSource();
            _pulseTask = new Task(async () =>
            {
                while (true)
                {
                    Pulse();
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 10));
                }
            }, _cancellationSource.Token);
        }

        public void Start()
        {
            Pulse();
            _pulseTask.Start();
        }

        public void Stop()
        {
            _cancellationSource.Cancel();
            _objects = new ConcurrentDictionary<ulong,IWowObject>();
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
            get{return _objects.Where(o => o.Value.Type == ObjectType.Player).Select(p => (WowPlayer) p.Value).ToList();}
        }

        public IEnumerable<WowUnit> Units
        {
            get { return _objects.Where(o => o.Value.Type == ObjectType.Unit).Select(u => (WowUnit) u.Value).ToList(); }
        }

        private void Pulse()
        {
            var objectManager = _reader.ReadUInt((uint) Offsets.WowObjectManager.BASE);
            var currentObject = _reader.ReadUInt(objectManager + (uint) Offsets.WowObjectManager.FIRST_OBJECT);
            var activeGuidList = new List<ulong>();

            while (currentObject != 0 && (currentObject & 1) == 0)
            {
                var objectType = _reader.ReadByte(currentObject + (uint) Offsets.WowObject.OBJECT_FIELD_TYPE);
                switch (objectType)
                {
                    case (byte) ObjectType.Unit:
                    {
                        var unit = new WowUnit(_reader, currentObject);
                        _objects.GetOrAdd(unit.Guid, unit);
                        activeGuidList.Add(unit.Guid);
                        break;
                    }
                    case (byte) ObjectType.Player:
                    {
                        var player = new WowPlayer(_process, _reader, currentObject);
                        _objects.GetOrAdd(player.Guid, player);
                        activeGuidList.Add(player.Guid);
                        break;
                    }
                }

                var nextObject = _reader.ReadUInt(currentObject + (uint) Offsets.WowObjectManager.NEXT_OBJECT);

                if (nextObject == currentObject)
                    break;

                currentObject = nextObject;
            }

            var deadGuids = _objects.Keys.Where(k => !activeGuidList.Contains(k)).Select(k => k);
            foreach (var guid in deadGuids)
            {
                IWowObject deadObject;
                _objects.TryRemove(guid, out deadObject);
            }
            
        }

        //var unitFieldsAddress = _reader.ReadUInt(objectAddress + (uint)Offsets.WowObject.DataPTR);
        //var dump = _reader.ReadBytes(unitFieldsAddress, 20000);
        //uint buffId = 687;
        //var buffIndex = dump.FindPattern(buffId);

    }
}
