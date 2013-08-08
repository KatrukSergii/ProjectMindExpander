using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shared.Extensions
{
    public static class ListUtilities<T>
    {
        /// <summary>
        /// Allows comparison of List<[ValueType]> or List<[IEqualityComparer]>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="otherList"></param>
        /// <returns></returns>
        public static bool EqualTo(List<T> list, List<T> otherList)
        {
            if (list.Count != otherList.Count)
            {
                return false;
            }

            Collection<string> x;

            var isValueType = typeof(T).IsValueType || typeof(T) == typeof(string);
            var isIComparable = typeof(IEqualityComparer<T>).IsAssignableFrom(typeof(T));

            // assume each list is in same order
            for (var i= 0; i < otherList.Count; i++)
            {
                var item = otherList[i];
                
                if (isValueType)
                {
                    if (!list[i].Equals(otherList[i]))
                    {
                        return false;
                    }
                }
                else if (isIComparable)
                {
                    if (!((IEqualityComparer<T>) list[i]).Equals(otherList[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    throw new ArgumentException("Can only compare equality for lists of value types or IEqualityComparer<T> objects");
                }
            }
            
            return true;
        }
    }
}
