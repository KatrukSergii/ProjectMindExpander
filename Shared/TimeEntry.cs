using System;

namespace Shared
{
    public class TimeEntry
    {
        public TimeSpan? LoggedTime { get; set; }
        public TimeSpan? ExtraTime { get; set; }
        public string Notes { get; set; }
        public int? WorkDetailId { get; set; }
    }
}
