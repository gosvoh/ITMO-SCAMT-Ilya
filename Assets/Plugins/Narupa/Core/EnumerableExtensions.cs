using System.Collections.Generic;

namespace Narupa.Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T First, T Second)> GetPairs<T>(this IEnumerable<T> enumerable)
        {
            var started = false;
            var prev = default(T);
            foreach (var e in enumerable)
            {
                if (!started)
                {
                    started = true;
                    prev = e;
                }
                else
                {
                    yield return (prev, e);
                    prev = e;
                }
            }
        }
    }
}