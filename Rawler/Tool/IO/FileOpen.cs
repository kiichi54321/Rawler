using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// ファイルを読み込み、一行分づつ子に流すクラスです。
    /// </summary>
    public class FileReadLines : RawlerMultiBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        ///　読み込むファイル名　Nullの時、ダイアログが開きます。
        /// </summary>
        public string FileName { get; set; }
        public RawlerBase FileNameTree { get; set; }

        protected string GetFileName()
        {
            if(FileNameTree !=null)
            {
                return  RawlerBase.GetText(this.GetText(), FileNameTree, this);
            }
            return this.FileName.Convert(this);
        }


        protected int skip = 0;

        /// <summary>
        /// 列名の時、飛ばす行数。初期値は０
        /// </summary>
        public int Skip
        {
            get { return skip; }
            set { skip = value; }
        }

        protected bool readEnd = false;

        public bool ReadEnd
        {
            get { return readEnd; }
            set { readEnd = value; }
        }

        /// <summary>
        /// ダイアログでの拡張子フィルターの指定
        /// </summary>
        public string ExtendFilter { get; set; }

        public override void Run(bool runChildren)
        {
            string filename = this.GetFileName();
            if (string.IsNullOrEmpty(filename))
            {
                ReportManage.ErrEmptyPropertyName(this, nameof(FileName));
                return;
            }
            if(System.IO.File.Exists(filename) == false)
            {
                ReportManage.ErrReport(this, "File「"+filename+"」は存在しません");
                return;
            }
            try
            {
                if (readEnd)
                {
                    string t = System.IO.File.ReadAllText(filename);
                    System.IO.StringReader sr = new System.IO.StringReader(t);
                    List<string> list = new List<string>();
                    while (sr.Peek() > -1)
                    {
                        string line = sr.ReadLine();
                        if (line.Length > 0)
                        {
                            list.Add(line);
                        }
                    }
                    sr.Close();
                    base.RunChildrenForArray(runChildren, list);
                }
                else
                {
                    var lines = System.IO.File.ReadLines(filename);
                    if (skip > 0)
                    {
                        lines = lines.Skip(skip);
                    }
                    base.RunChildrenForArray(runChildren, lines);
                }
            }
            catch(Exception ex)
            {
                ReportManage.ErrReport(this, FileName+"を開くのに失敗しました"+ex.Message);   
            }
        }

        protected string FilterStringCreate(string extend)
        {
            return "<> files (*.<>)|*.<>|All files (*.*)|*.*".Replace("<>", extend);
        }
    }
    

    /// <summary>
    /// 現在のFileReadLineの値を返します。上流にFileReadLinesがないと機能しません
    /// </summary>
    public class GetCurrentFileReadLine : RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var file = this.GetAncestorRawler().Where(n => n is FileReadLines);
            if (file.Count() > 0)
            {
                SetText(file.First().Text);
            }
            else
            {
                ReportManage.ErrUpperNotFound<FileReadLines>(this);
            }
            base.Run(runChildren);
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetCurrentFileReadLine>(parent);
        }
    }

    /// <summary>
    /// ファイルを開いてその内容全てを子に流す。
    /// </summary>
    public class FileReadAllText :RawlerBase
    {
        /// <summary>
        /// ファイルの名前
        /// </summary>
        public string FileName { get; set; }
        public override void Run(bool runChildren)
        {
            string filename = FileName.Convert(this);
            if(string.IsNullOrEmpty( filename))
            {
                filename = GetText();
            }
            if (string.IsNullOrEmpty(filename))
            {
                ReportManage.ErrEmptyPropertyName(this, nameof(FileName));
                return;
            }
            if (System.IO.File.Exists(filename) == false)
            {
                ReportManage.ErrReport(this, "File「" + filename + "」は存在しません");
                return;
            }
            try
            {
                SetText(System.IO.File.ReadAllText(filename));
            }
            catch(Exception ex)
            {
                ReportManage.ErrReport(this, filename + "を開くのに失敗しました" + ex.Message);
                return;
            }

            base.Run(runChildren);
        }

    }
}
