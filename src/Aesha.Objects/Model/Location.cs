using System;

namespace Aesha.Objects.Model
{
    public class Location
    {
        public Location(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    

        public float X { get;}
        public float Y { get; }
        public float Z { get; }

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

            return X == other.X && Y == other.Y && Z == other.Z;

        }
    }
}
