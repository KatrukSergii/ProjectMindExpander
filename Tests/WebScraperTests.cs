using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using Autofac;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    /// <summary>
    /// Summary description for WebScraperTests
    /// </summary>
    [TestClass]
    public class WebScraperTests
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
        
        /// <summary>
        /// INTEGRATION TEST
        /// </summary>
        [TestMethod]
        public void GetTimesheet_ValidTimesheetId_CorrectTimesheet()
        {
            var scraper = _container.Resolve<IWebScraper>(new NamedParameter("username", _username), new NamedParameter("password", _password));
            var timesheet = scraper.LoginAndGetTimesheet();
            var timesheethistoryView = scraper.GetTimesheetHistoryView(); // this updates the viewstate
            var oldTimesheet = scraper.GetApprovedTimesheet("61701"); // Note - it is possible to view other people's timesheets by sending a valid timesheet ID here
            Assert.IsNotNull(oldTimesheet);
            Assert.AreEqual("61701",oldTimesheet.TimesheetId);
        }

        /// <summary>
        /// INTEGRATION TEST
        /// </summary>
        [TestMethod]
        public void GetTimesheet_InvalidTimesheetId_EmptyTimesheet()
        {
            var scraper = _container.Resolve<IWebScraper>(new NamedParameter("username", _username), new NamedParameter("password", _password));
            var timesheet = scraper.LoginAndGetTimesheet();
            var timesheethistoryView = scraper.GetTimesheetHistoryView(); // this updates the viewstate
            try
            {
                var oldTimesheet = scraper.GetApprovedTimesheet("99999");
            }
            catch (ApplicationException ex)
            {
                Assert.AreEqual("The Timesheet has an invalid ID", ex.Message);
            }
        }
    }
}
