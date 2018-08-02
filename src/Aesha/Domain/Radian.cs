using System;
using System.Windows.Forms;
using Aesha.Core;

namespace Aesha.Domain
{
    public struct Radian
    {

        public Radian(float angle)
        {
            Angle = angle;
        }

        public float Angle { get; }

        public static implicit operator Radian(float angle) => new Radian(angle);
        public static implicit operator Radian(double angle) => new Radian((float)angle);

        public static Radian operator -(Radian a, Radian b)
        {
            return a.Angle - b.Angle;
        }

        public static Radian operator +(Radian a, Radian b)
        {
            return a.Angle + b.Angle;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case Radian radian:
                    return Math.Abs(radian.Angle - Angle) < 0.1;
                case float f:
                    return Math.Abs(f - Angle) < 0.1;
                case double d:
                    return Math.Abs(d - Angle) < 0.1;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Angle.GetHashCode();
        }

        public static Radian GetFaceRadian(Location destination, Location current)
        {
            var n = 270 - (Math.Atan2(current.Y - destination.Y, current.X - destination.X)) * 180 / Math.PI;
            var angle = (Math.PI / 180) * (n % 360);

            if (angle < 0f)
                angle = 2 * (float)Math.PI;

            return new Radian((float)angle);
        }

        public float AbsoluteDifference(float other)
        {
            return Math.Abs(this.Angle - other);
        }

        public TravelDirection GetDirectionOfTravel(Radian desiredAngle)
        {
            var diff = (Angle - desiredAngle).Angle;
            if (diff < 0f) diff += 2 * (float) Math.PI;


            return diff > Math.PI ? TravelDirection.Left : TravelDirection.Right;

        }

        public enum TravelDirection
        {
            Left,
            Right
        }

    }
}
