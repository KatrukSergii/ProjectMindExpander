using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using HtmlAgilityPack;
using Model;
using Shared;
using Shared.Enum;

namespace Communication
{
    public class HtmlParser : IHtmlParser
    {
        private const string ViewstateSelector = "//input[@name = '__VIEWSTATE']";

        public string GetControlName(ProjectTimeType projectTimeType, int projectTaskIndex, int dayIndex,
                                     TimeEntryFieldType timeEntryType)
        {
           throw new NotImplementedException();
        }

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
            
            const string loggedTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$txtLoggedTime{1}']";
            const string extraTimeSelector =  "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdExtraTime{1}']";
            const string notesSelector =  "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdNotes0{1}']";
            const string workDetailSelector =  "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdWorkDetailId0{1}']";

            const string nonProjectSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProject']/option[@selected='selected']";
            const string nonProjectTaskSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProjectTask']/option[@selected='selected']";
            
            const string nonProjectLoggedTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$txtLoggedTime{1}']";
            const string nonProjectExtraTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdExtraTime{1}']";
            const string nonProjectNotesSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdNotes{1}']";
            const string nonProjectWorkDetailSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdWorkDetailId{1}']";

            const string requiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequired{0}']";
            const string totalRequiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequiredTotal']";

            const string titleSelector = "//div[@class='subtitle']";
            const string loginErrorSelector = "//div[@id='login-error']/h4";

            const string timesheetIdSelector = "//input[@name = 'ctl00$C1$hdnTimesheetId']";

            var timesheet = new Timesheet();
            viewstate = string.Empty;

            HtmlNode.ElementsFlags.Remove("option");
            var htmlDoc = new HtmlDocument();
            var stringReader = new StringReader(timesheetHtml);

            htmlDoc.Load(stringReader);
            
            if (htmlDoc.DocumentNode != null)
            {
                AssertNoErrors(htmlDoc.DocumentNode, loginErrorSelector);

                // Timesheet title, e.g. '08 Jul 2013 to 14 Jul 2013 by Joe Bloggs (Draft)'
                timesheet.Title = htmlDoc.DocumentNode.SelectSingleNode(titleSelector).InnerText.Trim();

                timesheet.TimesheetId =
                    htmlDoc.DocumentNode.SelectSingleNode(timesheetIdSelector).Attributes["value"].Value;

                timesheet.ProjectTimeItems = GetTimesheetItems(htmlDoc.DocumentNode,projectSelector,projectTaskSelector,loggedTimeSelector,extraTimeSelector,notesSelector,workDetailSelector);
                timesheet.NonProjectActivityItems = GetTimesheetItems(htmlDoc.DocumentNode, nonProjectSelector, nonProjectTaskSelector, nonProjectLoggedTimeSelector, nonProjectExtraTimeSelector, nonProjectNotesSelector, nonProjectWorkDetailSelector );

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
                viewstate = HttpUtility.UrlEncodeUnicode(htmlDoc.DocumentNode.SelectSingleNode(ViewstateSelector).Attributes["value"].Value); 
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
        private void AssertNoErrors(HtmlNode htmlNode, string errorSelector)
        {
            var errorNode = htmlNode.SelectSingleNode(errorSelector);
            if (errorNode != null && !string.IsNullOrEmpty(errorNode.InnerText))
            {
                throw new InvalidOperationException(errorNode.InnerText);
            }
        }

        /// <summary>
        /// The required hours per day is a hidden field (should  be 7:30)
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="requiredHoursSelector"></param>
        /// <returns></returns>
        private List<TimeSpan> GetRequiredHours(HtmlNode htmlNode, string requiredHoursSelector)
        {
            var requiredHours = new List<TimeSpan>();

            for (int i = 0; i < 7; i++)
            {
                var requiredHoursNode = htmlNode.SelectSingleNode(string.Format(requiredHoursSelector, i.ToString()));
                var parsedTime = requiredHoursNode.Attributes["value"].Value == "0"
                                     ? TimeSpan.Zero
                                     : TimeSpan.Parse(requiredHoursNode.Attributes["value"].Value);
                requiredHours.Add(parsedTime);
            }

            return requiredHours;
        }

        private List<ProjectTaskTimesheetItem> GetTimesheetItems(HtmlNode htmlNode, string projectSelector, 
            string projectTaskSelector, 
            string loggedTimeSelector, 
            string extraTimeSelector, 
            string notesSelector, 
            string workDetailSelector)
        {
            var projectTimeItems = new List<ProjectTaskTimesheetItem>();
            
            HtmlNode projectCode = null;

            // index starts at 2 because of the naming of the ASP.net controls - i.e. first is ctl00$C1$ProjectGrid$ctl2
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

                    // always 7 days a week
                    for (int j = 0; j < 7; j++)
                    {
                        var timeValue = htmlNode.SelectSingleNode(string.Format(loggedTimeSelector, index, j.ToString(CultureInfo.InvariantCulture)));
                        var extraTimeValue = htmlNode.SelectSingleNode(string.Format(extraTimeSelector, index, j.ToString(CultureInfo.InvariantCulture)));
                        var notesValue = htmlNode.SelectSingleNode(string.Format(notesSelector, index, j.ToString(CultureInfo.InvariantCulture)));
                        var workDetailValue = htmlNode.SelectSingleNode(string.Format(workDetailSelector, index, j.ToString(CultureInfo.InvariantCulture)));

                        if (timeValue != null && timeValue.Attributes["value"] != null)
                        {
                            timesheetItem.TimeEntries[j].LoggedTime = TimeSpan.Parse(timeValue.Attributes["value"].Value);
                        }

                        if (extraTimeValue != null && extraTimeValue.Attributes["value"] != null)
                        {
                            timesheetItem.TimeEntries[j].ExtraTime =
                                TimeSpan.Parse(extraTimeValue.Attributes["value"].Value);
                        }

                        if (notesValue != null && notesValue.Attributes["value"] != null)
                        {
                            timesheetItem.TimeEntries[j].Notes = notesValue.Attributes["value"].Value;
                        }

                        if (workDetailValue != null && workDetailValue.Attributes["value"] != null)
                        {
                            timesheetItem.TimeEntries[j].WorkDetailId =
                                int.Parse(workDetailValue.Attributes["value"].Value);
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
