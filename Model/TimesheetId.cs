
namespace Model
{
    public class TimesheetId
    {
        public TimesheetId(string id, string dateString)
        {
            Id = id;
            DateString = dateString;
        }

        public string Id { get; set; }
        public string DateString { get; set; }
    }
}
