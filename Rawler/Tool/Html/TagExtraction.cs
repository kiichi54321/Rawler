using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// tag抽出
    /// </summary>
    [Serializable]
    [ContentProperty("Children")]
    public class Tags : RawlerMultiBase
    {
        public Tags()
            : base()
        {
        }

        //public TagExtraction(string Tag, bool isMulti, string ParameterFilter, string ContextFilter):base()
        //{
        //    this.Tag = Tag;
        //    this.isMulti = isMulti;
        //    this.ParameterFilter = ParameterFilter;
        //    this.ContextFilter = ContextFilter;
        //}

        public string Tag { get; set; }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        //bool isMulti = false;
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

        List<string> texts = new List<string>();
        public List<string> Texts
        {
            get { return texts; }
        
        }

        private bool isSingle = false;

        public bool IsSingle
        {
            get { return isSingle; }
            set { isSingle = value; }
        }

        private TagVisbleType tagVisbleType = TagVisbleType.Inner;

        public TagVisbleType TagVisbleType
        {
            get { return tagVisbleType; }
            set { tagVisbleType = value; }
        }

        public string ParameterFilter { get; set; }
        public string ContextFilter { get; set; }

        public string ClassName { get; set; }
        public string IdName { get; set; }
        public string TargetName { get; set; }

        private bool emptyReport = false;
        /// <summary>
        /// 空文字列の時、報告するか？
        /// </summary>
        public bool EmptyReport
        {
            get { return emptyReport; }
            set { emptyReport = value; }
        }
        private bool useRank = false;

        /// <summary>
        /// タグ階層を使う。子要素になっているタグは探索範囲から外れます。
        /// </summary>
        public bool UseTagRank
        {
            get { return useRank; }
            set { useRank = value; }
        }
        public string Itemprop { get; set; }

        public override void Run(bool runChildren)
        {
            if (this.Parent != null)
            {
                List<RawlerLib.MarkupLanguage.TagClass> list;
                if (UseTagRank)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(RawlerLib.MarkupLanguage.TagAnalyze.GetTag(GetText(), Tag));
                }
                else
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(RawlerLib.MarkupLanguage.TagAnalyze.GetAllTag(GetText(), Tag));
                }

                if (this.ParameterFilter != null && this.ParameterFilter.Length > 0)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(list.Where(n => n.Parameter.Contains(this.ParameterFilter)));
                }
                if (this.ContextFilter != null && this.ContextFilter.Length > 0)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(list.Where(n => n.Inner.Contains(this.ContextFilter)));
                }
                if (this.ClassName != null && this.ClassName.Length > 0)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(list.Where(n => n.CheckClassName(ClassName)));                    
                }
                if(string.IsNullOrEmpty(Itemprop) == false)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(list.Where(n => n.CheckItempropName(Itemprop)));                    
                }
                if (this.IdName != null && this.IdName.Length > 0)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(list.Where(n => n.CheckIdName(IdName)));                    
                }
                if (this.TargetName != null && this.TargetName.Length > 0)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(list.Where(n => n.CheckName(TargetName)));
                }

                if (true)
                {
                    List<string> list2 = new List<string>();
                    foreach (var tag in list)
                    {
                        string txt = string.Empty;
                        switch (tagVisbleType)
                        {
                            case TagVisbleType.Inner:
                                txt = tag.Inner;
                                break;
                            case TagVisbleType.Outer:
                                txt = tag.Outer;
                                break;
                            case TagVisbleType.Parameter:
                                txt = tag.Parameter;
                                break;
                            default:
                                txt = tag.Inner;
                                break;
                        }
                        list2.Add(txt);
                    }
                    if (emptyReport && list2.Count == 0)
                    {
                        ReportManage.ErrReport(this, "該当するものは一つも見つかりませんでした。");
                    }
                    texts = list2;

                    if (IsSingle)
                    {
                        if (list2.Count > 0)
                        {
                            SetText( list2.First());
                            RunChildren(runChildren);
                        } 
                    }
                    else
                    {
                        RunChildrenForArray(runChildren, list2);
                    }
                }

                //else
                //{
                //    if (list.Count > 0)
                //    {
                //        var tag = list.First();
                //        switch (tagVisbleType)
                //        {
                //            case TagVisbleType.Inner:
                //                this.text = tag.Inner;
                //                break;
                //            case TagVisbleType.Outer:
                //                this.text = tag.Outer;
                //                break;
                //            case TagVisbleType.Parameter:
                //                this.text = tag.Parameter;
                //                break;
                //            default:
                //                this.text = tag.Inner;
                //                break;
                //        }

                //        this.RunChildren(runChildren);
                //    }
                //    else
                //    {
                //        if (emptyReport)
                //        {
                //            ReportManage.ErrReport(this, "該当するものは一つも見つかりませんでした。");
                //        }
                //    }
                //}
            }
        }


        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Tags>(parent);
        }
    }
    /// <summary>
    /// Tagを取得する部分。
    /// </summary>
    public enum TagVisbleType
    {
        Inner,Outer, Parameter
    }


}
