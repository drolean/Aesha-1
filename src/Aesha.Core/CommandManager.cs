using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Aesha.Objects;
using Aesha.Objects.Infrastructure;
using Aesha.Objects.Model;

namespace Aesha.Core
{
    public class CommandManager
    {
        private readonly Process _process;
        private readonly ProcessMemoryReader _reader;

        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_KEYUP = 0x101;

        public CommandManager(Process process, ProcessMemoryReader reader)
        {
            _process = process;
            _reader = reader;
        }

        public void SetTarget(ulong guid)
        {
            _reader.WriteUInt64((uint)Offsets.WowGame.TargetLastTargetGuid, guid);
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


        public void SendG(Process process)
        {
            Win32Imports.PostMessage(process.MainWindowHandle, WM_KEYDOWN, 0x00000047, 0x00220001);
            Win32Imports.PostMessage(process.MainWindowHandle, WM_KEYUP, 0x00000047, 0xC0220001);
        }

        public void BeingMoveForward(Process process)
        {

        }


        public void SetPlayerFacing(Location destination)
        {
            var newFacing = GetFaceRadian(destination, ObjectManager.Me.Location);

            var thread = _process.Threads[0];
            var threadPtr = Win32Imports.OpenThread(2032639U, false, (uint)thread.Id);
            Win32Imports.SuspendThread(threadPtr);

            _reader.WriteFloat(ObjectManager.Me.BaseAddress + (uint)Offsets.WowObject.OBJECT_FIELD_ROTATION, newFacing);

            thread = _process.Threads[0];
            threadPtr = Win32Imports.OpenThread(2032639U, false, (uint)thread.Id);
            Win32Imports.ResumeThread(threadPtr);

            Thread.Sleep(50);

            Win32Imports.PostMessage(_process.MainWindowHandle, WM_KEYDOWN, (int)0x25, 0x14B0001);
            Win32Imports.PostMessage(_process.MainWindowHandle, WM_KEYUP, (int)0x25, (0x14B0001 + 0xC0000000));

        }

        public float GetFaceRadian(Location destination, Location current)
        {
            var n = 270 - (Math.Atan2(current.Y - destination.Y, current.X - destination.X)) * 180 / Math.PI;
            var angle = (Math.PI / 180) * (n % 360);

            if (angle < 0f)
                angle = 2 * (float)Math.PI; //6.283185f

            return (float)angle;
        }
    }
}
