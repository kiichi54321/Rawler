using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
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
            ReportManage.ErrEmptyPropertyName(this, nameof(Path));
            string path = Path.Convert(this);

            try
            {
                var p = PCLStorage.PortablePath.Combine(new string[] { PCLStorage.FileSystem.Current.LocalStorage.Path, path });
                var folder = PCLStorage.FileSystem.Current.GetFolderFromPathAsync(p);

                folder.Wait();
                Rawler.IO.IoState.CurrentFolder = folder.Result;
            }
            catch (Exception e)
            {
                ReportManage.ErrReport(this, e.Message);
            }

            base.Run(runChildren);
        }

        public string Path { get; set; }

        //private SpecialFolder workFolderType = SpecialFolder.none;

        //public SpecialFolder SpecialFolder
        //{
        //    get { return workFolderType; }
        //    set { workFolderType = value; }
        //}

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
