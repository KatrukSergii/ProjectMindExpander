using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Communication.Properties;
using HtmlAgilityPack;
using Shared;
using Shared.Interfaces;

namespace Communication
{
    /// <summary>
    /// Based on odetocode.com/articles/162.aspx
    /// </summary>
    public class WebScraper
    {
        private string _viewState;

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

        private ITimesheetParser _parser;

        public WebScraper(ITimesheetParser parser)
        {
            var baseUri = new Uri(Resources.baseUri);
            LOGINPAGE = new Uri(baseUri, Resources.loginPage);
            TIMESHEETPAGE = new Uri(baseUri,Resources.timesheetPage);
            USERNAME = Resources.username;
            PASSWORD = Resources.password;
            _parser = parser;
        }

        /// <summary>
        /// Reads the Viewstate hidden field from the HTML page
        /// </summary>
        /// <param name="s"></param>
        /// <returns>string representing the viewstate</returns>
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
        public CookieContainer LoginAndGetCookies()
        {
            // first, request the login form to get the viewstate value
            var webRequest = WebRequest.Create(LOGINPAGE) as HttpWebRequest;
            string responseData = GetResponseData(webRequest);

            // extract the viewstate value and build out POST data
            _viewState = ExtractViewState(responseData);
            string postData =
                String.Format(POSTDATATEMPLATE,
                            VIEWSTATENAME, _viewState, USERNAMECONTROLNAME, USERNAME, PASSWORDCONTROLNAME, PASSWORD, LOGINBUTTONCONTROLNAME);

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
            responseData = GetResponseData(webRequest);
            _viewState = ExtractViewState(responseData);
            //webRequest.GetResponse().Close();

            return cookies;
        }

        private string GetResponseData(HttpWebRequest webRequest)
        {
            string responseData;
            
            using (var webResponse = webRequest.GetResponse())
            {
                using (var responseReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    responseReader.Close();
                }

                webResponse.Close();
            }

            return responseData;
        }

        /// <summary>
        /// Constructs a Timesheet object from the contents of the HTML page
        /// </summary>
        /// <param name="authCookies"></param>
        /// <returns></returns>
        public Timesheet GetTimesheet(CookieContainer authCookies)
        {
            //// now we can send out cookie along with a request for the protected page
            var webRequest = WebRequest.Create(TIMESHEETPAGE) as HttpWebRequest;
            webRequest.CookieContainer = authCookies;

            var parser = new TimesheetParser();
            //var timesheet = parser.Parse(webRequest.GetResponse().GetResponseStream());

            var timesheet = new Timesheet();

            using (var responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                timesheet = parser.Parse(responseReader);
            }

            return timesheet;
        }

        
        /// <summary>
        /// Create a string for the postdata, e.g. param1=value1&param2=value2...&btnLogin=Login"
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string ConstructPostDataString(params string[] values)
        {
            if (values.Length % 2 != 0)
            {
                throw new ArgumentOutOfRangeException("There should be an even number of arguments for constructing the post data string"));
            }

            var sb = new StringBuilder();

            var counter = 0;

            for (int i = 0; i < values.Length; i++)
            {
                sb.Append(values[i]);

                if (counter == 0)
                {

                    sb.Append("=");
                }
                else if (counter == 1)
                {
                    sb.Append("&");
                }
                    
                if (i != values.Length - 1)
                {
                            
                }
                
                counter = i%2;

            }

            //"{0}={1}&{2}={3}&{4}={5}&{6}=Login";
            return sb.ToString();
        }


        public void UpdateTimeSheet(Timesheet timesheet, CookieContainer authCookies)
        {
            // first, request the login form to get the viewstate value
            var webRequest = WebRequest.Create(TIMESHEETPAGE) as HttpWebRequest;
            var responseReader = new StreamReader(
                webRequest.GetResponse().GetResponseStream()
                );
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();

            // extract the viewstate value and build out POST data
            _viewState = ExtractViewState(responseData);
            string postData =
                String.Format(POSTDATATEMPLATE,
                            VIEWSTATENAME, _viewState, USERNAMECONTROLNAME, USERNAME, PASSWORDCONTROLNAME, PASSWORD, LOGINBUTTONCONTROLNAME);

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

            

            //// Save single update
            //__EVENTTARGET	ctl00$btnSave
            //__VIEWSTATE	/wEPDwULLTEzODU1ODk3MjEPZBYCZg9kFgQCAQ9kFgICCA8WAh4HVmlzaWJsZWdkAgMPZBYUAgUPDxYCHwBoZBYMAgMPZBYCAgEPDxYCHgtOYXZpZ2F0ZVVybAUiaHR0cDovL3N1cHBvcnQuYXBwbGUuY29tL2tiL2RsMTUzMWRkAgUPZBYCAgEPDxYCHwEFM2h0dHA6Ly93d3cubWFjdXBkYXRlLmNvbS9hcHAvbWFjLzE1Njc1L2FwcGxlLXNhZmFyaWRkAgcPZBYCAgEPDxYCHwEFQGh0dHA6Ly93aW5kb3dzLm1pY3Jvc29mdC5jb20vZW4tdXMvaW50ZXJuZXQtZXhwbG9yZXIvZG93bmxvYWQtaWVkZAIJD2QWAgIBDw8WAh8BBU1odHRwOi8vaGVscC5wcm9qZWN0bWluZGVyLmNvbS9wcm9qZWN0bWluZGVyX3N1cHBvcnQvQnJvd3Nlcl9jb21wYXRpYmlsaXR5Lmh0bWRkAgsPZBYCAgEPDxYCHwEFQGh0dHA6Ly93aW5kb3dzLm1pY3Jvc29mdC5jb20vZW4tdXMvaW50ZXJuZXQtZXhwbG9yZXIvZG93bmxvYWQtaWVkZAIND2QWAgIBDw8WAh8BBTNodHRwOi8vd3d3Lm1hY3VwZGF0ZS5jb20vYXBwL21hYy8xNTY3NS9hcHBsZS1zYWZhcmlkZAIHDxYCHgtfIUl0ZW1Db3VudGZkAgkPZBYMAgMPFgIfAGhkAgcPDxYCHgRUZXh0BQxQZXRlIEpvaG5zb25kZAILDw9kFgIeB29uY2xpY2sFMHJldHVybiBTaG93Q2hhbmdlVXNlckxvZ2luUGFzc3dvcmREaWFsb2coZXZlbnQpO2QCDQ9kFgJmDxYCHwICBBYIZg9kFgJmDxUCUmh0dHA6Ly9oZWxwLnByb2plY3RtaW5kZXIuY29tL1BST0pFQ1RtaW5kZXJIZWxwLmh0bSNyZWxlYXNlX25vdGVzL05ld19mZWF0dXJlcy5odG0NTmV3IEZlYXR1cmVzIWQCAQ9kFgJmDxUCYGh0dHA6Ly9oZWxwLnByb2plY3RtaW5kZXIuY29tL1BST0pFQ1RtaW5kZXJIZWxwLmh0bSNwcm9qZWN0bWluZGVyX3N1cHBvcnQvSGludHMsX3RpcHNfJl9uZXdzLmh0bRJIaW50cywgVGlwcyAmIE5ld3NkAgIPZBYCZg8VAipodHRwOi8vcHJvamVjdC1zb2x1dGlvbnMuaXJpcy5jby51ay9mb3J1bS8MT25saW5lIEZvcnVtZAIDD2QWAmYPFQIzaHR0cDovL2hlbHAucHJvamVjdG1pbmRlci5jb20vUHJvamVjdE1pbmRlckhlbHAuaHRtE0hlbHAgJmFtcDsgVHJhaW5pbmdkAg8PDxYCHwBoZGQCEQ8WAh8AZxYEAgEPD2QWBB8EBRtyZXR1cm4gTGF1bmNoQ29udGV4dEhlbHAoKTseEGNvbnRleHQtaGVscC11cmwFPmh0dHA6Ly9oZWxwLnByb2plY3RtaW5kZXIuY29tL3RpbWVzaGVldHMvQ3VycmVudF9UaW1lc2hlZXQuaHRtZAICDw9kFgQfBAUbcmV0dXJuIExhdW5jaENvbnRleHRIZWxwKCk7HwUFPmh0dHA6Ly9oZWxwLnByb2plY3RtaW5kZXIuY29tL3RpbWVzaGVldHMvQ3VycmVudF9UaW1lc2hlZXQuaHRtZAINDxYCHwMFCVRpbWVzaGVldGQCDw8WAh8DBTIwMSBKdWwgMjAxMyB0byAwNyBKdWwgMjAxMyBieSBQZXRlIEpvaG5zb24gKERyYWZ0KWQCEQ9kFgICBA9kFgJmDw8WBB4HVG9vbFRpcAUaQ2xpY2sgdG8gc2F2ZSB5b3VyIGNoYW5nZXMeDU9uQ2xpZW50Q2xpY2sFG3JldHVybiBWYWxpZGF0ZUZvcm0oZXZlbnQpOxYCHhBkYXRhLXNhdmUtYnV0dG9uBQR0cnVlFgICAg8WAh4JaW5uZXJodG1sBQRTYXZlZAIXD2QWAgIBDxYCHwQFZXJldHVybiBzaG93UmVwb3J0UG9wdXAoJy9QUk9KRUNUbWluZGVyL1ByaW50YWJsZSBSZXBvcnRzJywgJ1ByaW50IFRpbWVzaGVldCcsICd0aW1lc2hlZXRpZCcsJzYwMjcxJyk7ZAIlD2QWFGYPZBYOZg8PFgIfAwUFRHJhZnRkZAIBDxYCHwBoFgICAQ8PFgIfAwURMDEgSmFuIDE5MDAgMDA6MDBkZAICD2QWAgIBDw8WAh8DBQ1TaGFoaWQgTmF2ZWVkZGQCAw8WAh8AaBYCAgMPFgIfAGgWAgIBDw8WAh8DBQEgZGQCBA8WAh8AaGQCBQ8WAh8AaGQCBg8W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
            //ctl00$hSave	Y
            //ctl00$C1$hdnTimesheetId	60271
            //ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0	6:20

        }
    }
}
