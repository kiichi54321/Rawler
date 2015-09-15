using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RawlerExpressLib.TestExtend;
using System.Collections.Generic;

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
