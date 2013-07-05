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
        public void Test1()
        {
            var scraper = new Scraper();
            scraper.Scrape();
        }

        ////private string LOGIN_URL = "https://secure.projectminder.com/LoginView.aspx?ReturnUrl=%2fTimesheets.aspx";
        ////private string SECRET_PAGE_URL = "https://secure.projectminder.com/TimesheetView.aspx";
        ////private string USERNAME = "pete.johnson@TESTiris.co.ukTEST";
        ////private string PASSWORD = "TOSTpassword123TOST";

        //private string ExtractViewState(string s)
        //{
        //    string viewStateNameDelimiter = "__VIEWSTATE";
        //    string valueDelimiter = "value=\"";

        //    int viewStateNamePosition = s.IndexOf(viewStateNameDelimiter);
        //    int viewStateValuePosition = s.IndexOf(valueDelimiter, viewStateNamePosition);

        //    int viewStateStartPosition = viewStateValuePosition + valueDelimiter.Length;
        //    int viewStateEndPosition = s.IndexOf("\"", viewStateStartPosition);

        //    return HttpUtility.UrlEncodeUnicode(
        //             s.Substring(
        //                viewStateStartPosition,
        //                viewStateEndPosition - viewStateStartPosition
        //             )
        //          );
        //}

        //[TestMethod]
        //public void Test()
        //{

        //    // first, request the login form to get the viewstate value

        //    var webRequest = WebRequest.Create(LOGIN_URL) as HttpWebRequest;
        //    var responseReader = new StreamReader(
        //          webRequest.GetResponse().GetResponseStream()
        //       );
        //    string responseData = responseReader.ReadToEnd();
        //    responseReader.Close();

        //    // extract the viewstate value and build out POST data
        //    string viewState = ExtractViewState(responseData);
        //    string postData =
        //          String.Format(
        //             "__VIEWSTATE={0}&txtUsername={1}&txtPassword={2}&btnLogin=Login",
        //             viewState, USERNAME, PASSWORD
        //          );

        //    // have a cookie container ready to receive the forms auth cookie
        //    CookieContainer cookies = new CookieContainer();

        //    // now post to the login form
        //    webRequest = WebRequest.Create(LOGIN_URL) as HttpWebRequest;
        //    webRequest.Method = "POST";
        //    webRequest.ContentType = "application/x-www-form-urlencoded";
        //    webRequest.CookieContainer = cookies;

        //    // write the form values into the request message
        //    StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
        //    requestWriter.Write(postData);
        //    requestWriter.Close();

        //    // we don't need the contents of the response, just the cookie it issues
        //    webRequest.GetResponse().Close();

        //    //// now we can send out cookie along with a request for the protected page
        //    webRequest = WebRequest.Create(SECRET_PAGE_URL) as HttpWebRequest;
        //    //webRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(USERNAME + ":" + PASSWORD)));
        //    webRequest.CookieContainer = cookies;

        //    responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());

        //    // and read the response
        //    responseData = responseReader.ReadToEnd();
        //    responseReader.Close();

        //    Console.Write(responseData);
        //}

    }



}
