using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;
using System.Xml;
using System.Xml.Linq;

namespace Rawler
{
    public class XPathSelectNodes : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<XPathSelectNodes>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var doc = new XDocument();
            
            doc.Document.LoadXml(this.GetText());
            var nodelist = doc.SelectNodes(xPath);
            List<string> list = new List<string>();
            foreach (XmlNode item in nodelist)
            {
                if (tagVisbleType == Tool.TagVisbleType.Inner)
                {
                    list.Add(item.InnerXml);
                }
                else if (tagVisbleType == Tool.TagVisbleType.Outer)
                {
                    list.Add(item.OuterXml);
                }
                else
                {
                    list.Add(item.OuterXml.Replace(item.InnerXml,""));
                }
            }
            base.RunChildrenForArray(runChildren, list);
        }

        public string xPath { get; set; }

        private TagVisbleType tagVisbleType = TagVisbleType.Inner;

        public TagVisbleType TagVisbleType
        {
            get { return tagVisbleType; }
            set { tagVisbleType = value; }
        }
        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }
}
