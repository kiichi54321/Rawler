using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.NPL
{
    public class LevenshteinDistance : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<LevenshteinDistance>(parent);
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
            if (Text2Tree == null)
            {
                ReportManage.ErrReport(this, "Text2Treeが空です。実行にはText2Treeが必要です。");
                return;
            }
            Text2Tree.SetParent(this);
            string t = RawlerBase.GetText(this.parent.Text, Text2Tree, this);

            var result = Compute(GetText(), t);

            if (result <= maxDistance)
            {
                SetText(result.ToString());

                base.Run(runChildren);
            }
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

        public RawlerBase Text2Tree { get; set; }

        private int maxDistance = 5;

        public int MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }


        /// <summary>
        /// レーベンシュタイン距離
        /// 参照 http://d.hatena.ne.jp/zecl/20090312/p1
        /// </summary>
        public static int Compute(string s, string t)
        {
            if (s == null) throw new ArgumentNullException("s");
            if (t == null) throw new ArgumentNullException("t");

            int x = s.Length;
            int y = t.Length;

            if (x == 0) return y;
            if (y == 0) return x;

            int[,] d = new int[x + 1, y + 1];

            for (int i = 0; i <= x; d[i, 0] = i++) ;
            for (int j = 0; j <= y; d[0, j] = j++) ;

            int cost = default(int);
            for (int i = 1; i <= x; i++)
            {
                for (int j = 1; j <= y; j++)
                {
                    cost = (s.Substring(i - 1, 1) != t.Substring(j - 1, 1) ? 1 : 0);
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1)
                                     , d[i - 1, j - 1] + cost);
                }
            }
            return d[x, y];
        }

    }
}
