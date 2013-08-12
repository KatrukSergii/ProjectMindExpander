using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Utility
{
    /// <summary>
    /// Produces a copy of an IList (uses Clone() where available)
    /// Copied from http://stackoverflow.com/questions/519461/cloning-listt
    /// usage: List<int> deepCopiedList = GenericCopier<List<int>>.DeepCopy(originalList);
    /// NB - Eventhandlers are not copied - must re-attach eventhandlers after the copy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class GenericCopier<T>
    {
        public static T DeepCopy(object objectToCopy)
        {
            // Get the object to return a copy instead of serializing/de-serializing
            if (typeof (ICloneable).IsAssignableFrom(typeof (T)))
            {
                return (T) ((ICloneable) objectToCopy).Clone();
            }
            
            if (typeof(IList<>).IsAssignableFrom(typeof(T)))
            {
                var newList = default(T);
                var genericTypeParameter = objectToCopy.GetType().GetGenericArguments()[0]; // assume only 1
                
                // Only continue if the items in the list are cloneable
                if (typeof (ICloneable).IsAssignableFrom(genericTypeParameter))
                {
                    foreach (var item in ((IEnumerable) objectToCopy))
                    {
                        var clone = ((ICloneable) item).Clone();
                        ((IList) newList).Add(clone);
                    }
                }
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
