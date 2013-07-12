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
        [TestMethod]
        public void GetTimesheet_GetProjectCodes_4ProjectCodes()
        {
            var timesheetParser = new TimesheetParser();
            var scraper = new WebScraper(timesheetParser);
            var authCookies = scraper.LoginAndGetCookies();
            var timesheet = scraper.GetTimesheet(authCookies);
            Assert.IsNotNull(timesheet);
            Assert.AreEqual(37.5, timesheet.TotalRequiredHours.TotalHours);
        }

        [TestMethod]
        public void UpdateTimeSheet_UpdatedTimesheetValues_UpdateValuesInTheResponse()
        {
            var timesheetParser = new TimesheetParser();
            var scraper = new WebScraper(timesheetParser);
            var authCookies = scraper.LoginAndGetCookies();
            var timesheet = new Timesheet();
            timesheet = scraper.UpdateTimeSheet(timesheet, authCookies);
        }  
    }



}
