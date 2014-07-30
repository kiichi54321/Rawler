using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.NPL
{
    public class PreprocessingForJapanese : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<PreprocessingForJapanese>(parent);
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
            var t = GetResult(GetText());
            this.SetText(t);
            base.Run(runChildren);
        }

        public string GetResult(string text)
        {
            var t = text;

            List<string> list = new List<string>();
            int count = 0;
            foreach (var item in RawlerLib.String.TextReadLines(t))
            {
                string tt = item.Trim();
                if (tt.Length > 0)
                {
                    list.Add(tt);
                    if (count > 1)
                    {
                        list.Add("\n");
                    }
                    count = 0;
                }
                else
                {
                    count++;
                }
            }
            if (list.Any())
            {
                t = list.Aggregate((n, m) => n +" "+ m);
            }
            else
            {
                t = string.Empty;
            }
            t = CSharp.Japanese.Kanaxs.Kana.ToHankaku(t);
            t = CSharp.Japanese.Kanaxs.Kana.ToZenkakuKana(t);
            t = CSharp.Japanese.Kanaxs.Kana.ToPadding(t);
            t = t.ToLower();
            t = t.Replace("～", "ー").Replace("－", "ー").Replace("─", "ー");
            return t;
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
