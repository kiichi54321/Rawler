using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    /// <summary>
    /// 文字列の前後の空白を取り除く
    /// </summary>
    public class Trim : RawlerBase
    {
        /// <summary>
        /// 文字列の前後の空白を取り除く
        /// </summary>
        public Trim()
            : base()
        {
        }

        /// <summary>
        /// 文字列の前後の空白を取り除く
        /// </summary>
        /// <param name="doChopReturnCode">改行コードも削除するか</param>
        public Trim(bool doChopReturnCode)
        {
            this.doChopReturnCode = doChopReturnCode;
        }



        public override string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    string t = GetText();
                    if (doChopReturnCode)
                    {
                       t = t.Replace("\n", "").Replace("\r", "");
                    }
                    if(DoChopTabCode)
                    {
                        t = t.Replace("\t", "");
                    }
                    if(DoChopSpace)
                    {
                        t = t.Replace(" ", "");
                    }
                    return t.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }

        }

        private bool doChopReturnCode = false;

        /// <summary>
        /// 改行コードを削除するか？
        /// </summary>
        public bool DoChopReturnCode
        {
            get { return doChopReturnCode; }
            set { doChopReturnCode = value; }
        }

        private bool doChopTabCode = false;


        /// <summary>
        /// tabコードを削除するか？
        /// </summary>
        public bool DoChopTabCode
        {
            get
            {
                return doChopTabCode;
            }

            set
            {
                doChopTabCode = value;
            }
        }

        private bool doChopSpace = false;


        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public bool DoChopSpace
        {
            get
            {
                return doChopSpace;
            }

            set
            {
                doChopSpace = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Trim>(parent);
        }
    }
}
