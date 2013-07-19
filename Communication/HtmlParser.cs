using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HtmlAgilityPack;
using Shared;
using Shared.Interfaces;

namespace Communication
{
    public class HtmlParser : IHtmlParser
    {
        private const string ViewstateSelector = "//input[@name = '__VIEWSTATE']";

        /// <summary>
        /// Return Timesheet object based upon the contents of a HTML string representing the timesheet webpage.
        /// Return the viewstate as an out parameter for further operations
        /// </summary>
        /// <param name="timesheetHtml"></param>
        /// <param name="viewstate"></param>
        /// <returns></returns>
        public Timesheet ParseTimesheet(string timesheetHtml, out string viewstate)
        {
            const string projectSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProject']/option[@selected='selected']";
            const string projectTaskSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProjectTask']/option[@selected='selected']";
            // TODO need Extra time, Notes, Work Details Id
            const string loggedTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$txtLoggedTime{1}']";

            const string nonProjectSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProject']/option[@selected='selected']";
            const string nonProjectTaskSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProjectTask']/option[@selected='selected']";
            
            // TODO need Extra time, Notes, Work Details Id
            const string nonProjectLoggedTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$txtLoggedTime{1}']";

            const string requiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequired{0}']";
            const string totalRequiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequiredTotal']";

            const string titleSelector = "//div[@class='subtitle']";
            const string loginErrorSelector = "//div[@id='login-error']/h4";

            const string timesheetIdSelector = "//input[@name = 'ctl00$C1$hdnTimesheetId}']";

            var timesheet = new Timesheet();
            viewstate = string.Empty;

            HtmlNode.ElementsFlags.Remove("option");
            var htmlDoc = new HtmlDocument();
            var stringReader = new StringReader(timesheetHtml);

            htmlDoc.Load(stringReader);
            
            if (htmlDoc.DocumentNode != null)
            {
                AssertNoError(htmlDoc.DocumentNode, loginErrorSelector);

                // Timesheet title, e.g. '08 Jul 2013 to 14 Jul 2013 by Joe Bloggs (Draft)'
                timesheet.Title = htmlDoc.DocumentNode.SelectSingleNode(titleSelector).InnerText.Trim();

                timesheet.TimesheetId =
                    htmlDoc.DocumentNode.SelectSingleNode(timesheetIdSelector).Attributes["value"].Value;

                timesheet.ProjectTimeItems = GetTimesheetItems(htmlDoc.DocumentNode,projectSelector,projectTaskSelector,loggedTimeSelector);
                timesheet.NonProjectActivityItems = GetTimesheetItems(htmlDoc.DocumentNode, nonProjectSelector, nonProjectTaskSelector, nonProjectLoggedTimeSelector);

                timesheet.RequiredHours = GetRequiredHours(htmlDoc.DocumentNode, requiredHoursSelector);
                timesheet.TotalRequiredHours = htmlDoc.DocumentNode.SelectSingleNode(totalRequiredHoursSelector).Attributes["value"].Value.ToTimeSpan();
                
                viewstate = htmlDoc.DocumentNode.SelectSingleNode(ViewstateSelector).Attributes["value"].Value;
            }

            return timesheet;
        }

        public string ParseViewState(string html)
        {
            var htmlDoc = new HtmlDocument();
            var stringReader = new StringReader(html);
            htmlDoc.Load(stringReader);
            string viewstate = string.Empty;

            if (htmlDoc.DocumentNode != null)
            {
                viewstate = htmlDoc.DocumentNode.SelectSingleNode(ViewstateSelector).Attributes["value"].Value;
            }

            return viewstate;
        }

        /// <summary>
        /// Throws invalidOperationException if there is an error
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="errorSelector"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private void AssertNoError(HtmlNode htmlNode, string errorSelector)
        {
            var errorNode = htmlNode.SelectSingleNode(errorSelector);
            if (errorNode != null && !string.IsNullOrEmpty(errorNode.InnerText))
            {
                throw new InvalidOperationException(errorNode.InnerText);
            }
        }

        private List<TimeSpan> GetRequiredHours(HtmlNode htmlNode, string requiredHoursSelector)
        {
            var requiredHours = new List<TimeSpan>();

            for (int i = 0; i < 7; i++)
            {
                var requiredHoursNode = htmlNode.SelectSingleNode(string.Format(requiredHoursSelector, i.ToString()));
                var parsedTime = requiredHoursNode.Attributes["value"].Value == "0"
                                     ? TimeSpan.MinValue
                                     : TimeSpan.Parse(requiredHoursNode.Attributes["value"].Value);
                requiredHours.Add(parsedTime);
            }

            return requiredHours;
        }

        private List<ProjectTaskTimesheetItem> GetTimesheetItems(HtmlNode htmlNode, string projectSelector, string projectTaskSelector, string loggedTimeSelector)
        {
            var projectTimeItems = new List<ProjectTaskTimesheetItem>();
            
            HtmlNode projectCode = null;

            var i = 2;
            
            while (projectCode != null || i == 2)
            {
                var index = i.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                
                var projectControlName = string.Format(projectSelector, index);
                projectCode = htmlNode.SelectSingleNode(projectControlName);
                
                if (projectCode != null)
                {
                    // assume that there is a task combo control for each project combo control;
                    HtmlNode taskCode = htmlNode.SelectSingleNode(string.Format(projectTaskSelector, index));
                    var timesheetItem = AddTimesheetItem(projectTimeItems, projectCode, taskCode);

                    for (int j = 0; j < 7; j++)
                    {
                        var timeValue = htmlNode.SelectSingleNode(string.Format(loggedTimeSelector, index, j.ToString(CultureInfo.InvariantCulture)));
                        if (timeValue.Attributes["Value"] != null)
                        {
                            timesheetItem[j].LoggedTime = TimeSpan.Parse(timeValue.Attributes["value"].Value);
                        }
                    }

                    i++;
                }
            }

            return projectTimeItems;
        }

        /// <summary>
        /// Construct a ProjectTaskTimesheetItem from Html fragment and add to the list of projectTimeItems
        /// </summary>
        /// <param name="projectTimeItems"></param>
        /// <param name="projectCode"></param>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        private ProjectTaskTimesheetItem AddTimesheetItem(List<ProjectTaskTimesheetItem> projectTimeItems, HtmlNode projectCode, HtmlNode taskCode )
        {
            var projectCodePickListItem = GetPickListItemFromCombobox(projectCode);
            var projectTaskCodePickListItem = GetPickListItemFromCombobox(taskCode);
            var timesheetItem = new ProjectTaskTimesheetItem(projectCodePickListItem, projectTaskCodePickListItem);
            projectTimeItems.Add(timesheetItem);

            return timesheetItem;
        }

        private PickListItem GetPickListItemFromCombobox(HtmlNode control)
        {
            return new PickListItem(int.Parse(control.Attributes["value"].Value), control.InnerText);
        }

        //    // Substring is in place of 'ends-with' (needed because some of the dropdown names end with ddlProjectTask)
        //    foreach (HtmlNode link in htmlDocumentNode.SelectNodes("//select['ddlProject' = substring(@name,string-length(@name) - string-length('ddlProject') +1)]/option[@selected='selected']"))
       

    }
}
