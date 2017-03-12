using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
{
    public class ChangeCurrentDataRow : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ChangeCurrentDataRow>(parent);
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
            var text = this.GetText();
            this.SetText(text);
            if (string.IsNullOrEmpty(text) == false)
            {
                var data = this.GetAncestorRawler().OfType<Data>().FirstOrDefault();
                if (data != null)
                {
                    data.ChangeCurrentDataRow(text);
                }
                else
                {
                    ReportManage.ErrReport(this, "上流にDataがありません");
                }
            }
            base.Run(runChildren);
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
