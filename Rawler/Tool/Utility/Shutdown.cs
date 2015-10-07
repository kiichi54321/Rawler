using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    /// <summary>
    /// 強制終了させる
    /// </summary>
    public class Shutdown:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            throw new Exception("Shutdownを実行しました");          
        }
    }
}
