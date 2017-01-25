using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ObjectManager.Infrastructure;
using ObjectManager.Model;

namespace ObjectManager
{
    public static class ObjectManager
    {
        private static Process _process;
        private static ProcessMemoryReader _reader;

        private static ConcurrentDictionary<ulong, IWowObject> _objects = new ConcurrentDictionary<ulong, IWowObject>();
        private static CancellationTokenSource _cancellationSource;
        private static Task _pulseTask;

        public static void Start(Process process)
        {
            AdministrativeRights.Ensure();

            if (process == null)
                throw new ArgumentNullException(nameof(process));

            _reader = new ProcessMemoryReader(process);
            _process = process;

            //read loginstate
            var state = _reader.ReadString((uint) 0xB41478, 10);
            //login
            //charselect


            _cancellationSource = new CancellationTokenSource();
            _pulseTask = new Task(async () =>
            {
                while (true)
                {
                    Pulse();
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 10));
                }
            }, _cancellationSource.Token);

            Pulse();
            _pulseTask.Start();
        }

        public static void Stop()
        {
            _cancellationSource.Cancel();
            _objects = new ConcurrentDictionary<ulong, IWowObject>();
        }

        public static WowPlayer Me
        {
            get { return Players.SingleOrDefault(p => p.IsActivePlayer); }
        }

        public static IEnumerable<WowPlayer> Players
        {
            get
            {
                return
                    _objects.Where(o => o.Value.Type == ObjectType.Player)
                        .Select(p => (WowPlayer) p.Value)
                        .ToList();
            }
        }

        public static void SetTarget(ulong guid)
        {
            _reader.WriteUInt64((uint)Offsets.WowGame.TargetLastTargetGuid, guid);
        }

        public static WowUnit GetMouseOverTarget()
        {
            try
            {
                var guid = _reader.ReadUInt64((uint)Offsets.WowGame.MouseOverGuid);
                var unit = _objects.SingleOrDefault(o => o.Value.Guid == guid);
                return (WowUnit)unit.Value;
            }
            catch (Exception)
            {
                return null;
            }

        }


        public static float GetDistance(Location dest, Location currentPos, bool UseZ)
        {
            float num = currentPos.X - dest.X;
            float num2 = currentPos.Y - dest.Y;
            float num3 = (dest.Z != 0f) ? (currentPos.Z - dest.Z) : 0f;
            if (UseZ)
            {
                return (float)Math.Sqrt((double)(((num * num) + (num2 * num2)) + (num3 * num3)));
            }
            return (float)Math.Sqrt((double)((num * num) + (num2 * num2)));
        }



        public static void SetPlayerFacing(Location destination)
        {

            var angle = GetFaceRadian(destination, ObjectManager.Me.Location);
            
            var thread = _process.Threads[0];
            var threadPtr = Win32Imports.OpenThread(2032639U, false, (uint) thread.Id);
            Win32Imports.SuspendThread(threadPtr);

            _reader.WriteFloat(Me.BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_ROTATION, angle);

            thread = _process.Threads[0];
            threadPtr = Win32Imports.OpenThread(2032639U, false, (uint)thread.Id);
            Win32Imports.ResumeThread(threadPtr);

            Thread.Sleep(50);

            Win32Imports.PostMessage(_process.MainWindowHandle, 0x100, (int) 0x25, 0x14B0001);
            Win32Imports.PostMessage(_process.MainWindowHandle, 0x101, (int) 0x25, (0x14B0001 + 0xC0000000));

        }

        private  static float GetFaceRadian(Location dest, Location currentPos)
        {
            return NegativeAngle((float)Math.Atan2((double)(dest.Y - currentPos.Y), (double)(dest.X - currentPos.X)));
        }

        private static float NegativeAngle(float angle)
        {
            if (angle < 0f)
            {
                angle += 2 * (float)Math.PI; //6.283185f
            }
            return angle;
        }

        public static IEnumerable<WowUnit> Units
        {
            get
            {
                return
                    _objects.Where(o => o.Value.Type == ObjectType.Unit && ((WowUnit) o.Value).Attributes.NPC == false)
                        .Select(u => (WowUnit) u.Value)
                        .ToList();
            }
        }

        public static IEnumerable<WowUnit> Npcs
        {
            get
            {
                return 
                    _objects.Where(o => o.Value.Type == ObjectType.Unit && ((WowUnit) o.Value).Attributes.NPC)
                        .Select(u => (WowUnit) u.Value)
                        .ToList();
            }
        }

        public static IEnumerable<IWowObject> Objects
        {
            get { return _objects.Values; }
        }

        private static void Pulse()
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
                    case (byte)ObjectType.Item:
                    {
                        break;
                    }
                    case (byte)ObjectType.GameObject:
                    {
                        var obj = new WowGameObject(_reader, currentObject);
                        _objects.GetOrAdd(obj.Guid, obj);
                        activeGuidList.Add(obj.Guid);
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

        //var bob = _reader.ReadUInt(_objectBaseAddress + (uint)Offsets.WowUnit.UNIT_FIELD_AURA);
        //var dump = _reader.ReadBytes(_unitFieldsAddress, 20000);
        //uint buffId = 687;
        //var buffIndex = dump.FindPattern(buffId);


    }
}
