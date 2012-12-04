using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


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
        }

        public string AttributeOrderString { get; set; }





        private List<string> order = new List<string>();


        private List<string> CreateOrderString()
        {
            Queue<RawlerBase> stack = new Queue<RawlerBase>();
            List<string> list = new List<string>();
            foreach (var item in this.Children)
            {
                stack.Enqueue(item);
            }


            while (stack.Count > 0)
            {
                var rawler = stack.Dequeue();
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
                    foreach (var item in rawler.Children)
                    {
                        stack.Enqueue(item);
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


        void FileSave_Commited(object sender, Data.EventDataRow e)
        {

            if (streamWriter != null)
            {
                foreach (var item2 in order)
                {
                    if (e.DataRow.DataDic.ContainsKey(item2))
                    {
                        var item = e.DataRow.DataDic[item2];

                        if (item.Count > 1)
                        {
                            item.ForEach(n => streamWriter.Write(n.Replace("\t"," ") + ","));
                        }
                        else
                        {
                            streamWriter.Write(item.First().Replace("\t"," "));
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
                        item.Value.ForEach(n => streamWriter.Write(n + ","));
                    }
                    else
                    {
                        streamWriter.Write(item.Value.First());
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
                    Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
                    saveDialog.Title = "保存ファイルの指定　FileSave:" + this.Comment;
                    if (string.IsNullOrEmpty(ExtendFilter) == false)
                    {
                        saveDialog.Filter = RawlerLib.Io.FilterStringCreate(ExtendFilter);
                    }
                    if (saveDialog.ShowDialog() == true)
                    {
                        fileName = saveDialog.FileName;
                    }
                }
                var existflag =  System.IO.File.Exists(fileName);
                    

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
                    //if (AttributeOrderString != null)
                //{
                //    streamWriter.WriteLine(AttributeOrderString.Replace(",", "\t"));
                //}

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
