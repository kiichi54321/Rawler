using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rawler.Tool;
using RawlerLib.MyExtend;

namespace RawlerParallel.MapReduce
{
    public class Reduce : Rawler.Tool.RawlerBase
    {

        BufferBlock<KeyValuePair<string, int>> input = new BufferBlock<KeyValuePair<string, int>>();


        public string FileName { get; set; } = "ReduceResult.txt";
        public bool FileSave { get; set; } = false;
        public int CutCount { get; set; } = 0;
        public int BatchSize { get; set; } = 10000;

        public  override void Run(bool runChildren)
        {
            input = new BufferBlock<KeyValuePair<string, int>>();

            BatchBlock<KeyValuePair<string, int>> batch = new BatchBlock<KeyValuePair<string, int>>(BatchSize);
            TransformBlock<KeyValuePair<string, int>[], Dictionary<string, int>> trans = new TransformBlock<KeyValuePair<string, int>[], Dictionary<string, int>>(l =>
            {
                return l.ToCountDictionary(n => n.Key, n => n.Value);
            });
            BatchBlock<Dictionary<string, int>> dicBatch = new BatchBlock<Dictionary<string, int>>(3);
            TransformBlock<Dictionary<string, int>[], Dictionary<string, int>> margeDic = new TransformBlock<Dictionary<string, int>[], Dictionary<string, int>>(n => {
                return n.Marge();
            });
            bool retrunFlag = true;
            BufferBlock<Dictionary<string, int>> result = new BufferBlock<Dictionary<string, int>>();

            ActionBlock<Dictionary<string, int>> returnAction = new ActionBlock<Dictionary<string, int>>(n => {
                if (retrunFlag) dicBatch.Post(n);
                else result.Post(n);
            });

            input.LinkTo(batch, new DataflowLinkOptions() { PropagateCompletion = true });
            batch.LinkTo(trans, new DataflowLinkOptions() { PropagateCompletion = true });
            trans.LinkTo(dicBatch, new DataflowLinkOptions() { PropagateCompletion = true });
            dicBatch.LinkTo(margeDic, new DataflowLinkOptions() { PropagateCompletion = true });
            margeDic.LinkTo(returnAction, new DataflowLinkOptions() { PropagateCompletion = true });

            SetText(GetText());
            base.Run(runChildren);
            retrunFlag = false;
          
            input.Complete();
            returnAction.Completion.Wait();

            IList<Dictionary<string, int>> list;
            if (result.TryReceiveAll(out list))
            {
                var r = list.Marge();
                if (FileSave)
                {
                    using (var f = System.IO.File.CreateText(FileName.Convert(this)))
                    {
                        foreach (var item in r.OrderByDescending(n => n.Value).Where(n => n.Value >= CutCount))
                        {
                            f.WriteLine(item.Key + "\t" + item.Value);
                            ReportManage.Report(this, item.Key + "\t" + item.Value, true, true);
                        }
                        ReportManage.Report(this, "FileWrite:" + FileName.Convert(this), true, true);
                    }
                }
                var reduce = this.GetUpperRawler<Reduce>();
                if (reduce != null)
                {
                    foreach (var item in r.Where(n => n.Value >= CutCount))
                    {
                        reduce.AddKeyValue(item);
                    }
                }
            }
            else
            {
                ReportManage.ErrReport(this, "TryReceiveAll 失敗");
            }       

        }

        public void AddKeyValue(KeyValuePair<string, int> key)
        {
         //   inputCollection.Add(key);
            input.Post(key);
        }

        public void AddKeyValue(string key, int value)
        {
        //    inputCollection.Add(new KeyValuePair<string, int>( key,value));
            input.Post(new KeyValuePair<string, int>(key, value));
        }
    }



    public class Map : RawlerBase
    {
        public int Value { get; set; } = 1;
        Reduce reduce = null;

        public override void Run(bool runChildren)
        {
            if(reduce == null)
            {
                reduce = this.GetUpperRawler<Reduce>();                
            }
            reduce.AddKeyValue(GetText(), Value);
            base.Run(runChildren);
        }
    }
}
