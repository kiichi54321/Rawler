using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using RawlerLib.MyExtend;

namespace TestRawlerToolProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Rawler.NPL.TFIDFAnalyze tfidf = new Rawler.NPL.TFIDFAnalyze();
            tfidf.AddDocument(new string[] { "1", "2", "3", "4", "4" });
            tfidf.AddDocument(new string[] { "2", "2", "3", "5", "5" });
            tfidf.AddDocument(new string[] { "2", "2", "4", "4", "4" });
            var list = tfidf.GetResult();

            Assert.AreEqual<string>(list.First().Word, "5");

        }

        [TestMethod]
        public void TinyTestMethod()
        {
            TinySegmenterDotNet.TinySegmenter ts = new TinySegmenterDotNet.TinySegmenter();
            Console.WriteLine(ts.SegmentExted("私は[任務完了]です").JoinText("_"));
            ts.AddWordDic("[任務完了]");
            ts.AddWordDic("[任務");
            Console.WriteLine(ts.SegmentExted("私は[任務完了]です").JoinText("_"));

        }

        [TestMethod]
        public void NgramTextMethod()
        {

            TinySegmenterDotNet.TinySegmenter ts = new TinySegmenterDotNet.TinySegmenter();
            Console.WriteLine(ts.SegmentExted("私は[任務完了]です").JoinText("_"));
            Console.WriteLine(ts.SegmentExted("私は[任務完了]です").Ngram(2,string.Empty).JoinText("_"));

            ts.AddWordDic("[任務完了]");
            ts.AddWordDic("[任務");
            Console.WriteLine(ts.SegmentExted("私は[任務完了]です").JoinText("_"));

        } 
        [TestMethod]
        public void 正規表現Method()
        {
            System.Text.RegularExpressions.Regex r1 = new System.Text.RegularExpressions.Regex("[ぁ-ん。、]", System.Text.RegularExpressions.RegexOptions.Compiled);
            System.Text.RegularExpressions.Regex r2 = new System.Text.RegularExpressions.Regex("[ぁ-ん。、][ぁ-ん。、]", System.Text.RegularExpressions.RegexOptions.Compiled);

            Action<string> write = (n) => { Console.WriteLine(n + ":" + r1.Match(n).Success + "\t" + r2.Match(n).Success); };

            write("あい");
            write("あ");
            write("かなしい");

        }

       

    }
}
