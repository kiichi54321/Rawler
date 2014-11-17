using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using RawlerLib.MyExtend;

namespace Rawler.Tool
{
    /// <summary>
    /// データを蓄積するRawlerクラス。
    /// </summary>
        [ContentProperty("Children")]
    [Serializable]
    
    public class Data : RawlerBase
    {
        /// <summary>
        /// データを蓄積するRawlerクラス。
        /// </summary>
        public Data()
            : base()
        {
            dataList.Add(currentDataRow);
           
        }

        public string FileName { get; set; }
         FileSaveMode fileSaveMode = FileSaveMode.Create;

        public FileSaveMode FileSaveMode
        {
            get { return fileSaveMode; }
            set { fileSaveMode = value; }
        }

        public RawlerBase FileNameTree { get; set; }


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

        /// <summary>
        /// DataListをバイナリで書きだし。戻すときは　List＜DataRow＞ で
        /// </summary>
        /// <param name="filename"></param>
        public void DataSave(string filename)
        {
            RawlerLib.ObjectLib.SaveToBinaryFile( dataList, filename);

        }

        bool endDataClear = false;

        public bool EndDataClear
        {
            get { return endDataClear; }
            set { endDataClear = value; }
        }

        /// <summary>
        /// バイナリで保存したデータをロードします。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<DataRow> DataLoad(string filename)
        {
            var list = RawlerLib.ObjectLib.LoadFromBinaryFile(filename);
            if (list is List<DataRow>)
            {
                return list as List<DataRow>;
            }
            else
            {
                return new List<DataRow>();
            }
        }

        [NonSerialized]
        List<DataRow> dataList = new List<DataRow>();
        Dictionary<string, DataRow> dataDic = new Dictionary<string, DataRow>();
        /// <summary>
        /// 蓄積されていくDataRow
        /// </summary>
        
        //public List<DataRow> DataList
        //{
        //    get { return dataList; }
        //}

        /// <summary>
        /// 蓄積されたDataRowのリストを取得します。NullのRowは含まれません。
        /// </summary>
        /// <returns></returns>
        public List<DataRow> GetDataRows()
        {
            List<DataRow> list = new List<DataRow>();

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
                currentDataRow = new DataRow();
                currentDataRow.AddData("Key",key);
                dataDic.Add(key, currentDataRow);
                dataList.Add(currentDataRow);
            }
        }



        DataRow currentDataRow = new DataRow();
        /// <summary>
        /// 現在のDataRowに書く。DataWriteがよびだすもの。
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void DataWrite(string attribute, string value,DataWriteType type,DataAttributeType attributeType)
        {
            if (type == DataWriteType.add)
            {
                currentDataRow.AddData(attribute, value,attributeType);
            }
            else if(type == DataWriteType.replace)
            {
                currentDataRow.ReplaceData(attribute, value,attributeType);
            }

            if (attribute.ToLower() == "key")
            {
                dataDic.Add(value, currentDataRow);
            }

        }
        public void DataWrite(string attribute, string value,DataWriteType type)
        {
            DataWrite(attribute, value, type, DataAttributeType.Text);
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
                if (Commited != null)
                {
                    Commited(this, new EventDataRow(currentDataRow));
                }
                currentDataRow = new DataRow();
                if (stock)
                {
                    dataList.Add(currentDataRow);
                }
            }

            
        }

        public override void Run(bool runChildren)
        {
            base.Run(runChildren);
            if(doLastFileSave)
            {
                if (SaveFileType == FileType.Tsv)
                {
                    TsvFileSave();
                }
                if(saveFileType == FileType.Json || saveFileType == FileType.改行区切りJson)
                {
                    JsonFileSave();
                }
            }

            if (EndDataClear)
            {
                dataDic.Clear();
                dataList.Clear();
                currentDataRow = new DataRow();
                dataList.Add(currentDataRow);
            }
        }

        private bool sortAttribute = true;
        protected bool doLastFileSave = true;

        FileType saveFileType = FileType.Tsv;

        public FileType SaveFileType { get { return saveFileType; } set { saveFileType = value; } }

        public void JsonFileSave()
        {
            string filename = this.FileName;
            if (this.FileNameTree != null)
            {
                filename = RawlerBase.GetText(this.GetText(), this.FileNameTree, this);
            }
            if (string.IsNullOrEmpty(filename) == false)
            {
                System.IO.StreamWriter sw = null;
                if (this.FileSaveMode == Tool.FileSaveMode.Create)
                {
                    sw = System.IO.File.CreateText(filename);
                }
                else if (this.FileSaveMode == Tool.FileSaveMode.Append)
                {
                    sw = System.IO.File.AppendText(filename);
                }
                if (saveFileType == FileType.Json)
                {
                    sw.WriteLine( Codeplex.Data.DynamicJson.Serialize( this.GetDataRows().Select(n=>n.DataDic)));
                }
                if(saveFileType == FileType.改行区切りJson)
                {                    
                    foreach (var item in this.GetDataRows())
                    {
                        sw.WriteLine(item.ToJson().Replace("\r", " ").Replace("\n", " "));
                    }
                }
                sw.Close();
                ReportManage.Report(this, filename + "作成完了", true, true);
            }
        }

        private void TsvFileSave()
        {
            if (doLastFileSave == false) return;            
            string filename = this.FileName;
            if (this.FileNameTree != null)
            {
                filename = RawlerBase.GetText(this.GetText(), this.FileNameTree, this);
            }
            if (string.IsNullOrEmpty(filename)==false)
            {
                System.IO.StreamWriter sw = null;
                if (this.FileSaveMode == Tool.FileSaveMode.Create)
                {
                    sw = System.IO.File.CreateText(filename);
                }
                else if (this.FileSaveMode == Tool.FileSaveMode.Append)
                {
                    sw = System.IO.File.AppendText(filename);
                }
                HashSet<string> hash = new HashSet<string>();
                foreach (var item2 in this.GetDataRows())
                {
                    foreach (var item3 in item2.DataDic.Keys)
                    {
                        hash.Add(item3);
                    }
                }
                var order = CreateOrderString();
                var list = hash.ToList();

                var except = list.Except(order).ToList();
                except.Sort();
                list = order.Union(except).ToList();
                foreach (var key in list)
                {
                    sw.Write(key);
                    sw.Write("\t");
                }
                sw.WriteLine();

                foreach (var row in this.GetDataRows())
                {
                    foreach (var key in list)
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
                sw.Close();
                ReportManage.Report(this, filename + "作成完了", true, true);
            }
                
            
        }

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

        
        public TableData CreateTable()
        {
            TableData table = new TableData();
            HashSet<string> hash = new HashSet<string>();
            foreach (var item2 in GetDataRows())
            {
                foreach (var item3 in item2.DataDic.Keys)
                {
                    hash.Add(item3);
                }
            }
            table.Head = hash.ToList();
            List<List<CellData>> list = new List<List<CellData>>();

            var dic = GetDataRows().SelectMany(n => n.GetCell(hash)).GroupBy(n => n.Key)
                .Where(n=>n.Select(m=>m.DataText).Distinct().Count()>1 || n.First().DataType == DataAttributeType.SourceUrl)
                .Select(n => new { Key = n.Key, Value = n.Select(m => m.DataText).JoinText("\t") })
                .ToDictionary(n => n.Key, n => n.Value);
            var keyList = dic.GroupBy(n => n.Value).Select(n => n.First().Key);
            var k = keyList;
            table.Head = k.ToList();
            foreach (var item in GetDataRows())
            {
                list.Add(item.GetCell(table.Head).ToList());
            }
            table.Rows = list;
            return table;
        }



        public string ToTsv()
        {
            StringBuilder strBuilder = new StringBuilder();
            var item = this;
//           foreach (var item in GetData())
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

            }
            return strBuilder.ToString();
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

        public event EventHandler DataNullEvent;

        public bool GetCurrentDataNull()
        {
            return currentDataRow.IsDataNull();
        }

        public DataRow GetCurrentDataRow()
        {
            return currentDataRow;
        }

        //private bool useReLogin = false;

        ///// <summary>
        ///// 再ログイン機能を使う
        ///// </summary>
        //public bool UseReLogin
        //{
        //    get { return useReLogin; }
        //    set { useReLogin = value; }
        //}
        /// <summary>
        /// 汎用性のある　List<Dictionary<string, List<string>>>　形式に変換する。
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
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            Data clone = new Data();
            RawlerLib.ObjectLib.FildCopy(this, clone);
            this.CloneEvent(clone);
            clone.SetParent(parent);
            clone.children.Clear();
            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
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
            public EventDataRow(DataRow row)
                : base()
            {
                DataRow = row;
            }
            /// <summary>
            /// イベント対象のDataRow
            /// </summary>
            public DataRow DataRow { get; set; }
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
                    return System.Xaml.XamlServices.Save(this);
                }
                catch
                {

                }
                //必殺エラー握りつぶし
                return System.Xaml.XamlServices.Save(new TableData() { Head = new List<string>() { "Err" }, Rows = new List<List<CellData>>() });
            }

            public IEnumerable<List<CellData>> GetSkipRows()
            {
                foreach (var item in Rows)
                {
                    if( item.Where(n=>n.DataText.Length>0 ).Count()>1)
                    {
                        yield return item;
                    }
                }
            }
        }

        public enum FileType { Tsv, Json ,改行区切りJson}

}
