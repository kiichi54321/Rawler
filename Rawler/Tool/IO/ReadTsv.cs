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
                    columnNameDic = new Dictionary<string, int>();
                    foreach (var item in topline.Split('\t'))
                    {
                        columnNameDic.Add(item, i);
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

        private Dictionary<string, int> columnNameDic = new Dictionary<string, int>();

        public Dictionary<string, int> ColumnNameDic
        {
            get { return columnNameDic; }
            set { columnNameDic = value; }
        }

         

        public string GetValue(string columnName)
        {
            var data = this.Text.Split('\t');
            if (columnNameDic.ContainsKey(columnName))
            {
                if (data.Length >= columnNameDic[columnName])
                {
                    try
                    {
                        return data[columnNameDic[columnName]];
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
        public string Default{get;set;}
        public override void Run(bool runChildren)
        {
            var file = this.GetUpperRawler<TsvReadLines>();
            if (file !=null)
            {
                string columnName = this.ColumnName;
                if (this.ColumnNameTree != null)
                {
                    columnName = RawlerBase.GetText(GetText(), ColumnNameTree, this);
                }
                if (string.IsNullOrEmpty(columnName) == false)
                {
                    if (file.ColumnNameDic.ContainsKey(columnName))
                    {
                        var i = file.ColumnNameDic[columnName];
                        var d = GetText().Split('\t');
                        if (d.Length > i)
                        {
                            SetText(d[i]);
                        }
                        else
                        {
                            SetText(Default);
                        }
                    }
                    else
                    {
                        ReportManage.ErrReport(this, "該当するキーがありません:" + columnName);
                    }
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
