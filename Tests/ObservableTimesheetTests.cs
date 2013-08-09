using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace Tests
{
    [TestClass]
    public class ObservableTimesheetTests
    {
        private ObservableTimesheet CreateDummyTimesheet()
        {
            var timesheet = new Timesheet();
            timesheet.TimesheetId = "timesheetId";
            timesheet.DummyTimeEntry = new TimeEntry()
                {
                    Notes = "notes",
                    ExtraTime = TimeSpan.FromHours(1),
                    LoggedTime = TimeSpan.FromHours(1),
                    WorkDetailId = null
                };

            var projectCode1 = new PickListItem(1, "projectCode1");

            var taskCode1 = new PickListItem(1, "p1taskCode1");
            var taskCode2 = new PickListItem(2, "p1taskCode2");
            var taskCode3 = new PickListItem(3, "p1taskCode3");

            var timesheetItem1 = new ProjectTaskTimesheetItem(projectCode1, taskCode1);

            // 7 days of the week
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(0) });
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(10) });
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(20) });
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(30) });
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(40) });
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(50) });
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(60) });

            var timesheetItem2 = new ProjectTaskTimesheetItem(projectCode1, taskCode3);

            // 7 days of the week
            timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(0) });
            timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(10) });
            timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(20) });
            timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(30) });
            timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(40) });
            timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(50) });
            timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(60) });

            var timesheetItem3 = new ProjectTaskTimesheetItem(projectCode1, taskCode3);

            // 7 days of the week
            timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(0) });
            timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(10) });
            timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(20) });
            timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(30) });
            timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(40) });
            timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(50) });
            timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(60) });

            timesheet.ProjectTimeItems.Add(timesheetItem1);
            timesheet.ProjectTimeItems.Add(timesheetItem2);
            timesheet.ProjectTimeItems.Add(timesheetItem3);

            timesheet.RequiredHours.AddRange(new List<TimeSpan> { 
                TimeSpan.FromHours(7.5), 
                TimeSpan.FromHours(7.5), 
                TimeSpan.FromHours(7.5), 
                TimeSpan.FromHours(7.5), 
                TimeSpan.FromHours(7.5), 
                TimeSpan.FromHours(7.5), 
                TimeSpan.FromHours(7.5)
            });

            timesheet.TotalRequiredHours = TimeSpan.FromHours(37.5);
            timesheet.Title = "timesheetTiele";

            var observableTimesheet = new ObservableTimesheet(timesheet);

            return observableTimesheet;
        }

        [TestMethod]
        public void IsChanged_ObservableTimesheetNoChanges_False()
        {
            var ts = CreateDummyTimesheet();
            Assert.IsFalse(ts.IsChanged);
        }

        [TestMethod]
        public void IsChanged_ChangedTitle_True()
        {
            var ts = CreateDummyTimesheet();
            ts.Title = "new title";
            Assert.IsTrue(ts.IsChanged);
        }

        [TestMethod]
        public void IsChanged_TitleChangedThenChangedBackToOriginal_False()
        {
            var ts = CreateDummyTimesheet();
            var oldtitle = ts.Title;
            ts.Title = "new title";
            Assert.IsTrue(ts.IsChanged);
            ts.Title = oldtitle;
            Assert.IsFalse(ts.IsChanged);
        }

        [TestMethod]
        public void AcceptChanges_ChangedTitle_IsChangedIsFalse()
        {
            var ts = CreateDummyTimesheet();
            ts.Title = "new title";
            Assert.IsTrue(ts.IsChanged);
            ts.AcceptChanges();
            Assert.IsFalse(ts.IsChanged);
            Assert.AreEqual("new title", ts.Title);
        }

        [TestMethod]
        public void RejectChanges_ChangedTitle_TitleRestoredAndIsChangedIsFalse()
        {
            var ts = CreateDummyTimesheet();
            var oldTitle = ts.Title;
            ts.Title = "new title";
            Assert.IsTrue(ts.IsChanged);
            ts.AbandonChanges();
            Assert.IsFalse(ts.IsChanged);
            Assert.AreEqual(oldTitle, ts.Title);
        }

        [TestMethod]
        public void IsChanged_ChangedTimeEntryProjectCode_True()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyTimeEntry.Notes = "new notes";
            Assert.IsTrue(ts.IsChanged);
        }

        [TestMethod]
        public void IsChanged_ProjectCodeChangedThenChangedBack_False()
        {
            var ts = CreateDummyTimesheet();
            var originalHours = ts.RequiredHours[0];
            ts.RequiredHours[0] = TimeSpan.FromMinutes(999);
            Assert.IsTrue(ts.IsChanged);
            ts.RequiredHours[0] = originalHours;
            Assert.IsFalse(ts.IsChanged);
        }
    }
}
