using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CommunicationTests
    {
        [TestMethod]
        public void Scrape_GetProjectCodes_4ProjectCodes()
        {
            var scraper = new Scraper();
            scraper.Scrape();
        }
    }



}
