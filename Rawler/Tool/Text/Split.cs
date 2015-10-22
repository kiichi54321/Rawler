using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class Split:RawlerMultiBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        string separator = null;

        public string Separator
        {
            get { return separator; }
            set { separator = value;
            separatorType = Tool.SeparatorType.Other;
            }
        }

        bool emptyTextSkip = false;
        StringSplitOptions stringSplitOptions = StringSplitOptions.None;
        public bool EmptyTextSkip
        {
            get { return emptyTextSkip; }
            set
            {
                emptyTextSkip = value;
                if (emptyTextSkip) { stringSplitOptions = StringSplitOptions.RemoveEmptyEntries; }
                else { stringSplitOptions = StringSplitOptions.None; }
            }
        }

        //int num = -1;

        //public int Num
        //{
        //    get { return num; }
        //    set { num = value; }
        //}

        public override void Run(bool runChildren)
        {
            string[] data;

            if(SeparatorType == SeparatorType.Other)
            {
                if (separator.Length > 0)
                {
                    data = GetText().Split(new string[] { separator },stringSplitOptions);
                }
                else
                {
                    ReportManage.ErrReport(this, "Split でsepartorの指定がありません");
                    return;
                }               
            }
            else
            {
                data = GetText().Split(separatorType.GetSeparators(new string[] { separator }), stringSplitOptions);
            }
            if (ElementAt.HasValue)
            {
                try
                {
                    string tmp = null;
                    if (ElementAt.Value > -1)
                    {
                        tmp = data[ElementAt.Value];
                    }
                    else
                    {
                        tmp = data[data.Length + ElementAt.Value];
                    }
                    this.SetText(tmp);
                    base.Run(runChildren);
                }
                catch
                {
                    ReportManage.ErrReport(this, "ElementAtの値がレンジから外れました。ElementAt:" + ElementAt.Value);
                }
            }           
            else
            {
                base.RunChildrenForArray(runChildren, data);
            }
        }


        private SeparatorType separatorType = SeparatorType.Tab;

        public SeparatorType SeparatorType
        {
            get { return separatorType; }
            set { separatorType = value; }
        }




        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Split>(parent);
        }
    }

    public enum SeparatorType
    {
        Tab, Comma,Space,Br_Tag,Other
    }

    public static partial class Extend
    {
        public static string GetSeparator(this SeparatorType s,string other)
        {
            if (s == SeparatorType.Comma) return ",";
            if (s == SeparatorType.Tab) return "\t";
            if (s == SeparatorType.Space) return " ";
            if (s == SeparatorType.Br_Tag) return "<br>";
            return other;
        }

        public static string[] GetSeparators(this SeparatorType s, string[] other)
        {
            if (s == SeparatorType.Comma) return new string[] { "," };
            if (s == SeparatorType.Tab) return new string[] { "\t" };
            if (s == SeparatorType.Space) return new string[] { " " };
            if (s == SeparatorType.Br_Tag) return new string[] { "<br>", "<br />", "<BR>", "<BR />" };
            return other;
        }

    }
}
