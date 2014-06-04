using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class ReportDataRow2Json:Report
    {
        public override void Run(bool runChildren)
        {
            try
            {
                this.Message = this.GetAncestorRawler().OfType<Data>().First().GetCurrentDataRow().ToJson();
                this.Visible = true;
                base.Run(runChildren);
            }
            catch
            {
                ReportManage.ErrReport(this, "上流にDataがありません。");
            }
        }
    }
}
