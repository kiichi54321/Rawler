using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{

    /// <summary>
    /// 上流のPageを取得する
    /// </summary>
    public class GetPageHtml:RawlerBase
    {
        /// <summary>
        /// 対象の名前。空の時は、無条件で上流にある初めのもの。
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// スキップする数。
        /// </summary>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Text
        /// </summary>
        public override string Text
        {
            get
            {
                var pages = this.GetAncestorRawler().Where(n => n is Page);
                pages = pages.Skip(Skip);
                if (pages.Count() > 0)
                {
                    var targetName = TargetName.Convert(this);
                    if (string.IsNullOrEmpty(targetName) == false)
                    {
                        if(pages.Where(n=>n.Name == targetName).Any())
                        {
                            return GetText(pages.Where(n => n.Name == targetName).First().Text);
                        }
                        else
                        {
                            ReportManage.ErrReport(this, "TargetName「"+targetName +"」が見つかりません");
                            return string.Empty;
                        }
                    }
                    else
                    {
                        return GetText(pages.First().Text);
                    }
                }
                else
                {
                    return string.Empty;
                }
            }

        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }


        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetPageHtml>(parent);
        }
    }

    /// <summary>
    /// 上流にあるPageにHTMLをセットする
    /// </summary>
    public class SetPageHtml:RawlerBase
    {
        /// <summary>
        /// 対象の名前。空の時は、無条件で上流にある初めのもの。
        /// </summary>
        public string TargetName { get; set; }
        /// <summary>
        /// SetするHTML
        /// </summary>
        public string Html { get; set; }

        public override void Run(bool runChildren)
        {
            var pages = this.GetAncestorRawler().Where(n => n is Page);
            if (pages.Count() > 0)
            {
                var html = Html.Convert(this);
                if(string.IsNullOrEmpty(html))
                {
                    html = GetText();
                }
                var targetName = TargetName.Convert(this);
                if (string.IsNullOrEmpty(targetName) == false)
                {
                    if (pages.Where(n => n.Name == targetName).Any())
                    {
                        pages.Where(n => n.Name == targetName).First().SetText(html);
                    }
                    else
                    {
                        ReportManage.ErrReport(this, "TargetName「" + targetName + "」が見つかりません");
                    }
                }
                else
                {
                    pages.First().SetText(html);
                }
                SetText(html);
            }
            else
            {
                ReportManage.ErrUpperNotFound<Page>(this);
            }

            base.Run(runChildren);  
        }

    }
}
