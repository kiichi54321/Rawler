using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class GetUrlParameter : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetUrlParameter>(parent);
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
            var paras = GetText().Split('?');
            List<string> list = new List<string>();
            if (paras.Length > 1)
            {
                foreach (var item in paras.Last().Split('&'))
                {
                    if (string.IsNullOrEmpty(ParameterName) == false)
                    {
                        var d = item.Split('=');
                        if (d.First() == ParameterName)
                        {
                            list.Add(d.Last());
                        }
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
            }
            else
            {
                ReportManage.ErrReport(this, "UrlParameter:?がありません。パラメータが見つかりませんでした");
            }
            base.RunChildrenForArray(runChildren, list);
        }

        public string ParameterName { get; set; }


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
