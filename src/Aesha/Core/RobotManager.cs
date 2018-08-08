using System;
using System.Threading;
using System.Threading.Tasks;
using Aesha.Interfaces;
using Aesha.Robots;
using Serilog;

namespace Aesha.Core
{
    public class RobotManager
    {
        private readonly CommandManager _commandManager;
        private readonly ILogger _logger;

        public RobotManager(CommandManager commandManager, ILogger logger)
        {
            _commandManager = commandManager;
            _logger = logger;
        }
        
        private Task _task;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationSource;
        private IRobot _robot;

        public void Start()
        {
            var waypointManager = new WaypointManager(Path.FromFile("LochModan.path"),_logger);
            _robot = new Warlock(_commandManager, waypointManager, _logger);
            //_robot = new Fisherman(_commandManager,_logger);
            
            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            _task = new Task(() =>
            {
                try
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        _commandManager.SetFocus();
                        _robot.Tick();
                        Task.Delay(100, _cancellationToken).Wait(_cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex,"An error occured whilst running Robot");
                    throw;
                }

            }, _cancellationToken, TaskCreationOptions.LongRunning);
            _task.Start();
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
