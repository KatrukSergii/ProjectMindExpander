using System;
using System.Configuration;
using Autofac;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
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
        private string _username;
        private string _password;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommunicationModule>();
            _container = builder.Build();
            _username = ConfigurationManager.AppSettings["username"];
            _password = ConfigurationManager.AppSettings["password"];
        }

        [TestMethod]
        public void GetTimesheet_GetProjectCodes_4ProjectCodes()
        {
            var timesheetParser = new HtmlParser();
            var scraper = new WebScraper(timesheetParser, _username, _password);
            var timesheet = scraper.LoginAndGetTimesheet();
            Assert.IsNotNull(timesheet);
            Assert.AreEqual(37.5, timesheet.TotalRequiredHours.TotalHours);
            TestHelper.PrettyPrintTimesheet(timesheet);
        }

        /// <summary>
        /// Adds 1 minutes onto the logged time for project 1 on Sunday
        /// </summary>
        [TestMethod]
        public void UpdateTimeSheet_UpdatedTimesheetValues_UpdateValuesInTheResponse()
        {
            var timesheetParser = new HtmlParser();
            var scraper = new WebScraper(timesheetParser, _username, _password);
            var timesheet = scraper.LoginAndGetTimesheet();
            TestHelper.PrettyPrintTimesheet(timesheet);
            
            // project 1 Sunday
            var originalTime = timesheet.ProjectTimeItems[0].TimeEntries[6].LoggedTime;
            timesheet.ProjectTimeItems[0].TimeEntries[6].LoggedTime = originalTime.Add(TimeSpan.FromMinutes(1));

            var updatedTimesheet = scraper.UpdateTimeSheet(timesheet);
            TestHelper.PrettyPrintTimesheet(updatedTimesheet);

            Assert.AreNotEqual(timesheet, updatedTimesheet);
            Assert.AreEqual(originalTime.Add(TimeSpan.FromMinutes(1)).TotalMinutes, timesheet.ProjectTimeItems[0].TimeEntries[6].LoggedTime.TotalMinutes);
        }  

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetLastTimesheet_IncorrectLogin_InvalidOperationException()
        {
            var timesheetParser = new HtmlParser();
            var scraper = new WebScraper(timesheetParser, _username, _password);
            try
            {
                _username = "foo";
                _password = "bar";
                var timesheet = scraper.LoginAndGetTimesheet();
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Unable to login", ex.Message);
            }
        }
    }



}
