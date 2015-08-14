using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawlerLib.MyExtend;
using System.Diagnostics;

namespace Rawler.Tool
{
    /// <summary>
    /// データの一行分を示す。終了時に親のDataに集めたデータを投げる。
    /// </summary>
    public class DataRow:RawlerBase,IData
    {
        DataRowObject currentDataRow = new DataRowObject();

        /// <summary>
        /// データを書き込む
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
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
        }

        public void DataWrite(string attribute, string value, DataWriteType type)
        {
            DataWrite(attribute, value, type, DataAttributeType.Text);
        }

        /// <summary>
        /// データが空のとき実行するTree
        /// </summary>
        public RawlerBase EmptyTree { get; set; }


        /// <summary>
        /// 必ず入って欲しいAttributes（カンマ区切り）
        /// </summary>
        public string MustAttributes { get; set; }

        public override void Run(bool runChildren)
        {
            currentDataRow = new DataRowObject();
            SetText(GetText());
            base.Run(runChildren);
            var d = this.GetUpperRawler<Data>();
            if (d != null)
            {
                if (MustAttributes.IsNullOrEmpty() == false)
                {
                    var must = MustAttributes.Split(',');
                    if (currentDataRow.Attributes.Intersect(must).Count() == must.Count())
                    {
                        d.AddDataRow(currentDataRow);
                    }
                }
                else
                {
                    d.AddDataRow(currentDataRow);
                }
                if (currentDataRow.IsDataNull())
                {
                    if (EmptyTree != null)
                    {
                        EmptyTree.SetParent(this);
                        EmptyTree.Run();
                    }
                }
            }
        }

        public DataRowObject GetCurrentDataRow()
        {
            return currentDataRow;
        }
    }



    /// <summary>
    /// データの一行分を格納するクラス。ListDictionary でデータを保持している。
    /// </summary>
    [Serializable]
    public class DataRowObject
    {
        private Dictionary<string, DataAttributeType> dataTypeDic = new Dictionary<string, DataAttributeType>();
        private Dictionary<string, List<string>> dataDic = new Dictionary<string, List<string>>();
        /// <summary>
        /// データの一行分を格納するクラス。
        /// </summary>
        public Dictionary<string, List<string>> DataDic
        {
            get { return dataDic; }
//            set { dataDic = value; }
        }

        public ICollection<string> Attributes
        {
            get
            {
                return dataDic.Keys;
            }
        }

        public void UpDate(DataRowObject row)
        {
            dataTypeDic = row.dataTypeDic;
            dataDic = row.dataDic;
        }

        /// <summary>
        /// データを加える。上書きせずリストに蓄積される。
        /// </summary>
        /// <param name="attribute">属性（Key）</param>
        /// <param name="value">値</param>
        public void AddData(string attribute, string value,DataAttributeType type)
        {
            List<string> list;
            dataTypeDic.GetValueOrAdd(attribute, type);

            if (dataDic.TryGetValue(attribute, out list) == false)
            {
                list = new List<string>();
                dataDic.Add(attribute, list);
            }
            list.Add(value);
        }

        public void AddData(string attribute, string value)
        {
            AddData(attribute, value, DataAttributeType.Text);
        }

        /// <summary>
        /// データを加える。上書きせずリストに蓄積される。
        /// </summary>
        /// <param name="attribute">属性（Key）</param>
        /// <param name="value">値</param>
        public void ReplaceData(string attribute, string value,DataAttributeType type)
        {
            List<string> list;
            dataTypeDic.GetValueOrAdd(attribute, type);
            if (dataDic.TryGetValue(attribute, out list) == false)
            {
                list = new List<string>();
                dataDic.Add(attribute, list);
            }
            list.Clear();
            list.Add(value);
        }

        public string ToJson()
        {
           return   JsonConvert.SerializeObject(dataDic);
        }

        /// <summary>
        /// 文字列に変換する。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();

            foreach (var item in dataDic.Keys)
            {
                strBuilder.Append(item);
                strBuilder.Append("\t");
                foreach (var item2 in dataDic[item])
                {
                    strBuilder.Append(item2);
                    strBuilder.Append("\t");
                }
                strBuilder.AppendLine();
            }
            return strBuilder.ToString();
        }


        //public IEnumerable<CellData> GetCell(IEnumerable<string> keys)
        //{
        //    List<CellData> list = new List<CellData>();
        //    foreach (var item in keys)
        //    {
        //        if( dataDic.ContainsKey(item))
        //        {
        //            list.Add(new CellData() { Key = item, Values = dataDic[item].Select(n=>n.Trim()).ToList() , DataType = dataTypeDic.GetValueOrDefault(item,DataAttributeType.Text) });
        //        }
        //        else
        //        {
        //            list.Add(new CellData() { Key = item, Values = new List<string>(), DataType = DataAttributeType.Text });
        //        }
        //    }
        //    return list;
        //}

        public IEnumerable<CellData> GetCell(IDictionary<string,int> dic)
        {
            List<CellData> list = new List<CellData>();
            foreach (var item in dic)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    string k = item.Key;
                    if (item.Value > 1) { k = item.Key + "_" + (i + 1); }

                    if (dataDic.ContainsKey(item.Key) && dataDic[item.Key].Count > i)
                    {
                        list.Add(new CellData() { Key = k, Value = dataDic[item.Key][i].Trim(), DataType = dataTypeDic.GetValueOrDefault(item.Key, DataAttributeType.Text) });
                    }
                    else
                    {
                        list.Add(new CellData() { Key = k, Value = string.Empty, DataType = DataAttributeType.Text });
                    }
                }
            }
            return list.OrderBy(n => n.DataType);
          //  return list;
        }

        /// <summary>
        /// attributeの内容を取ってくる（単数）。attributeがない時は空文字を返す。
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string GetValue(string attribute)
        {
            if (dataDic.ContainsKey(attribute))
            {
                return dataDic[attribute].First();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// attributeの内容を取ってくる（複数）。attributeがない時は空リストを返す。
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public List<string> GetValueList(string attribute)
        {
            if (dataDic.ContainsKey(attribute))
            {
                return dataDic[attribute];
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// データが入っていないか？
        /// </summary>
        /// <returns></returns>
        public bool IsDataNull()
        {
            if (dataDic.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [DebuggerDisplay("{Key}:{DataText})")]
    public class CellData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        //廃止
        //public List<string> Values { get; set; }
        public DataAttributeType DataType { get; set; }
        public string DataText
        {
            get { return Value; }
        }
    }

   


    public enum DataAttributeType
    {
        SourceUrl, Image, Url, Text,
    }
}
