using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
{
    public class ContainTextInFile : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ContainTextInFile>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public string FileName { get; set; }

        private bool result = true;
        /// <summary>
        /// 含まれるのを判定するならTrue　含まれないものを判定するならFalse
        /// </summary>
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }


        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (System.IO.File.Exists(FileName) == false)
            {
                var f = System.IO.File.CreateText(FileName);
                f.Close();
            }
            var txt = GetText();
            var flag = System.IO.File.ReadLines(FileName).Contains(txt);
            this.SetText(GetText());
            if (flag == result)
            {
                base.Run(runChildren);
            }

        }

        public void AddText(string txt)
        {
            using (var file = System.IO.File.AppendText(FileName))
            {
                file.WriteLine(txt);
            }
        }

        public int GetCountLines()
        {
            try
            {
                return System.IO.File.ReadLines(FileName).Count();
            }
            catch
            {
                ReportManage.ErrReport(this, "ファイルが存在しません");
            }
            return -1;
        }


        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


    }

    public class ContainTextInFileAddText : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ContainTextInFileAddText>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler().OfType<ContainTextInFile>();
            if (list.Any())
            {
                list.First().AddText(GetText());
            }
            else
            {
                ReportManage.ErrReport(this, "ContainTextInFileAddTextは上流にContainTextInFilegaが必要です");
            }

            base.Run(runChildren);
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }
        }


    }

    public class ContainTextInFileCountLines : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ContainTextInFileCountLines>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {


            base.Run(runChildren);
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                var list = this.GetAncestorRawler().OfType<ContainTextInFile>();
                if (list.Any())
                {
                    SetText(list.First().GetCountLines().ToString());
                }
                else
                {
                    ReportManage.ErrReport(this, "ContainTextInFileCountLinesは上流にContainTextInFilegaが必要です");
                }

                return text;
            }
        }


    }

}
