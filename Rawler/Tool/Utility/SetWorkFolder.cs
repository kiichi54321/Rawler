using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public enum SpecialFolder
    {
        none,Desktop,MyDocuments
    }

    public class SetWorkFolder : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<SetWorkFolder>(parent);
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
            string path = string.Empty;
            if (workFolderType == Tool.SpecialFolder.MyDocuments)
            {
                path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            if (workFolderType == Tool.SpecialFolder.Desktop)
            {
                path = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }
            if(FolderTree !=null)
            {
                var f = RawlerBase.GetText(GetText(), FolderTree, this);
                path = System.IO.Path.Combine(path,f);
            }
            else if (Folder != null)
            {
                path = System.IO.Path.Combine(path, Folder);
            }
            try
            {
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                System.IO.Directory.SetCurrentDirectory(path);
            }
            catch (Exception e)
            {
                ReportManage.ErrReport(this, e.Message);
            }
            SetText(System.IO.Directory.GetCurrentDirectory());

            base.Run(runChildren);
        }

        public string Folder { get; set; }
        public RawlerBase FolderTree { get; set; }


        private SpecialFolder workFolderType = SpecialFolder.none;

        public SpecialFolder SpecialFolder
        {
            get { return workFolderType; }
            set { workFolderType = value; }
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
}
