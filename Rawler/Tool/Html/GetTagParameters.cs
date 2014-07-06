using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class GetTagParameter:RawlerBase
    {
        public string ParameterName { get; set; }
        public override void Run(bool runChildren)
        {
            if (string.IsNullOrEmpty(ParameterName) == false)
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(ParameterName + "[ ]*=[\"|\'| ]*(.+?)[\"|\'| |$]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var r = regex.Match(GetText());
                if (r.Success)
                {
                    SetText(r.Groups[1].Value);
                    base.Run(runChildren);
                }
                else
                {
                    ReportManage.ErrReport(this, "GetTagParameterで指定した" + ParameterName + "が見つかりませんでした");
                }
            }
            else
            {
                ReportManage.ErrReport(this, "GetTagParameterで指定した" + ParameterName + "が空文字です");

            }
        }
    }
}
