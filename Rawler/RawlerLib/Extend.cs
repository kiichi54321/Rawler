using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerLib.Extend
{
 

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
            return Web.GetImageLink(Text);
        }

        public string TagClear()
        {
            return Web.HtmlTagAllDelete(Text);
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

        public static RawlerBase GetTesvValue(this RawlerBase rawler,string column)
        {
            return rawler.Add(new GetTsvValue() { ColumnName = column });
        }
        public static RawlerBase NextPage(this RawlerBase rawler)
        {
            return rawler.Add(new NextPage());
        }

    }
}
