using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aesha.Domain;
using Aesha.Infrastructure;
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
        private IRobot _robot;

        public void Start()
        {
            _enemyList = new List<string>()
            {
                "Forest Spider",
                "Stonetusk Boar"
            };

            var waypointManager = new WaypointManager(_commandManager, Path.FromFile("Goldshire.path"), _logger);
            _robot = new Warlock(_commandManager, waypointManager,_enemyList, _logger);

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
            _logger.Information("Checking for enemies attacking me or nearby");
            var target = GetEnemyAttackingMe() ?? GetNearestUntaggedMob();
            if (target != null)
            {
                _logger.Information($"Found target: {target}");
                _robot.SetTarget(target);
                _logger.Information($"Target set: {target}");
                _logger.Information("Switching to combat");
                return RobotState.Combat;
            }

            _logger.Information("No targets found. Remain passive");
            return RobotState.Passive;
        }

        private WowUnit GetNearestUntaggedMob()
        {
            var enemies = ObjectManager.Units.Where(u =>
                    u.Attributes.Tapped == false
                    && u.Health.Percentage == 100
                    && u.SummonedBy == null
                    && u.CreatureType != CreatureType.Critter
                    && u.Distance < 800)
                .OrderBy(u => u.Distance).ToList();

            var nearest = enemies.FirstOrDefault();
            return nearest ?? null;
        }

        private WowUnit GetEnemyAttackingMe()
        {
            var enemies = ObjectManager.Units.Where(u =>
                    u.Target == ObjectManager.Me 
                 || u.Target == ObjectManager.Me.Pet)
                .OrderBy(u => u.Distance).ToList();

            foreach (var mob in enemies)
            {
                if (!_enemyList.Contains(mob.Name))
                    _enemyList.Add(mob.Name);
                _logger.Information($"Adding mob {mob.Name} to enemy list");
            }

            var nearest = enemies.FirstOrDefault();
            return nearest ?? null;
        }

    }
}
