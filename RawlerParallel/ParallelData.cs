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
        BufferBlock<DataRowObject> DataRowObjectList = new System.Threading.Tasks.Dataflow.BufferBlock<Rawler.Tool.DataRowObject>();
        bool threadFlag = true;
        Task task;
        

        public override void Run(bool runChildren)
        {
            task = Task.Factory.StartNew(() => {
                while(threadFlag)
                {
                    IList<Rawler.Tool.DataRowObject> list;
                    if (DataRowObjectList.TryReceiveAll(out list))
                    {
                        foreach (var item in list)
                        {
                            NextDataRow(item);
                        }
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }).ContinueWith( (n)=>{
                IList<Rawler.Tool.DataRowObject> list;
                if (DataRowObjectList.TryReceiveAll(out list))
                {
                    foreach (var item in list)
                    {
                        NextDataRow(item);
                    }
                }
            });
            threadFlag = true;
            base.Run(runChildren);
            threadFlag = false;
            
        }

        public override void Dispose()
        {
            if(task !=null)
            {
                threadFlag = false;
                task.Wait();
                task.Dispose();
            }
            base.Dispose();
        }

        public override void AddDataRow(DataRowObject datarow)
        {
            DataRowObjectList.Post(datarow);
        }

       
    }
}
