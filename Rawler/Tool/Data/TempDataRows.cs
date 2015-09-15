using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    /// <summary>
    /// 一時的にDataRowを蓄積するところです。スレッドセーフです。
    /// </summary>
    public class TempDataRows : RawlerBase, IDataRows
    {
        System.Collections.Concurrent.ConcurrentQueue<DataRowObject> list = new System.Collections.Concurrent.ConcurrentQueue<DataRowObject>();

        public void AddDataRow(DataRowObject datarow)
        {
            list.Enqueue(datarow);
        }

        public override void Run(bool runChildren)
        {
            SetText(GetText());
            base.Run(runChildren);
            DataRowObject row;

            do
            {
                if (list.TryDequeue(out row))
                {
                    Data.AddDataRow(this, row);
                }
                else
                {
                    break;
                }
            } while (row != null);
            
        }
    }
}
