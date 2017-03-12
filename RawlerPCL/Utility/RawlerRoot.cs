using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    /// <summary>
    /// Rawlerのroot用のクラス。BackgroundWorker処理をしてくれる。
    /// </summary>
    public class RawlerRoot:RawlerLib.Thread.BackgroundWorker
    {
        public RawlerBase Rawler { get; set; }

        /// <summary>
        /// Rawlerのroot用のクラス。
        /// </summary>
        public RawlerRoot()
            : base()
        {
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
            reportevent = new EventHandler<RawlerLib.Event.EventStringArgs>(r_ReportEvent);
        }
        EventHandler<RawlerLib.Event.EventStringArgs> reportevent;


        void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (Rawler != null)
            {
                CreateReport();
                Rawler.Run();
            }
        }

        /// <summary>
        /// データをゲットする。
        /// </summary>
        /// <returns></returns>
        public ICollection<Data> GetData()
        {
            List<Data> list = new List<Data>();
            Queue<RawlerBase> queueRawler = new Queue<RawlerBase>();
            queueRawler.Enqueue(Rawler);
            while(queueRawler.Count>0)
            {
                RawlerBase tmp = queueRawler.Dequeue();
                if(tmp is Data)
                {
                    list.Add(tmp as Data);
                }
                foreach (var item in tmp.Children)
                {
                    queueRawler.Enqueue(item);
                }

            }
            return list;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public ICollection<RawlerBase> GetAllRalwer()
        //{
        //    List<RawlerBase> list = new List<RawlerBase>();
        //    Queue<RawlerBase> queueRawler = new Queue<RawlerBase>();
        //    queueRawler.Enqueue(Rawler);
        //    while (queueRawler.Count > 0)
        //    {
        //        RawlerBase tmp = queueRawler.Dequeue();
        //        list.Add(tmp as Data);
        //        foreach (var item in tmp.Children)
        //        {
        //            queueRawler.Enqueue(item);
        //        }

        //    }
        //    return list;
        //}

        /// <summary>
        /// 実行する。
        /// </summary>
        public void Run()
        {
            this.RunWorkerAsync();
        }

        /// <summary>
        /// Tsvでデータを保存します。
        /// </summary>
        /// <param name="filename"></param>
        public void SaveTsv(string filename)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false, Encoding.UTF8);
            
            foreach (var item in GetData())
            {
                HashSet<string> hash = new HashSet<string>();
                foreach (var item2 in item.GetDataRows())
                {
                    foreach (var item3 in item2.DataDic.Keys)
                    {
                        hash.Add(item3);
                    }
                }
                foreach (var key in hash)
                {
                    sw.Write(key);
                    sw.Write("\t");
                }
                sw.WriteLine();

                foreach (var row in item.GetDataRows())
                {
                    foreach (var key in hash)
                    {
                        if (row.DataDic.ContainsKey(key))
                        {
                            StringBuilder str = new StringBuilder();
                            bool flag = true;
                            foreach (var item5 in row.DataDic[key])
                            {
                                if (item5 != null)
                                {
                                    str.Append(item5.Replace("\n", "").Replace("\r", "").Replace("\t", "") + ",");
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                str.Length = str.Length - 1;
                            }
                            sw.Write(str.ToString());
                        }
                        sw.Write("\t");
                    }
                    sw.WriteLine();
                }
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine();
            }
            sw.Close();
        }


        public string ToTsv()
        {
            StringBuilder strBuilder = new StringBuilder();

            foreach (var item in GetData())
            {
                HashSet<string> hash = new HashSet<string>();
                foreach (var item2 in item.GetDataRows())
                {
                    foreach (var item3 in item2.DataDic.Keys)
                    {
                        hash.Add(item3);
                    }
                }
                foreach (var key in hash)
                {
                    strBuilder.Append(key);
                    strBuilder.Append("\t");
                }
                strBuilder.AppendLine();

                foreach (var row in item.GetDataRows())
                {
                    foreach (var key in hash)
                    {
                        if (row.DataDic.ContainsKey(key))
                        {
                            StringBuilder str = new StringBuilder();
                            bool flag = true;
                            foreach (var item5 in row.DataDic[key])
                            {
                                if (item5 != null)
                                {
                                    str.Append(item5.Replace("\n", "").Replace("\r", "").Replace("\t", "") + ",");
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                str.Length = str.Length - 1;
                            }
                            strBuilder.Append(str.ToString());
                        }
                        strBuilder.Append("\t");
                    }
                    strBuilder.AppendLine();
                }
                strBuilder.AppendLine();
                strBuilder.AppendLine();
                strBuilder.AppendLine();
            }
            return strBuilder.ToString();
        }

        private void CreateReport()
        {
            ReportManage.ReportEvnet += new EventHandler<ReportEvnetArgs>(ReportManage_ReportEvnet);
            ReportManage.ErrReportEvent += new EventHandler<ReportEvnetArgs>(ReportManage_ErrReportEvent);
            //Queue<RawlerBase> queueRawler = new Queue<RawlerBase>();
            
            //queueRawler.Enqueue(Rawler);
            //while (queueRawler.Count > 0)
            //{
            //    RawlerBase tmp = queueRawler.Dequeue();
            //    if (tmp is Report)
            //    {
            //        Report r = tmp as Report;
            //        r.ReportEvent -= reportevent;
            //        r.ReportEvent += reportevent;
            //    }
            //    foreach (var item in tmp.Children)
            //    {
            //        queueRawler.Enqueue(item);
            //    }
            //}
           
        }

        void ReportManage_ErrReportEvent(object sender, ReportEvnetArgs e)
        {
            this.ReportProgress(0, e);
        }

        void ReportManage_ReportEvnet(object sender, ReportEvnetArgs e)
        {
            this.ReportProgress(0, e);
        }

        void r_ReportEvent(object sender, RawlerLib.Event.EventStringArgs e)
        {
            this.ReportProgress(0, e.Text);
        }

        public void Save(string filename)
        {

        }

        public void Load(string filename)
        {

        }

    }
}
