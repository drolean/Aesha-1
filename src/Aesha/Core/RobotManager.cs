using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aesha.Domain;
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
        private List<string> _enemyList;
        private Hunter _robot;

        public void Start()
        {
            _enemyList = new List<string>()
            {
                "Nightbane Shadow Weaver",
                "Nightbane Dark Runner",
                "Nightbane Worgen"
            };
            
            var waypointManager = new WaypointManager(_commandManager,Path.FromFile("Ashenvale-Athalaxx.path"));
            _robot = new Hunter(_commandManager, waypointManager,_enemyList, _logger);

            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            _task = new Task(() =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    _robot.Tick(CheckState());
                    Thread.Sleep(100);
                }

            }, _cancellationToken);

            _task.Start();
        }

        public void Stop()
        {
            if (_cancellationToken.CanBeCanceled)
            {
                _cancellationSource.Cancel();
            }
        }


        private RobotState CheckState()
        {
            var target = GetEnemyAttackingMe() ?? GetNearestUntaggedMob();
            if (target != null)
            {
                _robot.SetTarget(target);
                return RobotState.Combat;
            }

            return RobotState.Passive;
        }

        private WowUnit GetNearestUntaggedMob()
        {
            var enemies = ObjectManager.Units.Where(u =>
                    _enemyList.Any(e => e.Contains(u.Name))
                    && u.Health.Percentage == 100
                    && u.Distance < 800)
                .OrderBy(u => u.Distance).ToList();

            var nearest = enemies.FirstOrDefault();
            return nearest ?? null;
        }

        private WowUnit GetEnemyAttackingMe()
        {
            var enemies = ObjectManager.Units.Where(u =>
                    _enemyList.Any(e => e.Contains(u.Name))
                    && u.Target == ObjectManager.Me)
                .OrderBy(u => u.Distance).ToList();

            var nearest = enemies.FirstOrDefault();
            return nearest ?? null;
        }

    }
}
