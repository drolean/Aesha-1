using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Domain;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Aesha.Robots.Actions;
using Serilog;

namespace Aesha.Core
{

    public class MappedKeyAction
    {
        public MappedKeyAction(char key, bool shift = false, bool ctrl = false, bool alt = false)
        {
            Key = key;
            Shift = shift;
            Ctrl = ctrl;
            Alt = alt;
        }

        public char Key { get; }
        public bool Shift { get; }
        public bool Ctrl { get; }
        public bool Alt { get; }
    }
    
    public class CommandManager
    {
        private readonly IWowProcess _process;
        private readonly IProcessMemoryReader _reader;
        private readonly KeyboardCommandDispatcher _keyboard;
        private readonly ILogger _logger;

        private CommandManager(IWowProcess process, IProcessMemoryReader reader, KeyboardCommandDispatcher keyboard, ILogger logger)
        {
            _process = process;
            _reader = reader;
            _keyboard = keyboard;
            _logger = logger;
        }

        private static CommandManager _commandManager;
        public static CommandManager GetDefault(IWowProcess process = null, IProcessMemoryReader reader = null, KeyboardCommandDispatcher keyboard = null, ILogger logger = null)
        {
            if (_commandManager != null)
                return _commandManager;

            if (process == null)throw new ArgumentNullException(nameof(process));
            if (reader == null)throw new ArgumentNullException(nameof(reader));
            if (keyboard == null)throw new ArgumentNullException(nameof(keyboard));
            if (logger == null)throw new ArgumentNullException(nameof(logger));

            _commandManager = new CommandManager(process, reader, keyboard, logger);
            return _commandManager;
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
            SendKey(MappedKeys.TargetLastTarget);
        }

        public void ClearTarget()
        {
            SendKey(MappedKeys.Esc);
        }

        public void SendKeyDown(MappedKeyAction action)
        {
            _logger.Information($"Sending key down: '{action.Key}'");
            _keyboard.SendKeyDown(action.Key);
        }

        public void SendKeyUp(MappedKeyAction action)
        {
            _logger.Information($"Sending key up: '{action.Key}'");
            _keyboard.SendKeyUp(action.Key);
        }

        public void StopMovingForward()
        {
            SendKeyUp(MappedKeys.Forward);
        }

        public void EvaluateAndPerform(IConditionalAction action)
        {
            var task = new Task(() =>
            {
                _logger.Information($"Evaulating action: {action.GetType()}");
                if (action.Evaluate())
                {
                    _logger.Information($"Invoking action {action.GetType()}");
                    action.Do();
                }
            });

            task.Start();
            task.Wait();
        }


        public void SendKey(MappedKeyAction action)
        {
            _logger.Information($"Sending key: '{action.Key}' Shift:{action.Shift} Ctrl:{action.Ctrl} Alt:{action.Alt}");
            if (action.Shift) _keyboard.SendShiftKey(action.Key);
            else _keyboard.SendKey(action.Key);
        }

        private void InternalSetPlayerFacing(Radian radian, MappedKeyAction nudgeKey)
        {
            var thread = _process.Threads[0];
            var threadPtr = Win32Imports.OpenThread(2032639U, false, (uint)thread.Id);
            Win32Imports.SuspendThread(threadPtr);

            _reader.WriteFloat(ObjectManager.Me.BaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_ROTATION, radian.Angle);

            thread = _process.Threads[0];
            threadPtr = Win32Imports.OpenThread(2032639U, false, (uint)thread.Id);
            Win32Imports.ResumeThread(threadPtr);

            Task.Delay(50).Wait();
            SendKey(nudgeKey);
            Task.Delay(50).Wait();
        }


        public void SetPlayerFacing(Location destination, float threshhold = 0.2f)
        {
            var radian = Radian.GetFaceRadian(destination, ObjectManager.Me.Location);
            var nudgeKey = MappedKeys.Left;

            var diff = Math.Abs(ObjectManager.Me.Rotation - radian.Angle);
            if (diff < threshhold)
            {
                _logger.Information($"Not setting facing. Lower than threshold: {diff}");
                return;
            }

            _logger.Information("Set player facing");
            InternalSetPlayerFacing(radian, nudgeKey);
        }


        public void Loot(IEnumerable<IWowObject> unitsToLoot)
        {
            
        }
        
        public ulong MouseOverUnit => _reader.ReadUInt64((uint)Offsets.WowGame.MouseOverGuid);
    }
}
