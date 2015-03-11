using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class RScript : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<RScript>(parent);
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
            string tmpFile = string.Empty;
            string file = ScriptFile;
            if (string.IsNullOrWhiteSpace(Script) == false)
            {
                tmpFile = System.IO.Path.GetTempFileName();
                var st = System.IO.File.CreateText(tmpFile);
                st.Write(Script);
                st.Close();
                file = tmpFile;
            }

            var process = System.Diagnostics.Process.Start(System.IO.Path.Combine(path, "Rscript.exe") +" "+file);
            process.WaitForExit();
            if (tmpFile != string.Empty)
            {
                System.IO.File.Delete(tmpFile);
            }
            base.Run(runChildren);
        }

        private string path = @"C:\Program Files\R\R-2.15.3\bin\x64";

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public string ScriptFile { get; set; }
        public string Script { get; set; }

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
}
