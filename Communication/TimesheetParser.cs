using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;
using Shared;
using Shared.Interfaces;

namespace Communication
{
    public class TimesheetParser : ITimesheetParser
    {
        public Timesheet Parse(System.IO.StreamReader streamReader)
        {
            const string projectSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProject']/option[@selected='selected']";
            const string projectTaskSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProjectTask']/option[@selected='selected']";
            // TODO need Extra time, Notes, Work Details Id
            const string loggedTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$txtLoggedTime{1}']";

            const string nonProjectSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProject']/option[@selected='selected']";
            const string nonProjectTaskSelector = "//select[@name = 'ctl00$C1$ProjectGrid$ctl{0}$ddlProjectTask']/option[@selected='selected']";
            // TODO need Extra time, Notes, Work Details Id
            const string nonProjectLoggedTimeSelector = "//input[@name = 'ctl00$C1$ProjectGrid$ctl{0}$txtLoggedTime{1}']";

            var timesheet = new Timesheet();

            HtmlNode.ElementsFlags.Remove("option");
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(streamReader);
            if (htmlDoc.DocumentNode != null)
            {
                timesheet.ProjectTimeItems = GetTimesheetItems(htmlDoc.DocumentNode,projectSelector,projectTaskSelector,loggedTimeSelector);
                timesheet.NonProjectActivityItems = GetTimesheetItems(htmlDoc.DocumentNode, nonProjectSelector, nonProjectTaskSelector, nonProjectLoggedTimeSelector);
            }

            return timesheet;
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
                            timesheetItem[j].LoggedTime = DateTime.Parse(timeValue.Attributes["value"].Value);
                        }
                    }

                    i++;
                }
            }

            return projectTimeItems;
        }

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

        //private List<PickListItem> GetSelectedProjectCodes(HtmlNode htmlDocumentNode)
        //{
        //    var projectCodes = new List<PickListItem>();
            
        //    // Substring is in place of 'ends-with' (needed because some of the dropdown names end with ddlProjectTask)
        //    foreach (HtmlNode link in htmlDocumentNode.SelectNodes("//select['ddlProject' = substring(@name,string-length(@name) - string-length('ddlProject') +1)]/option[@selected='selected']"))
        //    {
        //        var projectId = int.Parse(link.Attributes["value"].Value);

        //        if (projectCodes.All(x => x.Value != projectId))
        //        {
        //            // Don't add dupes
        //            projectCodes.Add(new PickListItem(int.Parse(link.Attributes["value"].Value), link.InnerText));
        //        }
        //    }

        //    return projectCodes;
        //}

    }
}
