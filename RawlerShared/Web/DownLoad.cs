using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
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
            if (Folder == null)
            {
                ReportManage.ErrEmptyPropertyName(this, nameof(Folder));
                return;
            }
            Folder.Parent = this;
            var client = this.GetAncestorRawler().OfType<WebClient>().DefaultIfEmpty(new WebClient()).FirstOrDefault();

            string url = Url;
            if (string.IsNullOrEmpty(url))
            {
                url = GetText();
            }
            var page = this.GetUpperRawler<Page>();
            string referer = string.Empty;
            if (page !=null)
            {
                referer = page.GetCurrentUrl();                
            }

            var data = client.HttpGetByte(url,referer);
            data.Wait();
            string fileName = string.Empty;
            if (string.IsNullOrEmpty(SaveName) == false)
            {
                fileName = SaveName.Convert(this);
            }
            else
            {
                fileName = System.IO.Path.GetFileNameWithoutExtension(url);
            }
            string ex = System.IO.Path.GetExtension(url);
            if (ex.Split('?').Length > 0) ex = ex.Split('?').First();

            Folder?.WriteFile(fileName + ex, data.Result);

            SetText(fileName + ex);

            base.Run(runChildren);
        }

        public string Url { get; set; }

        public string SaveName { get; set; }

        public IFolderBase Folder
        {
            get;
            set;
        }



    }
}
