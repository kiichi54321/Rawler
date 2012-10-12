using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class GetPageHtml:RawlerBase
    {
        public override string Text
        {
            get
            {
                var pages = this.GetAncestorRawler().Where(n => n is Page);
                if (pages.Count() > 0)
                {
                    return GetText(pages.First().Text);
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
