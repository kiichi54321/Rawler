using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class FileApendLine : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<FileApendLine>(parent);
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
        /// 書き込み先のファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (FileName != null)
            {
                try
                {
                    System.IO.File.AppendAllText(FileName, GetText() + "\n");
                }
                catch (Exception e)
                {
                    ReportManage.ErrReport(this, "ファイルの書き込みに失敗しました。"+e.Message);
                }
            }
            else
            {
                ReportManage.ErrReport(this, "ファイル名を指定してください");
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
}
