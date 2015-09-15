using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rawler.Tool;
using System.Collections.Concurrent;
using System.Reactive.Linq;

namespace RawlerParallel
{
    internal class DateValue<T>
    {
        public DateTime Date { get; set; }
        public T Value { get; set; }
    }

    
    public class ParallelQueue:Rawler.Tool.RawlerBase
    {
        BufferBlock<string> queue = new BufferBlock<string>();
     //   BufferBlock<DateValue<string>> workingBlock = new BufferBlock<DateValue<string>>();
        HashSet<string> completedHash = new HashSet<string>();
        BufferBlock<string> completdBlock = new BufferBlock<string>();
        System.Collections.Concurrent.BlockingCollection<DateValue<string>> workingList = new BlockingCollection<DateValue<string>>();
        Task workcheckTask;
        Task completedTask;
        System.Threading.CancellationTokenSource canceltoken = new System.Threading.CancellationTokenSource();

        public bool IsSingle { get; set; } = false;
        public string CompletedFileName { get; set; } = "ParallelQueueCompleted.txt";
        public bool UseCompletedFile { get; set; } = false;
        public int TimeOutMinutes { get; set; } = 10;

        public override void Run(bool runChildren)
        {

            //作業中のものをチェックして、規定時間以上かかったものをキューに戻す。
            //workcheckTask = Task.Factory.StartNew(() => {
            //    while(canceltoken.IsCancellationRequested == false)
            //    {
            //       var list =  workingList.Where(n => DateTime.Now - n.Date > TimeSpan.FromMinutes(TimeOutMinutes)).ToArray();
            //        foreach (var item in list)
            //        {
            //            queue.Post(item.Value);
            //            workingList.GetConsumingEnumerable()
            //        }
            //        DateValue<string> val;
            //        do
            //        {
                        
            //        }
            //        while (val != null);
            //        System.Threading.Thread.Sleep(1000);
            //    }
            //},canceltoken.Token, TaskCreationOptions.PreferFairness,TaskScheduler.Default);
           
                ReadCompletedFile();
                //完了処理。ファイルに書き込む。
                completedTask = Task.Factory.StartNew(() =>
                {
                    while (canceltoken.IsCancellationRequested == false)
                    {
                        IList<string> list;
                        if (completdBlock.TryReceiveAll(out list))
                        {
                            if (UseCompletedFile)
                            {
                                using (var fs = System.IO.File.AppendText(CompletedFileName))
                                {
                                    foreach (var item in list)
                                    {
                                        fs.WriteLine(item);
                                        if (IsSingle) completedHash.Add(item);
                                    }
                                }
                            }
                            foreach (var item in list)
                            {
                                if (IsSingle) completedHash.Add(item);
                            }
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                }, canceltoken.Token, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
            
            base.Run(runChildren);
        }

        protected void ReadCompletedFile()
        {
            if (UseCompletedFile)
            {
                if (System.IO.File.Exists(CompletedFileName))
                {
                    ReportManage.Report(this, CompletedFileName + "を読み込んでいます。", true, true);
                    foreach (var item in System.IO.File.ReadAllLines(CompletedFileName))
                    {
                        if (IsSingle) completedHash.Add(item);
                    }
                }
            }
        }


        public void Enqueue(string val)
        {
            if (IsSingle)
            {
                if (completedHash.Contains(val) == false)
                {
                    queue.Post(val);
                }
                else
                {
                    ReportManage.ErrReport(this, val +"は既に完了済なので追加できません。");
                }
            }
            else
            {
                queue.Post(val);
            }
        }

        public string Dequeue()
        {
            string val;
            if( queue.TryReceive(out val))
            {
            //    workingBlock.Post(new DateValue<string>() { Date = DateTime.Now, Value = val });
                return val;
            }
            else
            {
               // ReportManage.ErrReport(this, "Dequeueをするものがありませんでした。");
                return null;
            }
        }

        public void Completed(string val)
        {
            //workingから消す
            DateValue<string> val2;
            //if (workingBlock.TryReceive(n => n.Value == val, out val2))
            //{
                
            //}            
            //完了処理をする。
            if (completedTask != null)
            {
                completdBlock.Post(val);
            }
        }

        public override void Dispose()
        {
            canceltoken.Cancel();
            if(completedTask !=null)
            {
                completedTask.Wait();
                completedTask.Dispose();
            }
            if (workcheckTask != null)
            {
                workcheckTask.Wait();
                workcheckTask.Dispose();
            }

            base.Dispose();

        }




    }

    /// <summary>
    /// 上流のParallelQueueにデータを渡す。
    /// </summary>
    public class Enqueue:RawlerBase
    {
        /// <summary>
        /// Valueが空文字でないとき、親の文字列ではなくこちらを渡す。
        /// </summary>
        public string Value { get; set; }

        public override void Run(bool runChildren)
        {
            var queue = this.GetUpperRawler<ParallelQueue>();
            if (queue != null)
            {
                if (string.IsNullOrEmpty(Value))
                {
                    queue.Enqueue(GetText());                   
                }
                else
                {
                    queue.Enqueue(Value);
                }
                base.Run(runChildren);
            }
            else
            {
                ReportManage.ErrReport(this, "上流にParallelQueueがありません。");
            }
        }
    }

    /// <summary>
    /// 上流のParallelQueueから受け取る。繰り返し。
    /// </summary>
    public class Dequeue:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var queue = this.GetUpperRawler<ParallelQueue>();
            if (queue != null)
            {
                var deqeue= queue.Dequeue();
                while (deqeue !=null)
                {
                    SetText(deqeue);
                    base.Run(runChildren);
                    deqeue = queue.Dequeue();
                }
            }
            else
            {
                ReportManage.ErrReport(this, "上流にParallelQueueがありません。");
            }

        }
    }

    /// <summary>
    /// 上流のParallelQueueに完了したことを伝える。
    /// </summary>
    public class DequeueCompleted:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var queue = this.GetUpperRawler<ParallelQueue>();
            var dequeue = this.GetUpperRawler<Dequeue>();

            if(queue !=null && dequeue !=null)
            {
                queue.Completed(dequeue.Text);
            }
            else
            {
                ReportManage.ErrReport(this, "上流にParallelQueue,Dequeueがありません。");
            }

            base.Run(runChildren);
        }
    }


}
