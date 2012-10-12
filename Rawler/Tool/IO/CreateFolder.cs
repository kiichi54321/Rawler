using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class CreateFolder : RawlerBase
    {
        public string FolderName { get; set; }
        public override void Run(bool runChildren)
        {
            if (FolderName == null || FolderName.Length == 0)
            {
                ReportManage.ErrReport(this, "CreateFolderのFolderNameがありません");
            }
            else
            {
                if (System.IO.Directory.Exists(FolderName) == false)
                {
                    System.IO.Directory.CreateDirectory(FolderName);
                }

            }
            base.Run(runChildren);
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CreateFolder>(parent);
        }
    }
}
