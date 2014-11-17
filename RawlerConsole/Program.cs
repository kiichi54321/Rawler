using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;

namespace RawlerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Rawler.Tool.ReportManage.ErrReportEvent += ReportManage_ErrReportEvent;
            Rawler.Tool.ReportManage.ReportEvnet += ReportManage_ReportEvnet;

      //      args = new List<string>() { @"C:\Users\kiichi\Documents\TwitterData\hamano_satoshi\tweet.xaml" }.ToArray();
            if(args.Length>0)
            {
                try
                {
                    foreach (var item in args.Skip(1))
                    {
                        var d = item.Split('=');
                        if (d.Length > 1)
                        {
                            Rawler.Tool.TempVar.SetVar(d[0], d[1]);
                        }
                    }
                    RawlerBase rawler = (RawlerBase)System.Xaml.XamlServices.Load(args[0]);
                    rawler.SetParent();
                    rawler.Run();
                }
                catch(Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }
        }

        static void ReportManage_ReportEvnet(object sender, Rawler.Tool.ReportEvnetArgs e)
        {
            if(e.Visible)
            {
                if(e.ReturnCode)
                {
                    System.Console.WriteLine(e.Message);
                }
                else
                {
                    System.Console.Write(e.Message);
                }

            }
        }

        static void ReportManage_ErrReportEvent(object sender, Rawler.Tool.ReportEvnetArgs e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
}
