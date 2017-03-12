using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    public class Replace : RawlerBase
    {
        public Replace()
            : base()
        {
        }

        public Replace(string OldText, string NewText):base()
        {
            this.OldText = OldText;
            this.NewText = NewText;
        }

        public string OldText { get; set; }
        public string NewText { get; set; }

        public RawlerBase OldTextTree { get; set; }
        public RawlerBase NewTextTree { get; set; }

        public override string Text
        {
            get
            {
                if (this.OldText == null && this.OldTextTree == null )
                {
                    ReportManage.ErrReport(this, "入力文字列がNullです");
                    return string.Empty;
                }
                else
                {
                    if (this.Parent != null && this.Parent.Text != null)
                    {
                        if (ReplaceTexts == null || ReplaceTexts.Count == 0)
                        {
                            string oldText = OldText;
                            string newText = NewText;
                            if (newText == null) newText = string.Empty;
                            if (this.OldTextTree != null)
                            {
                                oldText = RawlerBase.GetText(this.Parent.Text, OldTextTree, this);
                            }
                            if (this.NewTextTree != null)
                            {
                                newText = RawlerBase.GetText(this.Parent.Text, NewTextTree, this);
                            }

                            return GetText().Replace(oldText, newText);
                        }
                        else
                        {
                            var t = GetText();
                            foreach (var item in ReplaceTexts)
                            {
                                t = item.Replace(t);
                            }
                            return t;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Replace>(parent);
        }

        public ReplaceTexts ReplaceTexts
        {
            get;
            set;
        }
    }

    public class ReplaceText
    {
        public string NewText { get; set; }
        public string OldText { get; set; }
        public string Replace(string text)
        {
            if (NewText == null) NewText = string.Empty;
            if (OldText != null)
            {
                return text.Replace(OldText, NewText);
            }
            else
            {
                return text;
            }
        }
    }

    public class ReplaceTexts : List<ReplaceText>
    {
    }
}
