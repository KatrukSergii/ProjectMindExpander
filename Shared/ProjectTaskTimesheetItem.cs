
using System;

namespace Shared
{
    public class ProjectTaskTimesheetItem
    {
        public ProjectTaskTimesheetItem(PickListItem projectCode, PickListItem taskCode)
        {
            ProjectCode = projectCode;
            TaskCode = taskCode;
            Monday = new TimeEntry();
            Tuesday = new TimeEntry();
            Wednesday = new TimeEntry();
            Thursday = new TimeEntry();
            Friday = new TimeEntry();
            Saturday = new TimeEntry();
            Sunday = new TimeEntry();
        }

        public PickListItem ProjectCode { get; set; }
        public PickListItem TaskCode { get; set; }
        public TimeEntry Monday { get; set; }
        public TimeEntry Tuesday { get; set; }
        public TimeEntry Wednesday { get; set; }
        public TimeEntry Thursday { get; set; }
        public TimeEntry Friday { get; set; }
        public TimeEntry Saturday { get; set; }
        public TimeEntry Sunday { get; set; }

        public TimeEntry this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Monday;
                        break;
                    case 1:
                        return Tuesday;
                        break;
                    case 2:
                        return Wednesday;
                        break;
                    case 3:
                        return Thursday;
                        break;
                    case 4:
                        return Friday;
                        break;
                    case 5:
                        return Saturday;
                        break;
                    case 6:
                        return Sunday;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("This is no day of the week with index" + index.ToString());
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Monday = value;
                        break;
                    case 1:
                        Tuesday = value;
                        break;
                    case 2:
                        Wednesday = value;
                        break;
                    case 3:
                        Thursday = value;
                        break;
                    case 4:
                        Friday = value;
                        break;
                    case 5:
                        Saturday = value;
                        break;
                    case 6:
                        Sunday = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("This is no day of the week with index" + index.ToString());
                }
            }
        }
    }
}
