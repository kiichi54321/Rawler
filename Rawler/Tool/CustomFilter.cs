using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// カスタマイズ可能なフィルターです。
    /// Check に任意のメソッドを入れてください。Trueのとき子が実行されます。
    /// </summary>
    public class CustomFilter:RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
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


        public Predicate<CustomFilter> Check { get; set; }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (Check != null)
            {
                if (Check.Invoke(this))
                {
                    this.RunChildren(runChildren);
                }
            }
        }

        protected override void CloneEvent(RawlerBase rawler)
        {
            base.CloneEvent(rawler);
            if (rawler is CustomFilter)
            {
                var r = (CustomFilter)rawler;
                r.Check = this.Check;
            }
        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            var clone = new CustomFilter();
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
