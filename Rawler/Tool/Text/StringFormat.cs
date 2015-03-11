using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
  
    public class StringFormat : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<StringFormat>(parent);
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
            if (string.IsNullOrEmpty(Format))
            {
                ReportManage.ErrReport(this, "Formatが空です");
            }
            else if (Args == null)
            {
                ReportManage.ErrReport(this, "Argsが空です");
            }
            else
            {
                List<string> list = new List<string>();
                foreach (var item in Args)
                {
                    item.SetParent(this.Parent);
                    item.SetParent();
                    list.Add(RawlerBase.GetText(GetText(), item,this.Parent));
                }
                SetText(string.Format(Format, list.ToArray()));
            }
            base.Run(runChildren);
        }

        public string Format { get; set; }
        public IEnumerable<RawlerBase> Args { get; set; }

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
