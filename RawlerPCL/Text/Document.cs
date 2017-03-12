using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Rawler
{
    /// <summary>
    /// テキストを指定できるシンプルなRawlerクラス。
    /// </summary>

    public class Document : RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }


        /// <summary>
        /// 子をセットする。
        /// </summary>
        /// <param name="list"></param>
        public void SetChildren(RawlerCollection list)
        {
            this.children = list;
            foreach (var item in children)
            {
                item.SetParent(this);
            }
        }


        public  string TextValue
        {
            get
            {
                return base.Text.Convert(this);
            }
            set
            {
                base.SetText(value);
            }
        }
        ///// <summary>
        ///// テキストをセットする。
        ///// </summary>
        ///// <param name="text"></param>
        //public void SetText(string text)
        //{
        //    this.text = text;
        //}

        /// <summary>
        /// 実行する。何もせず、子を実行する。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            RunChildren(runChildren);
        }

        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Document>(parent);
        }


    }
}
