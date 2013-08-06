﻿using System;

namespace Model
{
    public class TimeEntry : IObservable
    {
        public TimeSpan? LoggedTime { get; set; }
        public TimeSpan? ExtraTime { get; set; }
        public string Notes { get; set; }
        public int? WorkDetailId { get; set; }
    }
}
