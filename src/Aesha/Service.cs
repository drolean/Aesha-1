using System.Diagnostics;
using System.Linq;
using Aesha.Core;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Serilog;
using Topshelf;

namespace Aesha
{
    public class Service
    {
        private readonly ILogger _logger;
        private readonly IWowProcess _process;
        private RobotManager _robotManager;

        public Service(ILogger logger)
        {
            _logger = logger;
            var proc = Process.GetProcessesByName("WoW").FirstOrDefault();
            _process = new WowProcess(proc);
            ObjectManager.Start(_process);
        }

        public bool Start(HostControl hostControl)
        {
            var processMemoryReader = new ProcessMemoryReader(_process);
            var keyboard = new KeyboardCommandDispatcher(_process);
            var commandManager = new CommandManager(_process, processMemoryReader, keyboard);
            _robotManager = new RobotManager(commandManager, _logger);

            _robotManager.Start();
            
            return true;
        }

        public void Stop()
        {
            _robotManager.Stop();
        }
    }
}
