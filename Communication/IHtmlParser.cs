
using Model;

namespace Communication
{
    public interface IHtmlParser
    {
        Timesheet ParseTimesheet(string timesheetHtml, out string viewState, bool isApprovedTimesheet = false);

        TimesheetId GetLatestTimesheetId(string timesheetHistoryHtml);

        string ParseViewState(string html);
    }
}
