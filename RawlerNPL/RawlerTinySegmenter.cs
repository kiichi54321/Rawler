using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;

namespace RawlerNPL
{
    public class TinySegmenter:RawlerMultiBase
    {
        TinySegmenterDotNet.TinySegmenter seg = new TinySegmenterDotNet.TinySegmenter();
        public override void Run(bool runChildren)
        {
           var t =  seg.Segment(GetText());
           base.RunChildrenForArray(runChildren, t);
        }
    }
}
