using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 複数の変数に対し、同時に行う。
    /// </summary>
    class Parallel:RawlerBase
    {
        List<Document> documents = new List<Document>();

        public List<Document> Documents
        {
            get { return documents; }
            set { documents = value; }
        }
        public void addDocument(string text, RawlerBase rawlerTree)
        {
            var d = new Document();
            d.SetText(text);
            d.SetParent(this);
            var tree = rawlerTree.Clone();
            d.AddChildren(tree);
            documents.Add(d);
        }

        public void addDocument(string text, ICollection<RawlerBase> rawlerTrees)
        {
            var d = new Document();
            d.SetText(text);
            d.SetParent(this);
            foreach (var item in rawlerTrees)
            {
                var tree = item.Clone();
                d.AddChildren(tree);                
            }
            
            documents.Add(d);
        }


        public void DocumentsClear()
        {
            documents.Clear();
        }


        public override void Run(bool runChildren)
        {
            foreach (var item in documents)
            {
                item.Run();
            }

            base.Run(runChildren);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            var clone = new Parallel();
            RawlerLib.ObjectLib.FildCopy(this, clone);
            clone.SetParent(parent);
            this.CloneEvent(clone);
            clone.children.Clear();

            clone.DocumentsClear();
            foreach (var item in documents)
            {
                var child = (Document)item.Clone(clone);
                clone.documents.Add(child);
            }
            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
        }
    }
}
