using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    /// <summary>
    /// 絶対URIに変換する
    /// </summary>
    public class ConvertAbsoluteUri:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var p = this.GetUpperRawler<Page>();
            if(p !=null)
            {
                Uri uri = new Uri(new Uri(p.GetCurrentUrl()), GetText());
                SetText(uri.AbsoluteUri);
            }

            base.Run(runChildren);  
        }
    }
}
