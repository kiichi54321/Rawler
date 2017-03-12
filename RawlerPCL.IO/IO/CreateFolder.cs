using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    public class CreateFolder : RawlerBase
    {
        public string FolderName { get; set; }
        public RawlerBase FolderNameTree { get; set; }
        private SpecialFolder workFolderType = SpecialFolder.none;

        public SpecialFolder SpecialFolder
        {
            get { return workFolderType; }
            set { workFolderType = value; }
        }

        public override void Run(bool runChildren)
        {
            string path = string.Empty;
            if (workFolderType == Tool.SpecialFolder.MyDocuments)
            {
                path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            if (workFolderType == Tool.SpecialFolder.Desktop)
            {
                path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }

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
                 path = System.IO.Path.Combine(path,folder);
                 if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
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
