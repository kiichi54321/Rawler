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
using System.Text.RegularExpressions;

namespace RawlerTool.Tool
{
    /// <summary>
    /// InsertParameterWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InsertParameterWindow : Window
    {
        public InsertParameterWindow()
        {
            InitializeComponent();
        }

        List<KeyValueObject> KeyValueList = new List<KeyValueObject>();
        public bool? Analyze(string xaml)
        {
            var list= GetParameterList(xaml);
            this.xaml = xaml;
            if (list.Length > 0)
            {
                KeyValueList.Clear();
                stackPanel.Children.Clear();
                foreach (var item in list)
                {
                    var i = new KeyValueObject() { Key = item, Value = string.Empty };
                    var dt = this.Resources["KeyValueRow"] as DataTemplate;
                    var vt = dt.LoadContent() as FrameworkElement;
                    vt.DataContext = i;
                    stackPanel.Children.Add(vt);
                    KeyValueList.Add(i);
                }
                return this.ShowDialog();
            }
            else
            {
                return true;
            }
        }
        string xaml = string.Empty;

        public string Xaml
        {
            get { return xaml; }
            set { xaml = value; }
        }

        public string[] GetParameterList(string xaml)
        {
            List<string> list = new List<string>();
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\"@@([^\"]*)\"");
            foreach (Match item in regex.Matches(xaml))
            {
//                list.Add(item.Value);
                list.Add(item.Groups[1].Value);
                //    item.Value.Replace()
            }
            return list.ToArray();
        }

       

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in KeyValueList)
            {
                xaml = xaml.Replace("@@"+item.Key, item.Value);
            }
            this.DialogResult = true;
            this.Close();
        }


        public class KeyValueObject
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
