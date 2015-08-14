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
    public class Links : RawlerMultiBase
    {
        /// <summary>
        ///  Linkを取得するRawlerオブジェクト
        /// </summary>
        public Links()
            : base()
        {
        }

        /// <summary>
        /// URLの含まれている文字列
        /// </summary>
        public string UrlFilter { get; set; }
        /// <summary>
        /// Labelに含まれている文字列
        /// </summary>
        public string LabelFilter { get; set; }

        /// <summary>
        /// Tagに含まれている文字列
        /// </summary>
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

        /// <summary>
        /// aタグ以外にも反応させる場合
        /// </summary>
        public string TargetTag { get; set; }

        bool checkUrl = false;


        private void oldMethod(bool runChildren)
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
                    list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(), GetPageUrl(), TargetTag));
                }
                else
                {
                    list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(), null, TargetTag));
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
                list = new List<RawlerLib.Web.Link>(list.Where(n => n.TagWithoutUrl.Contains(TagFilter)));
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
           

        }
        private RawlerLib.Web.Link currentLink;

        private void NewMethod(bool runChildren)
        {
            List<RawlerLib.Web.Link> list;
            if (this.Parent is Links && ((Links)this.Parent).TargetTag == this.TargetTag)
            {
                var p = (Links)this.Parent;
                list = new List<RawlerLib.Web.Link>() { p.currentLink };
            }
            else
            {
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
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(), GetPageUrl(), TargetTag));
                    }
                    else
                    {
                        list = new List<RawlerLib.Web.Link>(RawlerLib.Web.GetLinkForHTML(GetText(), null, TargetTag));
                    }
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
                list = new List<RawlerLib.Web.Link>(list.Where(n => n.TagWithoutUrl.Contains(TagFilter)));
            }
            if (CheckUrl)
            {
                if (useAbsolutetLink)
                {
                    list = list.Where(n => Uri.IsWellFormedUriString(n.Url, UriKind.Absolute)).ToList();
                }
                else
                {
                    list = list.Where(n => Uri.IsWellFormedUriString(n.Url, UriKind.RelativeOrAbsolute)).ToList();
                }
            }

            if(list.Count == 0 && EmptyTree !=null)
            {
                EmptyTree.SetParent(this.Parent);
                EmptyTree.Run();
            }

            if (emptyReport && list.Count() == 0)
            {
                ReportManage.ErrReport(this, "対象が見つかりませんでした");
            }

            if (IsSingle == false)
            {
                RunChildrenForArray<RawlerLib.Web.Link>(runChildren, list, (n) => GetTextVisbleType(n) , (n) => { currentLink = n; });
            }
            else
            {
                if (this.texts.Count() > 0)
                {
                    SetText(GetTextVisbleType(list.First()));
                    RunChildren(runChildren);
                }
            }
        }

        private string GetTextVisbleType(RawlerLib.Web.Link link)
        {
            if (visbleType == LinkVisbleType.Label) return link.Label;
            if (visbleType == LinkVisbleType.Url) return link.Url;
            return link.Tag;            
        }

        /// <summary>
        /// 実行する
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.Parent != null && this.Parent.Text != null)
            {
             //   oldMethod(runChildren);
                NewMethod(runChildren);
 
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
 


        bool doClone = false;
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

        /// <summary>
        /// URLがURLとして適切かどうかをチェックします。
        /// </summary>
        public bool CheckUrl
        {
            get
            {
                return checkUrl;
            }

            set
            {
                checkUrl = value;
            }
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
