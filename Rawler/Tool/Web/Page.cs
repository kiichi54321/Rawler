using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    [ContentProperty("Children")]
    [Serializable]
    public class Page : RawlerBase
    {
        public Page()
            : base()
        {
        }

        public Page(string url)
            : base()
        {
            this.Url = url;
        }
        //public Page(string url, string encoding)
        //    : base()
        //{
        //    this.Url = url;
        //    Encoding = encoding;
        //}


        /// <summary>
        /// 初めに指定したURLを取得します。
        /// 現在読み込んでいるURLを取得するには GetCurrentUrl()を使ってください
        /// </summary>
        public virtual string Url { get; set; }

        protected string currentUrl = string.Empty;

        //現在読み込んでいるURLを取得します。
        public string GetCurrentUrl()
        {
            return currentUrl;
        }
        //        public string CurrentUrl { get; set; }

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

            }
        }

        private string referer = string.Empty;

        public string Referer
        {
            get { return referer; }
            set { referer = value; }
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


        public string GetCurrentPage()
        {
            var client = GetWebClient();
            this.text = client.HttpGet(this.currentUrl);
            return this.text;
        }

        protected string startUrl = string.Empty;


        /// <summary>
        /// 開始したURLです。URLを取得するときはこちらを使ってください。
        /// </summary>
        /// <returns></returns>
        public string GetStartUrl()
        {
            return startUrl;
        }

       protected  Stack<string> urlStack = new Stack<string>();
        bool oncePageLoad = true;

        /// <summary>
        /// ページを取得した履歴を持ち、一度、読み込んだところは無視する。
        /// </summary>
        public bool OncePageLoad
        {
            get { return oncePageLoad; }
            set { oncePageLoad = value; }
        }

        /// <summary>
        /// オブジェクト開始時にURLの履歴を削除します。
        /// </summary>
        bool clearUrlHistory = true;

        public bool ClearUrlHistory
        {
            get { return clearUrlHistory; }
            set { clearUrlHistory = value; }
        }

        protected HashSet<string> urlHash = new HashSet<string>();

        public void PushUrl(string url)
        {

            var u = url.Split('#');
            if (u.Length > 0)
            {
                url = u[0];
            }
            if (oncePageLoad)
            {
                if (urlHash.Add(url))
                {
                    urlStack.Push(url);
                }
                else
                {
                    if (visbleErr)
                    {
                        ReportManage.ErrReport(this, "すでに読み込んだURLです。スルーします。　" + url);
                    }
                }
            }
            else
            {
                urlStack.Push(url);
            }
        }

        public void Reload()
        {
            urlStack.Push(this.GetCurrentUrl());
        }

        string pastUrl = string.Empty;

        public override void Run(bool runChildren)
        {
            if (clearUrlHistory)
            {
                urlHash.Clear();
            }
            pastUrl = string.Empty;
            string url = string.Empty;

            if (this.Url != null && this.Url.Length > 0)
            {
                url = Url.Convert(this);
                if (url.Contains("http") == false)
                {
                    var prePage = this.GetAncestorRawler().Skip(1).OfType<Page>();
                    if (prePage.Any() == true)
                    {
                        url = System.IO.Path.Combine(prePage.First().GetCurrentUrl(), this.Url);
                    }
                }
                this.startUrl = url;
                PushUrl(url);
            }
            else
            {
                if (this.Parent != null)
                {
                    this.startUrl = GetText();
                    PushUrl(GetText());
                }
            }

            while (urlStack.Count > 0)
            {
                if (ReadPage(urlStack.Pop()))
                {
                    RunChildren(true);
                }
            }
        }

        /// <summary>
        /// ページロードなしで、Runをします。
        /// </summary>
        public void RunWithoutPageLoad()
        {
            RunChildren(true);
        }

        protected bool ReadPage(string url)
        {
            var client = GetWebClient();

            if (MethodType == Tool.MethodType.GET)
            {
                this.text = client.HttpGet(url);
            }
            else if (MethodType == Tool.MethodType.POST)
            {
                parameterDic.Clear();
                if (InputParameterTree != null)
                {                 
                    RawlerBase.GetText(GetText(), InputParameterTree, this);
                }
                List<KeyValue> list = new List<KeyValue>();
                foreach (var item in parameterDic)
                {
                    list.Add(new KeyValue() { Key = item.Key, Value = item.Value });
                }
                this.text = client.HttpPost(url, list);
            }

            this.currentUrl = url;
            this.pastUrl = this.currentUrl;

            if (this.Text.Length > 0)
            {
                return true;
            }
            else
            {
                if (client.ErrMessage.Contains("503"))
                {
                    ReportManage.Report(this, "待機します", true, true);
                    System.Threading.Thread.Sleep(new TimeSpan(0, 0, 30));
                    urlStack.Push(url);
                }
                else
                {
                    if (visbleErr)
                    {
                        ReportManage.ErrReport(this, url + "の読み込みに失敗しました。");
                    }
                    if (ErrorEvent != null)
                    {
                        ErrorEvent(this, new EventArgs());
                    }
                    if (ErrEventTree != null)
                    {
                        ErrEventTree.SetParent();
                        Document d = new Document() { TextValue = client.ErrMessage };
                        d.SetParent(this);
                        d.AddChildren(ErrEventTree);
                        d.Run();
                    }

                }
                return false;
            }
        }

        protected bool visbleErr = true;

        public bool VisbleErr
        {
            get { return visbleErr; }
            set { visbleErr = value; }
        }
        public RawlerBase ErrEventTree { get; set; }

        public event EventHandler ErrorEvent;

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public RawlerBase InputParameterTree { get; set; }

        protected Dictionary<string, string> parameterDic = new Dictionary<string, string>();

        public void AddParameter(string key, string value)
        {
            if (parameterDic.ContainsKey(key))
            {
                parameterDic[key] = value;
            }
            else
            {
                parameterDic.Add(key, value);
            }

        }

        bool useReferer = true;

        public bool UseReferer
        {
            get { return useReferer; }
            set { useReferer = value; }
        }

        public WebClient GetWebClient()
        {
            WebClient client = new WebClient();
            IRawler current = this.Parent;

            var list = this.GetAncestorRawler().Skip(1);
            //            list.Remove(this);
            var clients = list.Where(n => n is WebClient);
            if (clients.Count() > 0)
            {
                client = (WebClient)clients.First();
            }
            if (useReferer)
            {
                if (pastUrl == string.Empty)
                {
                    var pages = list.Where(n => n is Page);

                    if (pages.Count() > 0)
                    {
                        var prePage = (Page)pages.First();
                        client.Referer = prePage.GetCurrentUrl();
                    }
                }
                else
                {
                    client.Referer = pastUrl;
                }
                if (referer != null && referer.Length > 0)
                {
                    client.Referer = referer;
                }
            }
            else
            {
                client.Referer = string.Empty;
            }
            var enc = GetEncoding();
            if (enc != null)
            {
                client.Encoding = encoding;
            }
            return client;
        }
        private MethodType methodType = MethodType.GET;

        public MethodType MethodType
        {
            get { return methodType; }
            set { methodType = value; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            var clone = new Page();
            RawlerLib.ObjectLib.FildCopy(this, clone);
            clone.SetParent(parent);
            clone.children.Clear();
            this.CloneEvent(clone);
            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
        }

        /// <summary>
        /// ページヘの読み込み済みのPageオブジェクトを返す
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static Web.PageNoDownLoad CreatePage(string url,string html)
        {
            var page = new Web.PageNoDownLoad();
            page.SetPage(url, html);
            return page;
        }
    }

    public enum MethodType
    {
        GET, POST
    }
}
