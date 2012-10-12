using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// タグを消去する。
    /// </summary>
    public class TagClear:RawlerBase
    {
        public override string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    this.text = HtmlTagAllDelete(GetText());
                }
                else
                {
                    this.text = string.Empty;
                }
                return this.text;
            }
        }

        

        /// <summary>
        /// HTMLのタグを全部削除。また文字参照（&lt;など）も置換します
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string HtmlTagAllDelete(string html)
        {
            bool tagStart = false;

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            foreach (char c in html.ToCharArray())
            {
                if (tagStart == true)
                {
                    if (c.Equals('>'))
                    {
                        tagStart = false;
                        //tagの終了時にはスペースを加える
                        strBuilder.Append(' ');
                    }
                }
                else
                {
                    if (c.Equals('<'))
                    {
                        tagStart = true;
                    }
                    else
                    {
                        strBuilder.Append(c);
                    }
                }
            }
            strBuilder.Replace("&nbsp;", " ");
            strBuilder.Replace("&lt;", "<");
            strBuilder.Replace("&gt;", ">");
            strBuilder.Replace("&amp;", "&");
            strBuilder.Replace("&quot;", "\"");
            return strBuilder.ToString();


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
            return base.Clone<TagClear>(parent);
        }
    }
}
