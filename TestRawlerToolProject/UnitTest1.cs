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
    }
}
