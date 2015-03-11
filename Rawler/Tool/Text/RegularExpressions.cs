using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 正規表現を行います。
    /// </summary>
    public class RegularExpressions:RawlerMultiBase
    {
        private string pattern = string.Empty;

        public string Pattern
        {
            get { return pattern; }
            set { pattern = value; }
        }


        private bool emptyReport = false;
        /// <summary>
        /// 空文字列の時、報告するか？
        /// </summary>
        public bool EmptyReport
        {
            get { return emptyReport; }
            set { emptyReport = value; }
        }


        int groupNum = 0;

        public int GroupNum
        {
            get { return groupNum; }
            set {
                if (value < 0)
                {
                    groupNum = 0;
                }
                else
                {
                    groupNum = value;
                }
            }
        }

        public override void Run(bool runChildren)
        {
            List<string> list = new List<string>();
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, regexOption);
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(GetText()))
            {
                if (match.Groups.Count > groupNum)
                {
                    list.Add(match.Groups[groupNum].Value);
                }
            }

            if (list.Count == 0)
            {
                if (emptyReport)
                {
                    ReportManage.ErrReport(this, "対象が見つかりませんでした");
                }

            }
            else
            {
                this.RunChildrenForArray(runChildren, texts);
            }


        }


        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        private System.Text.RegularExpressions.RegexOptions regexOption = System.Text.RegularExpressions.RegexOptions.None;
        
        /// <summary>
        /// 正規表現のオプション。デフォルト値は、Noneです。
        /// </summary>
        public System.Text.RegularExpressions.RegexOptions RegexOption
        {
            get { return regexOption; }
            set { regexOption = value; }
        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<RegularExpressions>(parent);
        }
    }
}
