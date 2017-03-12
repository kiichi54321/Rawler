using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using RawlerLib.MyExtend;
using System.Text.RegularExpressions;
using Rawler.Core;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rawler
{
    /// <summary>
    /// Webページを読むためのClientです。
    /// </summary>

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

        protected async Task Sleep()
        {
            if (sleepSeconds > 0)
            {
                await Task.Delay((int)(1000 * sleepSeconds));
            }
        }
        /// <summary>
        /// 読み込んでいるURLをレポートする。
        /// </summary>
        public bool ReportUrl { get; set; } = false;

        public override void Run(bool runChildren)
        {
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
            if (enc == null)
            {
                enc = System.Text.Encoding.UTF8;
            }
            return enc;
        }



        //private CookieContainer cc = new CookieContainer();
        ///// <summary>
        ///// 今あるクッキーの数
        ///// </summary>
        ///// <returns></returns>
        //public int GetCookieCount()
        //{
        //    return cc.Count;
        //}

        //public void SetCookie(Uri uri,Cookie cookie)
        //{
        //    cc.Add(uri, cookie);
        //}


        HttpClientHandler handler = new System.Net.Http.HttpClientHandler();

        //public async Task< string> HttpGet(string url)
        //{
        //    await this.Sleep();
        //    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false);
        //    client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
        //    var result = await client.GetStringAsync(url);


        //    return result;
        //}

        //public async Task<byte[]> HttpGetByte(string url)
        //{
        //    await this.Sleep();
        //    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false);
        //    client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);

        //    var result = await client.GetByteArrayAsync(url);
        //    return result;
        //}



        //public async Task< string> HttpPost(string url, List<KeyValue> vals)
        //{
        //    await this.Sleep();
        //    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false);
        //    client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
        //    var r = await client.PostAsync(url, new FormUrlEncodedContent(vals.Select(n => new KeyValuePair<string, string>(n.Key, n.Value))));
        //    var r2 = await r.Content.ReadAsStringAsync();
        //    return r2;
        //}



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
        
        bool addUserAgent = false;

        public bool AddUserAgent
        {
            get { return addUserAgent; }
            set { addUserAgent = value; }
        }

        //string referer = string.Empty;

        //public string Referer
        //{
        //    get { return referer; }
        //    set { referer = value; }
        //}
      

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

        protected Dictionary<string,WeakReference<string>> casheDic = new Dictionary<string,WeakReference<string>>();
        protected string GetCashe(string url)
        {
            if(casheDic.ContainsKey(url))
            {
                string html;
                if(casheDic[url].TryGetTarget(out html))
                {
                    return html;
                }
                else
                {
                    casheDic.Remove(url);
                }
            }
            return null;
        }

        ///// <summary>
        ///// Basic認証用
        ///// </summary>
        //public RawlerLib.BasicAuthorization BasicAuthorization { get; set; }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public virtual async Task<string> HttpGet(string url, Encoding enc = null,string referer = default(string))
        {
            await Sleep();
            if(UseCache)
            {
                var h = GetCashe(url);
                if (h != null) return h;
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
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
                client.DefaultRequestHeaders.Referrer = new Uri(referer);
                var data = await client.GetByteArrayAsync(url);
                if (enc != null)
                {
                    result = enc.GetString(data, 0, data.Length);
                }
                else
                {
                    result = GetAutoEncoding(data, out encoder);
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
                    ReportManage.ErrReport(this, "HttpGet:" + url + "にリトライ待機中");
                    await Task.Delay(new TimeSpan(0, 0, 10 * count * count));
                    result = await this.HttpGet(url, enc);
                }
                else
                {
                    ReportManage.ErrReport(this, "HttpGet:" + url + "に失敗しました");
                    result = string.Empty;
                }
            }
            if(UseCache)
            {
                casheDic.GetValueOrAdd(url, new WeakReference<string>(result));
            }

            return result;
        }

        public string ErrMessage { get; set; }
        class DownloadData
        {
            public string Url { get; set; }
            public string HTML { get; set; }
        }



        //public virtual async Task<IEnumerable<string>> HttpGetAsync(IEnumerable<string> urls)
        //{
        //    UseCache = false;
        //    int ThreadNum = 4;
        //    System.Collections.Concurrent.ConcurrentStack<string> stack = new System.Collections.Concurrent.ConcurrentStack<string>(urls.Distinct());

        //    List<System.Threading.Tasks.Task<List<DownloadData>>> tasks = new List<System.Threading.Tasks.Task<List<DownloadData>>>();
        //    for (int i = 0; i < ThreadNum; i++)
        //    {
        //        var task = System.Threading.Tasks.Task.Factory.StartNew<List<DownloadData>>((n) =>
        //        {
        //            List<DownloadData> list = new List<DownloadData>();
                 
        //            while (true)
        //            {
        //                string url = string.Empty;
        //                if (stack.TryPop(out url))
        //                {
        //                    DownloadData dd = new DownloadData() { Url = url, HTML = await this.HttpGet(url) };
        //                    list.Add(dd);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //            c.Dispose();
        //            return list;
        //        }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
        //        if (task != null) tasks.Add(task);
        //    }
        //    System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        //    foreach (var item in tasks.SelectMany(n=>n.Result))
        //    {  
        //        casheDic.GetValueOrAdd(item.Url, item.HTML);
        //    }
        //    UseCache = true;
        //}
        

        public string GetAutoEncoding(byte[] data,out Encoding encoding)
        {
            var utf8 = System.Text.Encoding.UTF8.GetString(data,0,600);

            var p1 = "<meta http-equiv=\"content-type\" content=\"text/html; charset=(.*?)\"\\s*/?>";
            var p2 = "<meta charset=\"(.*?)\"\\s*/?>";
            encoding = System.Text.Encoding.UTF8;
            try
            {
                var head =utf8;
                var m1 = Regex.Match(head, p1, RegexOptions.IgnoreCase);
                if (m1.Success)
                {
                    if (m1.Groups[1].Value == "UTF-8") { return System.Text.Encoding.UTF8.GetString(data, 0,data.Length); }
                    encoding = System.Text.Encoding.GetEncoding(m1.Groups[1].Value);
                    return encoding.GetString(data,0,data.Length);
                }
                else
                {
                    var m2 = Regex.Match(head, p2, RegexOptions.IgnoreCase);
                    if (m2.Success)
                    {
                        if (m2.Groups[1].Value == "UTF-8") { return System.Text.Encoding.UTF8.GetString(data, 0, data.Length); }
                        encoding = System.Text.Encoding.GetEncoding(m2.Groups[1].Value);
                        return encoding.GetString(data,0,data.Length);
                    }
                }
            }
            catch(Exception e)
            {
                ReportManage.ErrReport(this, "エンコードの取得に失敗しました。"+e.Message);
            }
            //文字コード自動判別（日本語限定）
            var enc = RawlerLib.Jcode.GetCode(data);
            if (enc != null) return enc.GetString(data,0,data.Length);

            return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        }


        /// <summary>
        /// Image用　失敗時NULL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual async Task<byte[]> HttpGetByte(string url, string referer = default(string))
        {
            await Sleep();
            if (ReportUrl)
            {
                ReportManage.Report(this, "GET " + url, true, true);
            }

            byte[] data;
            try
            {               
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
                    client.DefaultRequestHeaders.Referrer = new Uri(referer);
                    data = await client.GetByteArrayAsync(url);
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
        public virtual async Task<string> HttpPost(string url, List<KeyValue> vals, string referer = default(string))
        {
            if (ReportUrl)
            {
                var t = vals.Select(n => n.Key + ":" + n.Value.Trim().Replace("\n","").Replace("\r","")).JoinText("\n");
                ReportManage.Report(this, "POST " + url+"\n"+t, true, true);
            }
            try
            {
                await Sleep();
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
                client.DefaultRequestHeaders.Referrer = new Uri(referer);
                var r = await client.PostAsync(url, new FormUrlEncodedContent(vals.Select(n => new KeyValuePair<string, string>(n.Key, n.Value))));
                if(encoder == null || encoder == System.Text.Encoding.UTF8)
                {
                    var r2 = await r.Content.ReadAsStringAsync();
                    return r2;
                }
                else
                {
                    var d = await r.Content.ReadAsByteArrayAsync();
                    return encoder.GetString(d, 0, d.Length);
                }
            }
            catch (Exception e)
            {
                if (visbleErr)
                {
                    ReportManage.ErrReport(this, "Url:" + url + " " + e.Message);
                }
                ErrMessage = e.Message;
            }
            return string.Empty;
        
        }

        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public virtual async Task<byte[]> HttpPostByte(string url, List<KeyValue> vals, string referer = default(string))
        {
            if (ReportUrl)
            {
                ReportManage.Report(this, "POST " + url, true, true);
            }
            try
            {
                await Sleep();
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
                client.DefaultRequestHeaders.Referrer = new Uri(referer);
                var r = await client.PostAsync(url, new FormUrlEncodedContent(vals.Select(n => new KeyValuePair<string, string>(n.Key, n.Value))));
                var d = await r.Content.ReadAsByteArrayAsync();

                return d;
            }
            catch (Exception e)
            {
                if (visbleErr)
                {
                    ReportManage.ErrReport(this, "Url:" + url + " " + e.Message);
                }
                ErrMessage = e.Message;
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
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }



}
