using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rawler
{
    /// <summary>
    /// タグの中身のパラメータを取得します。
    /// </summary>
    public class GetTagParameter:RawlerBase
    {
        public string ParameterName { get; set; }
        public override void Run(bool runChildren)
        {
            if (string.IsNullOrEmpty(ParameterName) == false)
            {
                Regex regex = new Regex(ParameterName + "[ ]*=[\"| ]*(.+?)[\"| |$]", RegexOptions.IgnoreCase);
                Regex regex1 = new Regex(ParameterName + "[ ]*=[\'| ]*(.+?)[\'| |$]", RegexOptions.IgnoreCase);
                var r = regex.Match(GetText());
                var r2 = regex1.Match(GetText());
                if (r.Success)
                {
                    SetText(r.Groups[1].Value);
                    base.Run(runChildren);
                }
                else if(r2.Success)
                {
                    SetText(r2.Groups[1].Value);
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

        public static string RunMethod(string html,string ParameterName)
        {
            if (string.IsNullOrEmpty(ParameterName) == false)
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(ParameterName + "[ ]*=[\"|\'| ]*(.+?)[\"|\'| |$]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var r = regex.Match(html);
                if (r.Success)
                {
                    return r.Groups[1].Value;
                }
                else
                {
                }
            }
            else
            {

            }
            return null;

        }
    }
}
