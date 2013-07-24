using System;
using Autofac;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;

namespace Tests
{
    /// <summary>
    /// Integration tests (because of asp.net authentication/ viewstate etc)
    /// </summary>
    [TestClass]
    public class CommunicationTests
    {
        private IContainer _container;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommunicationModule>();
            _container = builder.Build();
        }

        [TestMethod]
        public void GetTimesheet_GetProjectCodes_4ProjectCodes()
        {
            var timesheetParser = new HtmlParser();
            var scraper = new WebScraper(timesheetParser);
            var timesheet = scraper.LoginAndGetTimesheet();
            Assert.IsNotNull(timesheet);
            Assert.AreEqual(37.5, timesheet.TotalRequiredHours.TotalHours);
            TestHelper.PrettyPrintTimesheet(timesheet);
        }

        [TestMethod]
        public void UpdateTimeSheet_UpdatedTimesheetValues_UpdateValuesInTheResponse()
        {
            var timesheetParser = new HtmlParser();
            var scraper = new WebScraper(timesheetParser);
            var timesheet = scraper.LoginAndGetTimesheet();
            TestHelper.PrettyPrintTimesheet(timesheet);
            
            // project 1 monday
            timesheet.ProjectTimeItems[0].TimeEntries[0].LoggedTime = new TimeSpan(1,0,0);

            var updatedTimesheet = scraper.UpdateTimeSheet(timesheet);
            TestHelper.PrettyPrintTimesheet(updatedTimesheet);
        }  

    }



}
