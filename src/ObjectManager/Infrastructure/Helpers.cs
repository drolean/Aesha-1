using System;
using System.Linq;

namespace ObjectManager.Infrastructure
{
    public static class Helpers
    {
        public static int FindPattern(this byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    return i;
                }
            }

            return 0;
        }

        public static int FindPattern(this byte[] source, uint value)
        {
            var pattern = BitConverter.GetBytes(value);
            return FindPattern(source, pattern);
        }
    }
}
