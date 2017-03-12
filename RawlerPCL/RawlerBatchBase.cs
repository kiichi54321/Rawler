using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawlerLib.MyExtend;

namespace Rawler
{
    /// <summary>
    /// 一時的に蓄積する機能を提供する基本クラスです。
    /// 拡張するときは RunBatchをoverrideしてください。
    /// </summary>
    public class RawlerBatchBase :RawlerBase
    {
        /// <summary>
        /// 蓄積する量。負の値ときは、制限なし
        /// </summary>
        public int BatchSize { get; set; } = -1;
        
        List<string> stockList = new List<string>();
        bool connectEndEvent = false;
        /// <summary>
        /// バッチとして貯められたListです。
        /// </summary>
        protected string[] Array { get; set; }

        public override void Run(bool runChildren)
        {
            Array = null;
            if (connectEndEvent == false)
            {
                var parent = (ILoopEnd)this.GetUpperInterface<ILoopEnd>();
                parent.LoopEndEvent += Parent_EndRunEvent;
                connectEndEvent = true;
            }
            stockList.Add(GetText());
            

            if(BatchSize > 0 && stockList.Count >= BatchSize)
            {
                Array = stockList.ToArray();
                RunBatch(Array);
                stockList.Clear();
            }
        }

       /// <summary>
       /// 蓄積された配列を取得します。
       /// </summary>
       /// <returns></returns>
        public string[] GetArray()
        {
            return Array;
        }

        /// <summary>
        /// 蓄積されたものを実行するところです。
        /// </summary>
        /// <param name="list"></param>
        public virtual void RunBatch(IEnumerable<string> list)
        {            
            RunChildren(true);
        }

        private void Parent_EndRunEvent(object sender, EventArgs e)
        {
            RunBatch(stockList);
            stockList.Clear();
        }
    }

    /// <summary>
    /// 一時的に蓄積する機能を提供する基本クラスです。
    /// 拡張するときは RunBatchをoverrideしてください。
    /// </summary>
    public class RawlerBatchMultiBase : RawlerMultiBase
    {
        /// <summary>
        /// 蓄積する量。負の値ときは、制限なし
        /// </summary>
        public int BatchSize { get; set; } = -1;

        List<string> stockList = new List<string>();
        bool connectEndEvent = false;
        /// <summary>
        /// バッチとして貯められたListです。
        /// </summary>
        protected string[] Array { get; set; }

        public override void Run(bool runChildren)
        {
            Array = null;
            if (connectEndEvent == false)
            {
                var parent = (ILoopEnd)this.GetUpperInterface<ILoopEnd>();
                parent.LoopEndEvent += Parent_EndRunEvent;
                connectEndEvent = true;
            }
            stockList.Add(GetText());


            if (BatchSize > 0 && stockList.Count >= BatchSize)
            {
                Array = stockList.ToArray();
                RunBatch(Array);
                stockList.Clear();
            }
        }

        /// <summary>
        /// 蓄積された配列を取得します。
        /// </summary>
        /// <returns></returns>
        public string[] GetArray()
        {
            return Array;
        }

        /// <summary>
        /// 蓄積されたものを実行するところです。
        /// </summary>
        /// <param name="list"></param>
        public virtual void RunBatch(IEnumerable<string> list)
        {
            RunChildrenForArray(true,list);
        }

        private void Parent_EndRunEvent(object sender, EventArgs e)
        {
            RunBatch(stockList);
            stockList.Clear();
        }
    }


    /// <summary>
    /// 蓄積されたテキストをTSVにする。
    /// </summary>
    public class BatchToTsv:RawlerBatchBase
    {
        public override void RunBatch(IEnumerable<string> list)
        {
            SetText(list.JoinText("\t"));
            base.RunBatch(list);
        }
    }

    /// <summary>
    /// 蓄積されたテキストをTSVにする。
    /// </summary>
    public class BatchCount : RawlerBatchBase
    {
        public override void RunBatch(IEnumerable<string> list)
        {
            SetText(list.Count().ToString());
            base.RunBatch(list);
        }
    }
}
