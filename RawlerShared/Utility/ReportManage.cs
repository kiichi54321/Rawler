﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Rawler.Core;
using System.Threading.Tasks;

namespace Rawler
{
    public class ReportStock:RawlerBase
    {
        ObservableCollection<ReportEvnetArgs> reportList = new ObservableCollection<ReportEvnetArgs>();
        public ObservableCollection<ReportEvnetArgs> ReportList { get { return reportList; } }
        string fileName = "ErrReport.txt";
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        bool isStock = true;

        public bool IsStock
        {
            get { return isStock; }
            set { isStock = value; }
        }

        //private bool doSave = false;

        //public bool DoSave
        //{
        //    get { return doSave; }
        //    set { doSave = value; }
        //}


        public RawlerBase ErrReportTree { get; set; }

        public async Task AddReport(ReportEvnetArgs ea)
        {
            if (ea.IsErr && ErrReportTree != null)
            {
                await Task.Run(() => RawlerBase.GetText(ea.DateTime.ToFileTimeUtc() + "\t" + ea.Message, ErrReportTree, this));
            }
            if (isStock && ea.IsErr)
            {
                reportList.Add(ea);
            }
        }
    }
    /// <summary>
    /// Reportを管理するクラス
    /// </summary>
    public static class ReportManage
    {
        /// <summary>
        /// Errが起きた時のEventです。
        /// </summary>
        public static event EventHandler<ReportEvnetArgs> ErrReportEvent;
        /// <summary>
        /// Reportが起きた時のEventです。
        /// </summary>
        public static event EventHandler<ReportEvnetArgs> ReportEvnet;
        /// <summary>
        /// Rowが変更が起きた時のイベント
        /// </summary>
        public static event EventHandler<EventArgs> ChangeRowCount;
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

        /// <summary>
        /// Topレベルのコメントを参照する。
        /// </summary>
        public static bool VisbleTopObjectComment
        {
            get { return visbleTopObjectComment; }
            set { visbleTopObjectComment = value; }
        }

        private static string GetTopComment(RawlerBase sender)
        {
            if (visbleTopObjectComment)
            {
                return sender.GetAncestorRawler().Last().Comment + ":";
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


        private static void AddReportEventArgs(RawlerBase sender, ReportEvnetArgs args)
        {
            if (sender != null)
            {
                var reportStock = sender.GetUpperRawler<ReportStock>();
                if (reportStock != null)
                {
                    reportStock.AddReport(args);
                    return;
                }
            }

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

        /// <summary>
        /// Errをレポートする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="err"></param>
        public static void ErrReport(RawlerBase sender, string err)
        {
            ReportEvnetArgs args;
            if (sender != null)
            {
                args = new ReportEvnetArgs(sender, GetTopComment(sender) + "ERR:" + sender.GetType().Name + ":" + err, true, true);
            }
            else
            {
                args = new ReportEvnetArgs(sender, GetTopComment(sender) + "ERR:" + err, true, true);
            }
            AddReportEventArgs(sender, args);
            ErrReportEvent?.Invoke(sender, args);
        }

        /// <summary>
        /// 上流に指定の型が見つからない時のエラー
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawler"></param>
        public static void ErrUpperNotFound<T>(RawlerBase rawler)            
        {
            var t = typeof(T);
            ErrReport(rawler,"上流に「"+ t.Name+"」が見つかりません");
        }

        /// <summary>
        /// プロパティが空の時のエラー
        /// </summary>
        /// <param name="rawler"></param>
        /// <param name="propertyName"></param>
        public static void ErrEmptyPropertyName(RawlerBase rawler, string propertyName)
        {
            ErrReport(rawler, "プロパティ「"+propertyName+"」の値が空です。");
        }

        /// <summary>
        /// ReportProgress表示ようのレポート
        /// </summary>
        /// <param name="sender"></param>
        public static void ReportProgress(RawlerBase sender)
        {
            Report(sender, "NextDataRow");
        }


        public static void Report(RawlerBase sender, string message, bool returncode)
        {
            ReportEvnetArgs args = new ReportEvnetArgs(sender, GetTopComment(sender) + message, returncode);
            AddReportEventArgs(sender, args);
            if (message.Contains("NextDataRow"))
            {
                RowCount++;
            }
            else
            {
                ReportEvnet?.Invoke(sender, args);
            }
        }

        public static void Report(RawlerBase sender, string message)
        {
            ReportEvnetArgs args = new ReportEvnetArgs(sender, GetTopComment(sender) + message, true);
            AddReportEventArgs(sender,args);

            if (message.Contains("NextDataRow"))
            {
                RowCount++;
            }
            else
            {
                ReportEvnet?.Invoke(sender, args);
            }
        }

        public static void Report(RawlerBase sender, string message, bool returncode, bool visible)
        {
            ReportEvnetArgs args = new ReportEvnetArgs(sender, GetTopComment(sender) + message, returncode);
            AddReportEventArgs(sender,args);
            args.Visible = visible;
            if (message.Contains("NextDataRow"))
            {
                RowCount++;
            }
            else
            {
                ReportEvnet?.Invoke(sender, args);
            }
        }

        static int rowCount = 0;

        public static int RowCount
        {
            get { return ReportManage.rowCount; }
            set
            {
                if (rowCount != value)
                {
                    ReportManage.rowCount = value;

                    if (ChangeRowCount != null)
                    {
                        ChangeRowCount(null, new EventArgs());
                    }
                }
            }
        }

        public static void ResetRowCount()
        {
            RowCount = 0;
        }

    }

    public class ReportEvnetArgs : EventArgs
    {
        public string Message { get; set; }
        //      public RawlerBase Sender { get; set; }
        public bool IsErr { get; set; }
        public DateTime DateTime { get; set; }
        public bool Visible { get; set; }
        public bool ReturnCode { get; set; }
        public ReportEvnetArgs()
            :base()
        { }

        public ReportEvnetArgs(RawlerBase sender, string message, bool returncode)
            : base()
        {
            //        this.Sender = sender;
            this.Message = message;
            this.IsErr = false;
            this.DateTime = DateTime.Now;
            this.Visible = false;
            this.ReturnCode = returncode;
        }

        public ReportEvnetArgs(RawlerBase sender, string message, bool returncode, bool isErr)
            : base()
        {
            //       this.Sender = sender;
            this.Message = message;
            this.IsErr = isErr;
            this.DateTime = DateTime.Now;
            this.Visible = true;
            this.ReturnCode = returncode;
        }

        public string ToText()
        {
            var text = this.DateTime.ToLocalTime() + " " + Message;
            if (ReturnCode) return text + "\n";
            return text;
        }
    }


}
