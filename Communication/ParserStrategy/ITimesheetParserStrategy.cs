
using Model;

namespace Communication.ParserStrategy
{
    public interface ITimesheetParserStrategy
    {
        Timesheet ParseTimesheet(string timesheetHtml, out string viewstate);
    }
}
