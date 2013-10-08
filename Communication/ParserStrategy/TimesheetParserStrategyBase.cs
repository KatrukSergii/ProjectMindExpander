
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Model;

namespace Communication.ParserStrategy
{
    public abstract class TimesheetParserStrategyBase : ITimesheetParserStrategy
    {
        protected const string LoginErrorSelector = "//div[@id='login-error']/h4";

        //TODO move implementation in here and called the abstract methods
        public abstract Model.Timesheet ParseTimesheet(string timesheetHtml, out string viewstate);
        protected abstract List<TimeSpan> GetRequiredHours(HtmlNode htmlNode);
        protected abstract ProjectTaskTimesheetItem AddTimesheetItem(List<ProjectTaskTimesheetItem> projectTimeItems, HtmlNode projectCode, HtmlNode taskCode);

        protected abstract void PopulateTimesheetItem(HtmlNode htmlNode,
                                                      ref ProjectTaskTimesheetItem timesheetItem,
                                                      string index,
                                                      int dayIndex);
    }
}
