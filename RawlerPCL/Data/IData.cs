using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    /// <summary>
    /// DataWrite用のインターフェース
    /// </summary>
    public interface IDataWrite
    {
        string Attribute { get; set; }
        DataWriteType WriteType { get; set; }
    }

    /// <summary>
    /// Data用のインターフェース
    /// </summary>
    public interface IData
    {
        DataRowObject GetCurrentDataRow();
        void DataWrite(string attribute, string value, DataWriteType type, DataAttributeType attributeType);
        void DataWrite(string attribute, string value, DataWriteType type);
    }

    public interface IDataRows
    {
        void AddDataRow(DataRowObject datarow);
        
    }
}
