using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Aesha.Core;
using Aesha.Objects;
using Aesha.Objects.Infrastructure;
using Aesha.Objects.Model;
using Serilog;
using Serilog.Core;

namespace Aesha.Robots
{
    public class Fubars
    {
        private ProcessMemoryReader _memReader;
        private CommandManager _cm;
        private KeyboardCommandDispatcher _kcd;
        private Process _process;
        private Dictionary<int, Location> _path;
        private Logger _logger;

        public void Start(Process process)
        {

            _logger = new LoggerConfiguration().WriteTo.File("log.txt").CreateLogger();

            ObjectManager.Start(process);
            _process = process;
            _memReader = new ProcessMemoryReader(process);
            _cm = new CommandManager(process,_memReader);
            _kcd = new KeyboardCommandDispatcher(process);

            LoadPath("");

            Start();
            
        }

        private List<String> _enemyWhiteList = new List<string>()
        {
            "Felslayer",
            "Lesser Felguard",
            "Ghostpaw Runner"
        };


        private void LoadPath(string pathName)
        {
            var stream = File.OpenRead(@"Ashenvale-Athalaxx.path");
            var reader = new StreamReader(stream);

            _path = new Dictionary<int,Location>();
            var idx = 1;
            while (!reader.EndOfStream)
            {
                var location = reader.ReadLine();
                var locationPoints = location.Split(',');
                _path.Add(idx,new Location(Convert.ToSingle(locationPoints[0]),Convert.ToSingle(locationPoints[1])));
                idx++;
            }
        }

        private void Start()
        {
            var nearestWaypoint = FindNearestWaypoint();
            _cm.SetPlayerFacing(nearestWaypoint);

            BotLoop(nearestWaypoint);
        }


        private void BotLoop(Location currentWaypoint)
        {
            var nextWaypoint = GetNextWaypoint(currentWaypoint);
            AreaScan();
            MoveToWaypoint(currentWaypoint);
            //Thread.Sleep(2000);
            
            BotLoop(nextWaypoint);
        }


        private Location GetNextWaypoint(Location currentWaypoint)
        {
            var index = _path.FirstOrDefault(w => Equals(w.Value, currentWaypoint));
            return _path.ContainsKey(index.Key + 1) ? _path[index.Key + 1] : _path[1];
        }

        private Location FindNearestWaypoint()
        {
            float nearest = int.MaxValue;
            var nearestWaypoint = new Location(0, 0);
            foreach (var location in _path)
            {
                var distance = location.Value.GetDistanceTo(ObjectManager.Me.Location);
                if (distance < nearest)
                {
                    nearest = distance;
                    nearestWaypoint = location.Value;
                    _logger.Information($"Found nearest waypoint {nearestWaypoint}");
                }
            }

            return nearestWaypoint;
        }

        private void MoveToWaypoint(Location location, int stopAt = 30, bool continuousMode = true)
        {

            _logger.Information($"Setting player facing {location}");
            var newFacing = _cm.GetFaceRadian(location, ObjectManager.Me.Location);

            if (Math.Abs(newFacing - ObjectManager.Me.Rotation) > 0.5)
                _cm.SetPlayerFacing(location);

            _kcd.SetPlayerFacing(newFacing);

            _logger.Information($"Sending W key");
            _kcd.SendKeyDown('W');


            var distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            while (distanceToWaypoint >= stopAt)
            {
                Thread.Sleep(100);
                distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);

                //if (!ObjectManager.Me.IsMoving())
                //{
                //    _kcd.SendKeyDown('W');
                //    MoveToWaypoint(location, stopAt, continuousMode);
                //}
            }

            _logger.Information($"Moved to {location}. Distance to waypoint is {distanceToWaypoint}. Current location is {ObjectManager.Me.Location} Waiting 100ms");

            if (!continuousMode)
                _kcd.SendKeyUp('W');
        }

        private void AreaScan()
        {
            var enemies = ObjectManager.Units.Where(u => 
                    _enemyWhiteList.Any(e => e.Contains(u.Name))
                    && u.Health.Percentage == 100
                    && u.Distance < 800)
                    .OrderBy(u => u.Distance).ToList();
            
            var nearest = enemies.FirstOrDefault();
            if (nearest == null) return;

            _logger.Information($"Nearest enemy is {nearest}");

            _cm.SetPlayerFacing(nearest.Location);
            _cm.SetTarget(nearest.Guid);
            _kcd.SendKey('G');
            _kcd.SendKey('1');
            if (nearest.Distance > 400)
            {
                _logger.Information($"Distance is over 400. Moving closer to target");
                MoveToWaypoint(nearest.Location, 400, false);
            }
            
            WaitFor(() => nearest.Target == ObjectManager.Me.Pet);
            _kcd.SendKey('2');

            while (nearest.Health.Current != 0)
            {
                _logger.Information($"Waiting for {nearest} to die. Current health is {nearest.Health.Percentage}");
                _kcd.SendKey('3');
                Thread.Sleep(1500);

                if (!nearest.HasAura(13550)) _kcd.SendKey('4');

                Thread.Sleep(4500);
            }

        }

        private void WaitFor(Func<bool> condition)
        {
            while (!condition.Invoke())
                Thread.Sleep(500);
        }
    }
}
