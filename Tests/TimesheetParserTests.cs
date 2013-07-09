using System;
using System.IO;
using Autofac;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Shared.Interfaces;

namespace Tests
{
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
        [DeploymentItemAttribute("TestTimeSheet.htm")]
        public void Parse_FullTimesheet_ProjectTimeEntries()
        {
            Timesheet timesheet;
            using (var streamReader = new StreamReader("TestTimeSheet.htm"))
            {
                var parser = _container.Resolve<ITimesheetParser>();
                timesheet = parser.Parse(streamReader);
            }


            Assert.IsNotNull(timesheet);
            Assert.IsNotNull(timesheet.ProjectTimeItems);
            Assert.AreEqual(7, timesheet.ProjectTimeItems.Count);

            foreach (var projectTimeItem in timesheet.ProjectTimeItems)
            {
                Console.WriteLine(string.Format("Project: {0} ({1}), Task: {2} ({3})", projectTimeItem.ProjectCode.Name, projectTimeItem.ProjectCode.Value, projectTimeItem.TaskCode.Name, projectTimeItem.TaskCode.Value));
                for (int i = 0; i < 7; i++)
                {
                    Console.Write(projectTimeItem[i].LoggedTime.ToString() + ",");
                }
                Console.WriteLine("");
            }

        }

    }
}
