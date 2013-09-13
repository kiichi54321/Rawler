using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class AddAllInputParameter : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<AddAllInputParameter>(parent);
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
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("<input [^>]*>");
            System.Text.RegularExpressions.Regex r2 = new System.Text.RegularExpressions.Regex(@"(\w)*\s*=\s*" + "\"([^\"]*)\"");

            var p = this.GetAncestorRawler().OfType<Page>();
            if (p.Any() == false)
            {
                ReportManage.ErrReport(this, "上流にPageがありません");
                return;
            }
            var page = p.First();

            foreach (System.Text.RegularExpressions.Match item in r.Matches(GetText()))
            {
                var dic = GetParameter(item.Value);
                if (dic.ContainsKey("type") && dic["type"] == "radio")
                {
                    if (dic.ContainsKey("name") && dic.ContainsKey("value") && dic.ContainsKey("checked"))
                    {
                        page.AddParameter(dic["name"], dic["value"]);
                    }
                }
                else if (dic.ContainsKey("type") && dic["type"] == "submit")
                {
                }
                else
                {
                    if (dic.ContainsKey("name") && dic.ContainsKey("value"))
                    {
                        page.AddParameter(dic["name"], dic["value"]);
                    }
                }

            }
            base.Run(runChildren);
        }

        private Dictionary<string, string> GetParameter(string input)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            System.Text.RegularExpressions.Regex r2 = new System.Text.RegularExpressions.Regex(@"(\w*)\s*=\s*" + "\"([^\"]*)\"");
            foreach (System.Text.RegularExpressions.Match item in r2.Matches(input))
            {
                string key = item.Groups[1].Value;
                string val = item.Groups[2].Value;
                dic.Add(key, val);
            }
            return dic;
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
}
