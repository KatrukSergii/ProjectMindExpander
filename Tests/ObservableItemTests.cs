using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace Tests
{
    [TestClass]
    public class ObservableItemTests
    {
        private ObservableDummyTestObject CreateDummyTimesheet()
        {
            var timesheet = new DummyTestObject();
            timesheet.TimesheetId = "timesheetId";
            timesheet.DummyTimeEntry = new TimeEntry()
                {
                    Notes = "notes",
                    ExtraTime = TimeSpan.FromHours(1),
                    LoggedTime = TimeSpan.FromHours(1),
                   // WorkDetailId = null
                };

            var projectCode1 = new PickListItem(1, "projectCode1");

            var taskCode1 = new PickListItem(1, "p1taskCode1");
            var taskCode2 = new PickListItem(2, "p1taskCode2");
            var taskCode3 = new PickListItem(3, "p1taskCode3");

            var timesheetItem1 = new ProjectTaskTimesheetItem(projectCode1, taskCode1);

            // 7 days of the week
            timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(0) });
            //timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(10) });
            //timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(20) });
            //timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(30) });
            //timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(40) });
            //timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(50) });
            //timesheetItem1.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(60) });

            //var timesheetItem2 = new ProjectTaskTimesheetItem(projectCode1, taskCode3);

            //// 7 days of the week
            //timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(0) });
            //timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(10) });
            //timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(20) });
            //timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(30) });
            //timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(40) });
            //timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(50) });
            //timesheetItem2.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(60) });

            //var timesheetItem3 = new ProjectTaskTimesheetItem(projectCode1, taskCode3);

            //// 7 days of the week
            //timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(0) });
            //timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(10) });
            //timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(20) });
            //timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(30) });
            //timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(40) });
            //timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(50) });
            //timesheetItem3.TimeEntries.Add(new TimeEntry { LoggedTime = TimeSpan.FromMinutes(60) });

            timesheet.ProjectTimeItems.Add(timesheetItem1);
            //timesheet.ProjectTimeItems.Add(timesheetItem2);
            //timesheet.ProjectTimeItems.Add(timesheetItem3);

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

            timesheet.DummyValueTypeCollection = new List<int> { 0, 1, 2, 3, 4, 5 };
            
            var ObservableDummyTestObject = new ObservableDummyTestObject(timesheet);

            return ObservableDummyTestObject;
        }

        [TestMethod]
        public void IsChanged_ObservableDummyTestObjectNoChanges_False()
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

        // Unchanged
        [TestMethod]
        public void IsChanged_UnchangedTimesheet_False()
        {
            var ts = CreateDummyTimesheet();
            Assert.IsFalse(ts.IsChanged);
        }

        // String Property of Property changed
        [TestMethod]
        public void IsChanged_ChangedTimeEntryProjectCode_True()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyTimeEntry.Notes = "new notes";
            Assert.IsTrue(ts.IsChanged);
        }

        // Timespan Property of Property changed
        [TestMethod]
        public void IsChanged_ChangedTimeEntryLoggedTime_True()
        {
            var ts = CreateDummyTimesheet();
            Assert.IsFalse(ts.IsChanged);
            ts.DummyTimeEntry.LoggedTime = ts.DummyTimeEntry.LoggedTime.Add(TimeSpan.FromHours(1));
            Assert.IsTrue(ts.IsChanged);
        }

        [TestMethod]
        public void IsChanged_ChangedTimeEntryLoggedTimeReplaced_True()
        {
            var ts = CreateDummyTimesheet();
            Assert.IsFalse(ts.IsChanged);
            ts.DummyTimeEntry.LoggedTime = ts.DummyTimeEntry.LoggedTime = TimeSpan.FromMinutes(666);
            Assert.IsTrue(ts.IsChanged);
        }

        [TestMethod]
        public void IsChanged_ChangedProjectItemTimeEntryLoggedTime_True()
        {
            var ts = CreateDummyTimesheet();
            Assert.IsFalse(ts.IsChanged);
            ts.ProjectTimeItems[0].TimeEntries[6].LoggedTime = new TimeSpan(1, 0, 0);
            Assert.IsTrue(ts.IsChanged);
        }
        
        [TestMethod]
        public void IsChanged__ChangedProjectItemTimeEntryLoggedTimeObservableTimesheet_True()
        {
            var ts = new Timesheet();
            var projectCode = new PickListItem(1, "pc1");
            var taskCode = new PickListItem(1, "tc1");
            var projectItem = new ProjectTaskTimesheetItem(projectCode, taskCode);

            for (int i = 0; i < 7; i++)
            {
                projectItem.TimeEntries[i].LoggedTime = TimeSpan.FromMinutes(1);
            }

            ts.ProjectTimeItems.Add(projectItem);

            for (int i = 0; i < 7; i++)
            {
                ts.RequiredHours[i] = TimeSpan.FromHours(7.5);
            }

            var timesheet = new ObservableTimesheet(ts);
            Assert.IsFalse(timesheet.IsChanged);
            timesheet.ProjectTimeItems[0].TimeEntries[6].LoggedTime = new TimeSpan(1, 0, 0);
            Assert.IsTrue(timesheet.IsChanged);
        }

        // Timespan Property replaced with the same value
        [TestMethod]
        public void IsChanged_ChangedTimeEntryLoggedTimeSameValue_False()
        {
            var ts = CreateDummyTimesheet();
            var originalValue = ts.DummyTimeEntry.LoggedTime;
            ts.DummyTimeEntry.LoggedTime = originalValue;
            Assert.IsFalse(ts.IsChanged);
        }


        // Struct Collection changed
        [TestMethod]
        public void IsChanged_ProjectCodeChanged_False()
        {
            var ts = CreateDummyTimesheet();
            var originalHours = ts.RequiredHours[0];
            ts.RequiredHours[0] = TimeSpan.FromMinutes(999);
            Assert.IsTrue(ts.IsChanged);
        }

        // Struct collection changed and then changed back
        [TestMethod]
        public void IsChanged_RequiredHoursChangedThenChangedBack_False()
        {
            var ts = CreateDummyTimesheet();
            Assert.IsFalse(ts.IsChanged);
            var originalHours = ts.RequiredHours[0];
            ts.RequiredHours[0] = TimeSpan.FromMinutes(999);
            Assert.IsTrue(ts.IsChanged);
            ts.RequiredHours[0] = originalHours;
            Assert.IsFalse(ts.IsChanged);
        }

        // Value type collection - add item
        [TestMethod]
        public void IsChanged_DummyValueTypeCollectionAddItem_True()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyValueTypeCollection.Add(1);
            Assert.IsTrue(ts.IsChanged);
        }
        
        // Value type collection - add item, accept changes
        public void IsChanged_DummyValueTypeCollectionAddItemAccept_False()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyValueTypeCollection.Add(1);
            ts.AcceptChanges();
            Assert.IsFalse(ts.IsChanged);
        }

        // Value type collection - add item, accept changes, change item
        [TestMethod]
        public void IsChanged_DummyValueTypeCollectionAddAcceptChange_True()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyValueTypeCollection.Add(1);
            ts.AcceptChanges();
            ts.DummyValueTypeCollection[ts.DummyValueTypeCollection.Count - 1] = 9999;
            Assert.IsTrue(ts.IsChanged);
        }

        // value type collection - remove item
        [TestMethod]
        public void IsChanged_DummyValueTypeCollectionRemove_True()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyValueTypeCollection.RemoveAt(0);
            Assert.IsTrue(ts.IsChanged);
        }

        // value type collection - remove item, accept changes
        [TestMethod]
        public void IsChanged_DummyValueTypeCollectionRemoveAccept_False()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyValueTypeCollection.RemoveAt(0);
            ts.AcceptChanges();
            Assert.IsFalse(ts.IsChanged);
        }

        // value type collection - remove item, accept changes, change any item
        [TestMethod]
        public void IsChanged_DummyValueTypeCollectionRemoveAcceptChange_False()
        {
            var ts = CreateDummyTimesheet();
            ts.DummyValueTypeCollection.RemoveAt(0);
            ts.AcceptChanges();
            ts.DummyValueTypeCollection[1] = 999;
            Assert.IsTrue(ts.IsChanged);
        }


        // Collection of observable items - remove item
        [TestMethod]
        public void IsChanged_ProjectItemsChanged_True()
        {
            var ts = CreateDummyTimesheet();
            ts.ProjectTimeItems.RemoveAt(0);
            Assert.IsTrue(ts.IsChanged);
        }

        // Collection of observable items - remove item, accept changes
        [TestMethod]
        public void IsChanged_ProjectItemsChangedAccept_False()
        {
            var ts = CreateDummyTimesheet();
            ts.ProjectTimeItems.RemoveAt(0);
            Assert.IsTrue(ts.IsChanged);
            ts.AcceptChanges();
            Assert.IsFalse(ts.IsChanged);
        }

        // Collection of observable items property changed (value type property)
        [TestMethod]
        public void IsChanged_ProjectItemsPropertyChange_True()
        {
            var ts = CreateDummyTimesheet();
            ts.ProjectTimeItems[0].TaskCode.Name = "new name";
            Assert.IsTrue(ts.IsChanged);
        }

        // Collection of observable items property changed, then changed back (value type property)
        [TestMethod]
        public void IsChanged_ProjectItemsChangeThenChangeBack_False()
        {
            var ts = CreateDummyTimesheet();
            var originalName = ts.ProjectTimeItems[0].TaskCode.Name;
            ts.ProjectTimeItems[0].TaskCode.Name = "new name";
            Assert.IsTrue(ts.IsChanged);
            ts.ProjectTimeItems[0].TaskCode.Name = originalName;
            Assert.IsFalse(ts.IsChanged);
        }

        // Collection of observable items - add item
        [TestMethod]
        public void IsChanged_ProjectItemsAdd_True()
        {
            var ts = CreateDummyTimesheet();
            var item = new ProjectTaskTimesheetItem(new PickListItem(10, "item1"), new PickListItem(20, "item2"));
            ts.ProjectTimeItems.Add(new ObservableProjectTaskTimesheetItem(item));
            Assert.IsTrue(ts.IsChanged);
        }

        // Collection of observable items - add item, accept changes
        [TestMethod]
        public void IsChanged_ProjectItemsAddAccept_False()
        {
            var ts = CreateDummyTimesheet();
            var item = new ProjectTaskTimesheetItem(new PickListItem(10, "item1"), new PickListItem(20, "item2"));
            ts.ProjectTimeItems.Add(new ObservableProjectTaskTimesheetItem(item));
            ts.AcceptChanges();
            Assert.IsFalse(ts.IsChanged);
        }

        // Collection of observable items - add item, accept changes, change item (value type property)
        [TestMethod]
        public void IsChanged_ProjectItemsAddAcceptChange_True()
        {

            var ts = CreateDummyTimesheet();
            var item = new ProjectTaskTimesheetItem(new PickListItem(10, "item1"), new PickListItem(20, "item2"));
            ts.ProjectTimeItems.Add(new ObservableProjectTaskTimesheetItem(item));
            ts.AcceptChanges();
            ts.ProjectTimeItems[0].TaskCode = new ObservablePickListItem(new PickListItem(2121, "blah"));
            Assert.IsTrue(ts.IsChanged);
        }

        #region Totals
        [TestMethod]
        public void CalculateTotals_LoggedTimeProject1EachDay_CorrectTotalsUnchangedItems()
        {
            var ts = new Timesheet();
            var projectCode = new PickListItem(1, "pc1");
            var taskCode = new PickListItem(1, "tc1");
            var projectItem = new ProjectTaskTimesheetItem(projectCode, taskCode);
            
            for(int i=0; i < 7; i++)
            {
                projectItem.TimeEntries[i].LoggedTime = TimeSpan.FromHours(1);
            }

            ts.ProjectTimeItems.Add(projectItem);

            for (int i = 0; i < 7; i++ )
            {
                ts.RequiredHours[i] = TimeSpan.FromHours(7.5);
            }

            var timesheet = new ObservableTimesheet(ts);
            timesheet.CalculateTotals();
        }
        #endregion

    }
}
