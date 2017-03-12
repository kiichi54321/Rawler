using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawlerLib.MyExtend;
using Rawler.Core;
#if WINDOWS_UWP
using Windows.UI.Xaml.Markup;
#elif NETFX_CORE
using Portable.Xaml.Markup;
#endif
using Portable.Xaml;

namespace Rawler
{
    /// <summary>
    /// データを蓄積するRawlerクラス。
    /// </summary>
    public class Data : RawlerBase, IData, IDataRows
    {
        /// <summary>
        /// データを蓄積するRawlerクラス。
        /// </summary>
        public Data()
            : base()
        {
        //    dataList.Add(currentDataRow);

        }

        private bool endReport = true;

        public bool EndReport
        {
            get { return endReport; }
            set { endReport = value; }
        }



        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// このオブジェクトのテキスト。ただ、親のテキストをそのまま流す。
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }
        }

        bool endDataClear = false;

        /// <summary>
        /// 終了時データを削除する。
        /// </summary>
        public bool EndDataClear
        {
            get { return endDataClear; }
            set { endDataClear = value; }
        }

        public IFileBase File { get; set; }

        List<DataRowObject> dataList = new List<DataRowObject>();
        Dictionary<string, DataRowObject> dataDic = new Dictionary<string, DataRowObject>();
        FileSaveMode fileSaveMode = FileSaveMode.Completed;


        /// <summary>
        /// 蓄積されたDataRowのリストを取得します。NullのRowは含まれません。
        /// </summary>
        /// <returns></returns>
        public List<DataRowObject> GetDataRows()
        {
            List<DataRowObject> list = new List<DataRowObject>();

            foreach (var item in dataList.ToArray())
            {
                if (item.IsDataNull() == false)
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public void ChangeCurrentDataRow(string key)
        {
            if (dataDic.ContainsKey(key))
            {
                currentDataRow = dataDic[key];
            }
            else
            {
                currentDataRow = new DataRowObject();
                currentDataRow.AddData("Key", key);
                dataDic.Add(key, currentDataRow);
                dataList.Add(currentDataRow);
            }
        }



        DataRowObject currentDataRow = new DataRowObject();
        /// <summary>
        /// 現在のDataRowに書く。DataWriteがよびだすもの。
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void DataWrite(string attribute, string value, DataWriteType type, DataAttributeType attributeType)
        {
            if (type == DataWriteType.add)
            {
                currentDataRow.AddData(attribute, value, attributeType);
            }
            else if (type == DataWriteType.replace)
            {
                currentDataRow.ReplaceData(attribute, value, attributeType);
            }

            if (attribute.ToLower() == "key")
            {
                dataDic.Add(value, currentDataRow);
            }

        }
        public void DataWrite(string attribute, string value, DataWriteType type)
        {
            DataWrite(attribute, value, type, DataAttributeType.Text);
        }

        /// <summary>
        /// DataWriteの共通呼び出し
        /// </summary>
        /// <param name="rawler"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        public static void DataWrite(RawlerBase rawler, string attribute, string value, DataWriteType type, DataAttributeType attributeType)
        {
            var d = (IData)rawler.GetUpperInterface<IData>();
            if (d != null)
            {
                d.DataWrite(attribute, value, type, attributeType);
            }
            else
            {
                ReportManage.ErrUpperNotFound<IData>(rawler);
            }
        }

        bool errReportNullData = true;
        /// <summary>
        /// NextDataRow時にNullDataの時、エラーを報告する。
        /// </summary>
        public bool ErrReportNullData
        {
            get { return errReportNullData; }
            set { errReportNullData = value; }
        }


        /// <summary>
        /// AddDataRowの共通呼び出し。
        /// </summary>
        /// <param name="rawler"></param>
        /// <param name="datarow"></param>
        public static void AddDataRow(RawlerBase rawler, DataRowObject datarow)
        {
            var r = (IDataRows)rawler.GetUpperInterface<IDataRows>();
            if (r != null) r.AddDataRow(datarow);
            else ReportManage.ErrUpperNotFound<IDataRows>(rawler);
        }


        public virtual void AddDataRow(DataRowObject datarow)
        {
            currentDataRow.UpDate(datarow);
            NextDataRow();
        }

        /// <summary>
        /// 次のDataRowに行く。NextDataRowがよびだすもの。
        /// </summary>
        public void NextDataRow()
        {
            if (currentDataRow.IsDataNull())
            {
                if (ErrReportNullData)
                {
                    ReportManage.ErrReport(this, "DataがNullのまま、NextDataRowがよびだされました");
                }
                if (DataNullEvent != null)
                {
                    DataNullEvent(this, new EventArgs());
                }
            }
            else
            {
                NextDataRow(currentDataRow);
            }
        }

        public void NextDataRow(DataRowObject cDataRow)
        {
            ReportManage.Report(this, "NextDataRow");
            if (Commited != null)
            {
                Commited(this, new EventDataRow(cDataRow));
            }
            if(FileSaveMode == FileSaveMode.Sequential)
            {
                File?.SequentialWriteLine(CreateSaveOneLine( cDataRow,SaveFileType));
            }
            else
            {
                dataList.Add(cDataRow);
            }
            currentDataRow = new DataRowObject();

        }

        public override void Run(bool runChildren)
        {
            FileInit();
            base.Run(runChildren);
            if (fileSaveMode == FileSaveMode.Completed)
            {
                if (File != null)
                {
                    File.WriteLines(CreateSaveLines(SaveFileType));
                }
            }
            if(FileSaveMode == FileSaveMode.Sequential)
            {
                File.Close();
            }

            if (EndDataClear)
            {
                dataDic.Clear();
                dataList.Clear();
                currentDataRow = new DataRowObject();
                dataList.Add(currentDataRow);
            }
        }
        private List<string> order = new List<string>();
        public string AttributeOrderString { get; set; }

        private void FileInit()
        {
            if (FileSaveMode == FileSaveMode.Sequential)
            {
                File?.InitFile();
                if(SaveFileType == FileType.Tsv)
                {
                    if (AttributeOrderString != null)
                    {
                        order = new List<string>(AttributeOrderString.Split(','));
                    }
                    else
                    {
                        order = CreateOrderString();
                    }
                    File?.SequentialWriteLine(order.JoinText("\t"));
                }
            }
        }

     //   private bool sortAttribute = true;

        FileType saveFileType = FileType.Tsv;

        public FileType SaveFileType { get { return saveFileType; } set { saveFileType = value; } }


        /// <summary>
        /// 保存用のLineを出力する。
        /// </summary>
        /// <param name="ftype"></param>
        /// <returns></returns>
        public IEnumerable<string> CreateSaveLines(FileType ftype)
        {
            if(ftype == FileType.Json)
            {
                return new string[] { Newtonsoft.Json.Linq.JObject.FromObject(this.GetDataRows().Select(n => n.DataDic)).ToString() };
            }
            if(ftype == FileType.LTsv)
            {
                return ToLTSV();
            }
            if(ftype == FileType.Tsv)
            {
                return ToTsvLine();
            }
            return new string[] { };
        }


        public string CreateSaveOneLine(DataRowObject row,FileType ftype)
        {
            string line = string.Empty;
            switch (ftype)
            {
                case FileType.Tsv:
                    line = CreateTsvLine(row);
                    break;
                case FileType.Json:
                    line = Newtonsoft.Json.Linq.JObject.FromObject(row).ToString(Newtonsoft.Json.Formatting.None);
                    break;
                case FileType.LTsv:
                    line = row.ToLtsv();
                    break;
                default:
                    break;
            }
            return line;
        }

        private string CreateTsvLine(DataRowObject row)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item2 in order)
            {
                if (row.DataDic.ContainsKey(item2))
                {
                    var item = row.DataDic[item2];
                    if (item.First() != null)
                    {
                        var line = item.Select(n => n.ReadLines(true).Select(m => m.Replace("\t", " ").Trim()).JoinText(" ")).JoinText(",");
                        sb.Append(line);
                    }
                }
                sb.Append("\t");
            }
            var kList = row.DataDic.Keys.Except(order);
            if(kList.Any())
            {
                foreach (var item in kList)
                {
                    var r = row.DataDic[item].Select(n => n.ReadLines(true).Select(m => m.Replace("\t", " ").Trim()).JoinText(" ")).JoinText(",");
                    if (item.Length>0 & r.Length>0)
                    {
                        sb.Append(item + ":"+r);
                    }
                    else
                    {
                        sb.Append(r);
                    }
                    sb.Append("\t");

                }
            }
            return sb.ToString();
        }


        ///// <summary>
        ///// ファイルで保存する
        ///// </summary>
        //protected async void SaveFile()
        //{
        //    string filename = this.FileName.Convert(this);
        //    if (this.FileNameTree != null)
        //    {
        //        filename = RawlerBase.GetText(this.GetText(), this.FileNameTree, this);
        //    }
        //    if (string.IsNullOrEmpty(filename) == false)
        //    {

        //        var f = await IO.IoState.GetFileAsync(filename, FileSaveMode);
        //        using (var sw = new System.IO.StreamWriter(await f.OpenAsync(PCLStorage.FileAccess.ReadAndWrite)))
        //        {

        //            if (saveFileType == FileType.Json)
        //            {
        //                sw.WriteLine(Newtonsoft.Json.Linq.JObject.FromObject(this.GetDataRows().Select(n => n.DataDic)));
        //            }
        //            else if (saveFileType == FileType.Tsv)
        //            {
        //                sw.WriteLine(this.ToTsv());
        //            }
        //            else if (saveFileType == FileType.LTsv)
        //            {
        //                foreach (var item in this.GetDataRows())
        //                {
        //                    var d = item.DataDic.ToDictionary(n => n.Key, n => n.Value.JoinText(","));
        //                    sw.WriteLine(d.ToLtsvLine());
        //                }
        //            }

        //        }
        //        ReportManage.Report(this, filename + "作成完了", true, EndReport);
        //    }
        //}

        //public async void JsonFileSave()
        //{
        //    string filename = this.FileName.Convert(this);
        //    if (this.FileNameTree != null)
        //    {
        //        filename = RawlerBase.GetText(this.GetText(), this.FileNameTree, this);
        //    }
        //    if (string.IsNullOrEmpty(filename) == false)
        //    {
        //        var f = await IO.IoState.GetFileAsync(filename, FileSaveMode);
        //        using (var sw = new System.IO.StreamWriter(await f.OpenAsync(PCLStorage.FileAccess.ReadAndWrite)))
        //        {

        //            if (saveFileType == FileType.Json)
        //            {
        //                sw.WriteLine(Newtonsoft.Json.Linq.JObject.FromObject(this.GetDataRows().Select(n => n.DataDic)));
        //            }
        //        }
        //        ReportManage.Report(this, filename + "作成完了", true, EndReport);
        //    }
        //}

        //private async void TsvFileSave()
        //{
        //    if (doLastFileSave == false) return;
        //    string filename = this.FileName.Convert(this);
        //    if (this.FileNameTree != null)
        //    {
        //        filename = RawlerBase.GetText(this.GetText(), this.FileNameTree, this);
        //    }
        //    if (string.IsNullOrEmpty(filename) == false)
        //    {
        //        var f = await IO.IoState.GetFileAsync(filename, FileSaveMode);
        //        using (var sw = new System.IO.StreamWriter(await f.OpenAsync(PCLStorage.FileAccess.ReadAndWrite)))
        //        {
        //            sw.Write(ToTsv());
        //        }

        //        ReportManage.Report(this, filename + "作成完了", true, EndReport);
        //    }


        //}

        private List<string> CreateOrderString()
        {
            Queue<RawlerBase> stack = new Queue<RawlerBase>();
            List<string> list = new List<string>();
            foreach (var item in this.Children)
            {
                stack.Enqueue(item);
            }


            while (stack.Count > 0)
            {
                var rawler = stack.Dequeue();
                if ((rawler is Data) == false)
                {
                    if (rawler is IDataWrite)
                    {
                        var dw = rawler as IDataWrite;
                        if (string.IsNullOrEmpty(dw.Attribute) == false)
                        {
                            list.Add(dw.Attribute);
                        }
                    }
                    foreach (var item in rawler.Children)
                    {
                        stack.Enqueue(item);
                    }
                }
            }
            return new List<string>(list.Distinct());
        }

        public void ClearRows()
        {
            dataDic.Clear();
            dataList.Clear();
        }

        public TableData CreateTable()
        {
            TableData table = new TableData();
            var hash = GetDataRows().SelectMany(n => n.DataDic).GroupBy(n => n.Key).Select(n => new { n.Key, Value = n.Max(m => m.Value.Count) }).ToDictionary(n => n.Key, n => n.Value);

            //            table.Head = hash.ToList();
            List<List<CellData>> list = new List<List<CellData>>();
            list = GetDataRows().Select(n => n.GetCell(hash).ToList()).ToList();

            var dic = list.SelectMany(n => n).GroupBy(n => n.Key)
                .Where(n => n.Select(m => m.DataText).Distinct().Count() > 1 || n.First().DataType == DataAttributeType.SourceUrl)
                .Select(n => new { Key = n.Key, Value = n.Select(m => m.DataText).JoinText("\t") })
                .ToDictionary(n => n.Key, n => n.Value);
            var keyList = dic.GroupBy(n => n.Value).Select(n => n.First().Key);
            var k = keyList;
            table.Head = k.ToList();
            var h = new HashSet<string>(k);
            foreach (var item in list)
            {
                foreach (var item2 in item.Where(n => h.Contains(n.Key) == false).ToArray())
                {
                    item.Remove(item2);
                }
            }
            table.Rows = list;
            return table;
        }

        /// <summary>
        /// Json形式で返す。(デフォでは、改行なし)
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return ToJson(Newtonsoft.Json.Formatting.None);
        }
        /// <summary>
        /// Json形式で返す。
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToJson(Newtonsoft.Json.Formatting format)
        {
            var list = this.GetDataRows().Select(n => n.DataDic.ToDictionary(m => m.Key, m => m.Value.JoinText(","))).ToList();
            return Newtonsoft.Json.JsonConvert.SerializeObject(list, format);
        }

        /// <summary>
        /// 中身のデータをLTSVにする。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ToLTSV()
        {
            foreach (var item in this.GetDataRows())
            {
                var d = item.DataDic.ToDictionary(n => n.Key, n => n.Value.JoinText(","));
                yield return d.ToLtsvLine();
            }
        }

        /// <summary>
        /// 中身のデータをTSVにする。
        /// </summary>
        /// <returns></returns>
        public string ToTsv()
        {
            StringBuilder strBuilder = new StringBuilder();
            var item = this;
            {
                var hash = item.GetDataRows().SelectMany(n => n.DataDic).GroupBy(n => n.Key).Select(n => new { n.Key, Value = n.Max(m => m.Value.Count) }).ToDictionary(n => n.Key, n => n.Value);

                foreach (var key in hash)
                {
                    if (key.Value == 1)
                    {
                        strBuilder.Append(key.Key);
                        strBuilder.Append("\t");
                    }
                    else
                    {
                        for (int i = 0; i < key.Value; i++)
                        {
                            strBuilder.Append(key.Key);
                            strBuilder.Append("_" + (i + 1));
                            strBuilder.Append("\t");
                        }
                    }
                }
                strBuilder.AppendLine();

                foreach (var row in item.GetDataRows())
                {
                    foreach (var key in hash)
                    {
                        if (row.DataDic.ContainsKey(key.Key))
                        {
                            StringBuilder str = new StringBuilder();
                            bool flag = true;
                            var list = row.DataDic[key.Key];
                            for (int i = 0; i < key.Value; i++)
                            {
                                if (list.Count > i)
                                {
                                    str.Append(list[i]?.Replace("\n", "").Replace("\r", "").Replace("\t", "") + "\t");
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            //foreach (var item5 in row.DataDic[key.Key])
                            //{
                            //    if (item5 != null)
                            //    {
                            //        str.Append(item5.Replace("\n", "").Replace("\r", "").Replace("\t", "") + "\t");
                            //    }
                            //}
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

            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 中身のデータをTSVにする。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ToTsvLine()
        {
            if (this.dataList.Count == 0)
            {
                yield break;
            }

            StringBuilder strBuilder = new StringBuilder();
            var item = this;
            {
                var hash = item.GetDataRows().SelectMany(n => n.DataDic).GroupBy(n => n.Key).Select(n => new { n.Key, Value = n.Max(m => m.Value.Count) }).ToDictionary(n => n.Key, n => n.Value);

                foreach (var key in hash)
                {
                    if (key.Value == 1)
                    {
                        strBuilder.Append(key.Key);
                        strBuilder.Append("\t");
                    }
                    else
                    {
                        for (int i = 0; i < key.Value; i++)
                        {
                            strBuilder.Append(key.Key);
                            strBuilder.Append("_" + (i + 1));
                            strBuilder.Append("\t");
                        }
                    }
                }
                yield return strBuilder.ToString();

                foreach (var row in item.GetDataRows())
                {
                    strBuilder.Clear();
                    foreach (var key in hash)
                    {
                        if (row.DataDic.ContainsKey(key.Key))
                        {
                            StringBuilder str = new StringBuilder();
                            bool flag = true;
                            var list = row.DataDic[key.Key];
                            for (int i = 0; i < key.Value; i++)
                            {
                                if (list.Count > i)
                                {
                                    str.Append(list[i]?.Replace("\n", "").Replace("\r", "").Replace("\t", "") + "\t");
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
                    yield return strBuilder.ToString();
                }
            }
        }


        /// <summary>
        ///　すべての行数
        /// </summary>
        public int TotalRows
        {
            get
            {
                return GetDataRows().Count;
            }
        }
        /// <summary>
        /// すべてのカラム数
        /// </summary>
        public int TotalColumns
        {
            get
            {
                HashSet<string> hash = new HashSet<string>();
                foreach (var item2 in GetDataRows())
                {
                    foreach (var item3 in item2.DataDic.Keys)
                    {
                        hash.Add(item3);
                    }
                }
                return hash.Count;
            }
        }

        /// <summary>
        /// 平均のカラム数
        /// </summary>
        public double AvgColumns
        {
            get
            {
                return GetDataRows().Select(n => n.DataDic.SelectMany(m => m.Value).Count()).Average();
            }
        }

        private bool stock = true;

        /// <summary>
        /// データを蓄積する。false にするとメモリーに貯めこまない。
        /// </summary>
        public bool Stock
        {
            get { return stock; }
            set { stock = value; }
        }

        public FileSaveMode FileSaveMode
        {
            get
            {
                return fileSaveMode;
            }

            set
            {
                fileSaveMode = value;
            }
        }

        /// <summary>
        /// DataRowがNullの時のイベント
        /// </summary>
        public event EventHandler DataNullEvent;

        /// <summary>
        /// currentDataRowはNullかどうか
        /// </summary>
        /// <returns></returns>
        public bool GetCurrentDataNull()
        {
            return currentDataRow.IsDataNull();
        }

        /// <summary>
        /// CurrentDataRowを取得する。
        /// </summary>
        /// <returns></returns>
        public DataRowObject GetCurrentDataRow()
        {
            return currentDataRow;
        }


        ///// <summary>
        ///// 再ログイン機能を使う
        ///// </summary>
        //public bool UseReLogin
        //{
        //    get { return useReLogin; }
        //    set { useReLogin = value; }
        //}
        /// <summary>
        /// 汎用性のある　List　形式に変換する。
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, List<string>>> ToList()
        {
            List<Dictionary<string, List<string>>> list = new List<Dictionary<string, List<string>>>();

            foreach (var item in dataList)
            {
                list.Add(item.DataDic);
            }
            return list;
        }
   
        /// <summary>
        /// NextDataRow　が呼び出されたときに発生するイベント。
        /// </summary>
        public event EventHandler<EventDataRow> Commited;

        /// <summary>
        /// DataRowに関するEventArgs
        /// </summary>
        public class EventDataRow : EventArgs
        {
            /// <summary>
            /// DataRowに関するEventArgs         /// 
            /// </summary>
            public EventDataRow()
                : base()
            {

            }
            /// <summary>
            /// DataRowに関するEventArgs
            /// /// </summary>
            /// <param name="row"></param>
            public EventDataRow(DataRowObject row)
                : base()
            {
                DataRow = row;
            }
            /// <summary>
            /// イベント対象のDataRow
            /// </summary>
            public DataRowObject DataRow { get; set; }
        }


    }
    public class TableData
    {
        public List<string> Head { get; set; }
        public List<List<CellData>> Rows { get; set; }

        public string ToXAML()
        {
            try
            {
                return XamlServices.Save(this);
            }
            catch
            {

            }
            //必殺エラー握りつぶし
            return XamlServices.Save(new TableData() { Head = new List<string>() { "Err" }, Rows = new List<List<CellData>>() });
        }

        public IEnumerable<List<CellData>> GetSkipRows()
        {
            foreach (var item in Rows)
            {
                if (item.Where(n => n.DataText.Length > 0).Count() > 1)
                {
                    yield return item;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Head)
            {
                sb.Append(item + "\t");
            }
            sb.AppendLine();
            foreach (var item in Rows)
            {
                foreach (var m in item)
                {
                    sb.Append(m.DataText.Replace("\n", string.Empty) + "\t");
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Dataの中身をJson文字列で受け取る
    /// </summary>
    public class DataToJson:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var d = this.GetUpperRawler<Data>();
            if(d == null)
            {
                ReportManage.ErrUpperNotFound<Data>(this);
            }
            else
            {
                SetText(d.ToJson());
            }
            base.Run(runChildren);
        }
    }

    /// <summary>
    /// Dataの中身をJson文字列で受け取る
    /// </summary>
    public class DataClear : RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var d = this.GetUpperRawler<Data>();
            if (d == null)
            {
                ReportManage.ErrUpperNotFound<Data>(this);
            }
            else
            {
                
            }
            base.Run(runChildren);
        }
    }


    /// <summary>
    /// ファイルの保存形式
    /// </summary>
    public enum FileType {
        /// <summary>
        /// Tsv形式
        /// </summary>
        Tsv,
        /// <summary>
        /// Json形式
        /// </summary>
        Json,
        /// <summary>
        /// LTsv形式
        /// </summary>
        LTsv }

    public enum FileSaveMode
    {
        Sequential, Completed
    }
}
