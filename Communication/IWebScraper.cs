using Model;

namespace Communication
{
    /// <summary>
    /// TODO Move somewhere sensible and extract interface from Timesheet (so we don't need a reference to Communication)
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

    }
}