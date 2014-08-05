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
}
