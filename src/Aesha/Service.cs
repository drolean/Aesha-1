using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly Hunter _robot;
        private Task _task;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationSource;

        public Service(ILogger logger)
        {
            var process = Process.GetProcessesByName("WoW").FirstOrDefault();
            ObjectManager.Start(process);
            _robot =
                new Hunter(
                    new CommandManager(process, new ProcessMemoryReader(process), new KeyboardCommandDispatcher(process)),
                    Path.FromFile("Ashenvale-Athalaxx.path"),
                    new List<string>()
                    {
                        "Felslayer",
                        "Lesser Felguard",
                        "Ghostpaw Runner"
                    }, logger);
        }

        public bool Start(HostControl hostControl)
        {
            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            _task = new Task(() =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    _robot.Pulse();
                    Thread.Sleep(100);
                }
                
            }, _cancellationToken);

            _task.Start();
            return true;
        }

        public void Stop()
        {
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationSource.Cancel();
            }
        }
    }
}
