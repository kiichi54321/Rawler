using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool.Web
{
    /// <summary>
    /// ページをダウンロードすることなくPageとして扱うクラス。
    /// </summary>
    public class PageNoDownLoad:Page
    {        
        /// <summary>
        /// ページの情報をセットする。メソッドチェーンのため、自身を返す。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public PageNoDownLoad SetPage(string url,string html)
        {
            this.startUrl = url;
            this.currentUrl = url;
            this.SetText(html);
            return this;
        }

        public override string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                base.Url = value;
                this.startUrl = value;
                this.currentUrl = value;
            }
        }

        public List<string> HTMLs { get; set; }

        public override void Run(bool runChildren)
        {
            if(HTMLs !=null && HTMLs.Any())
            {
                foreach (var item in HTMLs)
                {
                    SetText(item);
                    base.Run(runChildren);
                }
            }
            else
            {
                //何もしない。
                base.Run(runChildren);
            }
        }
    }
}
