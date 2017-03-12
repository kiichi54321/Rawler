using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    public class CountData : RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        bool doSave = true;

        public override string Text
        {
            get
            {
                return GetText();
            }
        }

        public bool DoSave
        {
            get { return doSave; }
            set { doSave = value; }
        }

        int minCount = 0;

        public int MinCount
        {
            get { return minCount; }
            set { minCount = value; }
        }

        Dictionary<string, CountDic> dic = new Dictionary<string, CountDic>();


        public void AddCount(string group, string key)
        {
            if (dic.ContainsKey(group))
            {
                dic[group].Add(key);
            }
            else
            {
                var cd = new CountDic();
                cd.Add(key);
                dic.Add(group, cd);
            }
        }

        public void AddCount(string group, string key,int cnt)
        {
            if (dic.ContainsKey(group))
            {
                dic[group].Add(key,cnt);
            }
            else
            {
                var cd = new CountDic();
                cd.Add(key,cnt);
                dic.Add(group, cd);
            }
        }

        bool reportData = false;

        public bool ReportData
        {
            get { return reportData; }
            set { reportData = value; }
        }

        private int takeCount = -1;

        public int TakeCount
        {
            get { return takeCount; }
            set { takeCount = value; }
        }

        public void Write(string fileName)
        {
            using (var file = System.IO.File.AppendText(fileName))
            {
              //  file.WriteLine("Group\tKey\tCount");
                foreach (var item in dic)
                {
                    if (takeCount < 0)
                    {
                        foreach (var item2 in item.Value.Dic.OrderByDescending(n => n.Value))
                        {
                            if (item2.Value >= minCount)
                            {
                                file.WriteLine(item.Key + "\t" + item2.Key + "\t" + item2.Value);
                            }
                        }
                    }
                    if (takeCount > 0)
                    {
                        foreach (var item2 in item.Value.Dic.OrderByDescending(n => n.Value).Take(takeCount))
                        {
                            if (item2.Value >= minCount)
                            {
                                file.WriteLine(item.Key + "\t" + item2.Key + "\t" + item2.Value);
                            }
                        }

                    }
                }
            }
        }

        public void WriteSaveClear()
        {
            using (var file = System.IO.File.AppendText(filename))
            {
                //  file.WriteLine("Group\tKey\tCount");
                foreach (var item in dic)
                {
                    if (takeCount < 0)
                    {
                        foreach (var item2 in item.Value.Dic.OrderByDescending(n => n.Value))
                        {
                            if (item2.Value >= minCount)
                            {
                                file.WriteLine(item.Key + "\t" + item2.Key + "\t" + item2.Value);
                            }
                        }
                    }
                    if (takeCount > 0)
                    {
                        foreach (var item2 in item.Value.Dic.OrderByDescending(n => n.Value).Take(takeCount))
                        {
                            if (item2.Value >= minCount)
                            {
                                file.WriteLine(item.Key + "\t" + item2.Key + "\t" + item2.Value);
                            }
                        }

                    }
                }
            }
            dic.Clear();
        }

        public string ToTsv()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dic)
            {
                foreach (var item2 in item.Value.Dic.OrderByDescending(n => n.Value))
                {
                    sb.AppendLine(item.Key + "\t" + item2.Key + "\t" + item2.Value);
                }
            }
            return sb.ToString();
        }

        public string FileName { get; set; }

        /// <summary>
        /// ダイアログでの拡張子フィルターの指定
        /// </summary>
        public string ExtendFilter { get; set; }
        string filename = string.Empty;

        public override void Run(bool runChildren)
        {
            if (doSave)
            {
                if (FileName != null)
                {
                    filename = FileName;
                }
                else
                {
                    if (string.IsNullOrEmpty(filename))
                    {
                        ReportManage.ErrReport(this, "FileNameが空です。");
                        return;
                    }

                    //Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
                    //saveDialog.Title = "保存ファイルの指定　CountData:" + this.Comment;
                    //if (string.IsNullOrEmpty(ExtendFilter) == false)
                    //{
                    //    saveDialog.Filter = RawlerLib.Io.FilterStringCreate(ExtendFilter);
                    //}
                    //if (saveDialog.ShowDialog() == true)
                    //{
                    //    filename = saveDialog.FileName;
                    //}
                    //using (var file = System.IO.File.CreateText(filename))
                    //{
                    //    file.WriteLine("Group\tKey\tCount");
                    //}
                }
            }
            base.Run(runChildren);
            if (doSave || FileName != null)
            {
                try
                {
                    Write(filename);
                }
                catch (Exception e)
                {
                    ReportManage.ErrReport(this, "CountDataでファイルの書き込みに失敗しました。" + e.Message);
                }
            }
            if (reportData)
            {
                ReportManage.Report(this, this.ToTsv(), true, true);
            }
        }
    }

    public class CountAdd : RawlerBase
    {
        public string GroupName { get; set; }
        public RawlerBase GroupTree { get; set; }
        public RawlerBase AddNumTree { get; set; }

        public override void Run(bool runChildren)
        {
            string group = string.Empty;
            if (GroupTree != null && this.Parent != null)
            {
                GroupTree.SetParent();
                group = RawlerBase.GetText(this.Parent.Text, GroupTree, this);
            }
            else
            {
                if (GroupName != null)
                {
                    group = GroupName;
                }
            }
            int num = 1;
            if (AddNumTree != null && this.Parent != null)
            {
                int.TryParse(RawlerBase.GetText(this.Parent.Text, AddNumTree, this), out num);
            }

            var c = this.GetAncestorRawler().Where(n => n is CountData);
            if (c.Count() > 0)
            {
                ((CountData)c.First()).AddCount(group, GetText(),num);
            }
            else
            {
                ReportManage.ErrReport(this, "上流にCountDataがありません");
            }
            base.Run(runChildren);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return this.Clone<CountAdd>(parent);
        }
    }

    public class CountSave : RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler().OfType<CountData>();
            if (list.Any())
            {
                list.First().WriteSaveClear();
            }
            else
            {
                ReportManage.ErrReport(this, "上流にCountDataがありません");
            }
            base.Run(runChildren);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return this.Clone<CountSave>(parent);
        }

        public override string Text
        {
            get
            {
                return GetText();
            }
        }
    }


    public class CountDic
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();

        public Dictionary<string, int> Dic
        {
            get { return dic; }
            set { dic = value; }
        }

        public void Add(string key, int count)
        {
            if (key.Length > 0)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key] = dic[key] + count;
                }
                else
                {
                    dic.Add(key, count);
                }
            }
        }

        public void Add(string key)
        {
            Add(key, 1);
        }
    }
}
