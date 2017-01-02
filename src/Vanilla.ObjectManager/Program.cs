using System;
using System.Diagnostics;
using System.Linq;

namespace Vanilla.ObjectManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = Process.GetProcessesByName("WoW").First();  
            var objectManager = new ObjectManager(process);
            
            objectManager.Pulse();

            var me = objectManager.Me;
            var players = objectManager.Players;
            var units = objectManager.Units;

            Console.ReadLine();
            
        }
    }
}
