using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
{
    public class GetFileName : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetFileName>(parent);
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
            try
            {
                Uri u = new Uri(GetText());
                this.SetText(u.Segments.Last());
            }
            catch
            {
                this.SetText(string.Empty);
                ReportManage.ErrReport(this, "GetFileName:Uriの変換に失敗しました");
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
                return base.text;
            }
        }


    }
}
