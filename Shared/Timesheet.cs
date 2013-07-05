using System.Collections.Generic;

namespace Shared
{
    public class Timesheet
    {
        public Timesheet()
        {
            ProjectCodes = new List<PickListItem>();
        }

        public List<PickListItem> ProjectCodes { get; set; }
    }
}
