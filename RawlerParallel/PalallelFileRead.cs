using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;
using System.Threading.Tasks.Dataflow;
using System.Threading;

namespace RawlerParallel
{
    public class PalallelReadFile : RawlerBase
    {
        public string FileName { get; set; } = string.Empty;

        System.Collections.Concurrent.ConcurrentStack<string[]> q = new System.Collections.Concurrent.ConcurrentStack<string[]>();
        bool fileReadCompleted = false;
        public int BlockSize { get; set; } = 10000;
        public int MaxBlock { get; set; } = 20;

        CancellationTokenSource cancellationTokenSource;

        public async override void Run(bool runChildren)
        {
            fileReadCompleted = false;
            string file = FileName.Convert(this);
            if(string.IsNullOrEmpty(file))
            {
                file = GetText();
            }
            if(System.IO.File.Exists(file)==false)
            {
                ReportManage.ErrReport(this, "Fileが見つかりません「"+file+"」");
                return;
            }
            cancellationTokenSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() =>
            {
                List<string> list = new List<string>(BlockSize);

                foreach (var item in System.IO.File.ReadLines(FileName.Convert(this)))
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested) break;
                    bool flag = true;
                    do
                    {
                        if (q.Count < MaxBlock)
                        {
                            list.Add(item);
                            if(list.Count>BlockSize)
                            {
                                q.Push(list.ToArray());
                                list.Clear();
                            }
                            flag = false;
                            break;
                        }
                        else
                        {
                            Task.Delay(3).Wait();
                        }
                    }
                    while (flag);
                }
                fileReadCompleted = true;
            }, cancellationTokenSource.Token);
            base.Run(runChildren);
        }
        public override void Dispose()
        {
            if(cancellationTokenSource !=null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
            base.Dispose();
        }

        public string[] ReadLines()
        {
            do
            {
                string[] lines;
                if (q.TryPop(out lines))
                {                   
                    return lines;
                }
                Task.Delay(5).Wait();
            }
            while (fileReadCompleted == false);
            return null;
        }
    }

    public class PalallelFileReadLines:RawlerMultiBase
    {

        public override void Run(bool runChildren)
        {
            var p = this.GetUpperRawler<PalallelReadFile>();
            string[] l;
            if(p == null)
            {
                ReportManage.ErrUpperNotFound<PalallelReadFile>(this);
            }
            while(true)
            {
                l = p.ReadLines();
                if (l == null) break;
                RunChildrenForArray(true, l);
            }
        }
    }
}
