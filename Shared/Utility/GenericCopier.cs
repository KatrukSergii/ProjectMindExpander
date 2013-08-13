using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Shared.Interfaces;

namespace Shared.Utility
{
    /// <summary>
    /// Produces a copy of an IList (uses Clone() where available)
    /// Copied from http://stackoverflow.com/questions/519461/cloning-listt
    /// usage: List<int> deepCopiedList = GenericCopier<List<int>>.DeepCopy(originalList);
    /// NB - Eventhandlers are not copied - must re-attach eventhandlers after the copy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class GenericCopier<T> where T : new()
    {
        public static T DeepCopy(object objectToCopy)
        {
            // Get the object to return a copy instead of serializing/de-serializing
            if (typeof (ICloneable).IsAssignableFrom(typeof (T)))
            {
                var clone =  (T) ((ICloneable) objectToCopy).Clone();
                return clone;
            }
            
            if (TypeUtility.IsAssignableToGenericType(typeof(T),typeof(IList<>)))
            {
                var newList = new T();
                var genericTypeParameter = objectToCopy.GetType().GetGenericArguments()[0]; // assume only 1
                
                if (typeof (ICloneable).IsAssignableFrom(genericTypeParameter))
                {
                    foreach (var item in ((IEnumerable) objectToCopy))
                    {
                        var clone = ((ICloneable) item).Clone();
                        ((IList) newList).Add(clone);
                    }
                }
                else
                {
                    // Value type e.g. List<int>
                    foreach (var item in ((IEnumerable)objectToCopy))
                    {
                        ((IList)newList).Add(item);
                    }
                }

                return newList;
            }

            // using DataContractSerializer because I there were issues with the binaryFormatter not serializing (even though DCS is a bit slower)
            T copy;
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, objectToCopy);
                stream.Position = 0;
                copy = (T)serializer.ReadObject(stream);
            }

            return copy;
        }

        public static List<T> DeepCopyList<T2>(List<T2> originalList ) where T2 : T
        {
            var newList = new List<T>(originalList.Count);

            if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
            {
                originalList.ForEach(x => newList.Add((T)((ICloneable)x).Clone()));
            }
            else
            {
                originalList.ForEach(x => newList.Add(DeepCopy(x)));    
            }

            return newList;
        }
    }
}
