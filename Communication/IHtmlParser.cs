
using Model;

namespace Communication
{
    public interface IHtmlParser
    {
        Timesheet ParseTimesheet(string timesheetHtml, out string viewState);
        string ParseViewState(string html);
    }
}
