using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    public class ReportCurrentDataRow:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var d = (IData)this.GetUpperInterface<IData>();
            if (d != null)
            {
                ReportManage.Report(this, d.GetCurrentDataRow().ToString(),true,true);
            }
        }
    }
}
