using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using System.Text.RegularExpressions;

namespace RawlerLib.Extend
{

    public static class WebTool
    {
        public static string DownloadHtml(System.Net.WebClient wc, string url)
        {
            var data = wc.DownloadData(url);
            var text_utf8 = System.Text.Encoding.UTF8.GetString(data);

            var p1 = "<meta http-equiv=\"content-type\" content=\"text/html; charset=(.*)\">";
            var p2 = "<meta charset=\"(.*)\">";
            var encoding = System.Text.Encoding.UTF8;
            try
            {
                var head = text_utf8.Substring(0, 600);
                var m1 = Regex.Match(head, p1, RegexOptions.IgnoreCase);
                if (m1.Success)
                {
                    encoding = System.Text.Encoding.GetEncoding(m1.Groups[1].Value);
                    return encoding.GetString(data);
                }
                else
                {
                    var m2 = Regex.Match(head, p2, RegexOptions.IgnoreCase);
                    if (m2.Success)
                    {
                        encoding = System.Text.Encoding.GetEncoding(m2.Groups[1].Value);
                        return encoding.GetString(data);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return text_utf8;
        }
    }

    public static class Text
    {
        public static HtmlText ToHtml(this string html, string url)
        {
            return new HtmlText(html, url);
        }

        public static HtmlText ToHtml(this string html)
        {
            return new HtmlText(html, string.Empty);
        }

        /// <summary>
        /// タグの親にあるパラメータを継承する。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<MarkupLanguage.TagClass> ParameterInherit(this IEnumerable<MarkupLanguage.TagClass> list)
        {
            foreach (var item in list)
            {
                if (item.Parameter.Length > 0)
                {
                    MarkupLanguage.TagClass tag = item;
                    while (true)
                    {
                        if(tag.Parameter.Length > 0)
                        {
                            item.Parameter = tag.Parameter;
                            break;
                        }
                        if(tag.Parent !=null)
                        {
                            tag = tag.Parent;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return list;
        }
             
    }

    public class HtmlText
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public HtmlText(string text,string url)
        {
            Text = text;
            Url = url;
        }

        public IEnumerable<MarkupLanguage.TagClass> GetTag(string tag)
        {
            return MarkupLanguage.TagAnalyze.GetTag(Text, tag);
        }

        public IEnumerable<MarkupLanguage.TagClass> GetTopTag( string tag)
        {
            return MarkupLanguage.TagAnalyze.GetTopTag(Text, tag);
        }

        public IEnumerable<Web.Link> GetLink()
        {
            return Web.GetLinkForHTML(Text, Url);
        }

        public IEnumerable<Web.Link> GetImageLink()
        {
            return Web.GetImageLink(Text,Url);
        }

        public string TagClear()
        {
            return Web.HtmlTagAllDelete(Text);
        }
    }

    public static class XAML
    {
        /// <summary>
        /// XAMLのNULL値、空の文字列の値を消す。
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static string ConvertNullXAML(this string xaml)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[A-Za-z0-9_]*=\"{x:Null}\" ");
            System.Text.RegularExpressions.Regex r2 = new System.Text.RegularExpressions.Regex("[A-Za-z0-9_]*=\"\" ");
            System.Text.RegularExpressions.Regex r3 = new System.Text.RegularExpressions.Regex("[A-Za-z0-9_]*=\"True\" ");
            //var m = r.Match(xaml);
            StringBuilder sb = new StringBuilder(xaml);
            foreach (System.Text.RegularExpressions.Match item in r.Matches(xaml))
            {
                sb = sb.Replace(item.Value, "");
            }
            foreach (System.Text.RegularExpressions.Match item in r2.Matches(xaml))
            {
                sb = sb.Replace(item.Value, "");
            }
            foreach (System.Text.RegularExpressions.Match item in r3.Matches(xaml))
            {
                sb = sb.Replace(item.Value, "");
            }
            return sb.ToString();
        }

    }

    public static class RawlerExtend
    {
        public static Document ToRawlerDocument(this string text)
        {
            return new Document() { TextValue = text};
        }
        public static RawlerBase DataWrite(this RawlerBase rawler,string attribute)
        {
            return rawler.Add(new DataWrite() { Attribute = attribute });
        }
        public static RawlerBase DataWrite(this RawlerBase rawler, string attribute,DataAttributeType attributeType)
        {
            return rawler.Add(new DataWrite() { Attribute = attribute,AttributeType = attributeType });
        }


        public static RawlerBase TagClear(this RawlerBase rawler,ReplaceType replece)
        {
            return rawler.Add(new TagClear() { ReplaceType = replece });
        }
        public static RawlerBase Trim(this RawlerBase rawler)
        {
            return rawler.Add(new Trim());
        }
        

        public static RawlerBase ImageLinks(this RawlerBase rawler)
        {
            return rawler.Add(new ImageLinks());
        }
        public static RawlerBase Tags(this RawlerBase rawler, string tag)
        {
            return rawler.Add(new Tags() { Tag = tag });
        }
        public static RawlerBase Tags(this RawlerBase rawler, string tag, string className)
        {
            return rawler.Add(new Tags() { Tag = tag, ClassName = className });
        }

        public static RawlerBase Page(this RawlerBase rawler,string url)
        {
            return rawler.Add(new Page() { Url = url });
        }
        public static RawlerBase Page(this RawlerBase rawler)
        {
            return rawler.Add(new Page());
        }
        public static RawlerBase GetPageUrl(this RawlerBase rawler)
        {
            return rawler.Add(new GetPageUrl());
        }

        public static RawlerBase GetTsvValue(this RawlerBase rawler,string column)
        {
            return rawler.Add(new GetTsvValue() { ColumnName = column });
        }
        public static RawlerBase NextPage(this RawlerBase rawler)
        {
            return rawler.Add(new NextPage());
        }

    }
}
