using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
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

        public override void Run(bool runChildren)
        {
            string file = string.Empty;
            if (this.FileName != null)
            {
                file = this.FileName.Convert(this);
            }
            else
            {
                file = this.GetText();
            }

            if (Rawler.IO.IoState.FileExists(file)== Result)
            {
                base.Run(runChildren);
            }
        }
    }
}
