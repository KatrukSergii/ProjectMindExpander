
using System.Collections.Generic;
using Model;

namespace Communication
{
    public interface IHtmlParser
    {
        Timesheet ParseTimesheet(string timesheetHtml, out string viewState);

        Timesheet ParseApprovedTimesheet(string timesheetHtml, out string viewState);

        TimesheetId GetLatestTimesheetId(string timesheetHistoryHtml);

        string ParseViewState(string html);
    }
}
