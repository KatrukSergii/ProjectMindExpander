using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Utility
{
    public static class IEnumerableExtensions
    {
        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            int index = 0;
            foreach (var item in items)
            {
                if (predicate(item)) break;
                index++;
            }
            return index;
        }
    }
}
