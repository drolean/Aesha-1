namespace Aesha.Domain
{
    public class Power
    {
        public Power(int current, int max)
        {
            Current = current;
            Max = max;
            Percentage = Current == 0 && Max == 0 ? 0 : (current/max)*100;
        }

        public int Current { get; }
        public int Max { get; }
        public int Percentage { get; }

        public override string ToString()
        {
            return Max == 0 ? "NotApplicable" : $"({Current}/{Max}) {Percentage}%";
        }
    }
}