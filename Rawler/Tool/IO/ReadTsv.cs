using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class TsvReadLines : FileReadLines
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<TsvReadLines>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            string filename = GetFileName();
            if (string.IsNullOrEmpty( filename))
            {
                ReportManage.ErrReport(this, "FileNameが空です。");
             //   Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                //if (string.IsNullOrEmpty(ExtendFilter) == false)
                //{
                //    dialog.Filter = FilterStringCreate(ExtendFilter);
                //}
                //else
                //{
                //    dialog.Filter = FilterStringCreate("tsv");
                //}
                //if (dialog.ShowDialog() == true)
                //{
                //    filename = dialog.FileName;
                //}
            }

            try
            {
                if (this.ReadEnd)
                {
                    string t = System.IO.File.ReadAllText(filename);
                    System.IO.StringReader sr = new System.IO.StringReader(t);
                    List<string> list = new List<string>();
                    while (sr.Peek() > -1)
                    {
                        string line = sr.ReadLine();
                        if (line.Length > 0)
                        {
                            list.Add(line);
                        }
                    }
                    sr.Close();
                    base.RunChildrenForArray(runChildren, list);
                }
                else
                {
                    var lines = System.IO.File.ReadLines(filename);
                    var topline = System.IO.File.ReadLines(filename).First();
                    int i = 0;
                    attributeDic = new Dictionary<string, int>();
                    foreach (var item in topline.Split('\t'))
                    {
                        attributeDic.Add(item, i);
                        i++;
                    }
                    lines = lines.Skip(1);
                    if (skip > 0)
                    {
                        lines = lines.Skip(skip);
                    }
                    base.RunChildrenForArray(runChildren, lines);
                }
            }
            catch (Exception ex)
            {
                ReportManage.ErrReport(this, FileName + "を開くのに失敗しました" + ex.Message);
            }


        }

        private Dictionary<string, int> attributeDic = new Dictionary<string, int>();

        public string GetValue(string columnName)
        {
            var data = this.Text.Split('\t');
            if (attributeDic.ContainsKey(columnName))
            {
                if (data.Length >= attributeDic[columnName])
                {
                    try
                    {
                        return data[attributeDic[columnName]];
                    }
                    catch (Exception e)
                    {
                        ReportManage.ErrReport(this, e.Message + " ColumnName:" + columnName + " Text:" + this.Text);
                    }
                }
                else
                {
                    ReportManage.ErrReport(this, "キーが圏外です。:"+columnName);
                }
            }
            else
            {
                ReportManage.ErrReport(this,"該当するキーがありません:"+columnName);

            }
            return string.Empty;
        }


        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }

    /// <summary>
    /// 現在のFileReadLineの値を返します。上流にFileReadLinesがないと機能しません
    /// </summary>
    public class GetTsvValue : RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var file = this.GetAncestorRawler().OfType<TsvReadLines>();
            if (file.Count() > 0)
            {
                string columnName = this.ColumnName;
                if (this.ColumnNameTree != null)
                {
                    columnName = RawlerBase.GetText(GetText(), ColumnNameTree, this);
                }
                if (string.IsNullOrEmpty(columnName) == false)
                {
                    SetText(file.First().GetValue(columnName));
                }
                else
                {
                    ReportManage.ErrReport(this, "ColumnNameが空です。");
                }
            }
            else
            {
                ReportManage.ErrReport(this, "GetTsvValue の上流にTsvReadLinesがありません");
            }
            base.Run(runChildren);
        }

        public string ColumnName { get; set; }
        public RawlerBase ColumnNameTree { get; set; }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetTsvValue>(parent);
        }
    }
}
