using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;
using HtmlAgilityPack;
using RawlerExpressLib.Automation.Extend;
using RawlerLib.Extend;
using RawlerLib.MyExtend;
using RawlerLib.MarkupLanguage;

namespace RawlerExpressLib.Automation
{
    public class RawlerAutoSingelPage : RawlerBase
    {
        double rate違いの大きさの下限値 = 0.25;

        public override void Run(bool runChildren)
        {
            var webclient = this.GetUpperRawler<WebClient>();
            if (webclient != null && SampleUrls != null)
            {
                webclient.HttpGetAsync(SampleUrls);
                var urls = SampleUrls.OrderBy(n => Guid.NewGuid()).Take(2);
                PageData p1 = new PageData();
                PageData p2 = new PageData();
                p1.CreateData(webclient.HttpGet(urls.First()), urls.First());
                p2.CreateData(webclient.HttpGet(urls.Last()), urls.Last());
                var result = p1.Compair(p2);
                Page page = new Page();
                foreach (var item in result.Where(n => n.Index >= rate違いの大きさの下限値))
                {
                    Tags tag = new Tags() { Tag = "div", ParameterFilter = item.Tag, TagVisbleType = TagVisbleType.Outer };
                    tag.Add(new RawlerAutoSingleDataWrite());
                    page.Add(tag);
                }
                page.Add(new NextDataRow());
                var urlstack = new UrlStack() { Urls = new TextVauleList(SampleUrls) }.Add(page).GetRoot();
                this.Add(urlstack);
            }

            base.Run(runChildren);
        }

        public List<string> SampleUrls { get; set; }

        RawlerBase CreateRawlerTree(IEnumerable<string> urls)
        {
            var root = new Page();
            root.SetParent(this);

            var wc = root.GetWebClient();
            List<PageData> list = new List<PageData>();
            urls.Run((n) => list.Add(new PageData() { Url = n }));






            return root;
        }

        public class PageData
        {
            public string Html { get; set; }
            public string Url { get; set; }
            List<TagData> tagList = new List<TagData>();
            public HtmlNode DocumentNode { get; set; }
            List<KeyValuePair<string, int>> taglist2 = new List<KeyValuePair<string, int>>();

            public PageData()
            {

            }
            public PageData(string url)
            {
                using (System.Net.WebClient wc = new System.Net.WebClient() { Encoding = System.Text.Encoding.UTF8 })
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)");
                    this.CreateData(wc.DownloadString(url), url);
                }
            }

            public List<KeyValuePair<string, int>> Taglist2
            {
                get { return taglist2; }
                set { taglist2 = value; }
            }
            public List<TagData> TagList
            {
                get { return tagList; }
                set { tagList = value; }
            }
            public void CreateData(string html, string url)
            {
                this.Html = html;
                this.Url = url;
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Html);

                DocumentNode = doc.DocumentNode;
                analyze(DocumentNode, null);

                divtagDic = this.Html.ToHtml().GetTag("div").ParameterInherit().Where(n => n.Parameter.Length > 0).GroupBy(n => n.Parameter).ToDictionary(n => n.Key, n => n.ToList());

            }

            Dictionary<string, List<TagClass>> divtagDic;


            public List<CompairPageResult> Compair(PageData pd)
            {
                List<CompairPageResult> list = new List<CompairPageResult>();

                foreach (var item in this.divtagDic.Keys.Intersect(pd.divtagDic.Keys))
                {
                    var text1 = pd.divtagDic[item].Select(n => n.InnerWithoutChildren).JoinText("\n").ToHtml().TagClear().Trim();
                    var text2 = this.divtagDic[item].Select(n => n.InnerWithoutChildren).JoinText("\n").ToHtml().TagClear().Trim();
                    var diff = new RawlerExpressLib.Vendor.DiffMatchPatch.diff_match_patch();
                    var index2 = diff.diff_levenshtein(diff.diff_main(text1, text2));
                    list.Add(new CompairPageResult() { Index = index2 / (double)(Math.Max(text1.Length, text2.Length)), Tag = item });
                }
                return list;
            }

            public class CompairPageResult
            {
                public string Tag { get; set; }
                public double Index { get; set; }
            }

            public List<KeyValuePair<string, int>> GetTargetWordDiv(string targetWord)
            {
                return this.Html.ToHtml().GetTag("div").Where(n => n.Parameter.Length > 0).Where(n => n.Inner.Contains(targetWord)).Select(n => new KeyValuePair<string, int>(n.Parameter, n.Inner.Length)).OrderByDescending(n => n.Value).ToList();
            }

            private void analyze(HtmlNode node, HtmlNode divtag)
            {
                if (node.Name == "div")
                {
                    divtag = node;
                }
                if (divtag != null)
                {
                    if (node.Name == "#text" && node.InnerText.Replace("\n", String.Empty).Replace("\r", string.Empty).Trim().Length == 0)
                    {

                    }
                    else
                    {
                        tagList.Add(new TagData() { Node = node, DivTag = divtag, XPath = node.XPath });
                    }
                }

                foreach (var item in node.ChildNodes)
                {
                    analyze(item, divtag);
                }
            }

        }
        public class TagData
        {
            public HtmlNode Node { get; set; }
            public HtmlNode DivTag { get; set; }
            public string XPath { get; set; }
        }

        public class TagPair
        {
            public string XPath { get; set; }
            public TagData TagData1 { get; set; }
            public TagData TagData2 { get; set; }
            public double Index { get; set; }
        }


    }


    public static class AutoPage
    {

    }
}
