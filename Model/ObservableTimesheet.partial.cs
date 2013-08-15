using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Shared.Enum;

namespace Model
{
    public partial class ObservableTimesheet
    {
        partial void Initialize()
        {
            PropertyChanged += ObservableTimesheet_PropertyChanged;
        }

        private void ObservableTimesheet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // TODO may need to look into the performance of this 
            if (e.PropertyName == "IsChanged")
            {
                CalculateTotals();
            }
        }

        /// <summary>
        /// Return only the changes in a <controlname,stringValue> format (for conversion into a querystring)
        /// NB at the moment if there are any changes for a particular project/task then the logged time for all days is shown
        /// </summary>
        /// <returns>Changes to time entries only in ProjectTask->Day of Week then NonProjectTask->Day of week order </returns>
        public Dictionary<string, string> ExtractChanges()
        {
            var changes = new Dictionary<string, string>();

            if (!IsChanged)
            {
                return changes;
            }

            ExtractChangesInternal(ProjectTimeItems, changes);
            ExtractChangesInternal(NonProjectActivityItems, changes, isNonProjectTime: true);

            return changes;
        }

        /// <summary>
        /// Extract changes from a list of timesheet items (i.e. Project or Non-project)
        /// </summary>
        /// <param name="list">Either Project timesheet items or Non-Project activities</param>
        /// <param name="changes">Dictionary to add changes to</param>
        private void ExtractChangesInternal(IList<ObservableProjectTaskTimesheetItem> list, Dictionary<string,string> changes, bool isNonProjectTime = false)
        {
            // Rows
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item.IsChanged)
                {
                    // ProjectCode and TaskCode are read-only at the moment (as are the other properties on the timesheet)

                    // Columns
                    for (int j = 0; j < item.TimeEntries.Count; j++)
                    {
                        var timeEntry = item.TimeEntries[j];
                        if (timeEntry.IsChanged)
                        {
                            
                            if (timeEntry.LoggedTime != timeEntry.OriginalLoggedTime)
                            {
                                changes.Add(GetTimeControlName(i, j, TimeEntryFieldType.LoggedTime, isNonProjectTime), timeEntry.LoggedTime.ToString());
                            }

                            if (timeEntry.ExtraTime != timeEntry.OriginalLoggedTime)
                            {
                                changes.Add(GetTimeControlName(i, j, TimeEntryFieldType.ExtraTime, isNonProjectTime), timeEntry.ExtraTime.ToString());
                            }

                            if (timeEntry.Notes != timeEntry.OriginalNotes)
                            {
                                changes.Add(GetTimeControlName(i, j, TimeEntryFieldType.Notes, isNonProjectTime), timeEntry.Notes);
                            }

                            if (timeEntry.WorkDetailId != timeEntry.OriginalWorkDetailId)
                            {
                                changes.Add(GetTimeControlName(i, j, TimeEntryFieldType.WorkDetailId, isNonProjectTime), timeEntry.WorkDetailId.ToString());
                            }
                        }
                    }
                }
            }
        }


        private const string RowControlIdTemplate = "ctl00$C1$ProjectGrid$ctl{0}"; // parameter is padded to 2 characters (leading zero)
        private const string NonProjectRowControlIdTemplate = "ctl00$C1$InternalProjectGrid$ctl{0}"; // parameter is padded to 2 characters (leading zero)
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
        /// <param name="isNonProjectTime"></param>
        /// <returns></returns>
        private string GetTimeControlName(int rowIndex, int columnIndex, TimeEntryFieldType timeType, bool isNonProjectTime = false)
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
            
            // The ASP.NET control names for the rows start from 02 so we add 2 to the index and pad left
            var row = (rowIndex + 2).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
            var rowControl = isNonProjectTime ? NonProjectRowControlIdTemplate : RowControlIdTemplate;
            var controlName = string.Format(rowControl, row) + string.Format(ColumnControlIdTemplate, timeTypeName, columnIndex.ToString(CultureInfo.InvariantCulture));
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
                }
            }
        }

        /// <summary>
        /// Create totals for each day
        /// </summary>
        public void CalculateTotals()
        {
            var totalExtraHours = TimeSpan.Zero;
            var totalLoggedHours = TimeSpan.Zero;
            var totalRemainingCoreHours = TimeSpan.Zero;
            var totalCoreHours = TimeSpan.Zero;

            for (int i = 0; i < 7; i++)
            {
               var loggedHours = TimeSpan.Zero;
               var extraHours = TimeSpan.Zero;

                foreach (var item in ProjectTimeItems)
                {
                    loggedHours = loggedHours.Add(item.TimeEntries[i].LoggedTime);
                    extraHours = extraHours.Add(item.TimeEntries[i].ExtraTime);
                }

                foreach (var item in NonProjectActivityItems)
                {
                    loggedHours = loggedHours.Add(item.TimeEntries[i].LoggedTime);
                    extraHours = extraHours.Add(item.TimeEntries[i].ExtraTime);
                }

                var remainingCore = RequiredHours[i].Subtract(loggedHours.Add(extraHours));

                var totalsForDay = Totals[i];
                totalsForDay.LoggedHours = loggedHours;
                totalsForDay.ExtraHours = extraHours;
                totalsForDay.CoreHours = RequiredHours[i];
                totalsForDay.RemainingCoreHours = remainingCore;
                totalsForDay.AcceptChanges();

                totalLoggedHours = totalLoggedHours.Add(loggedHours);
                totalExtraHours = totalExtraHours.Add(extraHours);
                totalRemainingCoreHours = totalRemainingCoreHours.Add(remainingCore);
                totalCoreHours = totalCoreHours.Add(RequiredHours[i]);
            }


            var weekSummary = Totals[7];
            weekSummary.LoggedHours = totalLoggedHours;
            weekSummary.ExtraHours = totalExtraHours;
            weekSummary.CoreHours = totalCoreHours;
            weekSummary.RemainingCoreHours = totalRemainingCoreHours;
            weekSummary.AcceptChanges();
        }
    }
}
