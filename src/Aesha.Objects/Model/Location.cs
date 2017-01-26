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
            float dx = X - l.X;
            float dy = Y - l.Y;
            float dz = Z - l.Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
