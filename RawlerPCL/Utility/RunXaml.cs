using Portable.Xaml;
using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    /// <summary>
    /// ファイルのXAMLを読んで実行する。
    /// </summary>
    public class RunXaml:RawlerBase
    {
        public string FileName { get; set; }

        public override void Run(bool runChildren)
        {
            try
            {
                RawlerBase rawler = (RawlerBase)XamlServices.Load(FileName.Convert(this));
                rawler.SetParent(this);
                rawler.Run();
            }
            catch(Exception ex)
            {
                ReportManage.ErrReport(this, ex.ToString());
            }           
        }
    }
}
