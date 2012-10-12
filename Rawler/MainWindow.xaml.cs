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
using System.IO;
using System.Windows.Markup;

namespace Rawler
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            root.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(root_ProgressChanged);
            root.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(root_RunWorkerCompleted);
            //Tool.ReportManage.ReportEvnet += new EventHandler<Tool.ReportEvnetArgs>(ReportManage_ReportEvnet);
            //Tool.ReportManage.ErrReportEvent += new EventHandler<Tool.ReportEvnetArgs>(ReportManage_ErrReportEvent);
        }

        void ReportManage_ErrReportEvent(object sender, Tool.ReportEvnetArgs e)
        {
            textBox1.Text += e.Message + "\n";
        }

        void ReportManage_ReportEvnet(object sender, Tool.ReportEvnetArgs e)
        {
            if (e.Message.Contains("Person:"))
            {
                count++;
                textBlock1.Text = count.ToString() + "人";
            }
            else
            {
                textBox1.Text += e.Message+"\n";
            }
        }

        void root_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            DateTime endDate = DateTime.Now;
            var ts = endDate - startDate;
            textBox1.Text += "終了時刻:"+endDate.ToLocalTime()+"\n";
            textBox1.Text += "かかった時間:" + ts.TotalHours.ToString()+"\n";

            var list = root.GetData().First().ToList();

            MyLib.ObjectLib.SaveToBinaryFile(list, "Celebrity1.data");
  
            textBox1.Text += "完了";
        }

        int count = 0;
        void root_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            var report = e.UserState as Tool.ReportEvnetArgs;
            if (report.IsErr == false && report.Message.Contains("Person:"))
            {
                count++;
                textBlock1.Text = count.ToString() + "人";
            }
            else
            {
                textBox1.Text += report.Message + "\n";
            }
        }

        Tool.RawlerRoot root = new Tool.RawlerRoot();

        private void CreateRawler() 
        {
            Tool.Data data = new Tool.Data();
            root.Rawler = data;
            Tool.Page page = new Tool.Page();
            data.AddChildren(page);
            page.Url = "http://www.imdb.com/search/name?star_sign=aquarius&sort=starmeter,asc";
         //   page.Url = "http://www.imdb.com/search/name?sort=starmeter,asc&star_sign=aquarius&start=10251";
            page.Comment = "一覧ページ読み込み";

           

            Tool.TagExtraction tag1 = new Tool.TagExtraction();
            tag1.Tag = "table";
            tag1.ParameterFilter = "class=\"results\"";
            page.AddChildren(tag1);
            Tool.TagExtraction tag2 = new Tool.TagExtraction();
            tag2.Tag = "tr";
            tag2.IsMulti = true;
            tag2.ParameterFilter = "detailed";
            tag1.AddChildren(tag2);
            Tool.Link link = new Tool.Link();
            link.UseAbsolutetLink = true;
            link.UrlFilter = "/name/";
            link.VisbleType = Tool.LinkVisbleType.Url;
            tag2.AddChildren(link);

            Tool.Page page1 = new Tool.Page();
            link.AddChildren(page1);
            //飛んだ先のページ

            Tool.TagExtraction tag3 = new Tool.TagExtraction();
            page1.AddChildren(tag3);


            tag3.Tag = "table";
            tag3.ParameterFilter = "id=\"name-overview-widget-layout\"";

            Tool.ClipText clipText1 = new Tool.ClipText();
            tag3.AddChildren(clipText1);
            clipText1.StartClip = "<h1 class=\"header\">";
            clipText1.EndClip = "</h1>";
            Tool.DataWrite write2 = new Tool.DataWrite();
            clipText1.AddChildren(new Tool.DataWrite("Name"));
            clipText1.AddChildren(new Tool.Report("Person:", ""));
            Tool.ClipText clip2 = new Tool.ClipText("<div class=\"infobar\">", "</div>");
            tag3.AddChildren(clip2);

            Tool.Link link2 = new Tool.Link();
            clip2.AddChildren(link2);
            link2.VisbleType = Tool.LinkVisbleType.Label;
            link2.IsMulti = true;
            link2.AddChildren(new Tool.DataWrite("role"));

            Tool.ClipText clip3 = new Tool.ClipText("<div class=\"txt-block\">", "</div>");
            clip3.IsMulti = true;
            tag3.AddChildren(clip3);
            Tool.Contains contains1 = new Tool.Contains("<h4 class=\"inline\">Born:</h4>");
            clip3.AddChildren(contains1);

            Tool.Link link3 = new Tool.Link("/search/name?birth_year", "", false);
            link3.VisbleType = Tool.LinkVisbleType.Label;
            link3.AddChildren(new Tool.DataWrite("born-year"));

            Tool.Link link4 = new Tool.Link("/date/", "", false);
            link4.VisbleType = Tool.LinkVisbleType.Label;
            link4.AddChildren(new Tool.DataWrite("born-date"));

            Tool.Link link5 = new Tool.Link("/search/name?birth_place=", "", false);
            link5.VisbleType = Tool.LinkVisbleType.Label;
            link5.AddChildren(new Tool.DataWrite("born-place"));

            contains1.AddChildren(link3);
            contains1.AddChildren(link4);
            contains1.AddChildren(link5);
            page1.AddChildren(new Tool.NextDataRow());


            
            //Tool.DataWrite write1 = new Tool.DataWrite();
            //link.AddChildren(write1);
            //Tool.NextDataRow nextData = new Tool.NextDataRow();
            //write1.AddChildren(nextData);


            Tool.Link nLink = new Tool.Link();
            nLink.UrlFilter = "search/name?";
            nLink.LabelFilter = "Next";
            nLink.UseAbsolutetLink = true;
            page.AddChildren(nLink);
            Tool.NextPage nextPage = new Tool.NextPage();            
            nLink.AddChildren(nextPage);
           


//            MyLib.ObjectLib.SaveToBinaryFile(data, "data.xml");
           // MyLib.ObjectLib.SaveXML(data, data.GetType(), "data.xml");
            //textBox1.Text = MyLib.IO.TextFileRead("data.xml", Encoding.UTF8);

        }

        DateTime startDate;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            startDate = DateTime.Now;
            CreateRawler();

            rawlerView1.SetRawler(root.Rawler);
         //   root.Rawler.Run();
         //   root.RunWorkerAsync();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            List<Dictionary<string, List<string>>> list = MyLib.ObjectLib.LoadFromBinaryFile("Celebrity1.data") as List<Dictionary<string, List<string>>>;
        }
    }
}
