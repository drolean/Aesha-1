using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Aesha.Infrastructure;
using Aesha.Objects;
using Aesha.Objects.Model;

namespace Aesha.Core
{
    public class CommandManager
    {
        private readonly Process _process;
        private readonly ProcessMemoryReader _reader;
        private readonly KeyboardCommandDispatcher _keyboard;

        public CommandManager(Process process, ProcessMemoryReader reader, KeyboardCommandDispatcher keyboard)
        {
            _process = process;
            _reader = reader;
            _keyboard = keyboard;
        }

        public void SetTarget(ulong guid)
        {
            _reader.WriteUInt64((uint)Offsets.WowGame.TargetLastTargetGuid, guid);
        }


        public void SendKeyDown(char key)
        {
            _keyboard.SendKeyDown(key);
        }

        public void SendKeyUp(char key)
        {
            _keyboard.SendKeyUp(key);
        }


        public void SendKey(char key)
        {
            _keyboard.SendKey(key);
        }

        public WowUnit GetMouseOverTarget()
        {
            try
            {
                var guid = _reader.ReadUInt64((uint)Offsets.WowGame.MouseOverGuid);
                var unit = ObjectManager.Objects.SingleOrDefault(o => o.Guid == guid);
                return (WowUnit)unit;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public void GetNearestUntaggedMob()
        {

        }

        public void SetPlayerFacing(Location destination)
        {

            var newFacing = GetFaceRadian(destination, ObjectManager.Me.Location);
            if (Math.Abs(newFacing - ObjectManager.Me.Rotation) > 1.5)
            {
                var thread = _process.Threads[0];
                var threadPtr = Win32Imports.OpenThread(2032639U, false, (uint) thread.Id);
                Win32Imports.SuspendThread(threadPtr);

                _reader.WriteFloat(ObjectManager.Me.BaseAddress + (uint) Offsets.WowObject.OBJECT_FIELD_ROTATION,
                    newFacing);

                thread = _process.Threads[0];
                threadPtr = Win32Imports.OpenThread(2032639U, false, (uint) thread.Id);
                Win32Imports.ResumeThread(threadPtr);

                Thread.Sleep(50);
                _keyboard.SendKey('D');
            }
            else
            {
                _keyboard.SetPlayerFacing(newFacing);
            }
        }

        public float GetFaceRadian(Location destination, Location current)
        {
            var n = 270 - (Math.Atan2(current.Y - destination.Y, current.X - destination.X)) * 180 / Math.PI;
            var angle = (Math.PI / 180) * (n % 360);

            if (angle < 0f)
                angle = 2 * (float)Math.PI;

            return (float)angle;
        }
    }
}
