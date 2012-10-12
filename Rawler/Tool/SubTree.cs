using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class SubTree:RawlerBase
    {



        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// このオブジェクトのテキスト。親のテキストをそのまま流す。
        /// </summary>
        public override string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.Text;
                }
                return string.Empty;
            }

        }

        string resultText = string.Empty;

        /// <summary>
        /// SubTreeの結果のテキスト
        /// </summary>
        public string ResultText
        {
            get
            {
                Run();
                return resultText; 
            }
        }

        public override void Run(bool runChildren)
        {
            var last = this.GetDescendantRawler().Last();
            this.SetParent();
            base.Run(runChildren);
            resultText = last.Text;
            
        }


        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            var clone = new SubTree();
            RawlerLib.ObjectLib.FildCopy(this, clone);
            clone.SetParent(parent);
            this.CloneEvent(clone);
            clone.children.Clear();
            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
        }
    }
}
