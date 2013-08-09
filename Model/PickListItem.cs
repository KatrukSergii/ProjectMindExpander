
using System;
using System.Linq;

namespace Model
{
    [Serializable]
    public class PickListItem : IObservable
    {
        public PickListItem()
        {
        }

        public PickListItem(int value, string name)
        {
            Value = value;
            Name = name;
        }

        public int Value { get; set; }
        public string Name { get; set; }
    }
}
