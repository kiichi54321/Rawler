using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class CheckFileExists:RawlerBase
    {
        bool result = true;

        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        public string FileName { get; set; }
        public RawlerBase FileNameTree { get; set; }

        public override void Run(bool runChildren)
        {
            string file = string.Empty;
            if (this.FileName != null)
            {
                file = this.FileName;
            }
            else if (FileNameTree != null)
            {
                file = RawlerBase.GetText(GetText(), FileNameTree, this);
            }
            else
            {
                file = this.GetText();
            }

            if (System.IO.File.Exists(file)== Result)
            {
                base.Run(runChildren);
            }
        }
    }
}
