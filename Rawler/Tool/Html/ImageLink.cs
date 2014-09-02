using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// イメージタグを取りだすRawlerクラス。
    /// </summary>
    public class ImageLinks : RawlerMultiBase
    {
        /// <summary>
        /// イメージタグを取りだすRawlerクラス。
        /// </summary>
        public ImageLinks()
            : base()
        {
        }
        ///// <summary>
        ///// イメージタグを取りだすRawlerクラス。
        ///// </summary>
        ///// <param name="UrlFilter">含まれるURLの指定する</param>
        ///// <param name="LabelFilter">含まれるLabelの指定する（不完全）</param>
        ///// <param name="isMulti">複数か？</param>
        //public ImageLink(string UrlFilter, string LabelFilter, bool isMulti)
        //    : base()
        //{
        //    this.UrlFilter = UrlFilter;
        //    this.LabelFilter = LabelFilter;
        //    this.isMulti = isMulti;
        //}
        /// <summary>
        /// 含まれるURLの指定する
        /// </summary>
        public string UrlFilter { get; set; }
        /// <summary>
        /// 含まれるLabelの指定する（不完全）
        /// </summary>
        public string LabelFilter { get; set; }

        private LinkVisbleType visbleType = LinkVisbleType.Url;

        /// <summary>
        /// 子に流すデータの指定。
        /// </summary>
        public LinkVisbleType VisbleType
        {
            get { return visbleType; }
            set { visbleType = value; }
        }

        //private bool useAbsolutetLink = false;

        //public bool UseAbsolutetLink
        //{
        //    get { return useAbsolutetLink; }
        //    set { useAbsolutetLink = value; }
        //}


        private bool emptyReport = false;
        /// <summary>
        /// 空文字列の時、報告するか？
        /// </summary>
        public bool EmptyReport
        {
            get { return emptyReport; }
            set { emptyReport = value; }
        }

        private bool useAbsolutetLink = true;
        /// <summary>
        /// 絶対リンクで返す（初期値はTrue）
        /// </summary>
        public bool UseAbsolutetLink
        {
            get { return useAbsolutetLink; }
            set { useAbsolutetLink = value; }
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.Parent != null && this.Parent.Text != null)
            {
                if (true)
                {
                    List<RawlerLib.Web.Link> list;
                    if (useAbsolutetLink)
                    {
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetImageLink(GetText(), GetPageUrl()));
                        list.AddRange(RawlerLib.Web.GetBackImageLink(GetText(), GetPageUrl()));
                    }
                    else
                    {
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetImageLink(GetText()));
                        list.AddRange(RawlerLib.Web.GetBackImageLink(GetText(),string.Empty));
                    }


                    if (LabelFilter != null && LabelFilter.Length > 0)
                    {
                        list = new List<RawlerLib.Web.Link>(list.Where(n => n.Label.Contains(LabelFilter)));
                    }
                    if (UrlFilter != null && UrlFilter.Length > 0)
                    {
                        list = new List<RawlerLib.Web.Link>(list.Where(n => n.Url.Contains(UrlFilter)));

                    }
                    if (VisbleType == LinkVisbleType.Label)
                    {
                        SetTexts(list.Select(n => n.Label));

                    }
                    else if (VisbleType == LinkVisbleType.Url)
                    {
                        SetTexts(list.Select(n => n.Url));
                    }
                    else
                    {
                        SetTexts(list.Select(n => n.Tag)); ;
                    }
                    if (emptyReport && this.texts.Count() == 0)
                    {
                        ReportManage.ErrReport(this, "対象が見つかりませんでした");
                    }


                    RunChildrenForArray(runChildren, this.texts);

                }
                //else
                //{
                //    List<RawlerLib.Web.Link> list;

                //    list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetImageLink(GetText(), GetPageUrl()));
                //    if (LabelFilter != null && LabelFilter.Length > 0)
                //    {
                //        list = new List<RawlerLib.Web.Link>(list.Where(n => n.Label.Contains(LabelFilter)));
                //    }
                //    if (UrlFilter != null && UrlFilter.Length > 0)
                //    {
                //        list = new List<RawlerLib.Web.Link>(list.Where(n => n.Url.Contains(UrlFilter)));

                //    }
                //    if (list.Count > 0)
                //    {
                //        if (VisbleType == LinkVisbleType.Label)
                //        {
                //            this.text = list.First().Label;
                //        }
                //        else if (VisbleType == LinkVisbleType.Url)
                //        {
                //            this.text = list.First().Url;
                //        }
                //        else
                //        {
                //            this.text = list.First().Tag;
                //        }
                //    }
                //    else
                //    {
                //        this.text = string.Empty;
                //        if (emptyReport)
                //        {
                //            ReportManage.ErrReport(this, "対象が見つかりませんでした");
                //        }
                //    }

                //    RunChildren(runChildren);
                //}
            }
        }





        private string GetPageUrl()
        {
            IRawler currnet = this.Parent;
            while (currnet != null)
            {
                if (currnet is Page)
                {
                    var page = currnet as Page;
                    return page.GetCurrentUrl();
                }
                currnet = currnet.Parent;
            }
            return string.Empty;
        }

        /// <summary>
        /// このオブジェクトのテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return text;
            }

        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        bool doClone = false;
        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ImageLinks>(parent);
        }

    }
}

