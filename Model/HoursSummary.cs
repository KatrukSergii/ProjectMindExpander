using System;
using System.Collections.Generic;


namespace Model
{
    /// <summary>
    /// Only really needed for the UI but added here so that the ObservableItem template generates the 
    /// necessary INotifyPropertyChanged code
    /// </summary>
    [Serializable]
    public class HoursSummary : IObservable
    {  
        public TimeSpan CoreHours { get; set; }
        public TimeSpan ExtraHours { get; set; }
        public TimeSpan LoggedHours { get; set; }
        public TimeSpan RemainingCoreHours { get; set; }
    }
}

