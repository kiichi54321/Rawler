using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler.Condition
{
    /// <summary>
    /// 指定の文字が含まれる含まれないで、子をRunする。実質IF文。初期値では「含まれる」で判定する
    /// </summary>

    public class Contains : RawlerBase
    {
            /// <summary>
        /// 指定の文字が含まれる含まれないで、子をRunする。実質IF文。
        /// </summary>
        public Contains()
            : base()
        {
            init();
        }


        private void init()
        {

        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        /// <summary>
        /// 指定の文字が含まれる含まれないで、子をRunする。実質IF文。初期値では「含まれる」で判定する
        /// </summary>
        /// <param name="ContainsText">含まれるテキスト</param>
        public Contains(string ContainsText):base()
        {
            this.ContainsText = ContainsText;
        }
        /// <summary>
        /// 指定の文字が含まれる含まれないで、子をRunする。実質IF文。含まれないを指定するとき使う。
        /// </summary>
        /// <param name="ContainsText">含まれるテキスト</param>
        /// <param name="result">含まれないならFalse</param>
        public Contains(string ContainsText,bool result)
            : base()
        {
            this.ContainsText = ContainsText;
            this.Result = result;
        }
            
        /// <summary>
        /// 含まれる文字列（必須）
        /// </summary>
        public string ContainsText { get; set; }
        private bool result = true;
        /// <summary>
        /// 含まれるのを判定するならTrue　含まれないものを判定するならFalse
        /// </summary>
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }
        public RawlerBase TextTree { get; set; }
        public RawlerBase ContainsTextTree { get; set; }
        /// <summary>
        /// 含まれるか判定する。
        /// </summary>
        public bool CheckContains
        {
            get
            {
                return  this.Text.Contains(this.ContainsText);
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

        //public new void Run()
        //{
        //    Run(true);
        //}

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            string containsText = this.ContainsText.Convert(this);
            string text = this.GetText();
            if (ContainsTextTree != null)
            {
                containsText = RawlerBase.GetText(this.Parent.Text, ContainsTextTree, this);
            }
            if (TextTree != null)
            {
                text = RawlerBase.GetText(this.Parent.Text, TextTree, this);
            }

            if (text.Contains(containsText) == this.Result)
            {
                this.RunChildren(runChildren);
            }

            //if (ContainsTextTree == null)
            //{
            //    if (this.GetText().Contains(this.ContainsText) == this.Result)
            //    {
            //        this.RunChildren(runChildren);
            //    }
            //}
            //else
            //{
            //    string t = RawlerBase.GetText(this.Parent.Text, ContainsTextTree, this);
            //    if (this.GetText().Contains(t) == this.Result)
            //    {
            //        this.RunChildren(runChildren);
            //    }
            //}
        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Contains>(parent);
        }

    }
}
