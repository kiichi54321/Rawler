﻿using System;
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
            public Page():base()
            {
            }

            public Page(string url):base()
            {
                this.Url = url;
            }
            public Page(string url,string encoding)
                : base()
            {
                this.Url = url;
                Encoding = encoding;
            }

            //public Page( string encoding)
            //    : base()
            //{
            //    Encoding = encoding;

            //}

        /// <summary>
        /// 初めに指定したURLを取得します。
            /// 現在読み込んでいるURLを取得するには GetCurrentUrl()を使ってください
        /// </summary>
        public string Url { get; set; }

        private string currentUrl = string.Empty;

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

        private string startUrl = string.Empty;


        /// <summary>
        /// 開始したURLです。URLを取得するときはこちらを使ってください。
        /// </summary>
        /// <returns></returns>
        public string GetStartUrl()
        {
            return startUrl;
        }

        Stack<string> urlStack = new Stack<string>();
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

        HashSet<string> urlHash = new HashSet<string>();

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
                        ReportManage.ErrReport(this, "すでに読み込んだURLです。スルーします。　"+url);
                    }
                }
            }
            else
            {
                urlStack.Push(url);
            }
        }

        string pastUrl = string.Empty;
        //public override void Run(bool runChildren)
        //{
        //    var client = GetWebClient();
        //    //    this.CurrentUrl = this.Url;
        //    if (this.Url != null && this.Url.Length > 0)
        //    {
        //        this.startUrl = this.Url;
        //        this.text = client.HttpGet(this.Url);
        //        this.currentUrl = this.Url;
        //    }
        //    else
        //    {
        //        if (this.Parent != null)
        //        {
        //            this.startUrl = GetText();
        //            this.text = client.HttpGet(GetText());
        //            this.currentUrl = GetText();
        //        }
        //    }
        //    if (this.Text.Length > 0)
        //    {
        //        RunChildren(runChildren);
        //    }
        //    else
        //    {
        //        ReportManage.ErrReport(this, "ページの読み込みに失敗しました。");
        //    }
        //}
        public override void Run(bool runChildren)
        {
            if (clearUrlHistory)
            {
                urlHash.Clear();
            }
            pastUrl = string.Empty;

            if (this.Url != null && this.Url.Length > 0)
            {
                this.startUrl = this.Url;
                PushUrl(this.Url);
            }
            else
            {
                if (this.Parent != null)
                {
                    this.startUrl = GetText();
                    PushUrl(GetText());
                }
            }

            while(urlStack.Count>0)
            {
                if (ReadPage(urlStack.Pop()))
                {
                    RunChildren(true);
                }
            }
        }
        private bool ReadPage(string url)
        {
            var client = GetWebClient();

            this.text = client.HttpGet(url);
            this.pastUrl = this.currentUrl;
            this.currentUrl = url;

            if (this.Text.Length > 0)
            {
                return true;
            }
            else
            {
                if (client.ErrMessage.Contains("503"))
                {
                    ReportManage.Report(this,"待機します",true,true);
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

        private bool visbleErr = true;

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



        public void Run(string url)
        {
            var client = GetWebClient();
            this.text = client.HttpGet(url);
            this.currentUrl = url;


            if(this.Text.Length>0)
            {
                RunChildren(true);
            }
        }

        private WebClient GetWebClient()
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
            //while (current != null)
            //{
            //    if (current is WebClient)
            //    {
            //        client = current as WebClient;
            //        break;
            //    }
            //    current = current.Parent;
            //}
            var enc = GetEncoding();
            if(enc !=null)
            {
                client.Encoding = encoding;
            }
            return client;
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
    }
}