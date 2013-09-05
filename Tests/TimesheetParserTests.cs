using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autofac;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Model;
using Shared;

namespace Tests
{

    /// <summary>
    // 
    //      PROJECT ITEMS
    //      Project: 20198-LCS-DEV-ILB-2.1.0 (15493), Task: Coding - Roadmap (348975)
    //      10/07/2013 00:01:00,10/07/2013 00:02:00,10/07/2013 00:03:00,10/07/2013 00:04:00,10/07/2013 00:05:00,10/07/2013 00:06:00,01/01/0001 00:00:00,
    //      Project: 20198-LCS-DEV-ILB-2.1.0 (15493), Task: Ceremonies - Dev (354628)
    //      10/07/2013 00:11:00,10/07/2013 00:12:00,10/07/2013 00:13:00,10/07/2013 00:14:00,10/07/2013 00:15:00,01/01/0001 00:00:00,10/07/2013 00:17:00,
    //      Project: 20198-LCS-DEV-ILB-2.1.0 (15493), Task: Planning - Dev - Roadmap (348966)
    //      10/07/2013 00:21:00,10/07/2013 00:22:00,10/07/2013 00:23:00,10/07/2013 00:24:00,10/07/2013 00:25:00,10/07/2013 00:26:00,01/01/0001 00:00:00,
    //      Project: 20198-LCS-DEV-ILB-2.1.0 (15493), Task: Design - Dev - Roadmap (348970)
    //      10/07/2013 00:31:00,10/07/2013 00:32:00,10/07/2013 00:33:00,10/07/2013 00:34:00,10/07/2013 00:35:00,01/01/0001 00:00:00,10/07/2013 00:37:00,
    //      Project: 20198-LCS-DEV-ILB-2.1.0 (15493), Task: Build/Deploy (348988)
    //      10/07/2013 00:41:00,10/07/2013 00:42:00,10/07/2013 00:43:00,10/07/2013 00:44:00,10/07/2013 00:45:00,10/07/2013 00:46:00,01/01/0001 00:00:00,
    //      Project: 20198-LCS-DEV-ILB-2.1.0 (15493), Task: Handover - Dev (348998)
    //      10/07/2013 00:51:00,10/07/2013 00:52:00,10/07/2013 00:53:00,10/07/2013 00:54:00,10/07/2013 00:55:00,01/01/0001 00:00:00,10/07/2013 00:57:00,
    //      Project: 20598-ABS-TS-TC-Jade Hosting migration (16231), Task: Project Planning (363589)
    //      10/07/2013 01:01:00,10/07/2013 01:02:00,10/07/2013 01:03:00,10/07/2013 01:04:00,10/07/2013 01:05:00,10/07/2013 01:06:00,01/01/0001 00:00:00,
    //
    //      NON-PROJECT ITEMS
    //      Project: INT0011-INT - Admin (12), Task:  --not set-- (0)
    //      10/07/2013 00:01:00,10/07/2013 00:01:00,10/07/2013 00:01:00,10/07/2013 00:01:00,10/07/2013 00:01:00,10/07/2013 00:01:00,10/07/2013 00:01:00,
    //      Project: INT0001-INT - Holiday (Personal) (3), Task:  --not set-- (0)
    //      10/07/2013 00:02:00,10/07/2013 00:02:00,10/07/2013 00:02:00,10/07/2013 00:02:00,10/07/2013 00:02:00,10/07/2013 00:02:00,10/07/2013 00:02:00,
    //      Project: INT0008-INT - Internal Meeting (6), Task:  --not set-- (0)
    //      10/07/2013 00:03:00,10/07/2013 00:03:00,10/07/2013 00:03:00,10/07/2013 00:03:00,10/07/2013 00:03:00,10/07/2013 00:03:00,10/07/2013 00:03:00,
    //
    /// </summary>
    
    [TestClass]
    public class TimesheetParserTests
    {
        private IContainer _container ;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ContainerBuilder(); 
            builder.RegisterModule<CommunicationModule>();
            _container = builder.Build();
        }

        /// <summary>
        /// Make sure all 7 project time entries are read
        /// </summary>
        [TestMethod]
        [DeploymentItem("TestTimeSheet.htm")]
        public void Parse_FullTimesheet_ProjectTimeEntries()
        {
            Timesheet timesheet;
            string viewState = string.Empty;

            using (var streamReader = new StreamReader("TestTimeSheet.htm"))
            {
                var parser = _container.Resolve<IHtmlParser>();
                var htmlString  = streamReader.ReadToEnd();
                timesheet = parser.ParseTimesheet(htmlString, out viewState);
            }

            Assert.IsNotNull(timesheet);
            Assert.IsNotNull(timesheet.ProjectTimeItems);
            Assert.AreEqual(7, timesheet.ProjectTimeItems.Count);
            Assert.AreEqual("60621", timesheet.TimesheetId);
            TestHelper.PrettyPrintTimesheet(new ObservableTimesheet(timesheet));

        }

        [TestMethod]
        [DeploymentItem("TimesheetHistoryView.htm")]
        public void GetLatestTimesheetId_TimesheetHistoryHtml_LatestTimesheet()
        {

            using (var streamReader = new StreamReader("TimesheetHistoryView.htm"))
            {
                var parser = _container.Resolve<IHtmlParser>();
                var htmlString = streamReader.ReadToEnd();
                var latestTimesheetId = parser.GetLatestTimesheetId(htmlString);
                Assert.AreEqual("61701",latestTimesheetId.Id);
                Assert.AreEqual("12 Aug 2013", latestTimesheetId.DateString);
            }
        }


        [TestMethod]
        [DeploymentItem("TestApprovedTimesheet.htm")]
        public void ParseApprovedTimesheet_ApprovedTimesheetHtml_Timesheet()
        {

            using (var streamReader = new StreamReader("TimesheetHistoryView.htm"))
            {
                var parser = _container.Resolve<IHtmlParser>();
                var htmlString = streamReader.ReadToEnd();
                string viewState;
                var approvedTimesheet = parser.ParseTimesheet(htmlString, out viewState);
                Assert.AreEqual("61701", approvedTimesheet.TimesheetId);
                Assert.AreEqual("12 Aug 2013 to 18 Aug 2013 by Pete Johnson (Approved)", approvedTimesheet.Title);
            }
        }
    }
}
