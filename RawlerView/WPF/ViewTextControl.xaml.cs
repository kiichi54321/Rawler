<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rawler.RawlerLib.WPF
{
    /// <summary>
    /// ViewTextControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ViewTextControl : UserControl
    {
        public ViewTextControl()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            textBox1.Text = text;
            webBrowser1.NavigateToString(text);
        }
        public string Text
        {
            set
            {
                SetText(value);
            }
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rawler.RawlerLib.WPF
{
    /// <summary>
    /// ViewTextControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ViewTextControl : UserControl
    {
        public ViewTextControl()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            textBox1.Text = text;
            webBrowser1.NavigateToString(text);
        }
        public string Text
        {
            set
            {
                SetText(value);
            }
        }
    }
}
>>>>>>> 6ddbdbbc3a7813a9636b3cdd2ae14b324c102419
