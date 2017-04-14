using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    /// <summary>
    /// 指定したAttributeが空のとき実行する。
    /// </summary>
    public class IsDataAttributeEmpty:RawlerBase
    {
        public string Attribute { get; set; }

        public override void Run(bool runChildren)
        {
            var data = (IData)this.GetUpperInterface<IData>();
            if (data == null)
            {
                ReportManage.ErrUpperNotFound<IData>(this);
            }

            var currentRow = data.GetCurrentDataRow();
            var attribute = Attribute.Convert(this);
            SetText(GetText());
            if (currentRow.DataDic.TryGetValue(attribute, out List<string> list) == false)
            {
                base.Run(runChildren);
            }
            else
            {
                if (list.Any() == false)
                {
                    base.Run(runChildren);
                }
            }
        }
    }
}
