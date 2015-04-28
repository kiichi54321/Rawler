using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RawlerLib.MarkupLanguage;
using System.Linq;
using RawlerExpressLib.TestExtend;

namespace RawlerUnitTestProject
{
    [TestClass]
    public class TagsTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            var list = TagAnalyze.GetTag(wc.DownloadString("http://www.nicovideo.jp/ranking"), "div");
            list.ConsoleWriteLine(n=>n.StartTag);
        }

        [TestMethod]
        public void TestTags()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Encoding = System.Text.Encoding.GetEncoding("euc-jp");
            var list = TagAnalyze.GetTag(wc.DownloadString("http://www.j-magazine.or.jp/data_001.php"),new string[]{"div","h3"});
            list.ConsoleWriteLine(n => n.StartTag);

        }

        [TestMethod]
        public void TestTags2()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Encoding = System.Text.Encoding.GetEncoding("euc-jp");
            var list = TagAnalyze.GetTag("<div><h3></h3><div></div></div>", new string[] { "div", "h3" });
            list.ConsoleWriteLine(n => n.StartTag);

        }
    }


  

}
