using System;
using System.Linq;
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

        #region FixHours
        
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
        [TestMethod]
        public void FixHours_ExtraTimeAndLoggedTimeOnAllItemsIncludingNonProjectTimeForSunday_ExtraTimeCalculatedAndAddedToFirstItemAndRemovedFromOthers()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[0].TimeEntries[6].LoggedTime = TimeSpan.FromHours(8);  // 30 mins greater than required hours
            timesheet.ProjectTimeItems[0].TimeEntries[6].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.ProjectTimeItems[1].TimeEntries[6].LoggedTime = TimeSpan.FromMinutes(30);  // 60 mins 
            timesheet.ProjectTimeItems[1].TimeEntries[6].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.ProjectTimeItems[2].TimeEntries[6].LoggedTime = TimeSpan.FromMinutes(30);  // 90 mins
            timesheet.ProjectTimeItems[2].TimeEntries[6].ExtraTime = TimeSpan.FromMinutes(30);

            timesheet.NonProjectActivityItems[0].TimeEntries[6].LoggedTime = TimeSpan.FromMinutes(30);  // 120 mins greater than required hours
            timesheet.NonProjectActivityItems[0].TimeEntries[6].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.NonProjectActivityItems[1].TimeEntries[6].LoggedTime = TimeSpan.FromMinutes(30);  // 150 mins 
            timesheet.NonProjectActivityItems[1].TimeEntries[6].ExtraTime = TimeSpan.FromMinutes(30);
            timesheet.NonProjectActivityItems[2].TimeEntries[6].LoggedTime = TimeSpan.FromMinutes(30);  // 180 mins
            timesheet.NonProjectActivityItems[2].TimeEntries[6].ExtraTime = TimeSpan.FromMinutes(30);

            timesheet.AcceptChanges();
            timesheet.FixHours();
            // first item has 30 minutes of extra time
            Assert.AreEqual(TimeSpan.FromMinutes(180), timesheet.ProjectTimeItems[0].TimeEntries[6].ExtraTime, "first item");
            // second item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[1].TimeEntries[6].ExtraTime, "second item");
            // third item has no extra time
            Assert.AreEqual(TimeSpan.Zero, timesheet.ProjectTimeItems[2].TimeEntries[6].ExtraTime, "third item");
        }

        #endregion

        #region GetChanges

        [TestMethod]
        public void GetChanges_NoChanges_EmptyList()
        {
            var timesheet = CreateDummyTimesheet();
            var changes = timesheet.ExtractChanges();
            Assert.IsNotNull(changes);
            Assert.AreEqual(0, changes.Count);
        }

        [TestMethod]
        public void GetChanges_MondayTask1LoggedTimeChanged_CorrectChanges()
        {
            var timesheet = CreateDummyTimesheet();
            // Task1 monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(1);
            var changes = timesheet.ExtractChanges();
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0", changes.Keys.First());
            Assert.AreEqual("00:01:00", changes["ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0"]);
        }

        [TestMethod]
        public void GetChanges_MondayAllTasksLoggedTimeChanged_CorrectChanges()
        {
            var timesheet = CreateDummyTimesheet();
            // Task1 monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(1);
            // Task2 monday
            timesheet.ProjectTimeItems[1].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(2);
            // Task3 monday
            timesheet.ProjectTimeItems[2].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(3);

            var changes = timesheet.ExtractChanges();
            
            Assert.AreEqual(3, changes.Count);
            
            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0", changes.Keys.Take(1).Last());
            Assert.AreEqual("00:01:00", changes["ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl03$txtLoggedTime0", changes.Keys.Take(2).Last());
            Assert.AreEqual("00:02:00", changes["ctl00$C1$ProjectGrid$ctl03$txtLoggedTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl04$txtLoggedTime0", changes.Keys.Take(3).Last());
            Assert.AreEqual("00:03:00", changes["ctl00$C1$ProjectGrid$ctl04$txtLoggedTime0"]);
        }

        [TestMethod]
        public void GetChanges_MondayExtraTimeAndLoggedTimeChanged_CorrectChanges()
        {
            var timesheet = CreateDummyTimesheet();
            // Task1 monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(1);
            // Task1 monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(2);
            // Task2 monday
            timesheet.ProjectTimeItems[1].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(3);
            // Task2 monday
            timesheet.ProjectTimeItems[1].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(4);

            var changes = timesheet.ExtractChanges();

            Assert.AreEqual(4, changes.Count);

            // changes are ordered project+task then day of the week

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0", changes.Keys.Take(1).Last());
            Assert.AreEqual("00:01:00", changes["ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$hdExtraTime0", changes.Keys.Take(2).Last());
            Assert.AreEqual("00:02:00", changes["ctl00$C1$ProjectGrid$ctl02$hdExtraTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl03$txtLoggedTime0", changes.Keys.Take(3).Last());
            Assert.AreEqual("00:03:00", changes["ctl00$C1$ProjectGrid$ctl03$txtLoggedTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl03$hdExtraTime0", changes.Keys.Take(4).Last());
            Assert.AreEqual("00:04:00", changes["ctl00$C1$ProjectGrid$ctl03$hdExtraTime0"]);
        }

        [TestMethod]
        public void GetChanges_MondayAndSundayExtraTimeAndLoggedTimeChanged_CorrectChanges()
        {
            var timesheet = CreateDummyTimesheet();
            // Task1 Sunday
            timesheet.ProjectTimeItems[0].TimeEntries[6].LoggedTime = TimeSpan.FromMinutes(1);
            // Task1 Sunday
            timesheet.ProjectTimeItems[0].TimeEntries[6].ExtraTime = TimeSpan.FromMinutes(2);
            // Task1 monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(3);
            // Task1 monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(4);

            var changes = timesheet.ExtractChanges();

            Assert.AreEqual(4, changes.Count);

            // changes are ordered project+task then day of the week

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0", changes.Keys.Take(1).Last());
            Assert.AreEqual("00:03:00", changes["ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$hdExtraTime0", changes.Keys.Take(2).Last());
            Assert.AreEqual("00:04:00", changes["ctl00$C1$ProjectGrid$ctl02$hdExtraTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$txtLoggedTime6", changes.Keys.Take(3).Last());
            Assert.AreEqual("00:01:00", changes["ctl00$C1$ProjectGrid$ctl02$txtLoggedTime6"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$hdExtraTime6", changes.Keys.Take(4).Last());
            Assert.AreEqual("00:02:00", changes["ctl00$C1$ProjectGrid$ctl02$hdExtraTime6"]);
        }

        [TestMethod]
        public void GetChanges_MondayProjectAndNonProjectLoggedTimeChanged_CorrectChanges()
        {
            var timesheet = CreateDummyTimesheet();
            // Project Task1 Monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(1);
            timesheet.ProjectTimeItems[0].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(2);
            // Non-Project Task1 Modnay
            timesheet.NonProjectActivityItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(3);
            timesheet.NonProjectActivityItems[0].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(4);    

            var changes = timesheet.ExtractChanges();

            Assert.AreEqual(4, changes.Count);

            // changes are ordered project+task then day of the week

            // Project
            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0", changes.Keys.Take(1).Last());
            Assert.AreEqual("00:01:00", changes["ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0"]);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$hdExtraTime0", changes.Keys.Take(2).Last());
            Assert.AreEqual("00:02:00", changes["ctl00$C1$ProjectGrid$ctl02$hdExtraTime0"]);

            // Non-Project
            Assert.AreEqual("ctl00$C1$InternalProjectGrid$ctl02$txtLoggedTime0", changes.Keys.Take(3).Last());
            Assert.AreEqual("00:03:00", changes["ctl00$C1$InternalProjectGrid$ctl02$txtLoggedTime0"]);

            Assert.AreEqual("ctl00$C1$InternalProjectGrid$ctl02$hdExtraTime0", changes.Keys.Take(4).Last());
            Assert.AreEqual("00:04:00", changes["ctl00$C1$InternalProjectGrid$ctl02$hdExtraTime0"]);
        }


        // Fix hours - should not extract changes for non-fixed items
        [TestMethod]
        public void GetChanges_ExtraTimeAndLoggedTimeOnAllItemsThenFixHours_Changes()
        {
            var timesheet = CreateDummyTimesheet();
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = TimeSpan.FromHours(8);  // 30 mins greater than required hours

            timesheet.ProjectTimeItems[1].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 60 mins
            timesheet.ProjectTimeItems[1].TimeEntries[0].ExtraTime = TimeSpan.FromMinutes(30);   // <---- this should be removed

            timesheet.ProjectTimeItems[2].TimeEntries[0].LoggedTime = TimeSpan.FromMinutes(30);  // 90 mins

            timesheet.AcceptChanges();
            
            // Fix hours - should not extract changes for non-fixed items
            timesheet.FixHours();
            
            var changes = timesheet.ExtractChanges();

            Assert.AreEqual(2, changes.Count);

            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl02$hdExtraTime0", changes.Keys.Take(1).Last());
            Assert.AreEqual("01:30:00", changes["ctl00$C1$ProjectGrid$ctl02$hdExtraTime0"]);  // i.e. 90 mins extra time total
            Assert.AreEqual("ctl00$C1$ProjectGrid$ctl03$hdExtraTime0", changes.Keys.Take(2).Last());
            Assert.AreEqual("00:00:00", changes["ctl00$C1$ProjectGrid$ctl03$hdExtraTime0"]);  //  30 minutes deleted
        }
        #endregion
    }
}
