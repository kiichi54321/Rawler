using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// 文字列の前後の空白を取り除く
    /// </summary>
    [Serializable]
    [ContentProperty("Children")]
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
                    if (doChopReturnCode)
                    {
                        return GetText().Trim().Replace("\n", "").Replace("\r", "");
                    }
                    else
                    {
                        return GetText().Trim();
                    }
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



        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
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
