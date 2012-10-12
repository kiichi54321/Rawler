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
using System.Threading.Tasks;
using Rawler.Tool;
using System.Threading;
using ICSharpCode.AvalonEdit.Folding;
using System.Windows.Threading;

namespace RawlerTool
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window,IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();
            ReportManage.ErrReportEvent += new EventHandler<ReportEvnetArgs>(ReportManage_ErrReportEvent);
            ReportManage.ReportEvnet += new EventHandler<ReportEvnetArgs>(ReportManage_ReportEvnet);
            ReportManage.StockReport = false;
            init();
            System.Xaml.XamlSchemaContext x = new System.Xaml.XamlSchemaContext();
            var tmp = new Rawler.Tool.Data();
            tmp.Children.Add(new Rawler.Tool.Page());
            rawlerView1.SetRawler(tmp );
        }

        private void init()
        {
            foldingStrategy = new XmlFoldingStrategy();
            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();

            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();

            RawlerLib.UIData.UISyncContext = UISyncContext;
        }


        void ReportManage_ReportEvnet(object sender, ReportEvnetArgs e)
        {
            if (e.Visible)
            {
                string txt = e.DateTime.ToShortTimeString()+"\t"+ e.Message;
                if (e.ReturnCode)
                {
                    txt += "\n";
                }
                AddMessage(txt);
            }
            if (e.Message.Contains("NextDataRow"))
            {
                AddRowCount();
            }
        }
        int rowCount = 0;
        void ReportManage_ErrReportEvent(object sender, ReportEvnetArgs e)
        {
            string txt = e.DateTime.ToShortTimeString() + "\t" + e.Message;
            if (e.ReturnCode)
            {
                txt += "\n";
            }
            AddMessage(txt);
        }

        TaskScheduler UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();

        void AddRowCount()
        {
            Task reportProgressTask = Task.Factory.StartNew(() =>
            {
                rowCount++;
                textBlock2.Text = rowCount.ToString();
            },
                     CancellationToken.None,
                     TaskCreationOptions.None,
                     UISyncContext);
            reportProgressTask.Wait();
        }

        void AddMessage(string text)
        {
            Task reportProgressTask = Task.Factory.StartNew(() =>
            {

                {
                    try
                    {
                        textBox2.Text += text;
                    }
                    catch (Exception e)
                    {
                        textBox2.Text = text;
                    }
                }
            },
                      CancellationToken.None,
                      TaskCreationOptions.None,
                      UISyncContext);
            reportProgressTask.Wait();
        }
        RawlerBase rawler = null;
        Task task = null;
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (task != null && task.IsCompleted == false)
            {
                MessageBox.Show("実行中です");
                return;
            }
            tokenSource = new CancellationTokenSource();
            object obj = null;
            string xaml = textEditor.Text;
            var insertParameter = new Tool.InsertParameterWindow();
            if (insertParameter.Analyze(textEditor.Text) == true)
            {
                xaml = insertParameter.Xaml;
            }
            else
            {
                return;
            }

            try
            {
                obj = System.Xaml.XamlServices.Parse(xaml);
            }
            catch (Exception ex)
            {
                ReportManage.ErrReport(new RawlerBase(), "XAMLの形式がおかしいです" + ex.Message);

            }
            if (obj == null)
            {
                return;
            }
            if ((obj is Rawler.Tool.RawlerBase)==false)
            {
                ReportManage.ErrReport(new RawlerBase(), "キャストできませんでした。XAMLの形式がおかしいです");
            }
            try
            {
                rawler = (obj as Rawler.Tool.RawlerBase);
                rowCount = 0;
                rawler.SetParent();
                startDate = DateTime.Now;
                foreach (var item in rawler.GetConectAllRawler())
                {
                    item.BeginRunEvent += (o, arg) =>
                    {
                        tokenSource.Token.ThrowIfCancellationRequested();
                        while (pause)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    };
                }

                task = Task.Factory.StartNew(() => rawler.Run()).ContinueWith((t) => StopWatch());
            }
            catch (OperationCanceledException oce)
            {               
                ReportManage.ErrReport(new RawlerBase(), "キャンセルされました");
                MessageBox.Show("キャンセルされました");
            }
            catch (Exception ex)
            {
                ReportManage.ErrReport(new RawlerBase(), ex.Message);
            }
        }

        private bool pause = false;
        private DateTime startDate;

        private void StopWatch()
        {
            if (startDate != null)
            {
                var time = DateTime.Now - startDate;
                AddMessage("経過時間："+time.ToString());
            }
            MessageBox.Show("Complete");
        }



        public RawlerBase GetRawlerBase(string xaml)
        {
            object obj = null;
            try
            {
                obj = System.Xaml.XamlServices.Parse(textEditor.Text);
            }
            catch (Exception ex)
            {
                ReportManage.ErrReport(new RawlerBase(), "XAMLの形式がおかしいです" + ex.Message);

            }
            if (obj == null)
            {
                return null;
            }
            if ((obj is Rawler.Tool.RawlerBase) == false)
            {
                ReportManage.ErrReport(new RawlerBase(), "キャストできませんでした。XAMLの形式がおかしいです");
                return null;
            }
            else
            {
                return (obj as Rawler.Tool.RawlerBase);
            }

        }


        string FilterStringCreate(string extend)
        {
            return "<> files (*.<>)|*.<>|All files (*.*)|*.*".Replace("<>", extend);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = FilterStringCreate("xaml");
            if (dialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(dialog.FileName, textEditor.Text);
            }
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = FilterStringCreate("xaml");
            if (dialog.ShowDialog() == true)
            {
                textEditor.Text = System.IO.File.ReadAllText(dialog.FileName);
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (rawler != null)
            {
                textBox2.Text += "\n\n";
                foreach (Data item in rawler.GetDescendantRawler().Where(n => n is Data))
                {
                    textBox2.Text += item.ToTsv();
                    textBox2.Text += "\n\n\n";
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            var r = GetRawlerBase(textEditor.Text);
            textBox2.Text = System.Xaml.XamlServices.Save(r);
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            textBox2.Text = string.Empty;
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Text = string.Empty;

        }

        #region Folding
        FoldingManager foldingManager;
        AbstractFoldingStrategy foldingStrategy;

        void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
        }
        #endregion

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }

        public string Rawler2XAML(RawlerBase rawler)
        {
            StringBuilder xaml = new StringBuilder(System.Xaml.XamlServices.Save(rawler));
            xaml = xaml.Replace("\"{x:Null}\"", "Null").Replace(" Enable=\"True\"","").Replace(" Comment=\"\"","");
            
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\w*=Null");
            List<string> list = new List<string>();
            foreach (System.Text.RegularExpressions.Match item in regex.Matches(xaml.ToString()))
            {
                list.Add(item.Value);
            }

            foreach (var item in list.Distinct())
            {
                xaml = xaml.Replace(" " + item, string.Empty);            
            }


            return xaml.ToString();
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl1.SelectedItem == tabXAML)
            {
                var rawler = rawlerView1.GetRawler();
                textEditor.Text = Rawler2XAML(rawler);

                //System.Xaml.XamlSchemaContextSettings xamlSchemaContextSettings = new System.Xaml.XamlSchemaContextSettings();
                //xamlSchemaContextSettings.FullyQualifyAssemblyNamesInClrNamespaces = false;
                //System.Xaml.XamlSchemaContext xamlSchemaContext = new System.Xaml.XamlSchemaContext();
                //System.Xaml.XamlXmlWriterSettings xamlXmlWriterSettings = new System.Xaml.XamlXmlWriterSettings();
                //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                //{
                //    System.Xaml.XamlXmlWriter xamlXmlWriter = new System.Xaml.XamlXmlWriter(ms, xamlSchemaContext);
                //    x
                //}
            }
            else if (tabControl1.SelectedItem == tabTree)
            {
                var rawler = GetRawlerBase(textEditor.Text);
                if (rawler != null)
                {
                    rawlerView1.SetRawler(rawler);
                }
            }
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDailog = new Microsoft.Win32.SaveFileDialog();
            saveDailog.Filter = FilterStringCreate("tsv");
            if (saveDailog.ShowDialog() == true)
            {
                using (var stream = System.IO.File.CreateText(saveDailog.FileName))
                {
                    foreach (Data item in rawler.GetDescendantRawler().Where(n => n is Data))
                    {
                        stream.WriteLine(item.ToTsv());
                        stream.WriteLine();
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            pause = !pause;
            if (pause)
            {
                ReportManage.Report(null, "---Pause---");
                PauseResumeButton.Content = "Resume";
            }
            else
            {
                ReportManage.Report(null, "---Resume---");
                PauseResumeButton.Content = "Pause";
            }
        }


        public void Dispose()
        {
            if (task != null && tokenSource !=null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                task.Dispose();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var w = new Tool.TextConvertWindow();
            w.Show();
        }
    }
}