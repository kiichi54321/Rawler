<<<<<<< HEAD
﻿using System;
using Rawler.Tool;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace RawlerView.WPF
{
    public struct GroupParameterName
    {
        public string Group { get; set; }
        public string ParameterName { get; set; }

        public override int GetHashCode()
        {
            return (Group + ParameterName).GetHashCode();
        }
    }
    public interface IGroupParameterValue
    {
        string Group { get; set; }
        string ParameterName { get; set; }
        string Value { get; set; }
    }

    public static class RawlerRunMange
    {
        public static string BaseConvert(string fileName)
        {
            string xaml = System.IO.File.ReadAllText(GetAppPath() + "/" + fileName);


            return xaml;
        }

        static Dictionary<string, List<IGroupParameterValue>> parameterDic = new Dictionary<string, List<IGroupParameterValue>>();
        static DateTime startDate;
        static bool isBusy = false;
        static CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static void Cancel()
        {
            tokenSource.Cancel();
        }

        public static void AddParameterObjcet(IGroupParameterValue obj)
        {
            if (parameterDic.ContainsKey(obj.Group))
            {
                if (parameterDic[obj.Group].Where(n => n.ParameterName == obj.ParameterName).Any())
                {
                    ReportManage.ErrReport(null, "RawlerRunMangeのAddParameterObjcetですでに登録済みのキーを再登録します。");
                }
                parameterDic[obj.Group].Add(obj);
            }
            else
            {
                parameterDic.Add(obj.Group, new List<IGroupParameterValue>() { obj });
            }
        }

        public static void RunRawler(string fileName, string group)
        {
            string xaml = System.IO.File.ReadAllText(GetAppPath() + "/" + fileName);

            if (parameterDic.ContainsKey("Common"))
            {
                foreach (var item in parameterDic["Common"])
                {
                    xaml = xaml.Replace("@@" + item.ParameterName, item.Value);
                }
            }
            if (parameterDic.ContainsKey(group))
            {
                foreach (var item in parameterDic[group])
                {
                    xaml = xaml.Replace("@@" + item.ParameterName, item.Value);                    
                }
            }

            RunRawler(xaml);

        }

        public static void RunRawler(string xaml)
        {
            if (isBusy)
            {
                MessageBox.Show("実行中です");
                return;
            }
            string err = string.Empty;
            var rawler = Rawler.Tool.RawlerBase.Parse(xaml, out err);
            if (rawler != null)
            {
                try
                {
                    ReportManage.ResetRowCount();
                    //       rowCount = 0;
                    rawler.SetParent();
                    startDate = DateTime.Now;
                    foreach (var item in rawler.GetConectAllRawler())
                    {
                        item.BeginRunEvent += (o, arg) =>
                        {
                            tokenSource.Token.ThrowIfCancellationRequested();

                        };
                    }
                    isBusy = true;
                    Task.Factory.StartNew(() => rawler.Run(), tokenSource.Token).ContinueWith((t) => { StopWatch(); isBusy = false; });
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
            else
            {
                Rawler.Tool.ReportManage.ErrReport(null, err);
            }

        }
        private static void StopWatch()
        {
            if (startDate != null)
            {
                var time = DateTime.Now - startDate;
                ReportManage.Report(null, "経過時間：" + time.ToString(), true, true);
            }
            MessageBox.Show("Complete");
        }
        private static string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
=======
﻿using System;
using Rawler.Tool;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace RawlerView.WPF
{
    public struct GroupParameterName
    {
        public string Group { get; set; }
        public string ParameterName { get; set; }

        public override int GetHashCode()
        {
            return (Group + ParameterName).GetHashCode();
        }
    }
    public interface IGroupParameterValue
    {
        string Group { get; set; }
        string ParameterName { get; set; }
        string Value { get; set; }
    }

    public static class RawlerRunMange
    {
        public static string BaseConvert(string fileName)
        {
            string xaml = System.IO.File.ReadAllText(GetAppPath() + "/" + fileName);


            return xaml;
        }

        static Dictionary<string, List<IGroupParameterValue>> parameterDic = new Dictionary<string, List<IGroupParameterValue>>();
        static DateTime startDate;
        static bool isBusy = false;
        static CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static void Cancel()
        {
            tokenSource.Cancel();
        }

        public static void AddParameterObjcet(IGroupParameterValue obj)
        {
            if (parameterDic.ContainsKey(obj.Group))
            {
                if (parameterDic[obj.Group].Where(n => n.ParameterName == obj.ParameterName).Any())
                {
                    ReportManage.ErrReport(null, "RawlerRunMangeのAddParameterObjcetですでに登録済みのキーを再登録します。");
                }
                parameterDic[obj.Group].Add(obj);
            }
            else
            {
                parameterDic.Add(obj.Group, new List<IGroupParameterValue>() { obj });
            }
        }

        public static void RunRawler(string fileName, string group)
        {
            string xaml = System.IO.File.ReadAllText(GetAppPath() + "/" + fileName);

            if (parameterDic.ContainsKey("Common"))
            {
                foreach (var item in parameterDic["Common"])
                {
                    xaml = xaml.Replace("@@" + item.ParameterName, item.Value);
                }
            }
            if (parameterDic.ContainsKey(group))
            {
                foreach (var item in parameterDic[group])
                {
                    xaml = xaml.Replace("@@" + item.ParameterName, item.Value);                    
                }
            }

            RunRawler(xaml);

        }

        public static void RunRawler(string xaml)
        {
            if (isBusy)
            {
                MessageBox.Show("実行中です");
                return;
            }
            string err = string.Empty;
            var rawler = Rawler.Tool.RawlerBase.Parse(xaml, out err);
            if (rawler != null)
            {
                try
                {
                    ReportManage.ResetRowCount();
                    //       rowCount = 0;
                    rawler.SetParent();
                    startDate = DateTime.Now;
                    foreach (var item in rawler.GetConectAllRawler())
                    {
                        item.BeginRunEvent += (o, arg) =>
                        {
                            tokenSource.Token.ThrowIfCancellationRequested();

                        };
                    }
                    isBusy = true;
                    Task.Factory.StartNew(() => rawler.Run(), tokenSource.Token).ContinueWith((t) => { StopWatch(); isBusy = false; });
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
            else
            {
                Rawler.Tool.ReportManage.ErrReport(null, err);
            }

        }
        private static void StopWatch()
        {
            if (startDate != null)
            {
                var time = DateTime.Now - startDate;
                ReportManage.Report(null, "経過時間：" + time.ToString(), true, true);
            }
            MessageBox.Show("Complete");
        }
        private static string GetAppPath()
        {
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
>>>>>>> 6ddbdbbc3a7813a9636b3cdd2ae14b324c102419
