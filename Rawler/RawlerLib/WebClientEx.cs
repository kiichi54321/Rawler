using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace RawlerLib
{
    /// <summary>
    /// 標準のWebClientではクッキーが使えないため拡張
    /// </summary>
    public class WebClientEx: System.Net.WebClient
    {

        private CookieContainer cookieContainer =new CookieContainer();

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

        public string UserAgent { get; set; }
        public string Referer { get; set; }
        public BasicAuthorization BasicAuthorization { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);

            if (webRequest is HttpWebRequest)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                httpWebRequest.CookieContainer = this.cookieContainer;
                httpWebRequest.Referer = this.Referer;
                httpWebRequest.UserAgent = this.UserAgent;

                if(BasicAuthorization !=null)
                {
                    httpWebRequest.Headers[HttpRequestHeader.Authorization] = BasicAuthorization.GetAuthorization();
                }
             
               
            }
            return webRequest;
        }

        public string UploadValues2(Uri address,IEnumerable<Rawler.Tool.KeyValue> data)
        {
            var request = GetWebRequest(address);
            request.ContentType = "application/x-www-form-urlencoded";
            var dataString = CreateDataString(data);
            byte[] buffer = UploadValuesInternal(data);
            request.ContentLength = buffer.Length;
            request.Method = "POST";
            request.GetRequestStream();

            // ポスト・データの書き込み
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();

            // レスポンスの取得と読み込み
            WebResponse res = request.GetResponse();
            Stream resStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, Encoding);
            string html = sr.ReadToEnd();
            sr.Close();
            
            resStream.Close();
            return html;
        }


        /// <devdoc>
        ///    <para>Shared code for UploadValues, creates a memory stream of data to send</para>
        /// </devdoc>
        private byte[] UploadValuesInternal(IEnumerable<Rawler.Tool.KeyValue> data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(CreateDataString(data).ToString());
            return buffer;
        }





        public string CreateDataString(IEnumerable<Rawler.Tool.KeyValue> data)
        {
            StringBuilder values = new StringBuilder();
            string delimiter = string.Empty;
            foreach (var d in data)
            {
                values.Append(delimiter);
                values.Append(UrlEncode(d.Key));
                values.Append("=");
                values.Append(UrlEncode(d.Value));
                delimiter = "&";
            }
            return values.ToString();
        }

        private static string UrlEncode(string str)
        {
            if (str == null)
                return null;
            return UrlEncode(str, Encoding.UTF8);
        }

        private static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
                return null;
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        private static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int cSpaces = 0;
            int cUnsafe = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];

                if (ch == ' ')
                    cSpaces++;
                else if (!IsSafe(ch))
                    cUnsafe++;
            }

            // nothing to expand?
            if (!alwaysCreateReturnValue && cSpaces == 0 && cUnsafe == 0)
                return bytes;

            // expand not 'safe' characters into %XX, spaces to +s
            byte[] expandedBytes = new byte[count + cUnsafe * 2];
            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[offset + i];
                char ch = (char)b;

                if (IsSafe(ch))
                {
                    expandedBytes[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedBytes[pos++] = (byte)'+';
                }
                else {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
            }

            return expandedBytes;
        }
        private static char IntToHex(int n)
        {
//            Debug.Assert(n < 0x10);

            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'a');
        }

        private static bool IsSafe(char ch)
        {
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
                return true;

            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '\'':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

    }

    public class BasicAuthorization
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }

        public string GetAuthorization()
        {
            var namePassword = string.Format("{0}:{1}", UserName, PassWord);
            var chars = System.Text.Encoding.ASCII.GetBytes(namePassword);
            var base64 = Convert.ToBase64String(chars);
            return "Basic " +base64;
        }
    }
}
