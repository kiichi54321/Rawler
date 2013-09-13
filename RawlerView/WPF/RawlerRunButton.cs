using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerView.WPF
{
    public class RawlerRunButton:System.Windows.Controls.Button
    {
        public string RawlerXAMLFileName { get; set; }
        public string Group { get; set; }

        public RawlerRunButton()
            :base()
        {
        }

        protected override void OnClick()
        {
            var xaml = RawlerRunMange.BaseConvert(RawlerXAMLFileName);
            RawlerRunMange.RunRawler(xaml);
            base.OnClick();
        }

    }
}
