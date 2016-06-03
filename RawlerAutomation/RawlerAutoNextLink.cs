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

namespace RawlerExpressLib.Automation
{
    public class RawlerAutoNextLink : RawlerBase, IThrowCatch
    {
        public RawlerAutoNextLink()
            : base()
        {
            autoNextLink = LoadXAML();
            autoNextLink.SetParent(this);
        }

        private int count = 1;
        private int maxCount = 3;

        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }
        RawlerBase autoNextLink;
        HashSet<string> urlHash = new HashSet<string>();

        public override void Run(bool runChildren)
        {

            RawlerLib.Timer.StopWatch.Write("RawlerAutoNextLink urlListCreate");
            urlList = new System.Collections.Concurrent.ConcurrentBag<string>();
            RawlerLib.Timer.StopWatch.Write("RawlerAutoNextLink  autoNextLink.Run");

            autoNextLink.Run();
            RawlerLib.Timer.StopWatch.Write("RawlerAutoNextLink  autoNextLink.Run End");

            var page = this.GetUpperRawler<Page>();
            if (page != null)
            {
                if (maxCount > count)
                {
                    var url = page.GetCurrentUrl();
                    Uri url_uri = new Uri(url);
                    if (urlList.Any())
                    {
                        var test = urlList.Distinct().Where(n => new Uri(n).Host == url_uri.Host && urlHash.Contains(n) == false).ToList();
                        var nextUrl = urlList.Distinct().Where(n => new Uri(n).Host == url_uri.Host && urlHash.Contains(n) == false)
                            .Select(n => new { url = n, Distance = Rawler.NPL.LevenshteinDistance.Compute(url, n) })
                            .OrderBy(n => n.Distance);
                        if (nextUrl.Any())
                        {
                            page.PushUrl(nextUrl.First().url);
                            urlHash.Add(nextUrl.First().url);
                            count++;
                        }
                        urlHash.Add(url);

                    }
                    else
                    {
                        ReportManage.ErrReport(this, "NextLinkの取得がありません");
                    }
                }
            }
            RawlerLib.Timer.StopWatch.Write("RawlerAutoNextLink  End");

            base.Run(runChildren);
        }

        private RawlerBase LoadXAML()
        {
            //AutoNextPage.xamlの編集は、RawlerExpressClientのxamlフォルダにあるものを編集すると楽です。
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = myAssembly.GetManifestResourceStream("RawlerExpressLib.AutoNextPage.xaml");

            return (RawlerBase)System.Xaml.XamlServices.Load(stream);
        }
        System.Collections.Concurrent.ConcurrentBag<string> urlList = new System.Collections.Concurrent.ConcurrentBag<string>();

        public void Catch(string text)
        {
            urlList.Add(text);
        }
    }
}
