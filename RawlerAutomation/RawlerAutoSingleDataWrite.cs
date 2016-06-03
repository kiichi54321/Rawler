using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;
using HtmlAgilityPack;
using RawlerLib.MyExtend;
using RawlerExpressLib.Automation.Extend;
using RawlerLib.Extend;

namespace RawlerExpressLib.Automation
{
    public class RawlerAutoSingleDataWrite : Rawler.Tool.RawlerBase
    {
        public string CategoryName { get; set; }
        private bool anlyzed = false;

        public override void Run(bool runChildren)
        {
            if (anlyzed == false)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(GetText());
                var page = this.GetUpperRawler<Page>();
                if (page != null) baseUrl = page.GetCurrentUrl();

                foreach (var item in doc.DocumentNode.ChildNodes)
                {
                    this.Add(CreateRawler(item));
                }
                anlyzed = true;
                this.MargeChildren();
            }
            base.Run(runChildren);
        }

        public override string Text
        {
            get
            {
                return GetText();
            }
        }

        string baseUrl = string.Empty;

        private string GetCategoryName()
        {
            if (CategoryName.IsNullOrEmpty()) return string.Empty;
            return CategoryName + ".";
        }

        private RawlerBase CreateRawler(HtmlNode node)
        {
            RawlerBase rawler = null;

            bool flag次のノードを調べる = true;
            if (RawlerExpressLib.Automation.Extend.HTMLExtend.TargetAnalyzeTag.Contains(node.Name))
            {
                Tags tags = new Tags() { Tag = node.Name };
                if (node.Attributes.Where(n => n.Name == "class").Any())
                {
                    tags.ClassName = node.Attributes.Where(n => n.Name == "class").First().Value;
                }
                if (node.Attributes.Where(n => n.Name == "id").Any())
                {
                    tags.IdName = node.Attributes.Where(n => n.Name == "id").First().Value;
                }
                if (node.ChildNodes.Count() == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                {
                    tags.AddChildren(new DataWrite() { Attribute = GetCategoryName() + tags.ClassName });
                    flag次のノードを調べる = false;
                }
                if (node.Attributes.Where(n => n.Name == "style" && n.Value.Contains("background")).Any())
                {
                    tags.TagVisbleType = TagVisbleType.Outer;
                    rawler = tags.Add(new ImageLinks() { ImageType = ImageType.BackgroundImage }).DataWrite(CategoryName + "." + node.GetClassName() + "_Image", DataAttributeType.Image).GetRoot();
                }

                rawler = tags;
            }
            else if (node.Name == "a")
            {
                var url = node.OuterHtml.ToHtml(baseUrl).GetLink().FirstDefault<RawlerLib.Web.Link, string>(n => n.Url, null);
                if (url != null)
                {
                    {
                        rawler = new Links() { VisbleType = LinkVisbleType.Tag }.AddRange(
                            new Links() { VisbleType = LinkVisbleType.Url }.DataWrite(GetCategoryName() + node.GetClassName() + "_Link").GetRoot(),
                            new Links() { VisbleType = LinkVisbleType.Label }.DataWrite(GetCategoryName() + node.GetClassName() + "_Label").GetRoot()
                        );

                    }
                }
                else
                {
                    //URLがないAタグの場合。
                    Tags tags = new Tags() { Tag = node.Name };
                    if (node.Attributes.Where(n => n.Name == "class").Any())
                    {
                        tags.ClassName = node.Attributes.Where(n => n.Name == "class").First().Value;
                    }
                    if (node.Attributes.Where(n => n.Name == "id").Any())
                    {
                        tags.IdName = node.Attributes.Where(n => n.Name == "id").First().Value;
                    }
                    rawler = tags;
                }
                if (node.ChildNodes.Count == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                {
                    flag次のノードを調べる = false;
                }

            }
            else if (node.Name == "img")
            {

                var url = node.OuterHtml.ToHtml(baseUrl).GetImageLink().FirstDefault(n => n.Url, null);
                if (url != null)
                {
                    rawler = new ImageLinks().DataWrite(GetCategoryName() + node.GetClassName() + "_Image", DataAttributeType.Image).GetRoot();
                }

            }
            ///背景画像に反応させる。
            else if (node.Attributes.Where(n => n.Name == "style" && n.Value.Contains("background")).Any())
            {
                rawler = new ImageLinks() { ImageType = ImageType.BackgroundImage }.DataWrite(GetCategoryName() + node.GetClassName() + "_Image", DataAttributeType.Image).GetRoot();
            }
            else if (node.Name == "span")
            {
                Tags tags = new Tags() { Tag = node.Name };
                if (node.Attributes.Where(n => n.Name == "class").Any())
                {
                    tags.ClassName = node.Attributes.Where(n => n.Name == "class").First().Value;
                }
                if (node.ChildNodes.Count() == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                {
                    tags.AddChildren(new DataWrite() { Attribute = GetCategoryName() + tags.ClassName });
                    flag次のノードを調べる = false;
                }

                rawler = tags;
            }
            else
            {
                var t = node.OuterHtml.Replace("\n", "").Trim();
                if (t.Length > 0)
                {
                    rawler = new TagClear().Trim().Add(new DataWrite() { Attribute = GetCategoryName() + node.GetClassName() + "_" + node.Name }).GetRoot();
                    if (node.ChildNodes.Count == 1 && node.ChildNodes.Where(n => n.Name == "#text").Any())
                    {
                        flag次のノードを調べる = false;
                    }
                }
            }
            if (rawler != null && node.ChildNodes.Count == 1 && node.ChildNodes.Where(n => n.Name == "span").Any())
            {
                rawler.AddChildren(new DataWrite() { Attribute = GetCategoryName() + node.GetClassName() });
            }

            foreach (var item in node.ChildNodes)
            {
                if (flag次のノードを調べる)
                {
                    var r = CreateRawler(item);

                    if (r != null && rawler != null)
                    {
                        rawler.AddChildren(r);
                    }
                    else
                    {
                        if (r != null && rawler == null)
                        {
                            rawler = r;
                        }
                    }
                }

            }
            return rawler;

        }
    }

}
