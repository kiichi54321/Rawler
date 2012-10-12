using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib.MarkupLanguage
{
    internal class TagClass
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string StartTag { get; set; }
        public string EndTag { get; set; }
        private string baseText;
        private string parameter = string.Empty;

        public string Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }



        public TagClass(string tag, int start, string text)
        {
            this.Start = start;
            //                this.StartTag = startTag;
            this.EndTag = "</" + tag + ">";
            this.baseText = text;
            if (baseText != null)
            {
                int s = baseText.IndexOf('>', start);
                this.StartTag = baseText.Substring(start, s+1 - start);
                this.parameter = StartTag.Replace("<" + tag, "").Replace(">", "");


            }
        }

        public bool CheckClassName(string name)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("class[ ]*=[\"|\'| ]*"+name+"[\"|\'| |$]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var r = regex.Match(parameter);
            if (r.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckIdName(string name)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("id[ ]*=[\"|\'| ]*" + name + "[\"|\'| |$]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var r = regex.Match(parameter);
            if (r.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// textそのままのタグ
        /// </summary>
        /// <param name="text"></param>
        public TagClass(string text)
        {
            this.baseText = text;
            StartTag = string.Empty;
            EndTag = string.Empty;
            Start = 0;
        }

        public TagClass Parent { get; set; }
        private List<TagClass> children = new List<TagClass>();

        public List<TagClass> Children
        {
            get { return children; }
            set { children = value; }
        }

        public string Inner
        {
            get
            {
                if (baseText != null)
                {
                    if (this.End > 0)
                    {
                        return baseText.Substring(Start + StartTag.Length, End - (Start + StartTag.Length));
                    }
                    else
                    {
                        return baseText.Substring(Start + StartTag.Length);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string Outer
        {
            get
            {
                if (baseText != null)
                {
                    if (this.End > 0)
                    {
                        return baseText.Substring(Start, End + EndTag.Length - (Start));
                    }
                    else
                    {
                        return baseText.Substring(Start + StartTag.Length);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        //public static TagClass AllText(string text)
        //{
        //    TagClass tc = new TagClass(text);
        //    tc.End = -1;
        //    return tc;
        //}
    }
}
