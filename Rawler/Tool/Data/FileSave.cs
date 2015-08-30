using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RawlerLib.MyExtend;

namespace Rawler.Tool
{
    public class FileSave : Data
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public FileSave()
        {
            this.Commited += new EventHandler<EventDataRow>(FileSave_Commited);
            doLastFileSave = false;
            Stock = false;
        }

        public string AttributeOrderString { get; set; }





        private List<string> order = new List<string>();


        private List<string> CreateOrderString()
        {
            Stack<RawlerBase> stack = new Stack<RawlerBase>();
            List<string> list = new List<string>();
            foreach (var item in this.Children.Reverse())
            {
                stack.Push(item);
            }


            while (stack.Count > 0)
            {
                var rawler = stack.Pop();
                if ((rawler is Data) == false)
                {
                    if (rawler is IDataWrite)
                    {
                        var dw = rawler as IDataWrite;
                        if (string.IsNullOrEmpty(dw.Attribute) == false)
                        {
                            list.Add(dw.Attribute);
                        }
                    }
                    foreach (var item in rawler.Children.Reverse())
                    {
                        stack.Push(item);
                    }
                }
            }
            return new List<string>(list.Distinct());
        }

        /// <summary>
        /// このオブジェクトのテキスト。親のテキストをそのまま流す。
        /// </summary>
        public override string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.Text;
                }
                return string.Empty;
            }

        }

        private void CheckFileSizeAndCreate()
        {
            FileInfo fi = new FileInfo(currentFileName);
            if( fi.Length > 1024*1024*200)
            {
                streamWriter.Close();
                var file = RawlerLib.IO.GenerateFileName(baseFileName, (n) =>
                {
                    if (System.IO.File.Exists(n))
                    {
                        FileInfo fi2 = new FileInfo(n);
                        if (fi2.Length < 1024 * 1024 * 200) return true;
                        else return false;
                    }
                    else
                    {
                        return true;
                    }
                });
                currentFileName = file;
                FileInit(file);
            }
        }

        long count = 0;
        void FileSave_Commited(object sender, Data.EventDataRow e)
        {
            if (count % 10000== 0) CheckFileSizeAndCreate();
            count++;
            if (streamWriter != null)
            {                
                foreach (var item2 in order)
                {
                    if (e.DataRow.DataDic.ContainsKey(item2))
                    {
                        var item = e.DataRow.DataDic[item2];
                        if (item.First() != null)
                        {
                            if (item.Count > 1)
                            {
                                item.ForEach(n => streamWriter.Write(n.ReadLines().Select(m => m.Replace("\t", " ").Trim()).JoinText(" ") + ","));
                            }
                            else
                            {
                                streamWriter.Write(item.First().ReadLines().Select(m => m.Replace("\t", " ").Trim()).JoinText(" "));
                            }
                        }
                    }
                    streamWriter.Write("\t");
                }

                foreach (var item in e.DataRow.DataDic.OrderBy(n => n.Key).Where(n => order.Contains(n.Key)==false))
                {
                    if (item.Key.Length > 0)
                    {
                        streamWriter.Write(item.Key + ":");
                    }
                    if (item.Value.Count > 1)
                    {
                        item.Value.ForEach(n => streamWriter.Write(n.ReadLines().Select(m=> m.Replace("\t"," ").Trim()).JoinText(" ") + ","));
                    }
                    else
                    {
                        streamWriter.Write(item.Value.First().ReadLines().Select(m=> m.Replace("\t"," ").Trim()).JoinText(" "));
                    }
                    streamWriter.Write("\t");
                }
                streamWriter.WriteLine();
            }

        }


        /// <summary>
        /// ダイアログでの拡張子フィルターの指定
        /// </summary>
        public string ExtendFilter { get; set; }


        StreamWriter streamWriter = null;

        string baseFileName = string.Empty;
        string currentFileName = string.Empty;

        private void FileInit(string fileName)
        {
            var existflag = System.IO.File.Exists(fileName);


            if (FileSaveMode == Tool.FileSaveMode.Create)
            {
                streamWriter = File.CreateText(fileName);
            }
            else
            {
                streamWriter = File.AppendText(fileName);
            }

            if (AttributeOrderString != null)
            {
                order = new List<string>(AttributeOrderString.Split(','));
            }
            else
            {
                order = CreateOrderString();
            }

            if (FileSaveMode == Tool.FileSaveMode.Create)
            {
                order.ForEach(n => streamWriter.Write(n + "\t"));
                streamWriter.WriteLine();
            }
            else
            {
                if (existflag == false)
                {
                    order.ForEach(n => streamWriter.Write(n + "\t"));
                    streamWriter.WriteLine();
                }
            }

        }

        public override void Run(bool runChildren)
        {
            try
            {
                string fileName = this.FileName;
                if (FileNameTree != null)
                {
                    FileNameTree.SetParent();
                    FileNameTree.SetParent(this);
                    fileName = RawlerBase.GetText(GetText(), FileNameTree, this);
                }
                if (string.IsNullOrEmpty( fileName))
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        ReportManage.ErrReport(this, "FileNameが空です。");
                        return;
                    }                
                }

                baseFileName = fileName;
                currentFileName = fileName;
                FileInit(fileName);

                base.Run(runChildren);
            }
            catch (Exception e)
            {
                ReportManage.ErrReport(this, e.Message);
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
        }

        public override void Dispose()
        {
            
            if (streamWriter != null)
            {
                streamWriter.Dispose();
            }
            base.Dispose();
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<FileSave>(parent);
        }
    }

    public enum FileSaveMode
    {
        Create, Append
    }
}
