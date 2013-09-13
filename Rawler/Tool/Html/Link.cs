using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// Linkを取得するRawlerオブジェクト
    /// </summary>
    [ContentProperty("Children")]
    [Serializable]
    public class Link : RawlerMultiBase
    {
        /// <summary>
        ///  Linkを取得するRawlerオブジェクト
        /// </summary>
        public Link()
            : base()
        {
        }

        ///// <summary>
        ///// Linkを取得するRawlerオブジェクト
        ///// </summary>
        ///// <param name="UrlFilter">URLの含まれている文字列</param>
        ///// <param name="LabelFilter">Labelに含まれている文字列</param>
        ///// <param name="isMulti">単数か複数か</param>
        //public Link(string UrlFilter, string LabelFilter,bool isMulti)
        //    : base()
        //{
        //    this.UrlFilter = UrlFilter;
        //    this.LabelFilter = LabelFilter;
        //    this.isMulti = isMulti;
        //}

        ///// <summary>
        /////  Linkを取得するRawlerオブジェクト
        ///// </summary>
        ///// <param name="UrlFilter">URLの含まれている文字列</param>
        ///// <param name="LabelFilter">Labelに含まれている文字列</param>
        ///// <param name="isMulti">単数か複数か</param>
        ///// <param name="type">子に流すデータタイプ</param>
        //public Link(string UrlFilter, string LabelFilter, bool isMulti,LinkVisbleType type)
        //    : base()
        //{
        //    this.UrlFilter = UrlFilter;
        //    this.LabelFilter = LabelFilter;
        //    this.isMulti = isMulti;
        //    this.visbleType = type;
        //}

        ///// <summary>
        /////  Linkを取得するRawlerオブジェクト
        ///// </summary>
        ///// <param name="UrlFilter">URLの含まれている文字列</param>
        ///// <param name="LabelFilter">Labelに含まれている文字列</param>
        ///// <param name="isMulti">単数か複数か</param>
        ///// <param name="type">子に流すデータタイプ</param>
        ///// <param name="useAbsolutetLink">絶対リンクで返す</param>
        //public Link(string UrlFilter, string LabelFilter, bool isMulti, LinkVisbleType type, bool useAbsolutetLink)
        //    : base()
        //{
        //    this.UrlFilter = UrlFilter;
        //    this.LabelFilter = LabelFilter;
        //    this.isMulti = isMulti;
        //    this.visbleType = type;
        //    this.useAbsolutetLink = useAbsolutetLink;
        //}



        /// <summary>
        /// URLの含まれている文字列
        /// </summary>
        public string UrlFilter { get; set; }
        /// <summary>
        /// Labelに含まれている文字列
        /// </summary>
        public string LabelFilter { get; set; }

        public string TagFilter { get; set; }

        private LinkVisbleType visbleType = LinkVisbleType.Url;
        /// <summary>
        /// 子に流すデータタイプ（初期値はURL）
        /// </summary>
        public LinkVisbleType VisbleType
        {
            get { return visbleType; }
            set { visbleType = value; }
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

        private bool isSingle = false;

        public bool IsSingle
        {
            get { return isSingle; }
            set { isSingle = value; }
        }

        private bool emptyReport = false;
        /// <summary>
        /// 空文字列の時、報告するか？
        /// </summary>
        public bool EmptyReport
        {
            get { return emptyReport; }
            set { emptyReport = value; }
        }

        public string TargetTag { get; set; }

        //private bool useDistinct = false;

        //public bool UseDistinct
        //{
        //    get { return useDistinct; }
        //    set { useDistinct = value; }
        //}
        /// <summary>
        /// 実行する
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.Parent != null && this.Parent.Text != null)
            {

                List<RawlerLib.Web.Link> list;
                if (string.IsNullOrEmpty(TargetTag))
                {
                    if (useAbsolutetLink)
                    {
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(), GetPageUrl()));
                    }
                    else
                    {
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText()));
                    }
                }
                else
                {
                    if (useAbsolutetLink)
                    {
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(), GetPageUrl(),TargetTag));
                    }
                    else
                    {
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(),null, TargetTag));
                    }
                }

                if (LabelFilter != null && LabelFilter.Length > 0)
                {
                    list = new List<RawlerLib.Web.Link>(list.Where(n => n.Label.Contains(LabelFilter)));
                }
                if (UrlFilter != null && UrlFilter.Length > 0)
                {
                    list = new List<RawlerLib.Web.Link>(list.Where(n => n.Url.Contains(UrlFilter)));

                }
                if (TagFilter != null && TagFilter.Length > 0)
                {
                    list = new List<RawlerLib.Web.Link>(list.Where(n => n.Tag.Contains(TagFilter)));
                }


                if (VisbleType == LinkVisbleType.Label)
                {

                    {
                        this.texts = new List<string>(list.Select(n => n.Label));
                    }
                }
                else if (VisbleType == LinkVisbleType.Url)
                {
                    {
                        this.texts = new List<string>(list.Select(n => n.Url));
                    }
                }
                else
                {
                    {
                        this.texts = new List<string>(list.Select(n => n.Tag));
                    }
                }
                if (emptyReport && this.texts.Count() == 0)
                {
                    ReportManage.ErrReport(this, "対象が見つかりませんでした");
                }

                if (IsSingle == false)
                {
                    RunChildrenForArray(runChildren, this.texts);

                }
                else
                {
                    if (this.texts.Count() > 0)
                    {
                        SetText(this.texts.First());
                        RunChildren(runChildren);
                    }
                }
                //else
                //{
                //    List<RawlerLib.Web.Link> list;
                //    if (useAbsolutetLink)
                //    {
                //        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(), GetPageUrl()));
                //    }
                //    else
                //    {
                //        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText()));
                //    }
                //    if (LabelFilter != null && LabelFilter.Length > 0)
                //    {
                //        list = new List<RawlerLib.Web.Link>(list.Where(n => n.Label.Contains(LabelFilter)));
                //    }
                //    if (UrlFilter != null && UrlFilter.Length > 0)
                //    {
                //        list = new List<RawlerLib.Web.Link>(list.Where(n => n.Url.Contains(UrlFilter)));

                //    }
                //    if (TagFilter != null && TagFilter.Length > 0)
                //    {
                //        list = new List<RawlerLib.Web.Link>(list.Where(n => n.Tag.Contains(TagFilter)));
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
                //        if (emptyReport )
                //        {
                //            ReportManage.ErrReport(this, "対象が見つかりませんでした");
                //        }
                //    }

                //    RunChildren(runChildren);
                //}
            }
        }




        //bool isMulti = false;
        ///// <summary>
        ///// 複数か？
        ///// </summary>
        //public bool IsMulti
        //{
        //    get
        //    {
        //        return isMulti;
        //    }
        //    set
        //    {
        //        isMulti = value;
        //    }
        //}

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
        ///// <summary>
        ///// このオブジェクトのテキスト
        ///// </summary>
        //public override string Text
        //{
        //    get
        //    {

        //        if (text == string.Empty && doClone == false)
        //        {
        //            Run(false);
        //        }
        //        return text;
        //    }

        //}



        //private List<string> texts = new List<string>();
        ///// <summary>
        ///// このオブジェクトのテキスト（複数）
        ///// </summary>
        //public List<string> Texts
        //{
        //    get
        //    {
        //        if (texts.Count == 0 && doClone == false)
        //        {
        //            Run(false);
        //        }
        //        return texts;
        //    }

        //}


        bool doClone = false;
        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Link>(parent);
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
    }

    public class Links : Link
    {
        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Links>(parent);
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
    }



    /// <summary>
    ///　子に流すデータの種類
    /// </summary>
    public enum LinkVisbleType
    {
        /// <summary>
        /// URL
        /// </summary>
        Url,
        /// <summary>
        /// Label
        /// </summary>
        Label,
        /// <summary>
        /// Tag全体
        /// </summary>
        Tag
    }

}
