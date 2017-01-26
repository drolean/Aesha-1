﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aesha.Objects.Infrastructure;
using Aesha.Objects.Model;
using Common.Logging;

namespace Aesha.Objects
{
    public static class ObjectManager
    {
        private static Process _process;
        private static ProcessMemoryReader _reader;
        private static ILog _logger = LogManager.GetLogger(typeof(ObjectManager));

        private static ConcurrentDictionary<ulong, IWowObject> _objects = new ConcurrentDictionary<ulong, IWowObject>();
        private static CancellationTokenSource _cancellationSource;
        private static Task _pulseTask;

        public static void Start(Process process)
        {
            _logger.Debug("ObjectManager starting");
            AdministrativeRights.Ensure();

            if (process == null)
                throw new ArgumentNullException(nameof(process));

            _reader = new ProcessMemoryReader(process);
            _process = process;

            _cancellationSource = new CancellationTokenSource();
            _pulseTask = new Task(async () =>
            {
                while (!_cancellationSource.IsCancellationRequested)
                {
                    try
                    {
                        Pulse();
                        await Task.Delay(new TimeSpan(0, 0, 0, 0, 10));
                    }
                    catch (Exception)
                    {
                        _cancellationSource.Cancel();
                        throw;
                    }

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
                try
                {
                    return
                        _objects.Where(o => o.Value.Type == ObjectType.Player)
                            .Select(p => (WowPlayer) p.Value)
                            .ToList();
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("Error occured attempting to read Players", ex);
                    throw;
                }

            }
        }



        public static IEnumerable<WowUnit> Units
        {
            get
            {
                try
                {
                    return
                        _objects.Where(
                                o => o.Value.Type == ObjectType.Unit && ((WowUnit) o.Value).Attributes.NPC == false)
                            .Select(u => (WowUnit) u.Value)
                            .ToList();
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("Error occured attempting to read Units", ex);
                    throw;
                }

            }
        }

        public static IEnumerable<WowUnit> Npcs
        {
            get
            {
                try
                {
                    return
                        _objects.Where(o => o.Value.Type == ObjectType.Unit && ((WowUnit) o.Value).Attributes.NPC)
                            .Select(u => (WowUnit) u.Value)
                            .ToList();
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("Error occured attempting to read Npcs", ex);
                    throw;
                }

            }
        }

        public static IEnumerable<IWowObject> Objects
        {
            get
            {
                try
                {
                    return _objects.Values;
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("Error occured attempting to read Objects", ex);
                    throw;
                }
            }
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

    }
}