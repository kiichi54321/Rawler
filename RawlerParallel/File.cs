using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;
using System.Threading.Tasks.Dataflow;
using RawlerLib.MyExtend;

namespace RawlerParallel
{
    public class File:RawlerBase
    {

        BufferBlock<string> inputBlock = new BufferBlock<string>();
        public string FileName { get; set; }
        public int MaxFileSize { get; set; } = 1024;

        string currentFileName = string.Empty;
        string baseFileName = string.Empty;
        public override void Run(bool runChildren)
        {
            currentFileName = FileName.Convert(this);
            baseFileName = currentFileName;
            if(currentFileName.Length==0)
            {
                ReportManage.ErrReport(this, "FileNameがありません");
                return;
            }

            var f = CheckFileSizeAndCreate(System.IO.File.AppendText(currentFileName));
            int count = 0;
            ActionBlock<string> actionBlock = new ActionBlock<string>((n) => {
                if(count%10000==0)
                {
                    f = CheckFileSizeAndCreate(f);
                }
                f.WriteLine(n);
                ReportManage.Report(this, "NextDataRow");
                count++;
            });
            inputBlock = new BufferBlock<string>();
            inputBlock.LinkTo(actionBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            base.Run(runChildren);
            inputBlock.Complete();
            actionBlock.Completion.Wait();
            f.Close();

        }

        private System.IO.StreamWriter CheckFileSizeAndCreate(System.IO.StreamWriter streamWriter)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(currentFileName);
            if (fi.Length > 1024 * 1024 * MaxFileSize)
            {
                streamWriter.Close();
                var file = RawlerLib.IO.GenerateFileName(baseFileName, (n) =>
                {
                    if (System.IO.File.Exists(n))
                    {
                        System.IO.FileInfo fi2 = new System.IO.FileInfo(n);
                        if (fi2.Length < 1024 * 1024 * MaxFileSize) return true;
                        else return false;
                    }
                    else
                    {
                        return true;
                    }
                });
                currentFileName = file;
                streamWriter = System.IO.File.AppendText(currentFileName);
            }
            return streamWriter;
        }

        public void WriteLine(string text)
        {
            inputBlock.Post(text);
        }
    }

    public class FileWriteLine:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var file = this.GetUpperRawler<File>();
            if(file !=null)
            {
                file.WriteLine(GetText());
            }
            else
            {
                ReportManage.ErrUpperNotFound<File>(this);
            }
            base.Run(runChildren);
        }
    }


    public enum FileType
    {
        Tsv,Ltsv
    }


    public class FileWriteDataLine:DataRow
    {
        public FileType SaveType { get; set; } = FileType.Ltsv;

        File file = null;
        public override void Run(bool runChildren)
        {
            currentDataRow = new DataRowObject();
            SetText(GetText());
            base.Run(runChildren);
            if (file == null)
            {
                file = this.GetUpperRawler<File>();
                if (file == null)
                {
                    ReportManage.ErrUpperNotFound<File>(this);
                    return;
                }
            }

            if (MustAttributes.IsNullOrEmpty() == false)
            {
                var must = MustAttributes.Split(',');
                if (currentDataRow.Attributes.Intersect(must).Count() == must.Count())
                {
                    if (SaveType == FileType.Ltsv)
                    {
                        file.WriteLine(currentDataRow.ToLtsv());
                    }
                    else
                    {
                        file.WriteLine(currentDataRow.DataDic.Select(n=>n.Value.JoinText(",")).JoinText("\t"));
                    }
                }
            }
            else
            {
                if (SaveType == FileType.Ltsv)
                {
                    file.WriteLine(currentDataRow.ToLtsv());
                }
                else
                {
                    file.WriteLine(currentDataRow.DataDic.Select(n => n.Value.JoinText(",")).JoinText("\t"));
                }
            }
            if (currentDataRow.IsDataNull())
            {
                if (EmptyTree != null)
                {
                    EmptyTree.SetParent(this);
                    EmptyTree.Run();
                }
            }
        }

    }
}
