using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler
{
    /// <summary>
    /// 並列実行
    /// </summary>
    public class Concurrent:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            if (runChildren)
            {
                SetText(GetText());
                List<Task> list = new List<Task>();
                foreach (RawlerBase item in children)
                {
                    list.Add(Task.Factory.StartNew(() => item.Run()));
                }
                Task.WaitAll(list.ToArray());
            }
            
       
        }
    }


}
