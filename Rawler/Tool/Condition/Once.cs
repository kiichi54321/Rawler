using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 親の文字列を一度だけ通す。
    /// </summary>
    public class Once :RawlerBase
    {
        HashSet<string> hash = new HashSet<string>();

        public override void Run(bool runChildren)
        {
            var t = GetText();
            if(hash.Contains(t) == false)
            {
                hash.Add(t);
                SetText(t);
                base.Run(runChildren);
            }
        }
    }
}
