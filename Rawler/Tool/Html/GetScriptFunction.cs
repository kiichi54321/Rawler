using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class GetScriptFunction : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetScriptFunction>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"(\w*)\(([^)]*)\)");

            List<string> list = new List<string>();
            foreach (System.Text.RegularExpressions.Match item in r.Matches(GetText()))
            {
                if (string.IsNullOrEmpty(FunctionName))
                {
                    list.Add(item.Groups[0].Value);
                }
                else
                {
                    if (item.Groups[1].Value == FunctionName)
                    {
                        if (parameterOrder > -1)
                        {
                            var para = item.Value.Replace(FunctionName+"(","").Replace(")","").Split(',').ElementAtOrDefault(parameterOrder);
                            list.Add(para.Trim().Trim(new char[]{'\''}));
                        }
                        else
                        {
                            list.Add(item.Groups[2].Value);
                        }
                    }
                }
            }
            this.RunChildrenForArray(runChildren, list);
        }

        public string FunctionName { get; set; }
        int parameterOrder = -1;

        public int ParameterOrder
        {
            get { return parameterOrder; }
            set { parameterOrder = value; }
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }

    //public enum ScriptFunctionViewType
    //{
    //    Function,Parameter
    //}
}
