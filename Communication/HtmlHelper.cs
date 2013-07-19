using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Communication
{
    public static class HtmlHelper
    {
        public const string VIEWSTATENAME = "__VIEWSTATE";

        /// <summary>
        /// Create a string for the postdata, e.g. param1=value1&param2=value2...&btnLogin=Login"
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ConstructQuerystring(params string[] values)
        {
            if (values.Length % 2 != 0)
            {
                throw new ArgumentOutOfRangeException("There should be an even number of arguments for constructing the post data string");
            }

            var sb = new StringBuilder();

            var counter = 0;

            for (var i = 0; i < values.Length; i++)
            {
                sb.Append(values[i]);

                switch (counter)
                {
                    case 0:
                        sb.Append("=");
                        break;
                    case 1:
                        if (i != values.Length - 1)
                        {
                            sb.Append("&");
                        }
                        break;
                }

                counter = (i + 1) % 2; // alternate between 0 and 1 on each cycle

            }

            return sb.ToString();
        }

        public static string ConstructQuerystring(Dictionary<string, string> values)
        {
            var sb = new StringBuilder();
            var last = values.Last();

            foreach (var keyValuePair in values)
            {
                sb.Append(keyValuePair.Key);
                sb.Append("=");
                sb.Append(keyValuePair.Value);
                if (keyValuePair.Key != last.Key)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        ///// <summary>
        ///// Takes a querystring and appends extra values to the end
        ///// </summary>
        ///// <param name="querystring"></param>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //public static string QuerystringCombine(this string querystring, Dictionary<string, string> values)
        //{
        //    return querystring + ("&" + ConstructQueryString(values));
        //}
    }
}
