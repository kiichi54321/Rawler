using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Markup;
using RawlerLib.MyExtend;
using System.Text.RegularExpressions;

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
        double sleepSeconds = 0;

        /// <summary>
        /// ページ読み込むときに必ず停止する秒の設定。
        /// </summary>
        public double SleepSeconds
        {
            get { return sleepSeconds; }
            set { sleepSeconds = value; }
        }

        protected void Sleep()
        {
            if (sleepSeconds > 0)
            {
                System.Threading.Thread.Sleep((int)(1000 * sleepSeconds));
            }
        }
        /// <summary>
        /// 読み込んでいるURLをレポートする。
        /// </summary>
        public bool ReportUrl { get; set; } = false;

        public override void Run(bool runChildren)
        {
            wc = new RawlerLib.WebClientEx();
            base.Run(runChildren);
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


        /// <summary>
        /// eucで対象URLを読み込みます
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string HttpGet(string url)
        {
            Sleep();
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
      

        private bool visbleErr = true;

        public bool VisbleErr
        {
            get { return visbleErr; }
            set { visbleErr = value; }
        }
        private string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Win64; x64; Trident/6.0)";

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

        private bool useCache = false;
        /// <summary>
        /// キャッシュを効かせる。
        /// </summary>
        public bool UseCache
        {
            get { return useCache; }
            set { useCache = value; }
        }

        protected Dictionary<string, string> casheDic = new Dictionary<string, string>();

        /// <summary>
        /// Basic認証用
        /// </summary>
        public RawlerLib.BasicAuthorization BasicAuthorization { get; set; }

        [NonSerialized]
        RawlerLib.WebClientEx wc = new RawlerLib.WebClientEx();
        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public virtual string HttpGet(string url, Encoding enc)
        {
            Sleep();
            if(UseCache)
            {
                if (casheDic.ContainsKey(url)) return casheDic[url];
            }

            ErrMessage = string.Empty;
            string result = string.Empty;
            bool retry = false;
            if(ReportUrl)
            {
                ReportManage.Report(this, "GET " + url, true, true);
            }

            try
            {

                wc.Referer = referer;
                wc.UserAgent = userAgent;
                wc.BasicAuthorization = BasicAuthorization;
                    if (enc != null)
                    {
                        wc.Encoding = enc;
                        result = wc.DownloadString(url);
                    }
                    else
                    {
                        var data = wc.DownloadData(url);
                        result = GetAutoEncoding(data, out encoder);

                        //RawlerLib.Text.TxtEnc txtEnc = new RawlerLib.Text.TxtEnc();
                        //byte[] byteArray2 = wc.DownloadData(url);
                        //txtEnc.SetFromByteArray(ref byteArray2);

                        //txtEnc.Codec = "shift_jis";
                        //result = txtEnc.Text;
                    }
                
            }
            catch (Exception e)
            {
                if (visbleErr)
                {
                    ReportManage.ErrReport(this, "Url:" + url + " " + e.Message);
                }
                ErrMessage = e.Message;
                retry = false;
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
            if(UseCache)
            {
                casheDic.GetValueOrAdd(url, result);
            }

            return result;
        }

        public string ErrMessage { get; set; }
        class DownloadData
        {
            public string Url { get; set; }
            public string HTML { get; set; }
        }

        public WebClient GetCopy()
        {
            WebClient wc = new WebClient();
            wc.cc = this.cc;
            wc.AddUserAgent = this.AddUserAgent;
            wc.UserAgent = wc.UserAgent;
            return wc;
            
        }

        public virtual void HttpGetAsync(IEnumerable<string> urls)
        {
            UseCache = false;
            int ThreadNum = 4;
            System.Collections.Concurrent.ConcurrentStack<string> stack = new System.Collections.Concurrent.ConcurrentStack<string>(urls.Distinct());

            List<System.Threading.Tasks.Task<List<DownloadData>>> tasks = new List<System.Threading.Tasks.Task<List<DownloadData>>>();
            for (int i = 0; i < ThreadNum; i++)
            {
                var task = System.Threading.Tasks.Task.Factory.StartNew<List<DownloadData>>((n) =>
                {
                    List<DownloadData> list = new List<DownloadData>();
                    var c = this.GetCopy();
                    while (true)
                    {
                        string url = string.Empty;
                        if (stack.TryPop(out url))
                        {
                            DownloadData dd = new DownloadData() { Url = url, HTML = c.HttpGet(url) };
                            list.Add(dd);
                        }
                        else
                        {
                            break;
                        }
                    }
                    c.Dispose();
                    return list;
                }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
                if (task != null) tasks.Add(task);
            }
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
  

            foreach (var item in tasks.SelectMany(n=>n.Result))
            {  
                casheDic.GetValueOrAdd(item.Url, item.HTML);
            }
            UseCache = true;
        }
        

        public string GetAutoEncoding(byte[] data,out Encoding encoding)
        {
            var utf8 = System.Text.Encoding.UTF8.GetString(data);

            var p1 = "<meta http-equiv=\"content-type\" content=\"text/html; charset=(.*?)\"\\s*/?>";
            var p2 = "<meta charset=\"(.*?)\"\\s*/?>";
            encoding = System.Text.Encoding.UTF8;
            try
            {
                var head =utf8.Substring(0, 600);
                var m1 = Regex.Match(head, p1, RegexOptions.IgnoreCase);
                if (m1.Success)
                {
                    if (m1.Groups[1].Value == "UTF-8") { return utf8; }
                    encoding = System.Text.Encoding.GetEncoding(m1.Groups[1].Value);
                    return encoding.GetString(data);
                }
                else
                {
                    var m2 = Regex.Match(head, p2, RegexOptions.IgnoreCase);
                    if (m2.Success)
                    {
                        if (m2.Groups[1].Value == "UTF-8") { return utf8; }
                        encoding = System.Text.Encoding.GetEncoding(m2.Groups[1].Value);
                        return encoding.GetString(data);
                    }
                }
            }
            catch(Exception e)
            {
                ReportManage.ErrReport(this, "エンコードの取得に失敗しました。"+e.Message);
            }
            //文字コード自動判別（日本語限定）
            var enc = RawlerLib.Jcode.GetCode(data);
            if (enc != null) return enc.GetString(data);

            return utf8;
        }


        /// <summary>
        /// Image用　失敗時NULL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual byte[] HttpGetByte(string url)
        {
            Sleep();
            if (ReportUrl)
            {
                ReportManage.Report(this, "GET " + url, true, true);
            }

            byte[] data;
            try
            {
                wc.Referer = referer;
                wc.UserAgent = userAgent;
                wc.BasicAuthorization = BasicAuthorization;
                data = wc.DownloadData(url);


                //// リクエストの作成
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Uri.EscapeUriString(url));
                //req.CookieContainer = cc;

                //if (addUserAgent)
                //{
                //    req.UserAgent = userAgent;
                //}
                //if (referer != string.Empty)
                //{
                //    req.Referer = referer;
                //}
          

                ////             req.Referer = "http://www.pixiv.net/";
                //WebResponse res = req.GetResponse();

                //// レスポンスの読み取り
                //Stream resStream = res.GetResponseStream();
                ////b = new StreamReader(resStream);
                ////StreamReader sr = new StreamReader(resStream);
                //while (true)
                //{
                //    int b = resStream.ReadByte();

                //    if (b == -1)
                //    {
                //        break;
                //    }
                //    data.Add((byte)b);
                //}
                ////sr.Close();
                //resStream.Close();
            }
            catch (Exception e)
            {
                if (visbleErr)
                {
                    ReportManage.ErrReport(this, "Url:" + url + " " + e.Message);
                }
                data = null;
            }
            if (data != null)
            {
                return data;
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
            if (ReportUrl)
            {
                ReportManage.Report(this, "POST " + url, true, true);
            }
            try
            {
                Sleep();
                wc.Referer = referer;
                wc.UserAgent = userAgent;
                wc.BasicAuthorization = BasicAuthorization;

                System.Collections.Specialized.NameValueCollection list = new System.Collections.Specialized.NameValueCollection();
                foreach (var item in vals)
                {
                    list.Add(item.Key, item.Value);
                }
                var data2 = wc.UploadValues(url, list);
                return encoder.GetString(data2);
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
           
                //                throw new Exception("HttpGet:"+url+"に失敗しました");

            }
            return string.Empty;
        
        }

        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public virtual byte[] HttpPostByte(string url, List<KeyValue> vals)
        {
            if (ReportUrl)
            {
                ReportManage.Report(this, "POST " + url, true, true);
            }
            try
            {
                Sleep();
                wc.Referer = referer;
                wc.UserAgent = userAgent;
                wc.BasicAuthorization = BasicAuthorization;

                System.Collections.Specialized.NameValueCollection list = new System.Collections.Specialized.NameValueCollection();
                foreach (var item in vals)
                {
                    list.Add(item.Key, item.Value);
                }
                var data2 = wc.UploadValues(url, list);
                return data2;
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

                //                throw new Exception("HttpGet:"+url+"に失敗しました");

            }
            return null;

        }



        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }


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

        public override void Dispose()
        {
            if (wc != null) wc.Dispose();
            base.Dispose();
        }
    }



}
