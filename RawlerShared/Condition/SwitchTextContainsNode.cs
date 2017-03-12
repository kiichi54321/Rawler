using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler.Condition
{
    public class CaseText : CaseBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CaseText>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public string CheckText { get; set; }

        public override bool Check(string txt)
        {
            if (CheckText != null && CheckText.Length > 0)
            {
                return txt.Contains(CheckText);
            }
            else
            {
                ReportManage.ErrReport(this, "SwitchTextNodeオブジェクトでCheckTextの値がありません。");
                return false;
            }
        }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            base.Run(runChildren);
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }
        }


    }
}
