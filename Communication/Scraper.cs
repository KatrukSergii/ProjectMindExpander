using System;
using System.IO;
using System.Net;
using System.Web;
using Communication.Properties;
using HtmlAgilityPack;
using Shared;

namespace Communication
{
    /// <summary>
    /// Based on odetocode.com/articles/162.aspx
    /// </summary>
    public class Scraper
    {
        private readonly Uri LOGINPAGE;
        private readonly Uri TIMESHEETPAGE;

        private readonly string USERNAME;
        private readonly string PASSWORD;

        private const string VIEWSTATENAME = "__VIEWSTATE";
        private const string VALUEDELIMITER = "value=\"";
        private const string USERNAMECONTROLNAME = "txtUsername";
        private const string PASSWORDCONTROLNAME = "txtPassword";
        private const string LOGINBUTTONCONTROLNAME = "btnLogin";
        private const string POSTDATATEMPLATE = "{0}={1}&{2}={3}&{4}={5}&{6}=Login";
        
        public Scraper()
        {
            var baseUri = new Uri(Resources.baseUri);
            LOGINPAGE = new Uri(baseUri, Resources.loginPage);
            TIMESHEETPAGE = new Uri(baseUri,Resources.timesheetPage);
            USERNAME = Resources.username;
            PASSWORD = Resources.password;
        }

        private string ExtractViewState(string s)
        {

            var viewStateNamePosition = s.IndexOf(VIEWSTATENAME, StringComparison.Ordinal);
            var viewStateValuePosition = s.IndexOf(VALUEDELIMITER, viewStateNamePosition, StringComparison.Ordinal);

            var viewStateStartPosition = viewStateValuePosition + VALUEDELIMITER.Length;
            var viewStateEndPosition = s.IndexOf("\"", viewStateStartPosition, StringComparison.Ordinal);
            
            return HttpUtility.UrlEncodeUnicode(
                     s.Substring(
                        viewStateStartPosition,
                        viewStateEndPosition - viewStateStartPosition
                     )
                  );
        }

        /// <summary>
        /// Submit user credentials to the login page and then store the auth cookies in the cookie container
        /// </summary>
        private CookieContainer LoginAndGetCookies()
        {
            // first, request the login form to get the viewstate value

            var webRequest = WebRequest.Create(LOGINPAGE) as HttpWebRequest;
            var responseReader = new StreamReader(
                webRequest.GetResponse().GetResponseStream()
                );
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();

            // extract the viewstate value and build out POST data
            string viewState = ExtractViewState(responseData);
            string postData =
                String.Format(POSTDATATEMPLATE,
                            VIEWSTATENAME, viewState, USERNAMECONTROLNAME, USERNAME, PASSWORDCONTROLNAME, PASSWORD, LOGINBUTTONCONTROLNAME);

            // have a cookie container ready to receive the forms auth cookie
            var cookies = new CookieContainer();

            // post to the login form
            webRequest = WebRequest.Create(LOGINPAGE) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = cookies;

            // write the form values into the request message
            var requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postData);
            requestWriter.Close();

            // we don't need the contents of the response, just the cookie it issues
            webRequest.GetResponse().Close();

            return cookies;
        }

        private Timesheet GetTimeSheetData(CookieContainer authCookies)
        {
            //// now we can send out cookie along with a request for the protected page
            var webRequest = WebRequest.Create(TIMESHEETPAGE) as HttpWebRequest;
            webRequest.CookieContainer = authCookies;

            var timesheet = new Timesheet();

            using (var responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {

                HtmlNode.ElementsFlags.Remove("option");
                var htmlDoc = new HtmlDocument();
                htmlDoc.Load(responseReader);
                if (htmlDoc.DocumentNode != null)
                {
                    // Project Codes
                    foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//select[@name='ctl00$C1$ProjectGrid$ctl02$ddlProject']/option"))
                    {
                        timesheet.ProjectCodes.Add(new PickListItem(int.Parse(link.Attributes["value"].Value), link.InnerText));
                    }

                }
            }

            return timesheet;
        }

        public void Scrape()
        {
            var authCookies = LoginAndGetCookies();
            GetTimeSheetData(authCookies);
        }
    }
}
