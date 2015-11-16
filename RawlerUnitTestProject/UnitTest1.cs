using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RawlerExpressLib.TestExtend;
using System.Collections.Generic;
using System.Linq;
using RawlerLib.MyExtend;

namespace RawlerUnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            new class1().GetTypeName().ConsoleWriteLine();
            new class2().GetTypeName().ConsoleWriteLine();

            Console.WriteLine("こんにちわこんにちわ");
        }

        [TestMethod]
        public void TestDate()
        {
            var date = DateTime.Now;
            
            date.ToLongDateString().ConsoleWriteLine();
            date.ToLongTimeString().ConsoleWriteLine();
            date.ToShortDateString().ConsoleWriteLine();
            date.ToShortTimeString().ConsoleWriteLine();
            date.ToString().ConsoleWriteLine();


        }
        [TestMethod]
        public void Text()
        {
            string text = "[[key]][value].txt";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\[\w*\]");
            r.Matches(text).OfType<System.Text.RegularExpressions.Match>().Select(n => n.Value).ConsoleWriteLine();
        }

        [TestMethod]
        public void MoveForward()
        {
            var list = Enumerable.Range(0, 10).ToList();
            list.MoveForward(5);
            list.ConsoleWriteLine();
        }

        [TestMethod]
        public void MoveBack()
        {
            var list = Enumerable.Range(0, 10).ToList();
            list.MoveBack(5);
            list.ConsoleWriteLine();
        }
    }
    public class Node
    {
        public string Name { get; set; }
        List<Node> children = new List<Node>();
        public void Add(Node node) => children.Add(node);
    }

    


    public class class1
    {
        public string GetTypeName()
        {
            return this.GetType().DeclaringType.Name;
        }
    }

    public class class2:class1
    { }
}
