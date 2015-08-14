using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool.Script
{

    /// <summary>
    /// ラムダ式が使えるところ。
    /// </summary>
    public class LambdaExpression : RawlerBase
    {
        Func<string, string> func1;

        public override void Run(bool runChildren)
        {
            if (func1 == null)
            {
                CreateFunc();
            }
            if (func1 != null)
            {
                SetText(func1(GetText()));
                RunChildren(runChildren);
            }
            else
            {
                ReportManage.ErrReport(this, "ラムダ式の生成に失敗しました");
            }
        }

        protected void CreateFunc()
        {
            if (string.IsNullOrEmpty(Lambda) == false)
            {
                CreateLambda(Lambda);
            }
        }

        public string Lambda { get; set; }

        /// <summary>
        /// ラムダ式作成
        /// </summary>
        /// <param name="source"></param>
        public Func<string,string> CreateLambda(string source)
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
                 Func<string, string> test = (lambda);
                public Func<string,string> Test { get { return test; } }
            }";

            //入力したもので書き換え。
            source2 = source2.Replace("(lambda)", source);
            //コンパイル
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, source2);

            cr.Errors.OfType<CompilerError>().ToArray();
            //エラー処理
            foreach (CompilerError item in cr.Errors)
            {
                ReportManage.ErrReport(this, item.ToString());
            }
            if (cr.Errors.Count > 0) return null;

            //コンパイルしたアセンブリを取得
            var asm = cr.CompiledAssembly;
            //MainClassクラスのTypeを取得
            Type t = asm.GetType("FunctionClass");

            var instance = Activator.CreateInstance(t);
            var m = t.GetMember("test");

            var method = t.GetProperty("Test").GetValue(instance, null);

            return (Func<string, string>)method;
        }
    }

    public static class LambadLib
    {
        public struct LambadResult<T1,T2>
        {
            public CompilerError[] CompilerError { get; set; }
            public Func<T1, T2> Func { get; set; }
        }

        public static LambadResult<T1,T2> CompileLambda<T1,T2>(string source)
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
                 Func<string, string> test = (lambda);
                public Func<string,string> Test { get { return test; } }
            }";

            //入力したもので書き換え。
            source2 = source2.Replace("(lambda)", source);
            //コンパイル
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, source2);

            LambadResult<T1,T2> r = new LambadResult<T1,T2>();
            r.CompilerError = cr.Errors.OfType<CompilerError>().ToArray();

            if (cr.Errors.Count > 0) return r;

            //コンパイルしたアセンブリを取得
            var asm = cr.CompiledAssembly;
            //MainClassクラスのTypeを取得
            Type t = asm.GetType("FunctionClass");

            var instance = Activator.CreateInstance(t);
            var m = t.GetMember("test");

            var method = t.GetProperty("Test").GetValue(instance, null);
            r.Func = (Func<T1,T2>)method;

            return r;
        }
    }



    ///// <summary>
    ///// 
    ///// </summary>
    //[ContentProperty("ScriptText")]
    //public class Script:RawlerBase
    //{

    //    Assembly asm = null;

    //    public string Source { get; set; }

    //    public override void Run(bool runChildren)
    //    {
    //        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

    //        CompilerParameters cp = new CompilerParameters();
    //        cp.GenerateInMemory = true;
    //        cp.TreatWarningsAsErrors = false;
    //        cp.ReferencedAssemblies.Add("System.dll"); // Regex
    //        cp.ReferencedAssemblies.Add("System.Core.dll"); // Extensions

    //        string source = "class FunctionClass{"+  Source+"}";

    //        CompilerResults cr = provider.CompileAssemblyFromSource(cp, source);

    //        //コンパイルしたアセンブリを取得
    //        asm = cr.CompiledAssembly;
    //        //MainClassクラスのTypeを取得
    //        Type t = asm.GetType("FunctionClass");
    //        //EValメソッドを実行し、結果を取得
    //        double d = (double)t.InvokeMember("EVal",
    //            BindingFlags.InvokeMethod,
    //            null,
    //            null,
    //            null);

    //        base.Run(runChildren);
    //    }

    //    Func<string, string> test = (n) => {
    //        return DateTime.Now.ToLongDateString();
    //    };


    //    public string[] RunScript(string function,string text)
    //    {
    //       var t =  asm.GetType("FunctionClass");
    //        var m = t.GetMembers().Where(n => n.Name == function).First();


    //        //if (m.  == typeof(string))
    //        //{
    //        //    string result = (string)m.Invoke(null, new List<object>() { text }.ToArray());
    //        //    return new string[] { result };
    //        //}
    //        //else if (m.ReturnType == typeof(string[]))
    //        //{
    //        //    string[] result = (string[])m.Invoke(null, new List<object>() { text }.ToArray());
    //        //    return result;
    //        //}
    //        //else
    //        //{

    //        //}

    //        return null;


    //    }

    //    public override void Dispose()
    //    {

    //        base.Dispose();
    //    }
    //}


    ///// <summary>
    ///// 
    ///// </summary>
    //public class RunScript : RawlerMultiBase
    //{
    //    Func<string, string> func1;
    //    Func<string, string[]> func2;

    //    public override void Run(bool runChildren)
    //    {
    //        if(func1 == null && func2 == null)
    //        {
    //            CreateFunc();
    //        }
    //        if (func1 != null)
    //        {
    //            SetText(func1(GetText()));
    //            RunChildren(runChildren);
    //        }
    //        else if (func2 != null)
    //        {
    //            var list = func2(GetText());
    //            RunChildrenForArray(runChildren, list);
    //        }
    //        else
    //        {

    //        }




    //    }

    //    protected void CreateFunc()
    //    {
    //        if(string.IsNullOrEmpty( Lamda)==false)
    //        {
    //            CreateLamda(Lamda);
    //        }

    //    }

    //    public string Lamda { get; set; }

    //    protected void CreateLamda(string source)
    //    {

    //        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

    //        CompilerParameters cp = new CompilerParameters();
    //        cp.GenerateInMemory = true;
    //        cp.TreatWarningsAsErrors = false;
    //        cp.ReferencedAssemblies.Add("System.dll"); // Regex
    //        cp.ReferencedAssemblies.Add("System.Core.dll"); // Extensions

    //        string source2 = @"
    //        using System;
    //        using System.Linq;
    //        using System.Collections.Generic;

    //        class FunctionClass{
    //             Func<string, string> test = (lamda);
    //            public Func<string,string> Test { get { return test; } }
    //        }";

    //        source2 = source2.Replace("(lamda)", source);
    //        CompilerResults cr = provider.CompileAssemblyFromSource(cp, source2);

    //        foreach (CompilerError item in cr.Errors)
    //        {
    //            ReportManage.ErrReport(this, item.ToString());
    //        }
    //        if (cr.Errors.Count > 0) return;

    //        //コンパイルしたアセンブリを取得
    //        var asm = cr.CompiledAssembly;
    //        //MainClassクラスのTypeを取得
    //        Type t = asm.GetType("FunctionClass");

    //        var instance = Activator.CreateInstance(t);
    //        var m = t.GetMember("test");


    //        var method = t.GetProperty("Test").GetValue(instance,null);

    //        func1 = (Func<string, string>)method;
    //    }


    //}

}
