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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace Rawler.View
{
    /// <summary>
    /// RawlerControl.xaml の相互作用ロジック
    /// </summary>
    public partial class RawlerControl : UserControl
    {
        public RawlerControl()
        {
            InitializeComponent();
            rawlerRunControl1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(rawlerRunControl1_RunWorkerCompleted);


            //Assembly assembly = Assembly.GetExecutingAssembly();
            ////アセンブリで定義されている型をすべて取得する
            //Type[] ts = assembly.GetTypes();

            
            ////型の情報を表示する
            //foreach (Type t in ts)
            //{
            //    if (t.BaseType.FullName.Contains("Rawler.Tool"))
            //    {
            //        comboBox1.Items.Add(t);
            //    }
            //}
        }

        public void GetRawlerClass()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            //アセンブリで定義されている型をすべて取得する
            Type[] ts = assembly.GetTypes();


            //型の情報を表示する
            foreach (Type t in ts)
            {
                if (t.BaseType.FullName.Contains("Rawler.Tool"))
                {
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Content = t.FullName;
                    cbi.Tag = t;
                    comboBox1.Items.Add(cbi);
                }
            }

        }



        void rawlerRunControl1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (RunWorkerCompleted != null)
            {
                RunWorkerCompleted(sender, e);
            }
        }

        /// <summary>
        /// 取得が完了したときのイベント
        /// </summary>
        public event System.ComponentModel.RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Rawlerツリーをセットする。
        /// </summary>
        /// <param name="rawler"></param>
        public void SetRawler(Tool.RawlerBase rawler)
        {
            rawlerRunControl1.SetRawler(rawler);
            rawlerView1.SetRawler(rawler);
        }

        public void Run()
        {
            rawlerRunControl1.Run();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)comboBox1.SelectedItem;
            var instance = Activator.CreateInstance((Type)item.Tag);
            SetRawler((Tool.RawlerBase)instance);
        }
    }
}
