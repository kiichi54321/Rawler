using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class CreateFolder : RawlerBase
    {
        public string FolderName { get; set; }
        public RawlerBase FolderNameTree { get; set; }

        public override void Run(bool runChildren)
        {
            string folder = null;
            if(FolderNameTree !=null)
            {
                folder = RawlerBase.GetText(this.GetText(), FolderNameTree, this);
            }
            else
            {
                folder = FolderName;
            }

            if (folder == null || folder.Length == 0)
            {
                ReportManage.ErrReport(this, "CreateFolderのFolderNameがありません");
            }
            else
            {
                if (System.IO.Directory.Exists(folder) == false)
                {
                    System.IO.Directory.CreateDirectory(folder);
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
