using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rawler.Tool;
using System.Diagnostics;
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


    }
}
