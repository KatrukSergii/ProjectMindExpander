using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    public class DummyTestObject : IObservable
    {
        public DummyTestObject()
        {
            ProjectTimeItems = new List<ProjectTaskTimesheetItem>();
            NonProjectActivityItems = new List<ProjectTaskTimesheetItem>();
            RequiredHours = new List<TimeSpan>();
            DummyTimeEntry = new TimeEntry();
            DummyValueTypeCollection = new List<int>();
        }

        public string Title { get; set; }
        public string TimesheetId { get; set; }
        public List<ProjectTaskTimesheetItem> ProjectTimeItems { get; set; }
        public List<ProjectTaskTimesheetItem> NonProjectActivityItems { get; set; }
        public List<TimeSpan> RequiredHours { get; set; }
        public TimeSpan TotalRequiredHours { get; set; }
        public TimeEntry DummyTimeEntry { get; set; }
        public List<int> DummyValueTypeCollection { get; set; }
    }
}
