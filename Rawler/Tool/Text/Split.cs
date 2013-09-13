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

        public bool EmptyTextSkip
        {
            get { return emptyTextSkip; }
            set { emptyTextSkip = value; }
        }

        int num = -1;

        public int Num
        {
            get { return num; }
            set { num = value; }
        }

        public override void Run(bool runChildren)
        {
            string[] data;
            if (separatorType == SeparatorType.Tab)
            {
                data = GetText().Split('\t');
            }
            else if (separatorType == SeparatorType.Comma)
            {
                data = GetText().Split(',');
            }
            else
            {
                if (separator.Length > 0)
                {
                    data = GetText().Split(separator.ToCharArray());
                }
                else
                {
                    ReportManage.ErrReport(this, "Split でsepartorの指定がありません");
                    return;
                }
            }
            if (emptyTextSkip)
            {
                data = data.Where(n => n.Length > 0).ToArray();
            }
            if (num > -1)
            {
                if (data.Length >= num)
                {
                    this.SetText(data[num]);
                    base.Run(runChildren);
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
        Tab, Comma,Other
    }
}
