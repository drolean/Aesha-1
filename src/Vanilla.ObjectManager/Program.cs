using System;
using System.Diagnostics;
using Vanilla.ObjectManager.Infrastucture;

namespace Vanilla.ObjectManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var wowProcessId = Process.GetProcessesByName("WoW")[0].Id;
            var connection = new ProcessConnection(wowProcessId, new ProcessMemoryReader());
            connection.OpenProcessAndThread();
            
            var objectManager = new ObjectManager(connection);
            Console.WriteLine("Attaching to pid: " + wowProcessId);

            objectManager.GetObjects();
            
        }
    }
}
