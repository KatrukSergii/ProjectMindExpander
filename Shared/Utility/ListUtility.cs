using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shared.Utility
{
    public static class ListUtility
    {
        /// <summary>
        /// Allows comparison of List<[ValueType]> or List<[IEquatable]>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="otherList"></param>
        /// <returns></returns>
        public static bool EqualTo<T>(IList<T> list, IList<T> otherList)
        {
            if (list.Count != otherList.Count)
            {
                return false;
            }

            Collection<string> x;

            var isValueType = typeof(T).IsValueType || typeof(T) == typeof(string);
            var isIComparable = typeof(IEquatable<T>).IsAssignableFrom(typeof(T));

            // assume each list is in same order
            for (var i = 0; i < otherList.Count; i++)
            {
                var thisItem = list[i];
                var otherItem = otherList[i];

                if (isValueType)
                {
                    if (!thisItem.Equals(otherItem))
                    {
                        return false;
                    }
                }
                else if (isIComparable)
                {
                    if (!((IEquatable<T>)thisItem).Equals(otherItem))
                    {
                        return false;
                    }
                }
                else
                {
                    throw new ArgumentException("Can only compare equality for lists of value types or IEquatabler<T> objects");
                }
            }

            return true;
        }


        public static void AttachPropertyChangedEventHandlers(IList list, PropertyChangedEventHandler handler,
                                                       bool attach = true)
        {
            foreach (var item in list)
            {
                var propertyChangedItem = item as INotifyPropertyChanged;
                if (propertyChangedItem != null)
                {
                    if (attach)
                    {
                        propertyChangedItem.PropertyChanged += handler;
                    }
                    else
                    {
                        propertyChangedItem.PropertyChanged -= handler;
                    }
                }
            }
        }
    }
}
