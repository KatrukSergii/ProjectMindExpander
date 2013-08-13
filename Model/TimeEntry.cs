using System;

namespace Model
{
    [Serializable]
    public class TimeEntry : IObservable
    {
        public TimeEntry()
        {
            LoggedTime = TimeSpan.Zero;
            ExtraTime = TimeSpan.Zero;
            Notes = string.Empty;
        }

        public TimeSpan LoggedTime { get; set; }
        public TimeSpan ExtraTime { get; set; }
        public string Notes { get; set; }
        public int WorkDetailId { get; set; }
    }
}
