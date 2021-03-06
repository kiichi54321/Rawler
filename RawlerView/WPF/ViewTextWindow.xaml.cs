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
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Threading;
using RawlerView;

namespace Rawler.RawlerLib.WPF
{
    /// <summary>
    /// ViewTextWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ViewTextWindow : Window
    {
        public ViewTextWindow()
        {
            InitializeComponent();
            this.Closed += new EventHandler(ViewTextWindow_Closed);
        }

        bool isClosed = false;

        public bool IsClosed
        {
            get { return isClosed; }
            set { isClosed = value; }
        }

        void ViewTextWindow_Closed(object sender, EventArgs e)
        {
            IsClosed = true;
        }
        Dictionary<string, TabItem> tagDic = new Dictionary<string, TabItem>();

        public void AddText(string name ,string text)
        {
            if (name == null) name = string.Empty;
            if (tagDic.ContainsKey(name))
            {
                ((ViewTextControl)tagDic[name].Content).Text = text;
                tabControl1.SelectedItem = tagDic[name];
            }
            else
            {
                var tab = new TabItem() { Header = name };
                tab.Content = new ViewTextControl() { Text = text };
                tagDic.Add(name, tab);
                tabControl1.Items.Add(tab);
                tabControl1.SelectedItem = tagDic[name];
            }
          
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.Items.Clear();
        }
    }




    public static class ViewTextWindowManage
    {
        static ViewTextWindow window;

        public static void AddText(string name, string text)
        {
            ViewTask.UITask(() =>
            {
                if (window == null || window.IsClosed )
                {
                    window = new ViewTextWindow();
                }
                window.AddText(name, text);
                window.Show();
            });
        }
    }
}
