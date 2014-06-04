using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class DownLoad : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<DownLoad>(parent);
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
            var client = this.GetAncestorRawler().OfType<WebClient>().DefaultIfEmpty(new WebClient()).FirstOrDefault();
            string url = GetText();
            var data = client.HttpGetByte(GetText());
            string path = string.Empty;
            if (FolderNameTree != null)
            {
                path = RawlerBase.GetText(this.Parent.Text, FolderNameTree, this.Parent);
            }
            else
            {
                path = Folder;
            }
            if (path != null)
            {
                if (System.IO.Path.IsPathRooted(path) ==false)
                {
                    path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), path);
                }

                if (System.IO.Directory.Exists(path) == false)
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    catch(Exception e)
                    {
                        ReportManage.ErrReport(this, "ディレクトリーの作成に失敗しました" + e.Message);
                    }
                }
               
            }
            else
            {
                path = System.IO.Directory.GetCurrentDirectory();
            }
            string ex = System.IO.Path.GetExtension(url);
            if (ex.Split('?').Length > 0) ex = ex.Split('?').First();
            string fileName = string.Empty;
            if (SaveNameTree != null)
            {
                fileName =  RawlerBase.GetText(this.Parent.Text, SaveNameTree, this.Parent);
            }
            else
            {
                fileName = System.IO.Path.GetFileNameWithoutExtension(url);
            }
            path = System.IO.Path.Combine( path , fileName + ex);

            try
            {
                using (var writer = System.IO.File.Create(path))
                {
                    writer.Write(data,0, data.Length);
                    SetText(path);
                }
            }
            catch(Exception e)
            {
                ReportManage.ErrReport(this, "ファイルの保存に失敗しました" + e.Message +" Path:"+ path);
            }
            SetText(fileName + ex);

            base.Run(runChildren);
        }

        public RawlerBase SaveNameTree
        {
            get;
            set;
        }

        public string Folder { get; set; }

        public RawlerBase FolderNameTree
        {
            get;
            set;
        }



    }
}
