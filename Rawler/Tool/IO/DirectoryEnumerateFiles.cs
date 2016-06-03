using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool.IO
{
    public class DirectoryEnumerateFiles:RawlerMultiBase
    {
        public string DirectoryName { get; set; }
        public string SearchPattern { get; set; }
        public override void Run(bool runChildren)
        {
            if (DirectoryName == null)
            {
                ReportManage.ErrEmptyPropertyName(this, nameof(DirectoryName));
                return;
            }
            var list = System.IO.Directory.EnumerateFiles(DirectoryName.Convert(this), SearchPattern.Convert(this));
            base.RunChildrenForArray(true, list);
        }
    }
}
