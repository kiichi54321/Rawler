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

namespace Rawler.View
{
    /// <summary>
    /// RawlerRunControl.xaml の相互作用ロジック
    /// </summary>
    public partial class RawlerRunControl : UserControl
    {
        public RawlerRunControl()
        {
            InitializeComponent();
            rawlerRoot.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(root_ProgressChanged);
            rawlerRoot.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(root_RunWorkerCompleted);
        }

        void root_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            TimeSpan ts = dt - startTime;
            textBox1.Text += "\n" + "かかった時間：" + ts.ToString();
            if (RunWorkerCompleted != null)
            {
                RunWorkerCompleted(this, e);
            }
            if (showCompleteMessageBox)
            {
                MessageBox.Show("完了しました。");
            }
        }

        private bool showCompleteMessageBox = true;

        /// <summary>
        /// 完了のメッセージを出す。
        /// </summary>
        public bool ShowCompleteMessageBox
        {
            get { return showCompleteMessageBox; }
            set { showCompleteMessageBox = value; }
        }

        public Visibility VisibleRunButton
        {
            get { return button1.Visibility; }
            set { button1.Visibility = value; }
        }

        public Visibility VisbleDataViewButton
        {
            get { return button2.Visibility; }
            set { button2.Visibility = value; }
        }

        public Visibility VisbleToTsvButton
        {
            get { return button3.Visibility; }
            set { button3.Visibility = value; }
        }

        /// <summary>
        /// 取得が完了したときのイベント
        /// </summary>
        public event System.ComponentModel.RunWorkerCompletedEventHandler RunWorkerCompleted;

        string countLabel = "NextDataRow";

        /// <summary>
        /// 数え上げるレポートのラベル。初期値は"NextDataRow"でNextDataRowに反応する。
        /// </summary>
        public string CountLabel
        {
            get { return countLabel; }
            set { countLabel = value; }
        }

        public TextBox TextBox { get { return textBox1; } }

        private int count = 0;
        private int maxTextSize = 10000;

        public int MaxTextSize
        {
            get { return maxTextSize; }
            set { maxTextSize = value; }
        }

        void root_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            var report = e.UserState as Tool.ReportEvnetArgs;
            if (report.Message.Contains(countLabel))
            {
                count++;
                textBlock3.Text = count.ToString();
            }
            if (report.Visible)
            {
                if (textBox1.Text.Length > maxTextSize)
                {
                    textBox1.Text = textBox1.Text.Substring(maxTextSize /5);
                }
                textBox1.Text += report.Message;
                if (report.ReturnCode)
                {
                  textBox1.Text += "\n";
                }

                
            }
        }

        public void SetRawler(Tool.RawlerBase rawler)
        {
            rawlerRoot.Rawler = rawler;
        }

        Tool.RawlerRoot rawlerRoot = new Tool.RawlerRoot();

        public Tool.RawlerRoot RawlerRoot
        {
            get { return rawlerRoot; }
            set { rawlerRoot = value; }
        }
        DateTime startTime;

        public void Run()
        {
            startTime = DateTime.Now;
            rawlerRoot.Run();
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (rawlerRoot.Rawler == null)
            {
                MessageBox.Show("Rawler Tree was not selected .");
                return;
            }
            startTime = DateTime.Now;
            rawlerRoot.Run();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = FilterStringCreate("tsv");
            if (sfd.ShowDialog()== true)
            {
                rawlerRoot.SaveTsv(sfd.FileName);
                MessageBox.Show("完了");
            }
        }


        private string FilterStringCreate(string extend)
        {
            return "<> files (*.<>)|*.<>|All files (*.*)|*.*".Replace("<>", extend);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = rawlerRoot.ToTsv();
        }
    }
}
