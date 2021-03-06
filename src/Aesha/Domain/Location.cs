﻿using System;

namespace Aesha.Domain
{
    public class Location
    {
        public Location(float x, float y)
        {
            X = x;
            Y = y;
        }
    

        public float X { get;}
        public float Y { get; }
        
        public override string ToString()
        {
            return $"{X:##.000}/{Y:##.000}";
        }

        public float GetDistanceTo(Location l)
        {
            var result = (Math.Pow(X - l.X, 2) + Math.Pow(Y - l.Y, 2));
            return (float) result;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Location)) return false;

            var other = (Location) obj;

            return X == other.X && Y == other.Y;

        }
    }
}
