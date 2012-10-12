using System;
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
using System.Windows.Shapes;

namespace RawlerTwitter
{
    /// <summary>
    /// PinDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class PinDialog : Window
    {
        public PinDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public string PIN { get {return textBox1.Text; } }
    }
}
