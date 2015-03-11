using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class Range : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Range>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        double start = 0;

        public double Start
        {
            get { return start; }
            set { start = value; }
        }
        double end = 0;

        public double End
        {
            get { return end; }
            set { end = value; }
        }
        double step = 1;

        public double Step
        {
            get { return step; }
            set { step = value; }
        }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            List<string> list = new List<string>();
            double c = start;
            while (c <= end)
            {

                list.Add(c.ToString());
                c = c + step;
            }
            base.RunChildrenForArray(runChildren, list);
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
