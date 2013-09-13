using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class CheckDataRow : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CheckDataRow>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public int MinAttributeCount { get; set; }
        public RawlerBase FailedTree { get; set; }
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var data = this.GetAncestorRawler().OfType<Data>().FirstOrDefault();
            if (data != null)
            {
                if (data.GetCurrentDataRow().DataDic.Count >= MinAttributeCount)
                {
                    base.Run(runChildren);
                }
                else
                {
                    if (FailedTree != null)
                    {
                        FailedTree.SetParent(this);
                        FailedTree.Run();
                    }
                }
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


    }
}
