using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aesha.Domain;
using Aesha.Robots;
using Serilog;

namespace Aesha.Core
{
    public class RobotManager
    {
        private readonly Process _process;
        private readonly CommandManager _commandManager;
        private readonly ILogger _logger;

        public RobotManager(Process process, CommandManager commandManager, ILogger logger)
        {
            _process = process;
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
                "Felslayer",
                "Lesser Felguard",
                "Ghostpaw Runner"
            };
            
            _robot = new Hunter(_commandManager, Path.FromFile("Ashenvale-Athalaxx.path"),_enemyList, _logger);

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
