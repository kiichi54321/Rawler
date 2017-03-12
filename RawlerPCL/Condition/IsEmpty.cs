using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Condition
{
    /// <summary>
    /// 空文字であるかを判定します。
    /// </summary>
    public class IsEmpty:RawlerBase
    {
               /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        /// <summary>
        /// 空文字であるかを判定し、空文字なら、子をRunする。実質IF文。含まれないを指定するとき使う。
        /// </summary>
        /// <param name="result">空文字でないならFalse</param>
        public IsEmpty(bool result)
            : base()
        {
            this.Result = result;
        }

        /// <summary>
        /// 空文字であるかを判定し、空文字なら、子をRunする。実質IF文。含まれないを指定するとき使う。
        /// </summary>
        public IsEmpty()
            : base()
        {
            this.Result = result;
        }

        private bool result = true;
        /// <summary>
        ///　空文字でTrue　含まれないものを判定するならFalse
        /// </summary>
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }


        /// <summary>
        /// 含まれるか判定する。
        /// </summary>
        public bool CheckIsEmpty
        {
            get
            {
                if (this.Text == null || this.Text.Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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

            if (CheckIsEmpty == this.Result)
            {
                this.RunChildren(runChildren);
            }
        }


    }
}
