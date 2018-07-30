using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesha.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
        {
            var list = value.DefaultToEmptyIfNull();

            foreach (var item in list)
            {
                action(item);
            }
        }

        public static IEnumerable<T> DefaultToEmptyIfNull<T>(this IEnumerable<T> value)
        {
            return value ?? Enumerable.Empty<T>();
        }
    }
}
