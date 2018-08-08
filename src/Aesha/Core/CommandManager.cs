using System;
using System.Threading.Tasks;
using Aesha.Domain;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Aesha.Robots.Actions;
using Serilog;

namespace Aesha.Core
{
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
            _reader.WriteUInt64((uint)Offsets.WowGame.TargetLastTargetGuid, unit.Guid);
            SendKey(MappedKeys.TargetLastTarget);
            SetPlayerFacing(unit.Location);
        }

        public void ClearTarget()
        {
            //TODO send ESC key
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
                if (action.Evaluate())
                {
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


        public void SetPlayerFacing(Location destination, float threshold = 0.4f)
        {
            const float memoryWriteThreshold = 1.5f;

            var radian = Radian.GetFaceRadian(destination, ObjectManager.Me.Location);
            var nudgeKey = MappedKeys.Left;

            var difference = ObjectManager.Me.Rotation - radian.Angle;
            var absoluteDifference = Math.Abs(difference);
            if (absoluteDifference < threshold) return;

            if (absoluteDifference > memoryWriteThreshold)
            {
                _logger.Information("Set player facing using memory write");
                InternalSetPlayerFacing(radian, nudgeKey);
                return;
            }

            var direction = difference > 0 ? MappedKeys.Right : MappedKeys.Left;
            while (absoluteDifference > threshold)
            {
                SendKeyDown(direction);

                radian = Radian.GetFaceRadian(destination, ObjectManager.Me.Location);
                absoluteDifference = Math.Abs(ObjectManager.Me.Rotation - radian.Angle);

                if (absoluteDifference > memoryWriteThreshold)
                {
                    _logger.Information("Set player facing using memory write");
                    InternalSetPlayerFacing(radian, nudgeKey);
                    break;
                }
            }

            SendKeyUp(direction);
        }
        
        public ulong MouseOverUnit => _reader.ReadUInt64((uint)Offsets.WowGame.MouseOverGuid);

        public void SetFocus()
        {
            Win32Imports.SetForegroundWindow(_process.MainWindowHandle);
        }
    }
}
