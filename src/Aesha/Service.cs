using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aesha.Core;
using Aesha.Infrastructure;
using Aesha.Objects;
using Aesha.Robots;
using Serilog;
using Topshelf;

namespace Aesha
{
    public class Service
    {
        
        public Service(ILogger logger)
        {
            var process = Process.GetProcessesByName("WoW").FirstOrDefault();
            ObjectManager.Start(process);
            var robot = new Hunter(new CommandManager(process,new ProcessMemoryReader(process),new KeyboardCommandDispatcher(process)), Path.FromFile("Ashenvale-Athalaxx.path"),
                new List<string>()
                {
                    "Felslayer",
                    "Lesser Felguard",
                    "Ghostpaw Runner"
                }, logger);

            robot.Pulse();
        }

        public bool Start(HostControl hostControl)
        {
            
            return true;
        }

        public void Stop()
        {
            
        }
    }
}
