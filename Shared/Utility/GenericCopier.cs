using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
