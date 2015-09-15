using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;

namespace RawlerParallel
{
    public class ParallelRun : RawlerBase
    {
        public TaskParameterList TaskParameterList { get; set; } = new TaskParameterList();

        public int TaskNum { get; set; } = 1;

        public override void Run(bool runChildren)
        {
            if (runChildren)
            {
                SetText(GetText());
                List<Task> list = new List<Task>();

                KeyValueStore key = new KeyValueStore();
                foreach (var item in this.Children)
                {
                    key.AddChildren(item);
                }
                var xaml = key.ToXAML();
                string err;
                if (TaskParameterList.Count > 0)
                {
                    foreach (var item in TaskParameterList)
                    {
                        var r = (KeyValueStore)RawlerBase.Parse(xaml, out err);
                        foreach (var keyvalue in item)
                        {
                            r.SetKeyValue(keyvalue.Key, keyvalue.Value);
                        }
                        r.SetParent(this.Parent);
                        list.Add(Task.Factory.StartNew(() => r.Run()));
                    }
                }
                else
                {
                    for (int i = 0; i < TaskNum; i++)
                    {
                        var r = (KeyValueStore)RawlerBase.Parse(xaml, out err);
                        r.SetParent(this.Parent);
                        list.Add(Task.Factory.StartNew(() => r.Run()));
                    }
                }              
                Task.WaitAll(list.ToArray());
            }

           
        }
    }

    public class TaskParameterList:List<TaskParameter>
    {

    }

    public class TaskParameter:List<KeyValue>
    {

    }

}
