using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 報告するためのクラスです。
    /// </summary>
    public class Report:RawlerBase
    {
        public Report()
            : base()
        {
        }

            

        public string Header { get; set; }
        public string Footer { get; set; }
        private bool visible = true;
        public bool Visible { get { return visible; } set { visible = value; } }
        public string Message { get; set; }

        private bool viewParentText = true;
        private bool returncode = true;

        public bool ReturnCode
        {
            get { return returncode; }
            set { returncode = value; }
        }


        public bool ViewParentText
        {
            get { return viewParentText; }
            set { viewParentText = value; }
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override void Run(bool runChildren)
        {
            string text;
            if (viewParentText && string.IsNullOrEmpty(Message))
            {
                text = Header.Convert(this) + GetText() + Footer.Convert(this);
            }
            else
            {
                text = Header.Convert(this) + Message.Convert(this) + this.Footer.Convert(this);
            }
            ReportManage.Report(this, System.Net.WebUtility.HtmlDecode( text),returncode, visible);
            this.RunChildren(runChildren);
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Report>(parent);
        }
    }

    /// <summary>
    /// 経過を報告します。具体的には「.」を表示させます。
    /// </summary>
    public class ReportProgress : Report
    {
        public ReportProgress()
            :base()
        {
            this.ReturnCode = false;
            this.ViewParentText = false;
            this.Header = ".";
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ReportProgress>(parent);
        }
    }

    /// <summary>
    /// レポートに改行をいれる。
    /// </summary>
    public class ReportReturn : Report
    {
         public ReportReturn()
            :base()
        {
            this.ReturnCode = false;
            this.ViewParentText = false;
        }

         /// <summary>
         /// ObjectのName。表示用
         /// </summary>
         public override string ObjectName
         {
             get { return this.GetType().Name; }
         }
    }

    /// <summary>
    /// レポートに改行をいれる。
    /// </summary>
    public class ReportAddCount : Report
    {
        public ReportAddCount()
            : base()
        {
            this.ReturnCode = false;
            this.ViewParentText = false;
            this.Visible = false;
            this.Header = "NextDataRow";
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
    }

    /// <summary>
    /// レポートに改行をいれる。
    /// </summary>
    public class ReportCounter : Report
    {
        public ReportCounter()
            : base()
        {
            this.ReturnCode = true;
            this.ViewParentText = false;
            this.Visible = true;
        }

        int viewCount = 100;

        public int ViewCount
        {
            get { return viewCount; }
            set { viewCount = value; }
        }

        int count = 0;
        public override void Run(bool runChildren)
        {
            count++;
            if (count % viewCount == 0)
            {
                this.Message = count.ToString();
                base.Run(runChildren);
            }
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        /// <summary>
        /// clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ReportCounter>(parent);
        }
    }
}
