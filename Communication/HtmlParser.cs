﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using Communication.ParserStrategy;
using HtmlAgilityPack;
using Model;
using Shared;
using Shared.Enum;

namespace Communication
{
    /// <summary>
    /// HtmlParser extracts data from HTML page strings 
    /// </summary>
    public class HtmlParser : IHtmlParser
    {
        private readonly ITimesheetParserStrategy _timesheetParserStrategy;
        
        public HtmlParser()
        {
            _timesheetParserStrategy = new DraftTimesheetParserStrategy();        
        }

        public HtmlParser(TimesheetType timesheetType)
        {
            if (timesheetType == TimesheetType.Draft)
            {
                _timesheetParserStrategy = new DraftTimesheetParserStrategy();
            }
            else
            {
                _timesheetParserStrategy = new ApprovedTimesheetParserStrategy();
            }
        }

        public static string ViewstateSelector = "//input[@name = '__VIEWSTATE']";
        private readonly Regex _timesheetIdRegex = new Regex(@"javascript:OnSubmit\('0010',(?<timesheetId>\d+)\);");
        public static string LoginErrorSelector = "//div[@id='login-error']/h4";

        public TimesheetId GetLatestTimesheetId(string timesheetHistoryHtml)
        {
            var latestTimesheet = ParseTimesheetHistory(timesheetHistoryHtml).First();
            var timesheetId = new TimesheetId(latestTimesheet.Key, latestTimesheet.Value);
            return timesheetId;
        }

        /// <summary>
        /// Extracts a (sorted desc) list of timesheet Ids and dates
        /// </summary>
        /// <param name="timesheetHistoryHtml"></param>
        /// <returns></returns>
        private Dictionary<string, string> ParseTimesheetHistory(string timesheetHistoryHtml)
        {
            const string timesheetSelector = "//tr[@data-item-row-id]/td/div/a";
            var htmlDoc = new HtmlDocument();
            var stringReader = new StringReader(timesheetHistoryHtml);
            htmlDoc.Load(stringReader);

            var unsortedList = new Dictionary<string, DateTime>();
            var timesheetList = new Dictionary<string, string>();

            if (htmlDoc.DocumentNode != null)
            {
                var timeSheetNodes = htmlDoc.DocumentNode.SelectNodes(timesheetSelector);

                foreach (var hyperlinkNode in timeSheetNodes)
                {
                    var link = hyperlinkNode.Attributes["href"].Value;
                    var matches = _timesheetIdRegex.Match(link);
                    var id = matches.Groups[1].Value;
                    var date = hyperlinkNode.Attributes["title"].Value;
                    var parsedDate = DateTime.Parse(date);
                    unsortedList.Add(id, parsedDate);
                }

                // Sort the list so that the most recent timesheet is at the top
                foreach (var kvp in unsortedList.OrderByDescending(x => x.Value))
                {
                    timesheetList.Add(kvp.Key, kvp.Value.ToString("dd MMM yyyy"));
                }
            }

            return timesheetList;
        }

        ///// <summary>
        ///// Construct a timesheet from an approved (i.e. old) timesheet HTML string
        ///// Return the viewstate as an out parameter for further operations
        ///// </summary>
        ///// <param name="timesheetHtml"></param>
        ///// <param name="viewstate"></param>
        ///// <returns></returns>
        ///// <exception cref="ApplicationException">Invalid timesheet ID throws ApplicationException</exception>
        //public Timesheet ParseApprovedTimesheet(string timesheetHtml, out string viewstate)
        //{
        //    const string projectIdSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdSelectProjectId']/@value"; 
        //    const string projectNameSelector = "//span[@id = 'ctl00_C1_ProjectGrid_ctl{0}_lblProject']"; 

        //    const string projectTaskSelector = "//input[@id = 'ctl00_C1_ProjectGrid_ctl{0}_hdSelectProjectTaskId']";
        //    const string projectTaskNameSelector = "//span[@id = 'ctl00_C1_ProjectGrid_ctl{0}_lblProjectTask']";
        //    const string loggedTimeSelector = "//span[@id = 'ctl00_C1_ProjectGrid_ctl{0}_lblLoggedTime{1}']";

        //    const string extraTimeSelector = "//span[@id = 'ctl00_C1_ProjectGrid_ctl{0}_hdExtraTime{1}']";
        //    const string notesSelector = "//span[@id = 'ctl00_C1_ProjectGrid_ctl{0}_hdNotes{1}']";
        //    const string workDetailSelector = "//span[@id = 'ctl00_C1_ProjectGrid_ctl{0}_hdWorkDetailId{1}']";

        //    //const string nonProjectSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProject']/option[@selected='selected']";
        //    //const string nonProjectTaskSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProjectTask']/option[@selected='selected']";

        //    //const string nonProjectLoggedTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$txtLoggedTime{1}']";
        //    //const string nonProjectExtraTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdExtraTime{1}']";
        //    //const string nonProjectNotesSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdNotes{1}']";
        //    //const string nonProjectWorkDetailSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdWorkDetailId{1}']";

        //    //const string requiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequired{0}']";
        //    //const string totalRequiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequiredTotal']";

        //    const string titleSelector = "//div[@class='subtitle']";
        //    //const string LoginErrorSelector = "//div[@id='login-error']/h4";

        //    const string timesheetIdSelector = "//input[@name = 'ctl00$C1$hdnTimesheetId']";

        //    var timesheet = new Timesheet();
        //    viewstate = string.Empty;

        //    //HtmlNode.ElementsFlags.Remove("option");
        //    var htmlDoc = new HtmlDocument();
        //    var stringReader = new StringReader(timesheetHtml);

        //    htmlDoc.Load(stringReader);

        //    if (htmlDoc.DocumentNode != null)
        //    {
        //        AssertNoErrors(htmlDoc.DocumentNode, LoginErrorSelector);

        //        try
        //        {
        //            // Timesheet title, e.g. '08 Jul 2013 to 14 Jul 2013 by Joe Bloggs (Draft)'
        //            timesheet.Title = htmlDoc.DocumentNode.SelectSingleNode(titleSelector).InnerText.Trim();

        //            timesheet.TimesheetId =
        //                htmlDoc.DocumentNode.SelectSingleNode(timesheetIdSelector).Attributes["value"].Value;

        //           if (timesheet.TimesheetId.Equals("0"))
        //            {
        //                throw new ApplicationException("The Timesheet has an invalid ID");
        //            }

        //            timesheet.ProjectTimeItems = GetTimesheetItems(htmlDoc.DocumentNode, 
        //                                                                projectIdSelector, 
        //                                                                projectNameSelector,
        //                                                                projectTaskSelector, 
        //                                                                projectTaskNameSelector,
        //                                                                loggedTimeSelector,
        //                                                                extraTimeSelector, 
        //                                                                notesSelector,
        //                                                                workDetailSelector, 
        //                                                                isApprovedTimesheet:true);
                    
        //            //timesheet.NonProjectActivityItems = GetTimesheetItems(htmlDoc.DocumentNode, nonProjectSelector,
        //            //                                                      nonProjectTaskSelector,
        //            //                                                      nonProjectLoggedTimeSelector,
        //            //                                                      nonProjectExtraTimeSelector,
        //            //                                                      nonProjectNotesSelector,
        //            //                                                      nonProjectWorkDetailSelector);

        //           // timesheet.RequiredHours = GetRequiredHours(htmlDoc.DocumentNode, requiredHoursSelector);
        //            //timesheet.TotalRequiredHours =
        //            //    htmlDoc.DocumentNode.SelectSingleNode(totalRequiredHoursSelector).Attributes["value"].Value
        //            //                                                                                         .ToTimeSpan
        //            //        ();

        //            viewstate = htmlDoc.DocumentNode.SelectSingleNode(ViewstateSelector).Attributes["value"].Value;

        //        }
        //        catch (NullReferenceException ex)
        //        {
        //            // Log exception here
        //            throw new ApplicationException("Unable to load timesheet - an error has occurred");
        //        }
        //    }

        //    return timesheet;
        //}

        /// <summary>
        /// Return Timesheet object based upon the contents of a HTML string representing the timesheet webpage.
        /// Return the viewstate as an out parameter for further operations
        /// </summary>
        /// <param name="timesheetHtml"></param>
        /// <param name="viewstate"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Invalid timesheet ID throws ApplicationException</exception>
        public Timesheet ParseTimesheet(string timesheetHtml, out string viewstate, bool isApprovedTimesheet = false)
        {
            var timeSheet = _timesheetParserStrategy.ParseTimesheet(timesheetHtml, out viewstate);

            return timeSheet;

            //const string projectSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProject']/option[@selected='selected']";
            //const string projectTaskSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProjectTask']/option[@selected='selected']";

            //const string loggedTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$txtLoggedTime{1}']";
            //const string extraTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdExtraTime{1}']";
            //const string notesSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdNotes0{1}']";
            //const string workDetailSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$hdWorkDetailId0{1}']";

            //const string nonProjectSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProject']/option[@selected='selected']";
            //const string nonProjectTaskSelector = "//select[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$ddlInternalProjectTask']/option[@selected='selected']";

            //const string nonProjectLoggedTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$txtLoggedTime{1}']";
            //const string nonProjectExtraTimeSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdExtraTime{1}']";
            //const string nonProjectNotesSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdNotes{1}']";
            //const string nonProjectWorkDetailSelector = "//input[@name = 'ctl00$C1$InternalProjectGrid$ctl{0}$hdWorkDetailId{1}']";

            //const string requiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequired{0}']";
            //const string totalRequiredHoursSelector = "//input[@name = 'ctl00$C1$hdDailyRequiredTotal']";

            //const string titleSelector = "//div[@class='subtitle']";

            //const string timesheetIdSelector = "//input[@name = 'ctl00$C1$hdnTimesheetId']";

            //var timesheet = new Timesheet();
            //viewstate = string.Empty;

            //HtmlNode.ElementsFlags.Remove("option");
            //var htmlDoc = new HtmlDocument();
            //var stringReader = new StringReader(timesheetHtml);

            //htmlDoc.Load(stringReader);

            //if (htmlDoc.DocumentNode != null)
            //{
            //    AssertNoErrors(htmlDoc.DocumentNode, LoginErrorSelector);

            //    try
            //    {
            //        // Timesheet title, e.g. '08 Jul 2013 to 14 Jul 2013 by Joe Bloggs (Draft)'
            //        timesheet.Title = htmlDoc.DocumentNode.SelectSingleNode(titleSelector).InnerText.Trim();

            //        timesheet.TimesheetId =
            //            htmlDoc.DocumentNode.SelectSingleNode(timesheetIdSelector).Attributes["value"].Value;

            //        if (timesheet.TimesheetId.Equals("0"))
            //        {
            //            throw new ApplicationException("The Timesheet has an invalid ID");
            //        }

            //        timesheet.ProjectTimeItems = GetTimesheetItems(htmlDoc.DocumentNode, projectSelector,
            //                                                           string.Empty, // projectName 
            //                                                           projectTaskSelector, 
            //                                                           string.Empty, // taskName
            //                                                           loggedTimeSelector,
            //                                                           extraTimeSelector, notesSelector,
            //                                                           workDetailSelector);

            //        timesheet.NonProjectActivityItems = GetTimesheetItems(htmlDoc.DocumentNode, 
            //                                                            nonProjectSelector, 
            //                                                            string.Empty, // projectName
            //                                                            nonProjectTaskSelector,
            //                                                            string.Empty, // taskName
            //                                                            nonProjectLoggedTimeSelector,
            //                                                            nonProjectExtraTimeSelector,
            //                                                            nonProjectNotesSelector,
            //                                                            nonProjectWorkDetailSelector);

            //        timesheet.RequiredHours = GetRequiredHours(htmlDoc.DocumentNode, requiredHoursSelector);
            //        timesheet.TotalRequiredHours =
            //            htmlDoc.DocumentNode.SelectSingleNode(totalRequiredHoursSelector).Attributes["value"].Value
            //                                                                                                 .ToTimeSpan
            //                ();

            //        viewstate = htmlDoc.DocumentNode.SelectSingleNode(ViewstateSelector).Attributes["value"].Value;

            //    }
            //    catch (NullReferenceException ex)
            //    {
            //        // Log exception here
            //        throw new ApplicationException("Unable to load timesheet - an error has occurred");
            //    }
            //}

            //return timesheet;
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
        public static void AssertNoErrors(HtmlNode htmlNode, string errorSelector)
        {
            var errorNode = htmlNode.SelectSingleNode(errorSelector);
            if (errorNode != null && !string.IsNullOrEmpty(errorNode.InnerText))
            {
                throw new InvalidOperationException(errorNode.InnerText);
            }
        }

        ///// <summary>
        ///// The required hours per day is a hidden field (should  be 7:30)
        ///// </summary>
        ///// <param name="htmlNode"></param>
        ///// <param name="requiredHoursSelector"></param>
        ///// <returns></returns>
        //private List<TimeSpan> GetRequiredHours(HtmlNode htmlNode, string requiredHoursSelector)
        //{
        //    var requiredHours = new List<TimeSpan>();

        //    for (int i = 0; i < 7; i++)
        //    {
        //        var requiredHoursNode = htmlNode.SelectSingleNode(string.Format(requiredHoursSelector, i.ToString()));
        //        var parsedTime = requiredHoursNode.Attributes["value"].Value == "0"
        //                             ? TimeSpan.Zero
        //                             : TimeSpan.Parse(requiredHoursNode.Attributes["value"].Value);
        //        requiredHours.Add(parsedTime);
        //    }

        //    return requiredHours;
        //}

        //private List<ProjectTaskTimesheetItem> GetApprovedTimesheetItems(HtmlNode htmlNode, string projectSelector, 
        //    string projectTaskSelector,
        //    string loggedTimeSelector,
        //    string extraTimeSelector,
        //    string notesSelector,
        //    string workDetailsSelector)
        //{
        //    var projectTimeItems = new List<ProjectTaskTimesheetItem>();

        //    HtmlNode projectCode = null;
            
        //    // index starts at 2 because of the naming of the ASP.net controls - i.e. first is ctl00$C1$ProjectGrid$ctl2
        //    var i = 2;

        //    while (projectCode != null || i == 2)
        //    {
        //        var index = i.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
        //        i++;

        //    }
        //}

        //private List<ProjectTaskTimesheetItem> GetTimesheetItems(HtmlNode htmlNode, string projectSelector,
        //    string projectNameSelector,
        //    string projectTaskSelector,
        //    string projectTaskNameSelector,
        //    string loggedTimeSelector,
        //    string extraTimeSelector,
        //    string notesSelector,
        //    string workDetailSelector,
        //    bool isApprovedTimesheet = false)
        //{
        //    var projectTimeItems = new List<ProjectTaskTimesheetItem>();

        //    HtmlNode projectCode = null;

        //    // index starts at 2 because of the naming of the ASP.net controls - i.e. first is ctl00$C1$ProjectGrid$ctl2
        //    var i = 2;

        //    while (projectCode != null || i == 2)
        //    {
        //        var index = i.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');

        //        var projectControlName = string.Format(projectSelector, index);
        //        projectCode = htmlNode.SelectSingleNode(projectControlName);

        //        if (projectCode != null)
        //        {
        //            // assume that there is a task combo control for each project combo control;
        //            HtmlNode taskCode = htmlNode.SelectSingleNode(string.Format(projectTaskSelector, index));
        //            ProjectTaskTimesheetItem timesheetItem = null;
        //            if (isApprovedTimesheet)
        //            {
        //                var projectName = htmlNode.SelectSingleNode(string.Format(projectNameSelector, index));
        //                var taskName = htmlNode.SelectSingleNode(string.Format(projectTaskNameSelector, index));
        //                AddApprovedTimesheetItem(projectTimeItems, 
        //                                         projectCode.Attributes["value"].Value,
        //                                         projectName.InnerText,
        //                                         taskCode.Attributes["value"].Value,
        //                                         taskName.InnerText);   
        //            }
        //            else
        //            {
        //                AddTimesheetItem(projectTimeItems, projectCode, taskCode);
        //            }

        //            // always 7 days a week
        //            for (int j = 0; j < 7; j++)
        //            {
        //                if (isApprovedTimesheet)
        //                {
        //                    PopulateApprovedTimesheetItem(htmlNode, ref timesheetItem, index, j, loggedTimeSelector, extraTimeSelector, notesSelector,
        //                                                  workDetailSelector);
        //                }
        //                else
        //                {
        //                    PopulateTimesheetItem(htmlNode, ref timesheetItem, index, j, loggedTimeSelector, extraTimeSelector, notesSelector,
        //                                                 workDetailSelector);
        //                }

        //                var timeValue = htmlNode.SelectSingleNode(string.Format(loggedTimeSelector, index, j.ToString(CultureInfo.InvariantCulture)));
        //                var extraTimeValue = htmlNode.SelectSingleNode(string.Format(extraTimeSelector, index, j.ToString(CultureInfo.InvariantCulture)));
        //                var notesValue = htmlNode.SelectSingleNode(string.Format(notesSelector, index, j.ToString(CultureInfo.InvariantCulture)));
        //                var workDetailValue = htmlNode.SelectSingleNode(string.Format(workDetailSelector, index, j.ToString(CultureInfo.InvariantCulture)));

        //                if (timeValue != null && timeValue.Attributes["value"] != null)
        //                {
        //                    timesheetItem.TimeEntries[j].LoggedTime = TimeSpan.Parse(timeValue.Attributes["value"].Value);
        //                }

        //                if (extraTimeValue != null && extraTimeValue.Attributes["value"] != null)
        //                {
        //                    timesheetItem.TimeEntries[j].ExtraTime =
        //                        TimeSpan.Parse(extraTimeValue.Attributes["value"].Value);
        //                }

        //                if (notesValue != null && notesValue.Attributes["value"] != null)
        //                {
        //                    timesheetItem.TimeEntries[j].Notes = notesValue.Attributes["value"].Value;
        //                }

        //                if (workDetailValue != null && workDetailValue.Attributes["value"] != null)
        //                {
        //                    timesheetItem.TimeEntries[j].WorkDetailId =
        //                        int.Parse(workDetailValue.Attributes["value"].Value);
        //                }

        //            }

        //            i++;
        //        }
        //    }

        //    return projectTimeItems;
        //}

        private static void PopulateApprovedTimesheetItem(HtmlNode htmlNode, 
                                                    ref ProjectTaskTimesheetItem timesheetItem, 
                                                    string index, int dayIndex,
                                                    string loggedTimeSelector, 
                                                    string extraTimeSelector,
                                                    string notesSelector, 
                                                    string workDetailSelector)
        {

            var timeValue = htmlNode.SelectSingleNode(string.Format(loggedTimeSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
            var extraTimeValue = htmlNode.SelectSingleNode(string.Format(extraTimeSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
            var notesValue = htmlNode.SelectSingleNode(string.Format(notesSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
            var workDetailValue = htmlNode.SelectSingleNode(string.Format(workDetailSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));

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

        //private static void PopulateTimesheetItem(HtmlNode htmlNode, 
        //                                    ref ProjectTaskTimesheetItem timesheetItem, 
        //                                    string index, 
        //                                    int dayIndex,
        //                                    string loggedTimeSelector, string extraTimeSelector,
        //                                    string notesSelector, string workDetailSelector)
        //{
        //    var timeValue = htmlNode.SelectSingleNode(string.Format(loggedTimeSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
        //    var extraTimeValue = htmlNode.SelectSingleNode(string.Format(extraTimeSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
        //    var notesValue = htmlNode.SelectSingleNode(string.Format(notesSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));
        //    var workDetailValue = htmlNode.SelectSingleNode(string.Format(workDetailSelector, index, dayIndex.ToString(CultureInfo.InvariantCulture)));

        //    if (timeValue != null && timeValue.Attributes["value"] != null)
        //    {
        //        timesheetItem.TimeEntries[dayIndex].LoggedTime = TimeSpan.Parse(timeValue.Attributes["value"].Value);
        //    }

        //    if (extraTimeValue != null && extraTimeValue.Attributes["value"] != null)
        //    {
        //        timesheetItem.TimeEntries[dayIndex].ExtraTime =
        //            TimeSpan.Parse(extraTimeValue.Attributes["value"].Value);
        //    }

        //    if (notesValue != null && notesValue.Attributes["value"] != null)
        //    {
        //        timesheetItem.TimeEntries[dayIndex].Notes = notesValue.Attributes["value"].Value;
        //    }

        //    if (workDetailValue != null && workDetailValue.Attributes["value"] != null)
        //    {
        //        timesheetItem.TimeEntries[dayIndex].WorkDetailId =
        //            int.Parse(workDetailValue.Attributes["value"].Value);
        //    }
        //}

        ///// <summary>
        ///// Construct a ProjectTaskTimesheetItem from Html fragment and add to the list of projectTimeItems
        ///// </summary>
        ///// <param name="projectTimeItems"></param>
        ///// <param name="projectCode"></param>
        ///// <param name="taskCode"></param>
        ///// <returns></returns>
        //private ProjectTaskTimesheetItem AddTimesheetItem(List<ProjectTaskTimesheetItem> projectTimeItems, HtmlNode projectCode, HtmlNode taskCode)
        //{
        //    var projectCodePickListItem = GetPickListItemFromCombobox(projectCode);
        //    var projectTaskCodePickListItem = GetPickListItemFromCombobox(taskCode);
        //    var timesheetItem = new ProjectTaskTimesheetItem(projectCodePickListItem, projectTaskCodePickListItem);
        //    projectTimeItems.Add(timesheetItem);

        //    return timesheetItem;
        //}

        //private ProjectTaskTimesheetItem AddApprovedTimesheetItem(List<ProjectTaskTimesheetItem> projectTimeItems,
        //                                                          string projectCode, string projectName, string taskCode, string taskName)
        //{
        //    var projectCodePickListItem = new PickListItem(int.Parse(projectCode), projectName);
        //    var projectTaskCodePickListItem = new PickListItem(int.Parse(taskCode), taskName);
        //    var timesheetItem = new ProjectTaskTimesheetItem(projectCodePickListItem, projectTaskCodePickListItem);
        //    projectTimeItems.Add(timesheetItem);

        //    return timesheetItem;
        //}

        //    // Substring is in place of 'ends-with' (needed because some of the dropdown names end with ddlProjectTask)
        //    foreach (HtmlNode link in htmlDocumentNode.SelectNodes("//select['ddlProject' = substring(@name,string-length(@name) - string-length('ddlProject') +1)]/option[@selected='selected']"))

    }
}
