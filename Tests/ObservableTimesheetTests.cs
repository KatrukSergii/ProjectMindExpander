using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace Tests
{
    [TestClass]
    public class ObservableTimesheetTests
    {
        // Create timesheet with 3 dummy project+task items
        private ObservableTimesheet CreateDummyTimesheet()
        {
            var ts = new Timesheet();
            for(int i = 0; i < 7; i++)
            {
                ts.RequiredHours.Add(new TimeSpan(7,30,0));    
            }

            // Project items
            var projectCode1 = new PickListItem(0,"p1");
            var taskCode1 = new PickListItem(0, "t1");
            var timesheetItem1 = new ProjectTaskTimesheetItem(projectCode1, taskCode1);
            ts.ProjectTimeItems.Add(timesheetItem1);

            var projectCode2 = new PickListItem(1,"p2");
            var taskCode2 = new PickListItem(1, "t2");
            var timesheetItem2 = new ProjectTaskTimesheetItem(projectCode2, taskCode2);
            ts.ProjectTimeItems.Add(timesheetItem2);

            var projectCode3 = new PickListItem(2,"p3");
            var taskCode3 = new PickListItem(2, "t3");
            var timesheetItem3 = new ProjectTaskTimesheetItem(projectCode3, taskCode3);
            ts.ProjectTimeItems.Add(timesheetItem3);

            // Non-project items
            var npProjectCode1 = new PickListItem(0, "npp1");
            var nptaskCode1 = new PickListItem(0, "npt1");
            var nptimesheetItem1 = new ProjectTaskTimesheetItem(npProjectCode1, nptaskCode1);
            ts.NonProjectActivityItems.Add(nptimesheetItem1);

            var npProjectCode2 = new PickListItem(1, "npp2");
            var nptaskCode2 = new PickListItem(1, "npt2");
            var nptimesheetItem2 = new ProjectTaskTimesheetItem(npProjectCode2, nptaskCode2);
            ts.NonProjectActivityItems.Add(nptimesheetItem2);

            var npProjectCode3 = new PickListItem(2, "npp3");
            var nptaskCode3 = new PickListItem(2, "npt3");
            var nptimesheetItem3 = new ProjectTaskTimesheetItem(npProjectCode3, nptaskCode3);
            ts.NonProjectActivityItems.Add(nptimesheetItem3);

            var timeSheet = new ObservableTimesheet(ts);
            return timeSheet;
        }

        // Monday (Required hours = 7:30)

        [TestMethod]
        public void FixHours_NoExtraTime_Monday_NoChanges()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromHours(3);
            timesheet.AcceptChanges();
            timesheet.FixHours();
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime);
        }

        [TestMethod]
        public void FixHours_ExtraTimeOnLastProjectItem_Monday_ExtraTimeMovedToFirstItem()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[2].TimeEntries[0].LoggedTime = TimeSpan.FromHours(8);  // 30 mins greater than required hours
            timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.AcceptChanges();
            timesheet.FixHours();
            // last item has no extra time
            Assert.AreEqual(TimeSpan.Zero,  timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime);
            // first item has 30 minutes of extra time
            Assert.AreEqual(TimeSpan.FromMinutes(30), timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime);
        }

        [TestMethod]
        public void FixHours_ExtraTimeNotSpecified_ExtraTimeCalculatedAndAddedToFirstItem()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[1].TimeEntries[0].LoggedTime = TimeSpan.FromHours(8);  // 30 mins greater than required hours
            timesheet.AcceptChanges();
            timesheet.FixHours();
            // second item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[1].TimeEntries[0].ExtraTime);
            // first item has 30 minutes of extra time
            Assert.AreEqual(TimeSpan.FromMinutes(30), timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime);
        }

        [TestMethod]
        public void FixHours_ExtraTimeAndLoggedTimeOnAllItems_ExtraTimeCalculatedAndAddedToFirstItemAndRemovedFromOthers()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromHours(8);  // 30 mins greater than required hours
            timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.ProjectTimeItems[1].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 60 mins 
            timesheet.ProjectTimeItems[1].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.ProjectTimeItems[2].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 90 mins
            timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);

            timesheet.AcceptChanges();
            timesheet.FixHours();
            // first item has 30 minutes of extra time
            Assert.AreEqual(TimeSpan.FromMinutes(90), timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime, "first item");
            // second item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[1].TimeEntries[0].ExtraTime, "second item");
            // third item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime, "third item");
        }

        [TestMethod]
        public void FixHours_NoExtraTimeButNonProjectTimeLogged_NoChanges()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.NonProjectActivityItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromHours(3);
            timesheet.AcceptChanges();
            timesheet.FixHours();
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime);
        }

        [TestMethod]
        public void FixHours_ExtraTimeOnLastProjectItemWithNonProjectTime_ExtraTimeMovedToFirstItem()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[2].TimeEntries[0].LoggedTime = TimeSpan.FromHours(8);  // 60 mins greater than required hours (30 mins proj item, 30 mins non-proj)
            timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.NonProjectActivityItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);
            timesheet.AcceptChanges();
            timesheet.FixHours();
            // last item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime);
            // first item has 60 minutes of extra time
            Assert.AreEqual(TimeSpan.FromHours(1), timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime);
        }

        [TestMethod]
        public void FixHours_ExtraTimeAndLoggedTimeOnAllItemsIncludingNonProjectTime_ExtraTimeCalculatedAndAddedToFirstItemAndRemovedFromOthers()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromHours(8);  // 30 mins greater than required hours
            timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.ProjectTimeItems[1].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 60 mins 
            timesheet.ProjectTimeItems[1].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.ProjectTimeItems[2].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 90 mins
            timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);

            timesheet.NonProjectActivityItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 120 mins greater than required hours
            timesheet.NonProjectActivityItems[0].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.NonProjectActivityItems[1].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 150 mins 
            timesheet.NonProjectActivityItems[1].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.NonProjectActivityItems[2].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 180 mins
            timesheet.NonProjectActivityItems[2].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);

            timesheet.AcceptChanges();
            timesheet.FixHours();
            // first item has 30 minutes of extra time
            Assert.AreEqual(TimeSpan.FromMinutes(180), timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime, "first item");
            // second item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[1].TimeEntries[0].ExtraTime, "second item");
            // third item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[2].TimeEntries[0].ExtraTime, "third item");
        }

        // Sunday (Required hours = 7:30)
    }
}
