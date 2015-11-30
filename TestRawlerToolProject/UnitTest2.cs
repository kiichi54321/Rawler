using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rawler.Tool;
using System.Diagnostics;
using System.Linq;
using RawlerLib.MyExtend;

namespace TestRawlerToolProject
{
    [TestClass]
    public class WebToolTest
    {
        [TestMethod]
        public void AutoEncodingTest()
        {            
            WebClient wc = new WebClient();
            var data = wc.HttpGetByte("http://kakaku.com/kaden/");
            System.Text.Encoding encoding;
            var html = wc.GetAutoEncoding(data, out encoding);
            Trace.WriteLine(encoding);
            Console.WriteLine(encoding.BodyName);
            Console.WriteLine(html);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var Files = new string[] { "aaa.tsv", "bbb.tsv", "ccc.tsv" };
            var dic = Files.AsParallel().WithDegreeOfParallelism(6).Select(path => System.IO.File.ReadLines(path).ToCountDictionary(n => n.Split('\t').First())).Marge();

        }

    }
}
