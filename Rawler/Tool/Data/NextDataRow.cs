using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
        [ContentProperty("Children")]
    [Serializable]
    public class NextDataRow : RawlerBase,ILastObject
    {

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

        bool doReLoad = false;
        /// <summary>
        /// dataがNullの時、りロードするか？
        /// </summary>
        public bool DoPageReLoad
        {
            get { return doReLoad; }
            set { doReLoad = value; }
        }

        int pageReLoadCount = 5;

        public int PageReLoadCount
        {
            get { return pageReLoadCount; }
            set { pageReLoadCount = value; }
        }
        int pageCount = 0;

        public event EventHandler PageReLoadEvent;
        private bool ignoreDataNull = false;
        public bool IgnoreDataNull { get { return ignoreDataNull; } set { ignoreDataNull = value; } }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        public override void Run(bool runChildren)
        {
            Data data = null;

            IRawler current = this.Parent;
            while (current != null)
            {
                if (current is Data)
                {
                    data = current as Data;
                    break;
                }
                current = current.Parent;
            }
            if (data != null)
            {
                if (data.GetCurrentDataNull())
                {
                    if (ignoreDataNull == false)
                    {
                        ReportManage.ErrReport(this, "RowがNullです。Writeが動作していないようです。");
                        var list = this.GetAncestorRawler().Where(n => n is Page);
                        if (DoPageReLoad)
                        {
                            if (list.Count() > 0)
                            {
                                var p = list.First() as Page;
                                pageCount++;
                                if (PageReLoadCount < pageCount)
                                {
                                    ReportManage.Report(this, "再読み込み待機中。");
                                    System.Threading.Thread.Sleep(1000 * pageCount * pageCount);

                                    p.Run();
                                }
                                else
                                {
                                    ReportManage.ErrReport(this, "書き込み先のData クラスが見つかりませんでした。");
                                }
                            }
                        }
                    }
                }
                else
                {
                    pageCount = 0;
                    data.NextDataRow();
                    ReportManage.Report(this, "NextDataRow");
                }
            }
            else
            {
                ReportManage.ErrReport(this,"書き込み先のData クラスが見つかりませんでした。");
            }
            this.RunChildren(runChildren);
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<NextDataRow>(parent);
        }
    }
}
