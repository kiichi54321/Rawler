using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
    }
}
