using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enum;

namespace Model
{
    public partial class ObservableTimesheet
    {
        /// <summary>
        /// Return only the changes in a <controlname,stringValue> format (for conversion into a querystring)
        /// NB at the moment if there are any changes for a particular project/task then the logged time for all days is shown
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ExtractChanges()
        {
            var changes = new Dictionary<string, string>();

            if (!IsChanged)
            {
                return changes;
            }

            // Rows
            // The ASP.NET control names for the rows start from 02
            for(int i=2; i < ProjectTimeItems.Count + 2; i++)
            {
                var item = ProjectTimeItems[i];
                if (item.IsChanged)
                {
                    // ProjectCode and TaskCode are read-only at the moment

                    // Columns
                    for (int j = 0; j < item.TimeEntries.Count; j++)
                    {
                        var timeEntry = item.TimeEntries[j];
                        if (timeEntry.IsChanged)
                        {
                            //timeEntry.LoggedTime     
                            if (timeEntry.LoggedTime != timeEntry.OriginalLoggedTime)
                            {
                                changes.Add(GetTimeControlName(i,j, TimeEntryFieldType.LoggedTime), timeEntry.LoggedTime.ToString());
                            }

                            //timeEntry.ExtraTime
                            if (timeEntry.ExtraTime != timeEntry.OriginalLoggedTime)
                            {
                                changes.Add(GetTimeControlName(i, j, TimeEntryFieldType.LoggedTime), timeEntry.LoggedTime.ToString());
                            }
                            //timeEntry.Notes
                            //timeEntry.WorkDetailId
                        }
                    }
                }
            }

            return changes;
        }

        private const string RowControlIdTemplate = "ctl00$C1$ProjectGrid$ctl{0}"; // parameter is padded to 2 characters (leading zero)
        private const string ColumnControlIdTemplate = "${0}{1}";
        private const string LoggedTimeControl = "txtLoggedTime";
        private const string ExtraTimeControl = "hdExtraTime";
        private const string NotesControl = "hdNotes";
        private const string WorkDetailControl = "hdWorkDetail";

        /// <summary>
        /// Convert row and columnIndex into a string that represents the ASP.NET control name
        /// </summary>
        /// <param name="rowIndex">Represents the Project+Task item</param>
        /// <param name="columnIndex">Represents the day of the week (Monday = 0)</param>
        /// <param name="timeType">One of: LoggedTime, ExtraTime, Notes, WorkDetailId</param>
        /// <returns></returns>
        private string GetTimeControlName(int rowIndex, int columnIndex, TimeEntryFieldType timeType)
        {
            var timeTypeName = string.Empty;

            switch (timeType)
            {
                 case TimeEntryFieldType.ExtraTime:
                    timeTypeName = ExtraTimeControl;
                    break;
                case TimeEntryFieldType.LoggedTime:
                    timeTypeName = LoggedTimeControl;
                    break;
                case TimeEntryFieldType.Notes:
                    timeTypeName = NotesControl;
                    break;
                case TimeEntryFieldType.WorkDetailId:
                    timeTypeName = WorkDetailControl;
                    break;
            }

            var row = rowIndex.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
            var controlName = string.Format(RowControlIdTemplate, row) + string.Format(ColumnControlIdTemplate, timeTypeName, columnIndex.ToString(CultureInfo.InvariantCulture));
            return controlName;
        }


        /// <summary>
        /// Clears the extra time property for all project+task items on a given day of the week
        /// </summary>
        /// <param name="dayOfWeekIndex"></param>
        /// <param name="list"></param>
        private void ClearExtraTime(int dayOfWeekIndex, IEnumerable<ObservableProjectTaskTimesheetItem> list)
        {
            foreach (var item in list)
            {
                item.TimeEntries[dayOfWeekIndex].ExtraTime = TimeSpan.Zero;

            }
        }

        /// <summary>
        /// Totals the logged time for all project+task items on a given day of the week
        /// </summary>
        /// <param name="dayOfWeekIndex"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private TimeSpan GetTotalLoggedTime(int dayOfWeekIndex, IEnumerable<ObservableProjectTaskTimesheetItem> list)
        {
            var totalTime = TimeSpan.Zero;

            foreach (var item in list)
            {
                totalTime = totalTime.Add(item.TimeEntries[dayOfWeekIndex].LoggedTime);
            }

            return totalTime;
        }

        /// <summary>
        /// Fix extra time (based on the required hours)
        /// </summary>
        public void FixHours()
        {
            // if total time across all project+tasks is greated than required hours 
            // then clear extra time for all items and add the difference to the extra hours of the first item

            // for each day of the week 
            for (int i = 0; i < 7; i++)
            {
                var totalProjectTime = GetTotalLoggedTime(i,ProjectTimeItems);
                var totalNonProjectTime = GetTotalLoggedTime(i, NonProjectActivityItems);
                var totalLoggedTime = totalNonProjectTime.Add(totalProjectTime);

                if (totalLoggedTime > RequiredHours[i])
                {
                    var extraTime = totalLoggedTime.Subtract(RequiredHours[i]);

                    // Reset extra time for all items (on this particular day)
                    ClearExtraTime(i,ProjectTimeItems);
                    ClearExtraTime(i, NonProjectActivityItems);
                    ProjectTimeItems.First().TimeEntries[i].ExtraTime = extraTime;
                    AcceptChanges();
                }
            }
        }
    }
}
