using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawlerLib.MyExtend;

namespace Rawler.Tool
{
    /// <summary>
    /// 定義リストを取得する。
    /// </summary>
    public class DefinitionLists : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<DefinitionLists>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = RawlerLib.MarkupLanguage.TagAnalyze.GetTag(GetText(), "dl").ToList();
            if (ClassName.IsNullOrEmpty() ==false) list = list.Where(n => n.Parameter.Contains("class=\"" + ClassName + "\"")).ToList();
            if (IdName.IsNullOrEmpty()==false) list = list.Where(n => n.Parameter.Contains("id=\"" + IdName + "\"")).ToList();

            List<string> txtList = new List<string>();
            foreach (var item in list)
            {
                txtList.AddRange(RawlerLib.Web.GetTagContentList(item.Inner, "<dt", "</dd>", true));
            }
            base.RunChildrenForArray(true, txtList);
        }

        public string ClassName { get; set; }
        public string IdName { get; set; }



        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }

    /// <summary>
    /// DefinitionListを読み込んで一時的に貯める
    /// </summary>
    public class DefinitionListReader:RawlerBase
    {
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = RawlerLib.MarkupLanguage.TagAnalyze.GetTag(GetText(), "dl").ToList();
            if (ClassName.IsNullOrEmpty() == false) list = list.Where(n => n.Parameter.Contains("class=\"" + ClassName + "\"")).ToList();
            if (IdName.IsNullOrEmpty() == false) list = list.Where(n => n.Parameter.Contains("id=\"" + IdName + "\"")).ToList();

            definitionList = new List<KeyValuePair<string, string>>();
            foreach (var item in list)
            {
                foreach(var item2 in RawlerLib.Web.GetTagContentList(item.Inner, "<dt", "</dd>", true))
                {
                    var key = RawlerLib.MarkupLanguage.TagAnalyze.GetTag(item2, "dt", true);
                    var val = RawlerLib.MarkupLanguage.TagAnalyze.GetTag(item2, "dd", true);
                    definitionList.Add(new KeyValuePair<string, string>(key.First().Inner, val.First().Inner));
                }
            }
            RunChildren(runChildren);
        }

        public string ClassName { get; set; }
        public string IdName { get; set; }
        List<KeyValuePair<string, string>> definitionList = new List<KeyValuePair<string, string>>();

        public IEnumerable<KeyValuePair<string,string>> GetDefinition(string key)
        {
            return definitionList.Where(n => n.Key == key);
        }

    }

    public class GetDefinitionList:RawlerMultiBase
    {
        public string Dt { get; set; }

        public override void Run(bool runChildren)
        {
            var reader = this.GetUpperRawler<DefinitionListReader>();
            if(reader !=null)
            {
               var list =  reader.GetDefinition(Dt).Select(n=>n.Value);
                RunChildrenForArray(runChildren, list);
            }
            else
            {
                ReportManage.ErrReport(this, "上流に「DefinitionListReader」が必要です");
            }
        }
    }

}
