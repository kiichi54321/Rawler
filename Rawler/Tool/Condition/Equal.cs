using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class Equal : RawlerBase
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
        public Equal()
            : base()
        {
            this.Result = result;
        }

        private string equalCSV = string.Empty;
        private string[] equalArray = null;
        public string EqualCSV
        {
            get { return equalCSV; }
            set
            {
                equalCSV = value;
                equalArray = equalCSV.Split(',');
            }
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
        public bool Check
        {
            get
            {
                var t = GetText();
                if (equalArray.Any(n => n.Equals(t)))
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

            if (Check == this.Result)
            {
                this.RunChildren(runChildren);
            }
        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return this.Clone<Equal>(parent);
        }
    }
}
