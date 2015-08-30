using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace RawlerUnitTestProject
{
    [TestClass]
    public class ScriptUnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {           

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            CompilerParameters cp = new CompilerParameters();
            cp.GenerateInMemory = true;
            cp.TreatWarningsAsErrors = false;
            cp.ReferencedAssemblies.Add("System.dll"); // Regex
            cp.ReferencedAssemblies.Add("System.Core.dll"); // Extensions

            string source = @"
            using System;
            using System.Linq;
            using System.Collections.Generic;

            class FunctionClass{
                 Func<string, string> test = (n) => n + n;
                public Func<string,string> Test { get { return test; } }
            }";

            CompilerResults cr = provider.CompileAssemblyFromSource(cp, source);

            //コンパイルしたアセンブリを取得
            var asm = cr.CompiledAssembly;
            //MainClassクラスのTypeを取得
            Type t = asm.GetType("FunctionClass");

            var instance = Activator.CreateInstance(t);
            var m = t.GetMember("test");


            var method = t.GetProperty("Test").GetValue(instance);

            var type2 = method.GetType();

            var func = (Func<string, string>)method;

            var r = func("test");
            
            System.Console.WriteLine(r);
        }

        static Func<string, string> test = (n) => n + "_1";
        public static Func<string,string> Test { get { return test; } }

        Func<string, string> r = (n) =>
         {
             int c = int.Parse(n);
             if (c % 15 == 0)
             {
                 return "FizzBuzz";
             }
             else if (c % 3 == 0)
             {
                 return "Fizz";
             }
             else if(c % 5 == 0)
             {
                 return "Buzz";
             }
             else
             {
                 return n;
             }


         };

        [TestMethod]
        public void Test2()
        {
            var r = LambadLib.CompileLambda<int, int>("(n)=>n*n");
            System.Console.WriteLine(r.Func(2));
        }


        public static class LambadLib
        {
            public struct LambadResult<T1, T2>
            {
                public CompilerError[] CompilerError { get; set; }
                public Func<T1, T2> Func { get; set; }
            }

            public static LambadResult<T1, T2> CompileLambda<T1, T2>(string source)
            {
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                CompilerParameters cp = new CompilerParameters();
                cp.GenerateInMemory = true;
                cp.TreatWarningsAsErrors = false;
                cp.ReferencedAssemblies.Add("System.dll"); // Regex
                cp.ReferencedAssemblies.Add("System.Core.dll"); // Extensions

                string source2 = @"
            using System;
            using System.Linq;
            using System.Collections.Generic;

            class FunctionClass{
                 Func<(T1), (T2)> test = (lambda);
                public Func<(T1),(T2)> Test { get { return test; } }
            }";

                //入力したもので書き換え。
                source2 = source2.Replace("(T1)",typeof(T1).Name).Replace("(T2)",typeof(T2).Name).Replace("(lambda)", source);
                //コンパイル
                CompilerResults cr = provider.CompileAssemblyFromSource(cp, source2);

                LambadResult<T1, T2> r = new LambadResult<T1, T2>();
                r.CompilerError = cr.Errors.OfType<CompilerError>().ToArray();

                if (cr.Errors.Count > 0) return r;

                //コンパイルしたアセンブリを取得
                var asm = cr.CompiledAssembly;
                //MainClassクラスのTypeを取得
                Type t = asm.GetType("FunctionClass");

                var instance = Activator.CreateInstance(t);
                var m = t.GetMember("test");

                var method = t.GetProperty("Test").GetValue(instance, null);
                r.Func = (Func<T1, T2>)method;

                return r;
            }
        }


    }
}
