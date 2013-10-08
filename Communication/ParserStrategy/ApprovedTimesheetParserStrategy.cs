namespace Communication.ParserStrategy
{
    public class ApprovedTimesheetParserStrategy : TimesheetParserStrategyBase
    {
        public override Model.Timesheet ParseTimesheet(string timesheetHtml, out string viewstate)
        {
            throw new System.NotImplementedException();
        }

        protected override System.Collections.Generic.List<System.TimeSpan> GetRequiredHours(HtmlAgilityPack.HtmlNode htmlNode)
        {
            throw new System.NotImplementedException();
        }

        protected override Model.ProjectTaskTimesheetItem AddTimesheetItem(System.Collections.Generic.List<Model.ProjectTaskTimesheetItem> projectTimeItems, HtmlAgilityPack.HtmlNode projectCode, HtmlAgilityPack.HtmlNode taskCode)
        {
            throw new System.NotImplementedException();
        }

        protected override void PopulateTimesheetItem(HtmlAgilityPack.HtmlNode htmlNode, ref Model.ProjectTaskTimesheetItem timesheetItem, string index, int dayIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}
