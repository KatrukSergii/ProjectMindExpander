using System;
using Model;
using Shared;

namespace Tests
{
    public static class TestHelper
    {
        /// <summary>
        /// Write contents of the timesheet to the console
        /// </summary>
        /// <param name="timesheet"></param>
        public static void PrettyPrintTimesheet(ObservableTimesheet timesheet)
        {
            Console.WriteLine(timesheet.TimesheetId + ":  " + timesheet.Title + Environment.NewLine);

            Console.WriteLine("MON".PadRight(10) + "TUE".PadRight(10) + "WED".PadRight(10) + "THU".PadRight(10)  + "FRI".PadRight(10) + "SAT".PadRight(10) + "SUN".PadRight(10));
            
            Console.WriteLine(@"PROJECT ITEMS");
            
            foreach (var projectTimeItem in timesheet.ProjectTimeItems)
            {
                Console.WriteLine(string.Format("Project: {0} ({1}), Task: {2} ({3})", projectTimeItem.ProjectCode.Name, projectTimeItem.ProjectCode.Value, projectTimeItem.TaskCode.Name, projectTimeItem.TaskCode.Value));
                for (int i = 0; i < 7; i++)
                {
                    Console.Write(projectTimeItem.TimeEntries[i].LoggedTime.ToString().PadRight(10));
                }
                Console.WriteLine(Environment.NewLine);
            }
            
            Console.WriteLine(@"NON-PROJECT ITEMS");

            foreach (var projectTimeItem in timesheet.NonProjectActivityItems)
            {
                Console.WriteLine(string.Format("Project: {0} ({1}), Task: {2} ({3})", projectTimeItem.ProjectCode.Name, projectTimeItem.ProjectCode.Value, projectTimeItem.TaskCode.Name, projectTimeItem.TaskCode.Value));
                for (int i = 0; i < 7; i++)
                {
                    Console.Write(projectTimeItem.TimeEntries[i].LoggedTime.ToString().PadRight(10));
                }
                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
