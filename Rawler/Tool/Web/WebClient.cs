using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// Webページを読むためのClientです。
    /// </summary>
    [ContentProperty("Children")]
    [Serializable]
    public class WebClient : RawlerBase
    {

        private Encoding encoder = null;

        //public Encoding Encoding
        //{
        //    get { return encoder; }
        //    set { encoder = value; }
        //}

        private string encoding = string.Empty;
        /// <summary>
        /// 文字コードの指定　
        /// </summary>
        public string Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                encoding = value;
                encoder = GetEncoding();
            }
        }

        protected Encoding GetEncoding()
        {
            Encoding enc = null;
            if (encoding != null && encoding.Length > 0)
            {
                try
                {
                    enc = System.Text.Encoding.GetEncoding(encoding);
                }
                catch
                {
                    ReportManage.ErrReport(this, "文字コード:" + encoding + " は不正です。無視します。");
                    enc = null;
                }
            }
            return enc;
        }



        private CookieContainer cc = new CookieContainer();
        /// <summary>
        /// 今あるクッキーの数
        /// </summary>
        /// <returns></returns>
        public int GetCookieCount()
        {
            return cc.Count;
        }
        //private string id = "あなたのメールアドレス";
        //private string password = "あなたのパスワード";
        private bool hasLogin = false;


        //public void SetIdPasswd(string id, string pass)
        //{
        //    this.id = id;
        //    this.password = pass;
        //}



        /// <summary>
        /// eucで対象URLを読み込みます
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string HttpGet(string url)
        {

            return HttpGet(url, encoder);
        }


        private int count = 0;
        private int tryCount = 3;

        /// <summary>
        /// 試行回数
        /// </summary>
        public int TryCount
        {
            get { return tryCount; }
            set { tryCount = value; }
        }

        //protected bool useLogin = false;

        //protected event EventHandler LoginEvent;

        bool addUserAgent = false;

        public bool AddUserAgent
        {
            get { return addUserAgent; }
            set { addUserAgent = value; }
        }

        string referer = string.Empty;

        public string Referer
        {
            get { return referer; }
            set { referer = value; }
        }
        //protected void OnLogin()
        //{
        //    if (useLogin)
        //    {
        //        if (LoginEvent != null)
        //        {
        //            LoginEvent(this, new EventArgs());
        //        }
        //    }
        //}

        private bool visbleErr = true;

        public bool VisbleErr
        {
            get { return visbleErr; }
            set { visbleErr = value; }
        }
        private string userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows XP)";

        public string UserAgent
        {
            get { return userAgent; }
            set
            {
                userAgent = value;
                if (string.IsNullOrEmpty(userAgent) == false)
                {
                    addUserAgent = true;
                }
            }
        }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public virtual string HttpGet(string url, Encoding enc)
        {
            ErrMessage = string.Empty;
            string result = string.Empty;
            bool retry = false;
            try
            {

            if (enc != null)
            {
                RawlerLib.WebClientEx wc = new RawlerLib.WebClientEx();
                wc.Headers.Add("Referer", referer);
                wc.Headers.Add("UserAgent", userAgent);
                wc.CookieContainer = cc;
                wc.Encoding = enc;
                result = wc.DownloadString(url);
                return result;
            }
                // リクエストの作成
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(url));
                req.CookieContainer = cc;

                if (addUserAgent)
                {
                    req.UserAgent = userAgent;
                }
                if (referer != string.Empty)
                {
                    req.Referer = referer;
                }

                WebResponse res = req.GetResponse();

                // レスポンスの読み取り
                Stream resStream = res.GetResponseStream();

                if (encoder != null)
                {
                    StreamReader sr = new StreamReader(resStream, enc);
                    result = sr.ReadToEnd();

                    sr.Close();
                }
                else
                {
                    //応答データを受信するためのStreamを取得

                    List<byte> byteArray = new List<byte>();
                    int b;
                    while ((b = resStream.ReadByte()) > -1)
                    {
                        byteArray.Add((byte)b);
                    }

                    RawlerLib.Text.TxtEnc txtEnc = new RawlerLib.Text.TxtEnc();
                    byte[] byteArray2 = byteArray.ToArray();
                    txtEnc.SetFromByteArray(ref byteArray2);

                    txtEnc.Codec = "shift_jis";
                    result = txtEnc.Text;
                }
                resStream.Close();
                count = 0;
            }
            catch (Exception e)
            {
                if (visbleErr)
                {
                    ReportManage.ErrReport(this, "Url:" + url + " " + e.Message);
                }
                //if (e is System.UriFormatException)
                //{
                //    ReportManage.ErrReport(this,"["+url+"]は無効なURLです。");
                //}
                ErrMessage = e.Message;
                retry = false;
                //                throw new Exception("HttpGet:"+url+"に失敗しました");

            }
            if (retry)
            {
                count++;
                if (count <= tryCount)
                {

                    System.Threading.Thread.Sleep(new TimeSpan(0, 0, 10 * count * count));
                    result = this.HttpGet(url, enc);
                }
                else
                {
                    ReportManage.ErrReport(this, "HttpGet:" + url + "に失敗しました");
                    result = string.Empty;
                }
            }


            return result;
        }

        public string ErrMessage { get; set; }

        /// <summary>
        /// Image用　失敗時NULL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual byte[] HttpGetByte(string url)
        {

            List<byte> data = new List<byte>();
            try
            {
                // リクエストの作成
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(url));
                req.CookieContainer = cc;

                if (addUserAgent)
                {
                    req.UserAgent = userAgent;
                }
                if (referer != string.Empty)
                {
                    req.Referer = referer;
                }

                //             req.Referer = "http://www.pixiv.net/";
                WebResponse res = req.GetResponse();

                // レスポンスの読み取り
                Stream resStream = res.GetResponseStream();
                //b = new StreamReader(resStream);
                //StreamReader sr = new StreamReader(resStream);
                while (true)
                {
                    int b = resStream.ReadByte();

                    if (b == -1)
                    {
                        break;
                    }
                    data.Add((byte)b);
                }
                //sr.Close();
                resStream.Close();
            }
            catch (Exception e)
            {
                data = null;
            }
            if (data != null)
            {
                return data.ToArray();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public virtual string HttpPost(string url, List<KeyValue> vals)
        {

            string param = "";
            foreach (var k in vals)
            {
                param += String.Format("{0}={1}&", k.Key, k.Value);
            }
            byte[] data = System.Text.Encoding.UTF8.GetBytes(param);

            // リクエストの作成
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(url));
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            req.CookieContainer = cc;

            if (addUserAgent)
            {
                req.UserAgent = userAgent;
            }
            if (referer != string.Empty)
            {
                req.Referer = referer;
            }

            // ポスト・データの書き込み
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            WebResponse res = req.GetResponse();

            if (encoder == null)
            {
                encoder = System.Text.Encoding.UTF8;
            }

            // レスポンスの読み取り
            Stream resStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, encoder);
            string result = sr.ReadToEnd();
            sr.Close();
            resStream.Close();

            return result;
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        //public string Login(string id, string password)
        //{
        //    // ログイン・ページへのアクセス
        //    Dictionary<string, string> vals = new Dictionary<string, string>();
        //    vals.Add("mode", "login");
        //    vals.Add("pixiv_id", id);
        //    vals.Add("pass", password);

        //    this.id = id;
        //    this.password = password;

        //    string login = "http://www.pixiv.net/index.php";
        //    string html = HttpPost(login, vals);


        //    if (cc.Count > 0)
        //    {
        //        hasLogin = true;
        //        MyLib.Log.LogWriteLine("Login成功！");
        //    }

        //    //        html = HttpGet("http://www.pixiv.net/");
        //    return html;
        //}

        ///// <summary>
        ///// 再ログイン用
        ///// </summary>
        ///// <returns></returns>
        //public string Login()
        //{
        //    return Login(this.id, this.password);
        //}

        /// <summary>
        /// イベントをコピーする。
        /// </summary>
        /// <param name="rawler"></param>
        protected override void CloneEvent(RawlerBase rawler)
        {
            base.CloneEvent(rawler);
            //          this.LoginEvent = ((WebClient)rawler).LoginEvent;
        }

        /// <summary>
        /// Clone作成
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<WebClient>(parent);
        }
    }



}
