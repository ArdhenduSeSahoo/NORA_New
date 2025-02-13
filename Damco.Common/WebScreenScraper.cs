using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public class WebScreenScraper : WebClient
    {
        CookieContainer _cookieContainer = new CookieContainer();
        private string _previous = null;

        protected override WebRequest GetWebRequest(Uri address)
        {
            var result = (HttpWebRequest)base.GetWebRequest(address);
            result.CookieContainer = _cookieContainer;
            //Pretend to be a browser
            result.Accept = "text/html, application/xhtml+xml, */*";
            result.Headers.Add("Accept-Language", "en-US");
            result.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            result.Host = address.Host;
            if (_previous != null)
                result.Referer = _previous;
            _previous = address.ToString();
            result.ContentType = "application/x-www-form-urlencoded";
            result.Expect = null;
            return result;
        }
    }
}
