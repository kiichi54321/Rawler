using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// タグを消去する。
    /// </summary>
    public class RemoveHtmlComment : RawlerBase
    {
        public override string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    this.text = RemoveComment(GetText());
                }
                else
                {
                    this.text = string.Empty;
                }
                return this.text;
            }
        }

        private string RemoveComment(string html)
        {
            var list = RawlerLib.Web.GetTagContentList(html, "<!--", "-->",true);
            StringBuilder sb = new StringBuilder(html);
            foreach (var item in list)
            {
                sb = sb.Replace(item, "");
            }

            return sb.ToString();
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
            return base.Clone<RemoveHtmlComment>(parent);
        }
    }

}
