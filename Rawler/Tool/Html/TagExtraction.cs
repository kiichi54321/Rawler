using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using RawlerLib.Extend;

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




        public string Tag { get; set; }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }


        //List<string> texts = new List<string>();
        //public List<string> Texts
        //{
        //    get { return texts; }
        
        //}

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
        bool useHtmlAgilityPack = false;

        //public bool UseHtmlAgilityPack
        //{
        //    get { return useHtmlAgilityPack; }
        //    set { useHtmlAgilityPack = value; }
        //}

        //private void UsingHtmlAgilityPack(bool runChildren)
        //{
        //                        IEnumerable<HtmlNode> list;
        //            HtmlDocument doc = new HtmlDocument();
        //            doc.LoadHtml(GetText());
        //            if (UseTagRank)
        //            {
        //                list = doc.DocumentNode.ChildNodes.Where(n => n.Name == Tag);
        //            }
        //            else
        //            {
        //                list = GetAllTag(doc.DocumentNode.ChildNodes, Tag);
        //            }

        //            if (this.ParameterFilter != null && this.ParameterFilter.Length > 0)
        //            {
        //                if (ParameterFilter.Contains(":"))
        //                {
        //                    list = list.Where(n => n.Attributes.Select(m => m.Name + ":" + m.Value).Contains(ParameterFilter));
        //                }
        //                else
        //                {
        //                    list = list.Where(n => n.Attributes.Select(m => m.Name + ":" + m.Value).JoinText(" ").Contains(ParameterFilter));
        //                }
        //            }
        //            if (this.ContextFilter != null && this.ContextFilter.Length > 0)
        //            {
        //                list = list.Where(n => n.InnerHtml.Contains(this.ContextFilter));
        //            }
        //            if (this.ClassName != null && this.ClassName.Length > 0)
        //            {
        //                list = list.Where(n => n.Attributes.Where(m => m.Name == "class" && m.Value == ClassName).Any());
        //            }
        //            if (string.IsNullOrEmpty(Itemprop) == false)
        //            {
        //                list = list.Where(n => n.Attributes.Where(m => m.Name == "itemprop" && m.Value == Itemprop).Any());
        //            }
        //            if (this.IdName != null && this.IdName.Length > 0)
        //            {
        //                list = list.Where(n => n.Attributes.Where(m => m.Name == "id" && m.Value == IdName).Any());
        //            }
        //            if (this.TargetName != null && this.TargetName.Length > 0)
        //            {
        //                list = list.Where(n => n.Attributes.Where(m => m.Name == "name" && m.Value == TargetName).Any());
        //            }

        //            if (true)
        //            {
        //                List<string> list2 = new List<string>();
        //                foreach (var tag in list)
        //                {
        //                    string txt = string.Empty;
        //                    switch (tagVisbleType)
        //                    {
        //                        case TagVisbleType.Inner:
        //                            txt = tag.InnerHtml;
        //                            break;
        //                        case TagVisbleType.Outer:
        //                            txt = tag.OuterHtml;
        //                            break;
        //                        case TagVisbleType.Parameter:
        //                            txt = tag.Attributes.Select(n => n.Name + ":" + n.Value).JoinText(" ");
        //                            break;
        //                        default:
        //                            txt = tag.InnerHtml;
        //                            break;
        //                    }
        //                    list2.Add(txt);
        //                }
        //                if (emptyReport && list2.Count == 0)
        //                {
        //                    ReportManage.ErrReport(this, "該当するものは一つも見つかりませんでした。");
        //                }
        //                texts = list2;

        //                if (IsSingle)
        //                {
        //                    if (list2.Count > 0)
        //                    {
        //                        SetText(list2.First());
        //                        RunChildren(runChildren);
        //                    }
        //                }
        //                else
        //                {
        //                    RunChildrenForArray(runChildren, list2);
        //                }
        //            }
        //}
        private void OldMethod(bool runChildren)
        {
            List<RawlerLib.MarkupLanguage.TagClass> list;
            if (UseTagRank)
            {
                list = new List<RawlerLib.MarkupLanguage.TagClass>(RawlerLib.MarkupLanguage.TagAnalyze.GetTopTag(GetText(), Tag));
            }
            else
            {
                list = new List<RawlerLib.MarkupLanguage.TagClass>(RawlerLib.MarkupLanguage.TagAnalyze.GetTag(GetText(), Tag));
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
            if (string.IsNullOrEmpty(Itemprop) == false)
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
                        SetText(list2.First());
                        RunChildren(runChildren);
                    }
                }
                else
                {
                    RunChildrenForArray(runChildren, list2);
                }
            }
        }

        private RawlerLib.MarkupLanguage.TagClass currentTagClass;

        private IEnumerable<RawlerLib.MarkupLanguage.TagClass> ExtendList(List<RawlerLib.MarkupLanguage.TagClass> list)
        {
            foreach (var item in list)
            {
                yield return item;
                foreach (var item2 in ExtendList(item.Children))
                {
                    yield return item2;
                }
            }
        }

        private void NewMethod(bool runChildren)
        {
            List<RawlerLib.MarkupLanguage.TagClass> list;

            var test = this.GetAncestorRawler().Skip(1).TakeWhile(n => (n is Page) == false);
            var tags = this.GetAncestorRawler().Skip(1).TakeWhile(n => (n is Page) == false).OfType<Tags>().Where(n => n.Tag == this.Tag).FirstOrDefault();
            if (tags != null)
            {
                list = ExtendList( tags.currentTagClass.Children).ToList();
            }
            else
            {

                if (UseTagRank)
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(RawlerLib.MarkupLanguage.TagAnalyze.GetTopTag(GetText(), Tag));
                }
                else
                {
                    list = new List<RawlerLib.MarkupLanguage.TagClass>(RawlerLib.MarkupLanguage.TagAnalyze.GetTag(GetText(), Tag));
                }
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
            if (string.IsNullOrEmpty(Itemprop) == false)
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


            if (emptyReport && list.Count == 0)
            {
                ReportManage.ErrReport(this, "該当するものは一つも見つかりませんでした。");
            }
            texts = list.Select(n=>GetTagVisbleTypeText(n));


            if (IsSingle)
            {
                if (list.Count > 0)
                {
                    SetText(GetTagVisbleTypeText(list.First()));
                    currentTagClass = list.First();
                    RunChildren(runChildren);
                }
            }
            else
            {
                RunChildrenForArray<RawlerLib.MarkupLanguage.TagClass>(runChildren, list,(n)=> GetTagVisbleTypeText(n),(n)=>this.currentTagClass = n );
            }

        }

        private string GetTagVisbleTypeText(RawlerLib.MarkupLanguage.TagClass tag )
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
            return txt;
        }


        public static bool NewMethodType = false;

        public override void Run(bool runChildren)
        {
            if (this.Parent != null)
            {
                if (NewMethodType == false)
                {
                    OldMethod(runChildren);
                }
                else
                {
                    NewMethod(runChildren);
                }

            }



        }


        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Tags>(parent);
        }

        ///// <summary>
        ///// RawlerのTagsと同機能を提供する拡張メソッド
        ///// </summary>
        ///// <param name="html">target HTML</param>
        ///// <param name="tagname">target Tag </param>
        ///// <param name="dic">class , id , Parameter,Context に対応</param>
        ///// <param name="type">Inner,Outer, Parameter </param>
        ///// <returns>tags result</returns>
        //public static IEnumerable<string> Tags(this string html, string tagname,IDictionary<string,string> dic, TagVisbleType type)
        //{
        //    Tags tags = new Tags() { Tag = tagname, tagVisbleType = type };
        //    foreach (var pair in dic)
        //    {
        //        if (pair.Key == "class") { tags.ClassName = pair.Value.ToString(); }
        //        else if (pair.Key == "id") { tags.IdName = pair.Value.ToString(); }
        //        else if (pair.Key == "Parameter") { tags.ParameterFilter = pair.Value.ToString(); }
        //        else if (pair.Key == "Context") { tags.ContextFilter = pair.Value.ToString(); }                               
        //    }          

        //    RawlerBase.GetText(html, tags);

        //    return tags.Texts;
        //}
        ///// <summary>
        ///// RawlerのTagsと同機能を提供する拡張メソッド
        ///// </summary>
        ///// <param name="html">target HTML</param>
        ///// <param name="tagname">target Tag </param>
        ///// <param name="dic">class , id , Parameter,Context に対応</param>
        ///// <returns>tags result</returns>
        //public static IEnumerable<string> Tags(this string html, string tagname, IDictionary<string, string> dic)
        //{
        //    return Tags(html, tagname, dic, TagVisbleType.Inner);
        //}
        ///// <summary>
        ///// RawlerのTagsと同機能を提供する拡張メソッド
        ///// </summary>
        ///// <param name="html">target HTML</param>
        ///// <param name="tagname">target Tag </param>
        ///// <param name="className">target class</param>
        ///// <param name="dic">class , id , Parameter,Context に対応</param>
        ///// <returns>tags result</returns>       
        //public static IEnumerable<string> Tags(this string html, string tagname, string className, TagVisbleType type)
        //{        
        //    return Tags(html, tagname,new Dictionary<string,string>(){ {"class" , className}  } , type);
        //}
        ///// <summary>
        ///// RawlerのTagsと同機能を提供する拡張メソッド
        ///// </summary>
        ///// <param name="html">target HTML</param>
        ///// <param name="tagname">target Tag </param>
        ///// <param name="className">target class</param>
        ///// <param name="dic">class , id , Parameter,Context に対応</param>
        ///// <returns>tags result</returns> 
        //public static IEnumerable<string> Tags(this string html, string tagname, string className)
        //{
        //    return Tags(html, tagname, new Dictionary<string, string>() { { "class", className } }, TagVisbleType.Inner);
        //}


        //public IEnumerable<HtmlNode> GetAllTag(HtmlNodeCollection list, string tag)
        //{
        //    foreach (var item in list)
        //    {
        //        if(item.Name == tag)
        //        {
        //            yield return item;
        //        }
        //        foreach (var item2 in  GetAllTag( item.ChildNodes,tag))
        //        {
        //            yield return item2;
        //        }
        //    }
        //}

    }
    /// <summary>
    /// Tagを取得する部分。
    /// </summary>
    public enum TagVisbleType
    {
        Inner,Outer, Parameter
    }

}
