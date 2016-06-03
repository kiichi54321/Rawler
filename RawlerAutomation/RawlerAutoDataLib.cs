using HtmlAgilityPack;
using Rawler.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawlerLib.Extend;

namespace RawlerExpressLib.Automation
{
    

    class RawlerAutoDataLib
    {
        /// <summary>
        /// Htmlを入力にして適切なDataWriteを行う。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="attribute"></param>
        /// <param name="html"></param>
        public static void AutoHtmlDataWrite(Data data, string attribute, string html, string url)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            foreach (var item in doc.DocumentNode.ChildNodes)
            {
                AutoHtmlDataWrite(data, attribute, item, url);
            }
        }

        /// <summary>
        /// Htmlを入力にして適切なDataWriteを行う。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="attribute"></param>
        /// <param name="html"></param>
        public static void AutoHtmlDataWrite(Data data, string attribute, HtmlAgilityPack.HtmlNode node, string url)
        {
            if (node.Name == "#text")
            {
                if (node.InnerText.Trim().Length > 0)
                {
                    data.DataWrite(attribute, node.InnerText.Trim(), DataWriteType.add, DataAttributeType.Text);
                }
            }
            else if (node.Name == "img")
            {
                data.DataWrite(attribute + "_img", node.OuterHtml.ToHtml(url).GetImageLink().First().Url, DataWriteType.add, DataAttributeType.Image);
            }
            else if (node.Name == "a")
            {
                data.DataWrite(attribute + "_link", node.OuterHtml.ToHtml(url).GetLink().First().Url, DataWriteType.add, DataAttributeType.Url);
                foreach (var item in node.ChildNodes)
                {
                    AutoHtmlDataWrite(data, attribute, item, url);
                }
            }
            else
            {
                if (node.ChildNodes.Count == 1 && node.ChildNodes.First().Name == "#text")
                {
                    if (node.ChildNodes.First().InnerText.Trim().Length > 0)
                    {
                        data.DataWrite(attribute + "_" + node.Name, node.ChildNodes.First().InnerText.Trim(), DataWriteType.add, DataAttributeType.Text);
                    }
                }
                else
                {
                    foreach (var item in node.ChildNodes)
                    {
                        AutoHtmlDataWrite(data, attribute, item, url);
                    }
                }
            }
        }
    }
}
