using System;

namespace Shared
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Parses a string into a timespan (allows for values greater than 24 hours)
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string inputString)
        {
            var timeSpan = new TimeSpan(int.Parse(inputString.Split(':')[0]),    // hours
                            int.Parse(inputString.Split(':')[1]),    // minutes
                            0);
            return timeSpan;
        }
    }
}
