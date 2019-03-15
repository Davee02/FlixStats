using System;
using System.Collections.Generic;
using System.Linq;

namespace NetflixStatizier.Utilities
{
    public static class EnumerableUtilities
    {
        public static IEnumerable<T> GetNth<T>(this IEnumerable<T> list, int n)
        {
            if(n < 1)
                throw new ArgumentOutOfRangeException(nameof(n), "The n must not be smaller than 1");

            var enumerable = list.ToList();
            for (var i = 0; i < enumerable.Count; i += n)
                yield return enumerable.ElementAt(i);
        }
    }
}
