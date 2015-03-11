using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class CompareInt : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CompareInt>(parent);
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
            int tmp;
            this.SetText(GetText());
            if (int.TryParse(GetText(), out tmp))
            {
                if (CompareType != null)
                {
                    CompareType = Tool.CompareType.over;
                }
                bool flag = false;

                if (CompareType == Tool.CompareType.over)
                {
                    if (CompareValue <= tmp)
                    {
                        flag = true;
                    }
                }
                if (CompareType == Tool.CompareType.under)
                {
                    if (CompareValue >= tmp)
                    {
                        flag = true;
                    }
                }
                if (CompareType == Tool.CompareType.beyond)
                {
                    if (CompareValue < tmp)
                    {
                        flag = true;
                    }
                }
                if (CompareType == Tool.CompareType.below)
                {
                    if (CompareValue > tmp)
                    {
                        flag = true;
                    }
                }

                if (flag)
                {
                    base.Run(runChildren);
                }
            }
        }

        public int CompareValue { get; set; }

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

        public CompareType CompareType { get; set; }


    }
    public enum CompareType
    {
        over, under, below, beyond
    }
}
