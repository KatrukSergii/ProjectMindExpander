
namespace Model
{
    public class PickListItem
    {
        public PickListItem(int value, string name)
        {
            Value = value;
            Name = name;
        }

        public int Value { get; set; }
        public string Name { get; set; }
    }
}
