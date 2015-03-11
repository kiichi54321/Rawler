using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class View:RawlerBase
    {
        /// <summary>
        /// このオブジェクトのテキスト
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

        public override void Run(bool runChildren)
        {
            List<string> list = new List<string>();

            if (this.Parent is Imulti)
            {
                list.AddRange(((Imulti)this.Parent).Texts);
            }
            else
            {
                list.Add(this.Parent.Text);
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
            var clone = new View();
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
