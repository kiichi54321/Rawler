using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RawlerLib
{
    /// <summary>
    /// 標準のWebClientではクッキーが使えないため拡張
    /// </summary>
    public class WebClientEx: System.Net.WebClient
    {

        private CookieContainer cookieContainer;

        public CookieContainer CookieContainer
        {
            get
            {
                return cookieContainer;
            }
            set
            {
                cookieContainer = value;
            }
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);

            if (webRequest is HttpWebRequest)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                httpWebRequest.CookieContainer = this.cookieContainer;
            }

            return webRequest;
        }
    }
}
