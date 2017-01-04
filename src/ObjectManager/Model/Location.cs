namespace ObjectManager.Model
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
    }
}
