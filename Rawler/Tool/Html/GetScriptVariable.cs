using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class GetScriptVariable:RawlerBase
    {
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// このオブジェクトのテキスト
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();

            }

        }


        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            GetVar();
            this.RunChildren(runChildren);

        }


        private Dictionary<string, string> varDic = new Dictionary<string, string>();

        void GetVar()
        {
            if (this.Parent != null)
            {
                varDic.Clear();
                var scriptList = RawlerLib.MarkupLanguage.TagAnalyze.GetTag(GetText(), "Script",true);
                System.Text.RegularExpressions.Regex functionRegrex = new System.Text.RegularExpressions.Regex("", System.Text.RegularExpressions.RegexOptions.Multiline);
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"var ([^=]*)=([^;^\n]*)[\n|;]", System.Text.RegularExpressions.RegexOptions.Multiline);
                foreach (var item2 in scriptList)
                {
                    foreach (System.Text.RegularExpressions.Match item in regex.Matches(item2.Inner))
                    {
                        string name = item.Groups[1].Value;
                        string value = item.Groups[2].Value;
                        value = value.Trim();
                        name = name.Trim();
                        if (value.Substring(0, 1) == "\"")
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        if (varDic.ContainsKey(name))
                        {
                            varDic[name] = value;
                        }
                        else
                        {
                            varDic.Add(name, value);
                        }
                    }
                }
            }
        }

        public string GetValue(string varName)
        {
            if (varDic.ContainsKey(varName))
            {
                return varDic[varName];
            }
            else
            {
                return null;
            }
            
        }


        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetScriptVariable>(parent);
            //var clone = new GetScriptVariable();
            //RawlerLib.ObjectLib.FildCopy(this, clone);

            //clone.SetParent(parent);
            //this.CloneEvent(clone);
            //clone.children.Clear();
            //foreach (var item in this.Children)
            //{
            //    var child = item.Clone(clone);
            //    clone.AddChildren(child);
            //}
            //return clone;
        }
    }
}
