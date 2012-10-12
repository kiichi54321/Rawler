using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class Braek:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            string txt = this.Parent.Text;
            base.Run(runChildren);
        }
    }
}
