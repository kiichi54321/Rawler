using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;
using RawlerLib.MyExtend;

namespace RawlerParallel
{
    public class ParallelRun : RawlerBase
    {
        public TaskParameterList TaskParameterList { get; set; } = new TaskParameterList();

        public int TaskNum { get; set; } = 1;

        public string TaskLTSV { get; set; }
        public string TaskLTSVFile { get; set; }

        public override void Run(bool runChildren)
        {
            if (runChildren)
            {
                SetText(GetText());
                if(TaskLTSV.NullIsEmpty().Length>0)
                {
                    TaskParameterList.LTSV = TaskLTSV.Convert(this);
                }
                if(TaskLTSVFile.NullIsEmpty().Length>0)
                {
                    TaskParameterList.FileName = TaskLTSVFile.Convert(this);
                }
                TaskParameterList.Load(this);
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
        public string LTSV { get; set; }
        public string FileName { get; set; }

        public void Load(RawlerBase rawler)
        {
            var file = FileName.Convert(rawler);
            if(string.IsNullOrEmpty(file) == false)
            {
                if( System.IO.File.Exists(file) ==true)
                {
                    LTSV = System.IO.File.ReadAllText(file);
                }
            }
            var ltsv = LTSV.Convert(rawler);
            if (string.IsNullOrEmpty(ltsv) == false)
            {
                foreach (var item in ltsv.ReadLines())
                {
                    TaskParameter tp = new TaskParameter();
                    foreach(var dic in  item.ParseLtsvLine())
                    {
                        tp.Add(new KeyValue(dic.Key, dic.Value));
                    }
                    if(tp.Count>0) this.Add(tp);
                }
            }
            //ConvertにConvertを適用する。
            foreach (var item in this.SelectMany(n=> n))
            {
                item.Value = item.Value.Convert(rawler);
            }
        }
    }

    public class TaskParameter:List<KeyValue>
    {

    }

}
