using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;

namespace Rawler.Tool
{
    /// <summary>
    /// 上流のPageにHttpHeaderを加える
    /// </summary>
    public class AddHttpHeader:RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<AddInputParameter>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public string Key { get; set; }
        public string Value { get; set; }

        public override void Run(bool runChildren)
        {
            var page = this.GetUpperRawler<Page>();
            if (page == null) ReportManage.ErrUpperNotFound<Page>(this);
            var val = GetText();
            if(Value != null)
            {
                val = Value.Convert(this);
            }
            if(page !=null)
            {
                page.AddHttpHeader(this.Key.Convert(this), val);
            }
            base.Run(runChildren);
        }
    }
}
