using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class GetPageHtml:RawlerBase
    {
        public string TargetName { get; set; }

        public override string Text
        {
            get
            {
                var pages = this.GetAncestorRawler().Where(n => n is Page);
                if (pages.Count() > 0)
                {
                    if (string.IsNullOrEmpty(TargetName) == false)
                    {
                        if(pages.Where(n=>n.Name == TargetName).Any())
                        {
                            return GetText(pages.Where(n => n.Name == TargetName).First().Text);
                        }
                        else
                        {
                            ReportManage.ErrReport(this, "TargetName「"+TargetName +"」が見つかりません");
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
}
