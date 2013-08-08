using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Utility
{
    /// <summary>
    /// Copied from http://stackoverflow.com/questions/519461/cloning-listt
    /// usage: List<int> deepCopiedList = GenericCopier<List<int>>.DeepCopy(originalList);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class GenericCopier<T>
    {
        public static T DeepCopy(object objectToCopy)
        {
            //using (var memoryStream = new MemoryStream())
            //{
                //var binaryFormatter = new BinaryFormatter();
                //binaryFormatter.Serialize(memoryStream, objectToCopy);
                //memoryStream.Seek(0, SeekOrigin.Begin);
                //return (T)binaryFormatter.Deserialize(memoryStream);
            //}

            // using DataContractSerializer because I had issues with the binaryFormatter not serializing (even though it's a bit slower)
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
