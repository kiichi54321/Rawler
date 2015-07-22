using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using RawlerLib.MyExtend;

namespace Rawler.TextTool
{
    public class Tsv2Text:Rawler.Tool.RawlerBase
    {
        public string TargetColumn { get; set; }
        public string IgnoreColumns { get; set; }

        public override void Run(bool runChildren)
        {
            var tsv= this.GetUpperRawler<TsvReadLines>();
            
            var ignore = IgnoreColumns.NullIsEmpty().Split(',').ToList().Adds(new string[]{TargetColumn});
            var list = tsv.GetColumns().Where(n => ignore.Contains(n) == false);
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item + ":" + tsv.GetValue(item) + ",");
            }
            sb.Length = sb.Length - 1;

            SetText(tsv.GetValue(TargetColumn).Replace("{","｛").Replace("}","｝") + "{" + sb.ToString() + "}");

            base.Run(runChildren);
        }
    }
}
