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
using RawlerView.Form.Core;

namespace RawlerView.Form
{
    /// <summary>
    /// FormWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FormWindow : Window
    {
        public FormWindow()
        {
            InitializeComponent();
        }

        public void SetUp(IEnumerable<Form.Core.FormParts> list)
        {
            this.ItemsControl.ItemsSource = list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
