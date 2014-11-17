using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;
using System.Windows.Markup;

namespace Rawler.NPL
{
    public class TinySegmenter:RawlerMultiBase
    {
        TinySegmenterDotNet.TinySegmenter seg = new TinySegmenterDotNet.TinySegmenter();
        public string DicFile { get; set; }
        public PreSegmentBase PreSegment { get; set; }
        bool init = false;

        public override void Run(bool runChildren)
        {
            if(init == false)
            {
                if (PreSegment != null)
                {
                    seg.PreSegmentFunc = (n) =>PreSegment.Run( new TinySegmenterDotNet.PreSegment(n).PreprocessingForJapanese());
                }
                else
                {
                    seg.PreSegmentFunc = (n) => new TinySegmenterDotNet.PreSegment(n).PreprocessingForJapanese();
                }
                if(string.IsNullOrEmpty(DicFile)==false && System.IO.File.Exists(DicFile))
                {
                    seg.AddRangeWordDic(System.IO.File.ReadLines(DicFile));
                }
                init = true;
            }

           var t =  seg.SegmentExted(GetText());
           base.RunChildrenForArray(runChildren, t);
        }
    }

    [ContentProperty("Child")]
    public class PreSegmentBase
    {
        public PreSegmentBase Child { get; set; }

        protected virtual TinySegmenterDotNet.PreSegment ChangePreSegment(TinySegmenterDotNet.PreSegment pre)
        {
            return pre;
        }

        public TinySegmenterDotNet.PreSegment Run(TinySegmenterDotNet.PreSegment pre)
        {
            pre = ChangePreSegment(pre);
            if(Child !=null) return Child.Run(pre);
            return pre;
        }
    }

    public class TakeUrl:PreSegmentBase
    {
        protected override TinySegmenterDotNet.PreSegment ChangePreSegment(TinySegmenterDotNet.PreSegment pre)
        {
            return pre.TakeUrl();
        }
    }
    public class TakeMention : PreSegmentBase
    {
        protected override TinySegmenterDotNet.PreSegment ChangePreSegment(TinySegmenterDotNet.PreSegment pre)
        {
            return pre.TakeMention();
        }
    }
    public class TakeRegex : PreSegmentBase
    {
        public string Pattern { get; set; }
        protected override TinySegmenterDotNet.PreSegment ChangePreSegment(TinySegmenterDotNet.PreSegment pre)
        {
            return pre.TakeRegex(Pattern);
        }
    }
    public class SkipUrl:PreSegmentBase
    {
        protected override TinySegmenterDotNet.PreSegment ChangePreSegment(TinySegmenterDotNet.PreSegment pre)
        {
            return pre.SkipUrl();
        }
    }
    public class SkipMention:PreSegmentBase
    {
        protected override TinySegmenterDotNet.PreSegment ChangePreSegment(TinySegmenterDotNet.PreSegment pre)
        {
            return pre.SkipMention();
        }
    }
    public class SkipRegex : PreSegmentBase
    {
        public string Pattern { get; set; }
        protected override TinySegmenterDotNet.PreSegment ChangePreSegment(TinySegmenterDotNet.PreSegment pre)
        {
            return pre.SkipRegex(Pattern);
        }
    }

}
