using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawlerLib.MyExtend;

namespace Rawler.Web
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
            if (string.IsNullOrEmpty(url) == false)
            {
                this.startUrl = url;
                this.currentUrl = url;
            }
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

        public string Html { get; set; }
        Stack<string> HTMLs { get; set; }
        public bool UseParentPage { get; set; } = false;

        public override void Run(bool runChildren)
        {
            if (UseParentPage)
            {
                var page = this.GetUpperRawler<Page>();
                if (page != null)
                {
                    HTMLs.Push(page.GetCurrentPage().WaitResult());
                    this.Url = page.GetCurrentUrl();
                }
            }
            if(string.IsNullOrEmpty( Html )==false)
                {
                HTMLs.Push(Html.Convert(this));
            }
            do
            {
                SetText(HTMLs.Pop());
                RunChildren(runChildren);
            }
            while (HTMLs.Peek() != null);

        }
    }

    /// <summary>
    /// 上流のPageNoDownLoadにテキストを渡す。
    /// </summary>
    public class SetPageNoDownLoad : RawlerBase
    {
        public string Html { get; set; }
        public string Url { get; set; }
        public override void Run(bool runChildren)
        {
            var page = this.GetUpperRawler<PageNoDownLoad>();
            if(page !=null)
            {
                page.SetPage(Url.Convert(this), Html.Convert(this));
            }
            base.Run(runChildren);
        }
    }

}
