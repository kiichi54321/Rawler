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

namespace RawlerTool.Tool
{
    /// <summary>
    /// TextConvertWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TextConvertWindow : Window
    {
        public TextConvertWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBox2.Text = System.Web.HttpUtility.HtmlEncode(textBox1.Text);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Clipboard.GetText();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = System.Web.HttpUtility.HtmlDecode(textBox2.Text);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
        }
    }
}
