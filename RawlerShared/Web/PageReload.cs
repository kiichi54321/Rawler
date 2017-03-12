using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;
using System.Threading.Tasks;

namespace Rawler
{
    public class PageReload : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<PageReload>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        int reloadCount = 5;

        public int ReloadCount
        {
            get { return reloadCount; }
            set { reloadCount = value; }
        }

        private double sleepSeconds = 10;

        /// <summary>
        /// スリープする時間です。
        /// </summary>
        public double SleepSeconds
        {
            get { return sleepSeconds; }
            set { sleepSeconds = value; }
        }

        public RawlerBase OverCountTree { get; set; }


        string tmpUrl = string.Empty;
        int tmpCount = 0;
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var page = this.GetAncestorRawler().OfType<Page>().FirstOrDefault();
            if (page != null)
            {
                if (tmpUrl == page.GetCurrentUrl())
                {
                    tmpCount++;
                    if (tmpCount >= reloadCount)
                    {
                        ReportManage.ErrReport(this, "ReloadCount:規定数のリロード回数を超えました。");
                        OverCountTree.SetParent(this);
                        OverCountTree.Run();
                        return;
                    }
                }
                else
                {
                    tmpUrl = page.GetCurrentUrl();
                    tmpCount = 0;
                }
                page.Reload();
                Task.Delay((int)(sleepSeconds * 1000)).Wait();
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
                return base.Text;
            }
        }


    }
}
