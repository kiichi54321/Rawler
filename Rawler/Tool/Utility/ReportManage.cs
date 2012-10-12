﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Rawler.Tool
{
    public static class ReportManage
    {
        public static event EventHandler<ReportEvnetArgs> ErrReportEvent;
        public static event EventHandler<ReportEvnetArgs> ReportEvnet;

        private static ObservableCollection<ReportEvnetArgs> reportList = new ObservableCollection<ReportEvnetArgs>();

        public static ObservableCollection<ReportEvnetArgs> ReportList
        {
            get { return ReportManage.reportList; }
        }

        public static void ListClear()
        {
            reportList.Clear();
        }

        private static bool visbleTopObjectComment = false;

        public static bool VisbleTopObjectComment
        {
            get { return visbleTopObjectComment; }
            set { visbleTopObjectComment = value; }
        }

        private static string GetTopComment(RawlerBase sender)
        {
            if (visbleTopObjectComment)
            {
                return sender.GetAncestorRawler().Last().Comment+":";
            }
            else
            {
                return string.Empty;
            }
        }

        private static bool stockReport = true;

        public static bool StockReport
        {
            get { return ReportManage.stockReport; }
            set { ReportManage.stockReport = value; }
        }

        private static bool stockErrReport = true;

        public static bool StockErrReport
        {
            get { return ReportManage.stockErrReport; }
            set { ReportManage.stockErrReport = value; }
        }


        private static void AddReportEventArgs(ReportEvnetArgs args)
        {
            if (args.IsErr)
            {
                if (stockErrReport)
                {
                    reportList.Add(args);
                }
            }
            else
            {
                if (stockReport)
                {
                    reportList.Add(args);
                }
            }
        }


        public static void ErrReport(RawlerBase sender, string err)
        {
            ReportEvnetArgs args = new ReportEvnetArgs(sender,GetTopComment(sender)+ "ERR:"+err,true, true);
            AddReportEventArgs(args);
            if (ErrReportEvent != null)
            {
                ErrReportEvent(sender,args);
            }
        }

        public static void Report(RawlerBase sender, string message,bool returncode)
        {
            ReportEvnetArgs args = new ReportEvnetArgs(sender, GetTopComment(sender) + message, returncode);
            AddReportEventArgs(args);
            if (ReportEvnet != null)
            {
                ReportEvnet(sender, args);
            }
        }

        public static void Report(RawlerBase sender, string message)
        {
            ReportEvnetArgs args = new ReportEvnetArgs(sender, GetTopComment(sender) + message, true);
            AddReportEventArgs(args);
            if (ReportEvnet != null)
            {
                ReportEvnet(sender, args);
            }
        }

        public static void Report(RawlerBase sender, string message,bool returncode,bool visible)
        {
            ReportEvnetArgs args = new ReportEvnetArgs(sender, GetTopComment(sender) + message, returncode);
            AddReportEventArgs(args);
            args.Visible = visible;
            if (ReportEvnet != null)
            {
                ReportEvnet(sender, args);
            }
        }

    }

    public class ReportEvnetArgs : EventArgs
    {
        public string Message { get; set; }
  //      public RawlerBase Sender { get; set; }
        public bool IsErr {get;set;}
        public DateTime DateTime { get; set; }
        public bool Visible { get; set; }
        public bool ReturnCode { get; set; }        
        public ReportEvnetArgs(RawlerBase sender, string message,bool returncode)
            : base()
        {
    //        this.Sender = sender;
            this.Message = message;
            this.IsErr = false;
            this.DateTime = DateTime.Now;
            this.Visible = false;
            this.ReturnCode = returncode;
        }

        public ReportEvnetArgs(RawlerBase sender, string message,bool returncode, bool isErr)
            : base()
        {
     //       this.Sender = sender;
            this.Message = message;
            this.IsErr = isErr;
            this.DateTime = DateTime.Now;
            this.Visible = true;
            this.ReturnCode = returncode;
        }
    }


}