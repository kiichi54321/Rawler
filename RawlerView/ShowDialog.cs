using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
namespace Rawler.View
{
    public class ShowOpenDialog:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|tsv files (*.tsv)|*.tsv|All files (*.*)|*.*"; 
            if(dialog.ShowDialog()== true)
            {
                SetText(dialog.FileName);
            }

            base.Run(runChildren);
        }
    }

    public class ShowSaveDialog : RawlerBase
    {
        public override void Run(bool runChildren)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|tsv files (*.tsv)|*.tsv|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                SetText(dialog.FileName);
            }
            base.Run(runChildren);
        }
    }
}
