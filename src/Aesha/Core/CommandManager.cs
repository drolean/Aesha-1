using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Aesha.Domain;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Serilog;

namespace Aesha.Core
{

    public static class MappedKey
    {
        public const char Left = 'A';
        public const char Right = 'D';
        public const char Forward = 'W';
        public const char Backward = 'S';
        public const char TargetLastTarget = 'G';
        public const char ActionBar1 = '1';
        public const char ActionBar2 = '2';
        public const char ActionBar3 = '3';
        public const char ActionBar4 = '4';
        public const char ActionBar5 = '5';
        public const char ActionBar6 = '6';
        public const char ActionBar7 = '7';
        public const char ActionBar8 = '8';
        public const char ActionBar9 = '9';
        public const char ActionBar10 = '0';

    }

    public class CommandManager
    {
        private readonly IWowProcess _process;
        private readonly IProcessMemoryReader _reader;
        private readonly KeyboardCommandDispatcher _keyboard;
        private readonly ILogger _logger;

        public CommandManager(IWowProcess process, IProcessMemoryReader reader, KeyboardCommandDispatcher keyboard, ILogger logger)
        {
            _process = process;
            _reader = reader;
            _keyboard = keyboard;
            _logger = logger;
        }

        public void SetTarget(IWowObject unit)
        {
            var radian = Radian.GetFaceRadian(unit.Location, ObjectManager.Me.Location);
            var rotationDiff = Math.Abs(ObjectManager.Me.Rotation - radian.Angle);
            if (rotationDiff > 0.5f)
            {
                _logger.Information($"Rotation difference greater than tolerance: {rotationDiff}");
                SetPlayerFacing(unit.Location);
            }

            _reader.WriteUInt64((uint)Offsets.WowGame.TargetLastTargetGuid, unit.Guid);
            _keyboard.SendKey(MappedKey.TargetLastTarget);
        }

        public void SendKeyDown(char key)
        {
            _logger.Information($"Sending key down: '{key}'");
            _keyboard.SendKeyDown(key);
        }

        public void SendKeyUp(char key)
        {
            _logger.Information($"Sending key up: '{key}'");
            _keyboard.SendKeyUp(key);
        }


        public void SendKey(char key)
        {
            _logger.Information($"Sending key: '{key}'");
            _keyboard.SendKey(key);
        }

        private void InternalSetPlayerFacing(Radian radian, char nudgeKey)
        {
            var thread = _process.Threads[0];
            var threadPtr = Win32Imports.OpenThread(2032639U, false, (uint)thread.Id);
            Win32Imports.SuspendThread(threadPtr);

            _reader.WriteFloat(ObjectManager.Me.BaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_ROTATION, radian.Angle);

            thread = _process.Threads[0];
            threadPtr = Win32Imports.OpenThread(2032639U, false, (uint)thread.Id);
            Win32Imports.ResumeThread(threadPtr);

            Thread.Sleep(50);
            _keyboard.SendKey(nudgeKey);
            Thread.Sleep(50);
        }


        public void SetPlayerFacing(Location destination)
        {
            var radian = Radian.GetFaceRadian(destination, ObjectManager.Me.Location);
            var nudgeKey = MappedKey.Left;

            _logger.Information("Set player facing");
            InternalSetPlayerFacing(radian, nudgeKey);
        }


        public void Loot(IEnumerable<IWowObject> unitsToLoot)
        {
            var unitsLooted = new List<IWowObject>(); 

            for (var x = 700; x <= 1150; x += 30)
            {
                for (var y = 450; y <= 850; y += 20)
                {
                    Cursor.Position = new Point(x, y);
                    Thread.Sleep(10);
                    if (MouseOverUnit > 0)
                    {
                        var unit = unitsToLoot.SingleOrDefault(u => u.Guid == MouseOverUnit);
                        if (unit == null)
                            continue;

                        if (unit == ObjectManager.Me.Pet)
                            continue;

                        if (unitsLooted.Contains(unit))
                            continue;
                            
                        _keyboard.SendShiftClick(new Point(x, y));
                        Console.WriteLine($"Attempting to loot unit: {unit}");
                        Thread.Sleep(500);
                        unitsLooted.Add(unit);

                        var outstandingWork = false;
                        foreach (var u in unitsToLoot)
                        {
                            if (!unitsLooted.Contains(u))
                                outstandingWork = true;
                        }

                        if (!outstandingWork) return;
                    }
                }
            }

            _keyboard.SendShiftClick(new Point(1000, 900));
        }
        
        public ulong MouseOverUnit => _reader.ReadUInt64((uint)Offsets.WowGame.MouseOverGuid);
    }
}
