
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Model
{
    public class ProjectTaskTimesheetItem : IObservable
    {
        public ProjectTaskTimesheetItem(PickListItem projectCode, PickListItem taskCode)
        {
            ProjectCode = projectCode;
            TaskCode = taskCode;
            TimeEntries = new List<TimeEntry>(7);
            
            for (int i = 0; i < 7; i++)
            {
                TimeEntries.Add(new TimeEntry());    
            }

            PropertyInfo p;
        }

        public PickListItem ProjectCode { get; set; }
        public PickListItem TaskCode { get; set; }
        
        /// <summary>
        /// TimeEntry for each day of the week (Monday = TimeEntries[0], Sunday = TimeEntries[6])
        /// </summary>
        public List<TimeEntry> TimeEntries { get; set; }
    }
}
