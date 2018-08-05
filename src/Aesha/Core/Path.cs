using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aesha.Domain;

namespace Aesha.Core
{
    public class Path
    {
        private readonly Dictionary<int, Location> _entries = new Dictionary<int, Location>();

        public static Path FromFile(string filename)
        {
            var stream = File.OpenRead($"paths\\{filename}");
            var reader = new StreamReader(stream);

            var path = new Path();
            var idx = 1;
            while (!reader.EndOfStream)
            {
                var location = reader.ReadLine();
                var locationPoints = location.Split(',');
                path._entries.Add(idx, new Location(Convert.ToSingle(locationPoints[0]), Convert.ToSingle(locationPoints[1])));
                idx++;
            }

            return path;
        }

        public Location GetNextWaypoint(Location currentWaypoint)
        {
            var index = _entries.FirstOrDefault(w => Equals(w.Value, currentWaypoint));
            var nextWaypoint =  _entries.ContainsKey(index.Key + 1) ? _entries[index.Key + 1] : _entries[1];
            return nextWaypoint;
        }

        public Location FindNearestWaypoint(Location startPosition)
        {
            float nearest = int.MaxValue;
            var nearestWaypoint = new Location(0, 0);
            foreach (var location in _entries)
            {
                var distance = location.Value.GetDistanceTo(startPosition);
                if (distance < nearest)
                {
                    nearest = distance;
                    nearestWaypoint = location.Value;
                }
            }

            return nearestWaypoint;
        }

    }
}
