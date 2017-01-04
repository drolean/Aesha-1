namespace ObjectManager.Model
{
    public class Health
    {
        public Health(int current, int max)
        {
            Current = current;
            Max = max;
            Percentage = (current/max)*100;
        }

        public int Current { get; }
        public int Max { get; }
        public int Percentage { get; }
    }
}
