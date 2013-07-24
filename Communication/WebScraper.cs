﻿using Communication.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using Model;

namespace Communication
{
    /// <summary>
    /// Based on odetocode.com/articles/162.aspx
    /// </summary>
    public class WebScraper
    {
        private readonly Uri LOGINPAGE;
        private readonly Uri TIMESHEETPAGE;

        private readonly string USERNAME;
        private readonly string PASSWORD;

        private const string USERNAMECONTROLNAME = "txtUsername";
        private const string PASSWORDCONTROLNAME = "txtPassword";
        private const string LOGINBUTTONCONTROLNAME = "btnLogin";

        private readonly IHtmlParser _parser;
        private string _viewState;
        private readonly CookieContainer _cookies;

        public WebScraper(IHtmlParser parser)
        {
            var baseUri = new Uri(Resources.baseUri);
            LOGINPAGE = new Uri(baseUri, Resources.loginPage);
            TIMESHEETPAGE = new Uri(baseUri,Resources.timesheetPage);
            USERNAME = Resources.username;
            PASSWORD = Resources.password;
            _parser = parser;
            _cookies = new CookieContainer();
            var policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            HttpWebRequest.DefaultCachePolicy = policy;
        }

        /// <summary>
        /// Submit user credentials to the login page, store the auth cookies in the cookie container and return a timesheet
        /// </summary>
        public Timesheet LoginAndGetTimesheet()
        {
            var webRequest = WebRequest.Create(LOGINPAGE) as HttpWebRequest;
            string responseData = GetResponseData(webRequest);

            _viewState = _parser.ParseViewState(responseData);
            //_viewState = _parser.ParseViewState(responseData);

            string postData = HtmlHelper.ConstructQuerystring(HtmlHelper.VIEWSTATENAME, _viewState,
                                                                USERNAMECONTROLNAME, USERNAME, 
                                                                PASSWORDCONTROLNAME, PASSWORD, 
                                                                LOGINBUTTONCONTROLNAME, "Login");

            responseData = GetWebResponse(LOGINPAGE, _cookies, postData);

            var timesheet = _parser.ParseTimesheet(responseData, out _viewState);
            return timesheet;
        }

        /// <summary>
        /// Returns the reponse data as a string from the webRequest
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
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
        /// Return HTTP response as a string
        /// </summary>
        /// <param name="webpageUri"></param>
        /// <param name="authCookies"></param>
        /// <returns></returns>
        private string GetWebResponse(Uri webpageUri, CookieContainer authCookies, string postData = "")
        {
            var webRequest = WebRequest.Create(webpageUri) as HttpWebRequest;
            webRequest.CookieContainer = authCookies;
            webRequest.Method = string.IsNullOrEmpty(postData) ? "GET" : "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(postData))
            {
                // write the form values into the request message
                var requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(postData);
                requestWriter.Close();
            }

            var response = GetResponseData(webRequest);
            return response;
        }

        ///// <summary>
        ///// Constructs a Timesheet object from the contents of the HTML page
        ///// </summary>
        ///// <returns></returns>
        //public Timesheet GetTimesheet()
        //{
        //    var parser = new HtmlParser();
        //    var response = GetWebResponse(TIMESHEETPAGE, _cookies);

        //    Timesheet timesheet = parser.ParseTimesheet(response, out _viewState);

        //    return timesheet;
        //}

        /// <summary>
        /// Submit timesheet changes and read in the newly saved timesheet
        /// </summary>
        /// <param name="timesheet">timesheet with changes</param>
        /// <returns></returns>
        public Timesheet UpdateTimeSheet(Timesheet timesheet)
        {
            if (_cookies.Count == 0)
            {
                throw new InvalidOperationException("Cannot update timesheet as login cookies have not been set");    
            }

            string postData = HtmlHelper.ConstructQuerystring(
                                                       //VIEWSTATENAME, _viewState,  // We don't seem to need the viewstate - cool!
                                                      USERNAMECONTROLNAME, USERNAME,
                                                      PASSWORDCONTROLNAME, PASSWORD,
                                                      "__EVENTTARGET", "ctl00$btnSave",
                                                      "ctl00$hSave", "Y",
                                                      "ctl00$C1$hdnTimesheetId", timesheet.TimesheetId);

            var changes = ExtractChanges(timesheet);
            var changesQueryString = HtmlHelper.ConstructQuerystring(changes);
            postData += "&" + changesQueryString;
           
            // post to the login form
            var webRequest = WebRequest.Create(TIMESHEETPAGE) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = _cookies;

            // write the form values into the request message
            var requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postData);
            requestWriter.Close();

            string reponseData;

            using (var responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                reponseData = responseReader.ReadToEnd();
            }

            var parser = new HtmlParser();
            var updatedTimesheet = parser.ParseTimesheet(reponseData, out _viewState);

            return updatedTimesheet;
        }

        /// <summary>
        /// Finds all timesheet items that have changes and returns the new values string pairs
        /// </summary>
        /// <returns></returns>
        private Dictionary<string,string> ExtractChanges(Timesheet timesheet)
        {

            var changes = new Dictionary<string, string> {{ "ctl00$C1$ProjectGrid$ctl02$txtLoggedTime0", "3:20" }};
            return changes;
        }
    }
}
