using Model;

namespace Communication
{
    /// <summary>
    /// TODO Move to a more sensible project and extract interface from Timesheet (so we don't need a reference to Communication)
    /// Webscraper is responsible to sending and receiving data to the website (and maintaining viewstate etc)
    /// Based on odetocode.com/articles/162.aspx
    /// </summary>
    public interface IWebScraper
    {
        /// <summary>
        /// Submit user credentials to the login page, store the auth cookies in the cookie container and return a timesheet
        /// </summary>
        ObservableTimesheet LoginAndGetTimesheet();

        /// <summary>
        /// Submit timesheet changes and read in the newly saved timesheet
        /// </summary>
        /// <param name="timesheet">timesheet with changes</param>
        /// <returns></returns>
        ObservableTimesheet UpdateTimeSheet(ObservableTimesheet timesheet);

        /// <summary>
        /// Gets a specific timesheet by timesheet ID
        /// </summary>
        /// <param name="timesheetId"></param>
        /// <returns></returns>
        ObservableTimesheet GetTimesheet(string timesheetId);


        /// <summary>
        /// Get the Timesheet History page as a string
        /// </summary>
        /// <returns></returns>
        string GetTimesheetHistoryView();
    }
}