using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            _logger = new LoggerConfiguration().WriteTo.File("log-.txt").CreateLogger();

            ObjectManager.Start(process);
            _process = process;
            _memReader = new ProcessMemoryReader(process);
            _cm = new CommandManager(process,_memReader);
            _kcd = new KeyboardCommandDispatcher(process);

            LoadPath("");

            Start();

            


        }


        private void LoadPath(string pathName)
        {
            var stream = File.OpenRead(@"2018-07-29 06-05-56.path");
            var reader = new StreamReader(stream);

            _path = new Dictionary<int,Location>();
            var idx = 1;
            while (!reader.EndOfStream)
            {
                var location = reader.ReadLine();
                var locationPoints = location.Split(',');
                _path.Add(idx,new Location(Convert.ToSingle(locationPoints[0]),Convert.ToSingle(locationPoints[1]),0));
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
            MoveToWaypoint(currentWaypoint);
            var nextWaypoint = GetNextWaypoint(currentWaypoint);




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
            var nearestWaypoint = new Location(0, 0, 0);
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

        private void MoveToWaypoint(Location location)
        {
            _logger.Information($"Setting player facing {location}");

            var newFacing = _cm.GetFaceRadian(location, ObjectManager.Me.Location);
            _kcd.SetPlayerFacing(newFacing);
            
            _logger.Information($"Sending W key");
            _kcd.SendKeyDown('W');

            var distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            while (distanceToWaypoint > 30)
            {
                _logger.Information($"Moving to {location}. Distance to waypoint is {distanceToWaypoint}. Current location is {ObjectManager.Me.Location} Waiting 100ms");
                AreaScan();
                Thread.Sleep(100);
                distanceToWaypoint = location.GetDistanceTo(ObjectManager.Me.Location);
            }
        }

        private void AreaScan()
        {
            
        }
    }
}
