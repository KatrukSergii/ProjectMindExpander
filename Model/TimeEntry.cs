using System;

namespace Model
{
    public class TimeEntry : IObservable
    {
        public TimeEntry()
        {
            LoggedTime = null;
            ExtraTime = null;
            Notes = string.Empty;
            WorkDetailId = null;
        }

        public TimeSpan? LoggedTime { get; set; }
        public TimeSpan? ExtraTime { get; set; }
        public string Notes { get; set; }
        public int? WorkDetailId { get; set; }
    }
}
