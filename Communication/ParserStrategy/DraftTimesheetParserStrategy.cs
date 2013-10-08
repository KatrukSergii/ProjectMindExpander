
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HtmlAgilityPack;
using Model;
using Shared;

namespace Communication.ParserStrategy
{
    public class DraftTimesheetParserStrategy : TimesheetParserStrategyBase
    {
        protected const string ProjectSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProject']/option[@selected='selected']";
        protected const string ProjectTaskSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProjectTask']/option[@selected='selected']";
         
        protected const string LoggedTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$txtLoggedTime{1}']";
        protected const string ExtraTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdExtraTime{1}']";
        protected const string NotesSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdNotes0{1}']";
        protected const string WorkDetailSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdWorkDetailId0{1}']";
         
        protected const string NonProjectSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProject']/option[@selected='selected']";
        protected const string NonProjectTaskSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProjectTask']/option[@selected='selected']";
         
        protected const string NonProjectLoggedTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$txtLoggedTime{1}']";
        protected const string NonProjectExtraTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdExtraTime{1}']";
        protected const string NonProjectNotesSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdNotes{1}']";
        protected const string NonProjectWorkDetailSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdWorkDetailId{1}']";
         
        protected const string RequiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequired{0}']";
        protected const string TotalRequiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequiredTotal']";
         
        protected const string TitleSelector = "//div[@class='subtitle']";
        protected const string TimesheetIdSelector = "//input[@name = 'ctl00$C1$hdnTimesheetId']";

        /// <summary>
        /// Return Timesheet object based upon the contents of a HTML string representing the timesheet webpage.
        /// Return the viewstate as an out parameter for further operations
        /// </summary>
        /// <param name="timesheetHtml"></param>
        /// <param name="viewstate"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Invalid timesheet ID throws ApplicationException</exception>
        public override Timesheet ParseTimesheet(string timesheetHtml, out string viewstate)
        {

            var timesheet = new Timesheet();
            viewstate = string.Empty;

            HtmlNode.ElementsFlags.Remove("option");
            var htmlDoc = new HtmlDocument();
            var stringReader = new StringReader(timesheetHtml);

            htmlDoc.Load(stringReader);

            if (htmlDoc.DocumentNode != null)
            {
                HtmlParser.AssertNoErrors(htmlDoc.DocumentNode, LoginErrorSelector);

                try
                {
                    // Timesheet title, e.g. '08 Jul 2013 to 14 Jul 2013 by Joe Bloggs (Draft)'
                    timesheet.Title = htmlDoc.DocumentNode.SelectSingleNode(TitleSelector).InnerText.Trim();

                    timesheet.TimesheetId =
                        htmlDoc.DocumentNode.SelectSingleNode(TimesheetIdSelector).Attributes["value"].Value;

                    if (timesheet.TimesheetId.Equals("0"))
                    {
                        throw new ApplicationException("The Timesheet has an invalid ID");
                    }

                    timesheet.ProjectTimeItems = GetTimesheetItems(htmlDoc.DocumentNode);

                    timesheet.NonProjectActivityItems = GetTimesheetItems(htmlDoc.DocumentNode);

                    timesheet.RequiredHours = GetRequiredHours(htmlDoc.DocumentNode);
                    timesheet.TotalRequiredHours =
                        htmlDoc.DocumentNode.SelectSingleNode(TotalRequiredHoursSelector).Attributes["value"].Value
                                                                                                             .ToTimeSpan
                            ();

                    viewstate = htmlDoc.DocumentNode.SelectSingleNode(HtmlParser.ViewstateSelector).Attributes["value"].Value;

                }
                catch (NullReferenceException ex)
                {
                    // Log exception here
                    throw new ApplicationException("Unable to load timesheet - an error has occurred");
                }
            }

            return timesheet;
        }

        private List<ProjectTaskTimesheetItem> GetTimesheetItems(HtmlNode htmlNode)
        {
            var projectTimeItems = new List<ProjectTaskTimesheetItem>();

            HtmlNode projectCode = null;

            // index starts at 2 because of the naming of the ASP.net controls - i.e. first is ctl00$C1$ProjectGrid$ctl2
            var i = 2;

            while (projectCode != null || i == 2)
            {
                var index = i.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');

                var projectControlName = string.Format(ProjectSelector, index);
                projectCode = htmlNode.SelectSingleNode(projectControlName);

                if (projectCode != null)
                {
                    // assume that there is a task combo control for each project combo control;
                    HtmlNode taskCode = htmlNode.SelectSingleNode(string.Format(ProjectTaskSelector, index));
                    ProjectTaskTimesheetItem timesheetItem = AddTimesheetItem(projectTimeItems, projectCode, taskCode);
                   
                    // always 7 days a week
                    for (int j = 0; j < 7; j++)
                    {
                        
                        PopulateTimesheetItem(htmlNode, ref timesheetItem, index, j);

                        var timeValue = htmlNode.SelectSingleNode(string.Format(LoggedTimeSelector, index, j.ToString(CultureInfo.InvariantCulture)));
                        var extraTimeValue = htmlNode.SelectSingleNode(string.Format(ExtraTimeSelector, index, j.ToString(CultureInfo.InvariantCulture)));
                        var notesValue = htmlNode.SelectSingleNode(string.Format(NotesSelector, index, j.ToString(CultureInfo.InvariantCulture)));
                        var workDetailValue = htmlNode.SelectSingleNode(string.Format(WorkDetailSelector, index, j.ToString(CultureInfo.InvariantCulture)));

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
        /// The required hours per day is a hidden field (should  be 7:30)
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="requiredHoursSelector"></param>
        /// <returns></returns>
        protected override List<TimeSpan> GetRequiredHours(HtmlNode htmlNode)
        {
            var requiredHours = new List<TimeSpan>();

            for (int i = 0; i < 7; i++)
            {
                var requiredHoursNode = htmlNode.SelectSingleNode(string.Format(RequiredHoursSelector, i.ToString()));
                var parsedTime = requiredHoursNode.Attributes["value"].Value == "0"
                                     ? TimeSpan.Zero
                                     : TimeSpan.Parse(requiredHoursNode.Attributes["value"].Value);
                requiredHours.Add(parsedTime);
            }

            return requiredHours;
        }


        /// <summary>
        /// Construct a ProjectTaskTimesheetItem from Html fragment and add to the list of projectTimeItems
        /// </summary>
        /// <param name="projectTimeItems"></param>
        /// <param name="projectCode"></param>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        protected override ProjectTaskTimesheetItem AddTimesheetItem(List<ProjectTaskTimesheetItem> projectTimeItems, HtmlNode projectCode, HtmlNode taskCode)
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

        protected override void PopulateTimesheetItem(HtmlNode htmlNode,
                                          ref ProjectTaskTimesheetItem timesheetItem,
                                          string index,
                                          int dayIndex)
        {
            
            Ensure.ArgumentNotNull(timesheetItem, "timesheetItem");

            var timeValue = htmlNode.SelectSingleNode(string.Format(LoggedTimeSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
            var extraTimeValue = htmlNode.SelectSingleNode(string.Format(ExtraTimeSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
            var notesValue = htmlNode.SelectSingleNode(string.Format(NotesSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
            var workDetailValue = htmlNode.SelectSingleNode(string.Format(WorkDetailSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));

            if (timeValue != null && timeValue.Attributes["value"] != null)
            {
                timesheetItem.TimeEntries[dayIndex].LoggedTime = TimeSpan.Parse(timeValue.Attributes["value"].Value);
            }

            if (extraTimeValue != null && extraTimeValue.Attributes["value"] != null)
            {
                timesheetItem.TimeEntries[dayIndex].ExtraTime =
                    TimeSpan.Parse(extraTimeValue.Attributes["value"].Value);
            }

            if (notesValue != null && notesValue.Attributes["value"] != null)
            {
                timesheetItem.TimeEntries[dayIndex].Notes = notesValue.Attributes["value"].Value;
            }

            if (workDetailValue != null && workDetailValue.Attributes["value"] != null)
            {
                timesheetItem.TimeEntries[dayIndex].WorkDetailId =
                    int.Parse(workDetailValue.Attributes["value"].Value);
            }
        }
    }
}
