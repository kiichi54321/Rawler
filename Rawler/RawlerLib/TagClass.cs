using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib.MarkupLanguage
{
    public class TagClass
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string StartTag { get; set; }
        public string EndTag { get; set; }
        private string baseText;
        private string parameter = string.Empty;
        private bool IsSingleTag = false;
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
                int single = baseText.IndexOf("/>", start);
                this.StartTag = baseText.Substring(start, s + 1 - start);
                if (single > 0 && s > single)
                {
                    IsSingleTag = true;
                    this.parameter = StartTag.Replace("<" + tag, "").Replace("/>", "");
                }
                else
                {
                     this.parameter = StartTag.Replace("<" + tag, "").Replace(">", "");
                }

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

        public bool CheckItempropName(string name)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("itemprop[ ]*=[\"|\'| ]*" + name + "[\"|\'| |$]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
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

        public bool CheckName(string name)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("name[ ]*=[\"|\'| ]*" + name + "[\"|\'| |$]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
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
                    if (IsSingleTag == false)
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
                        return string.Empty;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string InnerWithoutChildren
        {
            get
            {
                var inner = this.Inner;
                if (inner == null) return inner;
                StringBuilder sb = new StringBuilder(inner);
                foreach (var item in Children)
                {
                    sb = sb.Replace(item.Outer, string.Empty);
                }
                return sb.ToString();
            }
        }


        public string Outer
        {
            get
            {
                if (baseText != null)
                {
                    if (IsSingleTag)
                    {
                        return baseText.Substring(Start + StartTag.Length);
                    }
                    else
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
