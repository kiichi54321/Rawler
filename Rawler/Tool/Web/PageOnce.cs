using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 一度だけ実行する。スタートページとカレントページが同じ時に実行する。
    /// </summary>
    public class PageOnce:Rawler.Tool.RawlerBase
    {

        public override void Run(bool runChildren)
        {
            var page = this.GetAncestorRawler().OfType<Page>();

            if (page.Any())
            {
                if (page.First().GetStartUrl() == page.First().GetCurrentUrl())
                {
                    base.Run(runChildren);
                }
            }
            else
            {
                ReportManage.ErrReport(this, "Pageオブジェクトが上流にありません");
            }


        }

        /// <summary>
        /// このオブジェクトのテキスト。親のテキストをそのまま流す。
        /// </summary>
        public override string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.Text;
                }
                return string.Empty;
            }

        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<PageOnce>(parent);
        }
    }
}
