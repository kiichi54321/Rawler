using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    /// <summary>
    /// tagで分割する
    /// </summary>
    public class TagSplit : RawlerMultiBase
    {
        public string Tag { get; set; }
        public string ClassName { get; set; }
        public override void Run(bool runChildren)
        {
            string txt = GetText();
            var list = RawlerLib.MarkupLanguage.TagAnalyze.GetTag(txt, Tag);
            if (string.IsNullOrEmpty(ClassName) == false) list = list.Where(n => n.CheckClassName(ClassName));
            List<string> textList = new List<string>();
            var list2 = list.ToArray();            
            for (int i = 0; i < list2.Length; i++)
            {
                string t = string.Empty;   
                if(i<list2.Length-1)
                {
                    int s = list2[i].Start;
                    int e = list2[i+1].Start-1;
                    t = txt.Substring(s, e - s);
                }
                else
                {
                    int s = list2[i].Start;
                    t = txt.Substring(s);
                    if(list2[i].Parent !=null)
                    {
                        t = txt.Substring(s, list2[i].Parent.End - s - list2[i].Parent.EndTag.Length);
                    }
                }
                textList.Add(t);
            }
            RunChildrenForArray(runChildren, textList);
        }
    }
}
