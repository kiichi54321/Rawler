using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 子以下を無視します。
    /// </summary>
    public class Ignore:RawlerBase
    {
        bool isIgnore = true;

        /// <summary>
        /// 無視するときはTrue　デフォ値はTrue
        /// </summary>
        public bool IsIgnore
        {
            get { return isIgnore; }
            set { isIgnore = value; }
        }


        public override void Run(bool runChildren)
        {
            if (isIgnore == false)
            {
                this.RunChildren(runChildren);
            }
        }

        /// <summary>
        /// このオブジェクトのテキスト
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }

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
            var clone = new Ignore();
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
