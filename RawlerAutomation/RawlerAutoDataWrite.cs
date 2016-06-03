using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Rawler.Tool;
using RawlerLib.Extend;
using RawlerLib.MyExtend;
using RawlerExpressLib.Automation.Extend;

namespace RawlerExpressLib.Automation
{
    [System.Diagnostics.DebuggerDisplay("{Struct}({Index})")]
    public class HTMLAnalyzeResult
    {
        public string Struct { get; set; }
        public int Count { get; set; }
        public double Index { get { return Struct.Length * Count; } }
        public IEnumerable<HtmlNode> Nodes { get; set; }
        public string BaseUrl { get; set; }
        public string BaseHtml { get; set; }

        Rawler.NPL.TFIDFAnalyze urlTfidf = new Rawler.NPL.TFIDFAnalyze();
        Rawler.NPL.TFIDFAnalyze imageTfidf = new Rawler.NPL.TFIDFAnalyze();

        bool enableGetSubUrlLink = true;
        public bool EnableGetSubUrlLink { get { return enableGetSubUrlLink; } set { enableGetSubUrlLink = value; } }
        bool iconImageColumn = true;

        public RawlerBase CreateRawlerTree(RawlerBase rawler)
        {
            RawlerLib.Timer.StopWatch.Write("CreateRawlerTree Start");
            var baseUrl = BaseUrl;
            foreach (var item in Nodes)
            {
                urlTfidf.AddDocument(item.OuterHtml.ToHtml(baseUrl).GetLink().Select(n => n.Url));
                imageTfidf.AddDocument(item.OuterHtml.ToHtml(baseUrl).GetImageLink().Select(n => n.Url));
            }
            if (rawler == null)
            {
                rawler = new RawlerBase();
            }
            //RawlerTreeを組み立てて、同じものを纏める。

            var list = Nodes.GroupBy(n => n.GetHTMLWithoutValue()).Select(n => CreateRawler(n.First())).ToArray().GroupBy(n => n.ToXAML()).Select(n => n.First()).ToArray();
            RawlerLib.Timer.StopWatch.Write("CreateRawlerTree AddStart");
            foreach (var item in list)
            {
                item.AddFirst(new GetPageUrl().DataWrite("SourceUrl", DataAttributeType.SourceUrl).GetRoot());
                item.Add(new NextDataRow());
                rawler.Add(item);
            }
            RawlerLib.Timer.StopWatch.Write("CreateRawlerTree MargeStart");

            rawler.MargeChildren();
            RawlerLib.Timer.StopWatch.Write("CreateRawlerTree End");

            return rawler;
        }

        static List<string> targetTag = new List<string>() { "div", "dd", "dl", "dt", "table", "tr", "td", "th", "ul", "ol", "li", "h1", "h2", "h3", "h4", "h5", "h6","p","strong" };


        public RawlerBase CreateRawler(HtmlNode node)
        {
            var baseUrl = BaseUrl;
            RawlerBase rawler = null;

            bool flag次のノードを調べる = true;
            if (targetTag.Contains(node.Name))
            {
                Tags tags = new Tags() { Tag = node.Name };
                if (node.Attributes.Where(n => n.Name == "class").Any())
                {
                    tags.ClassName = node.Attributes.Where(n => n.Name == "class").First().Value;
                }
                if (node.Attributes.Where(n => n.Name == "id").Any())
                {
                    tags.IdName = node.Attributes.Where(n => n.Name == "id").First().Value;
                }
                if (node.ChildNodes.Count() == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                {
                    tags.AddChildren(new DataWrite() { Attribute = tags.ClassName });
                    flag次のノードを調べる = false;
                }
                if (node.Attributes.Where(n => n.Name == "style" && n.Value.Contains("background")).Any())
                {
                    tags.TagVisbleType = TagVisbleType.Outer;
                    rawler = tags.Add(new ImageLinks() { ImageType = ImageType.BackgroundImage }).DataWrite(node.GetClassName() + "_Image", DataAttributeType.Image).GetRoot();
                }

                rawler = tags;
            }
            else if (node.Name == "a")
            {
                var resultUrlTFIDF = urlTfidf.GetResult(node.OuterHtml.ToHtml(baseUrl).GetLink().Select(n => n.Url));
                var url = node.OuterHtml.ToHtml(baseUrl).GetLink().FirstDefault<RawlerLib.Web.Link, string>(n => n.Url, null);
                if (url != null)
                {
                    //IDF が0以下の時、すべてのドキュメントで存在する。                
                    if (urlTfidf.IDFDic.GetValueOrDefault(url) !=null && urlTfidf.IDFDic.GetValueOrDefault(url).Value <= 0)
                    {
                        rawler = null;
                        flag次のノードを調べる = false;
                    }
                    else
                    {
                        if (resultUrlTFIDF.GetTakeTopValue(n => n.TFIDF).Where(n => n.Word == url).Any())
                        {
                            rawler = new Links() { VisbleType = LinkVisbleType.Tag }.AddRange(
                                new Links() { VisbleType = LinkVisbleType.Url }.DataWrite(node.GetClassName() + "_MainLink", DataAttributeType.Url).GetRoot());
                            if (node.ChildNodes.Count == 1 && node.ChildNodes.First().Name == "#text")
                            {
                                rawler.Add(new Links() { VisbleType = LinkVisbleType.Label }.DataWrite(node.GetClassName() + "_MainLabel").GetRoot());
                            }
                        }
                        else
                        {
                            rawler = new Links() { VisbleType = LinkVisbleType.Tag }.AddRange(
                                new Links() { VisbleType = LinkVisbleType.Url, Enable = enableGetSubUrlLink }.DataWrite(node.GetClassName() + "_SubLink").GetRoot(),
                                new Links() { VisbleType = LinkVisbleType.Label }.DataWrite(node.GetClassName() + "_SubLabel").GetRoot()
                            );

                        }
                    }
                }
                else
                {
                    //URLがないAタグの場合。
                    Tags tags = new Tags() { Tag = node.Name };
                    if (node.Attributes.Where(n => n.Name == "class").Any())
                    {
                        tags.ClassName = node.Attributes.Where(n => n.Name == "class").First().Value;
                    }
                    if (node.Attributes.Where(n => n.Name == "id").Any())
                    {
                        tags.IdName = node.Attributes.Where(n => n.Name == "id").First().Value;
                    }
                    rawler = tags;
                }
                if (node.ChildNodes.Count == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                {
                    flag次のノードを調べる = false;
                }

            }
            else if (node.Name == "img")
            {
                var resultImgeTFIDF = imageTfidf.GetResult(node.OuterHtml.ToHtml(baseUrl).GetImageLink().Select(n => n.Url));

                var url = node.OuterHtml.ToHtml(baseUrl).GetImageLink().FirstDefault(n => n.Url, null);
                if (url != null)
                {
                    if (imageTfidf.IDFDic.Count>0 && imageTfidf.IDFDic.GetValueOrDefault(url).Value <= 0)
                    {
                        rawler = null;
                        flag次のノードを調べる = false;
                    }
                    else
                    {
                        if (resultImgeTFIDF.GetTakeTopValue(n => n.TFIDF).Where(n => n.Word == url).Any())
                        {
                            rawler = new ImageLinks().DataWrite(node.GetClassName() + "_Image", DataAttributeType.Image).GetRoot();
                        }
                        else
                        {
                            if (iconImageColumn)
                            {
                                rawler = new DataWrite() { AttributeTree = new ImageLinks() { VisbleType = LinkVisbleType.Label }, Value = "1" };
                            }
                            else
                            {
                                rawler = new ImageLinks().DataWrite(node.GetClassName() + "_Icon", DataAttributeType.Image).GetRoot();
                            }
                        }
                    }
                }
            }
            ///背景画像に反応させる。
            else if (node.Attributes.Where(n => n.Name == "style" && n.Value.Contains("background")).Any())
            {
                rawler = new ImageLinks() { ImageType = ImageType.BackgroundImage }.DataWrite(node.GetClassName() + "_Image", DataAttributeType.Image).GetRoot();
            }
            else if (node.Name == "span")
            {
                Tags tags = new Tags() { Tag = node.Name };
                if (node.Attributes.Where(n => n.Name == "class").Any())
                {
                    tags.ClassName = node.Attributes.Where(n => n.Name == "class").First().Value;
                }
                if (node.ChildNodes.Count() == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                {
                    tags.AddChildren(new DataWrite() { Attribute = tags.ClassName });
                    flag次のノードを調べる = false;
                }

                rawler = tags;
            }
            else if (node.Name == "#comment")
            {
                flag次のノードを調べる = false;
            }
            else
            {
                var t = node.OuterHtml.Replace("\n", "").Trim();
                if (t.Length > 0)
                {
                    rawler = new TagClear().Trim().Add(new DataWrite() { Attribute = node.GetClassName() + "_" + node.Name }).GetRoot();
                    if (node.ChildNodes.Count == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                    {
                        flag次のノードを調べる = false;
                    }
                }
            }
            if (rawler != null && node.ChildNodes.Count == 1 && node.ChildNodes.Where(n => n.Name == "span").Any())
            {
                rawler.AddChildren(new DataWrite() { Attribute = node.GetClassName() });
            }

            foreach (var item in node.ChildNodes)
            {
                if (flag次のノードを調べる)
                {
                    var r = CreateRawler(item);

                    if (r != null && rawler != null)
                    {
                        rawler.AddChildren(r);
                    }
                    else
                    {
                        if (r != null && rawler == null)
                        {
                            rawler = r;
                        }
                    }
                }

            }
            return rawler;
        }


        /// <summary>
        /// 最も構造として長く、繰り返しているのが多いものを取得する。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<HTMLAnalyzeResult> GetListHTMLStruct(string html, string url, bool EnableGetSubUrlLink)
        {
            RawlerLib.Timer.StopWatch.Write("GetListHTMLStruct Start");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            RawlerLib.Timer.StopWatch.Write("GetListHTMLStruct LoadHtml");
            var list = doc.DocumentNode.GetAnalyzeResults().Where(n=>n.StructHTML.Length>0).GroupBy(n => n.StructHTML)
                .Select(n => new HTMLAnalyzeResult() { Struct = n.Key, Count = n.Count(), Nodes = n.Select(m => m.Node), BaseHtml = html, BaseUrl = url, EnableGetSubUrlLink = EnableGetSubUrlLink })
                .Where(n => n.Count > 1 && n.Struct.Length > 0).OrderByDescending(n => n.Index).ToArray();
            List<HTMLAnalyzeResult> list2 = new List<HTMLAnalyzeResult>();
            RawlerLib.Timer.StopWatch.Write("GetListHTMLStruct Reverse Start " + list.Count());
            int i = 0;

            foreach (var item in list.OrderBy(n => n.Index))
            {
                bool flag = true;
                foreach (var item2 in list)
                {
                    if (item2.Struct == item.Struct)
                    {
                        break;
                    }
                    if (item.Struct.Length < item2.Struct.Length && item2.Struct.Contains(item.Struct))
                    {
                        flag = false;
                        break;
                    }
                    i++;
                }
                if (flag) list2.Add(item);
            }
            RawlerLib.Timer.StopWatch.Write("GetListHTMLStruct End " + i);
            var l = list2.OrderByDescending(n => n.Index);
            return l;
        }

        public static IEnumerable<HTMLAnalyzeResult> GetListHTMLStruct(Uri uri, bool EnableGetSubUrlLink)
        {
            WebClient wc = new WebClient() { AddUserAgent = true };
            return GetListHTMLStruct(wc.HttpGet(uri.AbsoluteUri), uri.AbsolutePath, EnableGetSubUrlLink);
        }

        /// <summary>
        /// urlとHtmlを入力として、結果を返す。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="EnableGetSubUrlLink"></param>
        /// <param name="takeNum"></param>
        /// <param name="maxPage"></param>
        /// <returns></returns>
        public static ResultData GetDataList(string url, bool EnableGetSubUrlLink, int takeNum, int maxPage)
        {
            ReportStock rs = new ReportStock();
            WebClient wc = new WebClient() { AddUserAgent = true };
            rs.Add(wc);
            var rawler = wc.Page(url).AddRange(
                new RawlerAutoListDataWrite() { TakeNum = takeNum, EnableGetSubUrlLink = EnableGetSubUrlLink },
                new RawlerAutoNextLink() { MaxCount = maxPage },
                new RawlerAutoTable()).GetRoot();
            rawler.Run();
            return new ResultData() { Data = rawler.GetDescendantRawler().OfType<Data>().DataSort().ToArray(), Reports = rs.ReportList };
        }

        public static Rawler.Tool.RawlerBase GetDataListRawlerBase(string url, bool EnableGetSubUrlLink, int takeNum, int maxPage)
        {
            ReportStock rs = new ReportStock();
            WebClient wc = new WebClient() { AddUserAgent = true };
            rs.Add(wc);
            return  wc.Page(url).Add(new RawlerAutoListDataWrite() { TakeNum = takeNum, EnableGetSubUrlLink = EnableGetSubUrlLink }).Add(new RawlerAutoNextLink() { MaxCount = maxPage }).GetRoot();
        }

        /// <summary>
        /// urlとHtmlを入力として、結果を返す。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        /// <param name="EnableGetSubUrlLink"></param>
        /// <param name="takeNum"></param>
        /// <returns></returns>
        public static ResultData GetDataListByHtml(string url,string html, bool EnableGetSubUrlLink, int takeNum)
        {
            ReportStock rs = new ReportStock();
            WebClient wc = new WebClient() { AddUserAgent = true };
            rs.Add(wc);
            var rawler = wc.Add(Page.CreatePage(url, html).AddRange(
                new RawlerAutoListDataWrite() { TakeNum = takeNum, EnableGetSubUrlLink = EnableGetSubUrlLink },
                new RawlerAutoTable()).GetRoot());
            rawler.Run();
            return new ResultData() { Data = rawler.GetDescendantRawler().OfType<Data>().DataSort().ToArray(), Reports = rs.ReportList };
        }


        public static ResultData GetDataSinglePages(IEnumerable<string> list)
        {
            ReportStock rs = new ReportStock();
            WebClient wc = new WebClient() { AddUserAgent = true };
            rs.Add(wc);
            var rawler = wc.Add(new Data()).Add(new RawlerAutoSingelPage() { SampleUrls = list.ToList() }).GetRoot();
            rawler.Run();
            var xaml = rawler.ToXAML();
            return new ResultData() { Data = rawler.GetDescendantRawler().OfType<Data>(), Reports = rs.ReportList };

        }



        public class ResultData
        {
            public IEnumerable<Data> Data { get; set; }
            public IEnumerable<ReportEvnetArgs> Reports { get; set; }
        }
    }


    public class RawlerAutoListDataWrite : RawlerMultiBase
    {

        bool enableGetSubUrlLink = true;
        public bool EnableGetSubUrlLink { get { return enableGetSubUrlLink; } set { enableGetSubUrlLink = value; } }
     //   bool iconImageColumn = true;

        int takeNum = 3;

        public int TakeNum
        {
            get { return takeNum; }
            set { takeNum = value; }
        }

        protected string GetUrl()
        {
            Rawler.Tool.GetPageUrl url = new GetPageUrl();
            url.SetParent(this.Parent);
            url.Run();
            return url.Text;
        }
        bool created = false;

        public override void Run(bool runChildren)
        {
            if (created == false)
            {
                var html2 = GetText();
                var html = new StringBuilder(html2);

                html2.ToHtml().GetTag("head").Run((n) => html = html.Replace(n.Outer, string.Empty));

                var rawler = HTMLAnalyzeResult.GetListHTMLStruct(html.ToString(), GetUrl(), EnableGetSubUrlLink).Take(takeNum).Select(n => n.CreateRawlerTree(new Data() { ErrReportNullData = false }.Add(new GetPageHtml())).GetRoot()).OfType<Data>();

                this.Add(new Rawler.Tool.Concurrent()).AddRange(rawler.ToArray());
                created = true;
                RawlerLib.Timer.StopWatch.Write("AutoListRawlerTree 作成完了");
            }

            //var rawler = HTMLAnalyzeResult.GetListHTMLStruct(GetText(), GetUrl(), EnableGetSubUrlLink).Take(takeNum);
            //var list = new List<Data>();
            //Parallel.ForEach(rawler,(n)=>
            //    {
            //        list.Add(n.CreateRawlerTree(new Data().Add(new GetPageHtml()).GetRoot()) as Data);
            //    });


            //this.AddRange(list.ToArray());


            base.Run(runChildren);
            RawlerLib.Timer.StopWatch.Write("AutoListRawlerTree 実行完了");

        }

        //   public RawlerBase CreatedRawlerTree { get; set; }

    }
}
