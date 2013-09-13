using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 親のテキストの前と後ろにテキストを足す。
    /// </summary>
    public class AppendText:RawlerBase
    {
        /// <summary>
        /// Header　親のTextの前に足すText
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Footer 親のTextの後に足すText
        /// </summary>
        public string Footer { get; set; }
        public string Value { get; set; }
        public override void Run(bool runChildren)
        {
            string t =string.Empty;
            if (string.IsNullOrEmpty(Value))
            {
                t = GetText();
            }
            else
            {
                t = Value;
            }
            if (Header != null)
            {
                t = Header + t;
            }
            if (HeaderTree != null)
            {
                t = RawlerBase.GetText(this.Parent.Text, HeaderTree, this.Parent) + t;
            }
            if (Footer != null)
            {
                t = t + Footer;
            }
            if (FooterTree != null)
            {
                t = t + RawlerBase.GetText(this.Parent.Text, FooterTree, this.Parent);
            }
            SetText(t);
            base.Run(runChildren);
        }
        public RawlerBase HeaderTree { get; set; }
        public RawlerBase FooterTree { get; set; }

        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<AppendText>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion
    }
}
