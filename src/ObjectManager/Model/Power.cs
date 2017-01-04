namespace ObjectManager.Model
{
    public class Power
    {
        public Power(int current, int max)
        {
            Current = current;
            Max = max;
            Percentage = (current / max) * 100;
        }

        public int Current { get; }
        public int Max { get; }
        public int Percentage { get; }
    }
}