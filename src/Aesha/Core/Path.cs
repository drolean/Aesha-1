using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aesha.Domain;

namespace Aesha.Core
{
    public class Path
    {
        public readonly Dictionary<int, Location> Entries = new Dictionary<int, Location>();

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
                path.Entries.Add(idx, new Location(Convert.ToSingle(locationPoints[0]), Convert.ToSingle(locationPoints[1])));
                idx++;
            }

            return path; 
        }

        public int GetNextWaypointIndex(int currentWaypointIndex)
        {
            return Entries.ContainsKey(currentWaypointIndex + 2) ? currentWaypointIndex + 2 : 1;
        }

        public int FindNearestWaypointIndex(Location startPosition)
        {
            float nearest = int.MaxValue;
            var waypointIndex = 0;
            foreach (var location in Entries)
            {
                var distance = location.Value.GetDistanceTo(startPosition);
                if (distance < nearest)
                {
                    nearest = distance;
                    waypointIndex = location.Key;
                }
            }

           
            return waypointIndex;
        }

    }
}
