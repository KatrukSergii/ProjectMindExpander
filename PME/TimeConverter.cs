using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Shared;

namespace PME
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Timespan to string
            var displayString = ((TimeSpan) value).ToShortString();
            return displayString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // string to Timespan
            var timeSpan = TimeSpan.Zero;

            if (value != null)
            {
                TimeSpan.TryParse(value.ToString(), out timeSpan);
            }

            return timeSpan;
        }
    }
}
