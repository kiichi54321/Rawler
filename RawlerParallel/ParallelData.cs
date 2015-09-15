using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;
using System.Threading.Tasks.Dataflow;

namespace RawlerParallel
{
    public class ParallelData : FileSave, IDisposable
    {
         ActionBlock<DataRowObject> DataRowBlock;

        public override void Run(bool runChildren)
        {
            DataRowBlock = new ActionBlock<DataRowObject>((n) => NextDataRow(n));
         
            base.Run(runChildren);
        }

        public override void Dispose()
        {
            DataRowBlock.Complete();
            DataRowBlock.Completion.Wait();
           
            base.Dispose();
        }

        public override void AddDataRow(DataRowObject datarow)
        {
            DataRowBlock.Post(datarow);
        }

       
    }
}
