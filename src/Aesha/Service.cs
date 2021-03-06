﻿using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
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
            ObjectManager.Start(_process, new ProcessMemoryReader(_process));
        }
        
        public bool Start(HostControl hostControl)
        {
            var processMemoryReader = new ProcessMemoryReader(_process);

            var keyboard = KeyboardCommandDispatcher.GetKeyboard(_process);
            var commandManager = CommandManager.GetDefault(_process, processMemoryReader, keyboard,_logger);
            
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
