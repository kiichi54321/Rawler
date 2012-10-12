using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class RemoveWordByRegular : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<RemoveWordByRegular>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion
    
        
        private string pattern = string.Empty;

        public string Pattern
        {
            get { return pattern; }
            set { pattern = value; }
        }


        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, regexOption);
            StringBuilder sb = new StringBuilder(GetText());
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(GetText()))
            {
                sb.Replace(match.Value, string.Empty);
            }
            this.SetText(sb.ToString());
            base.Run(runChildren);
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
