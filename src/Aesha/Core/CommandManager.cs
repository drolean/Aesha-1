using System;
using System.Threading;
using Aesha.Domain;
using Aesha.Infrastructure;
using Aesha.Interfaces;

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
        private readonly ProcessMemoryReader _reader;
        private readonly KeyboardCommandDispatcher _keyboard;

        public CommandManager(IWowProcess process, ProcessMemoryReader reader, KeyboardCommandDispatcher keyboard)
        {
            _process = process;
            _reader = reader;
            _keyboard = keyboard;

        }

        public void SetTarget(IWowObject unit)
        {
            SetPlayerFacing(unit.Location);
            _reader.WriteUInt64((uint)Offsets.WowGame.TargetLastTargetGuid, unit.Guid);
            _keyboard.SendKey(MappedKey.TargetLastTarget);
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
            _keyboard.SendKeyDown(nudgeKey);
            _keyboard.SendKeyUp(nudgeKey);
        }


        public void SetPlayerFacing(Location destination, bool instant = true)
        {
            const float radianTolerance = 0.4f;

            var radian = Radian.GetFaceRadian(destination, ObjectManager.Me.Location);
            if (radian.AbsoluteDifference(ObjectManager.Me.Rotation) < radianTolerance)
                return;

            var nudgeKey = MappedKey.Left;
            if (instant)
            {
                InternalSetPlayerFacing(radian, nudgeKey);
                return;
            }

            while (radian.AbsoluteDifference(ObjectManager.Me.Rotation) < radianTolerance)
            {
                var currentRadian = new Radian(ObjectManager.Me.Rotation);
                var direction = currentRadian.GetDirectionOfTravel(radian);
                
                var radiansPerCycle = (float)Math.PI / 10f;
                if (direction == Radian.TravelDirection.Left) radiansPerCycle *= -1;
                
                var nextRadian = currentRadian.Angle - radiansPerCycle;
                InternalSetPlayerFacing(nextRadian, nudgeKey);
                
                radian = Radian.GetFaceRadian(destination, ObjectManager.Me.Location);
                nudgeKey = nudgeKey == MappedKey.Left ? MappedKey.Right : MappedKey.Left;
            }

        }

       


    }
}
