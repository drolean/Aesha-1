using System;
using Vanilla.ObjectManager.Infrastucture;

namespace Vanilla.ObjectManager
{
    public class ObjectManager
    {
        public ObjectManager(ProcessConnection connection)
        {
            _processConnection = connection;
        }

        private enum ObjTypes : byte
        {
            None = 0,
            Item = 1,
            Container = 2,
            Unit = 3,
            Player = 4,
            GameObject = 5,
            DynamicObject = 6,
            Corpse = 7
        }

        private static readonly uint objectManagerOffset = 0x00B41414;
        private static readonly uint firstObjectOffset = 0xac;
        private static readonly uint nextObjectOffset = 0x3c;
        private static readonly uint objectTypeOffset = 0x14;
        private readonly ProcessConnection _processConnection;

        public void GetObjects()
        {
            var objectManager = _processConnection.Memory.ReadUInt(objectManagerOffset);
            var currentObject = _processConnection.Memory.ReadUInt(objectManager + firstObjectOffset);

            while (currentObject != 0 && (currentObject & 1) == 0)
            {
                var objectType = _processConnection.Memory.ReadByte(currentObject + objectTypeOffset);
                var objectTypeName = Enum.GetName(typeof(ObjTypes), objectType);
                
                Console.WriteLine($"Found a object of type {objectTypeName} at 0x{currentObject:X8}");

                var nextObject = _processConnection.Memory.ReadUInt(currentObject + nextObjectOffset);

                if (nextObject == currentObject)
                    break;
                
                currentObject = nextObject;
            }
        }
    }
}
