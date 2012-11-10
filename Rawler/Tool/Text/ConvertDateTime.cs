using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class ConvertDateTime : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ConvertDateTime>(parent);
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
            DateTime dt;
            if (DateTime.TryParse(GetText(), out dt))
            {
                if (Format != null)
                {
                    this.SetText(dt.ToString(Format));
                }
                else
                {
                    this.SetText(dt.ToString());
                }
                base.Run(runChildren);
            }
            else
            {
                if (ErrText != null)
                {
                    SetText(ErrText);
                }
                else
                {
                    SetText("失敗:" + GetText());
                }

                ReportManage.ErrReport(this,GetText() + "はDateTime.TryParseに失敗ました。");
            }

        }

        public string ErrText { get; set; }
        public string Format { get; set; }
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
