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

            SetEndAction(() => {
                DataRowBlock.Complete();
                DataRowBlock.Completion.Wait();
            });
         
            base.Run(runChildren);
           

        }

        private void ParallelData_EndRunEvent(object sender, EventArgs e)
        {
            DataRowBlock.Complete();
            DataRowBlock.Completion.Wait();
        }

        public override void Dispose()
        {         
           
            base.Dispose();
        }

        public override void AddDataRow(DataRowObject datarow)
        {
            DataRowBlock.Post(datarow);
        }

       
    }
}
