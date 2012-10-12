using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class CustomText :RawlerBase
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
                if (CreateText != null)
                {
                    return CreateText.Invoke(this);
                }
                ReportManage.ErrReport(this, "CreateTextがありません");
                return string.Empty;
            }

        }

        public delegate string CreateTextDelegate(RawlerBase rawler);

        public CreateTextDelegate CreateText { get; set; }
        


        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {

             this.RunChildren(runChildren);
            
        }

        protected override void CloneEvent(RawlerBase rawler)
        {
            base.CloneEvent(rawler);
            if (rawler is CustomText)
            {
                var r = (CustomText)rawler;
                r.CreateText = this.CreateText;
            }
        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            var clone = new CustomText();
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
